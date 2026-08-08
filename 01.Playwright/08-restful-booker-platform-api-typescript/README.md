# restful-booker-platform — API & hybrid automation (Playwright + TypeScript)

API and API/UI automation against
[automationintesting.online](https://automationintesting.online), Mark
Winteringham's **restful-booker-platform** — a working hotel booking site built
as a set of separate services behind one gateway.

**56 tests, 8 spec files, all green.** 44 pure API tests and 12 hybrid tests that
cross the HTTP/browser boundary in both directions. Eight of the 56 are known
defects, held open deliberately (see below).

Where `06-the-internet-ui-typescript` covers breadth and
`07-uitestingplayground-ui-typescript` covers awkward UI elements, this suite is
about a **system**: six services, a shared session, and a browser that has to
agree with the API about what exists.

> The sources for this suite are kept outside the public repository; this file
> documents the work, the coverage and the defects it found.

---

## 🚀 Running

```bash
npm install
npx playwright install chromium

npm test              # everything - 56 tests
npm run test:api      # 44 API tests, no browser is launched at all
npm run test:ui       # 12 hybrid tests
npm run test:headed   # watch the hybrid tests in a real browser
npm run report        # open the HTML report
npm run typecheck     # tsc --noEmit
```

Credentials are `admin` / `password`, published on the site's own landing page.
There is nothing secret in this repository.

---

## 🧭 The scenario, end to end

### What is under test

Six services, reached through one gateway at `/api/*`:

| Service | Route | Guarded? | What it owns |
|---|---|---|---|
| Auth | `/api/auth/*` | — | Issues, validates and (nominally) revokes tokens |
| Room | `/api/room` | writes only | The hotel's rooms |
| Booking | `/api/booking` | reads and deletes | Stays, and the rules about overlapping them |
| Message | `/api/message` | **not at all** | Guest enquiries from the contact form |
| Branding | `/api/branding` | writes only | Hotel name, address, description |
| Report | `/api/report` | yes | A calendar view built from bookings |

They do not share conventions. Room creation answers `200 {"success": true}`;
booking creation answers `201` with the new resource. Room and booking wrap
validation errors in `{"errors": [...]}`; the message service returns a bare
array. A missing token is `401` on a read and `403` on a delete. **Documenting
those inconsistencies is a large part of what this suite is for** — a client
written against one service's conventions silently mishandles another's.

### How a test is wired

```
spec  ──▶  fixture  ──▶  client  ──▶  ApiClient  ──▶  Playwright request
                │            │
                │            └── typed helpers (createRoomAsync, …)
                └── janitor: undo stack, drained after every test
```

* **`core/apiClient.ts`** attaches the session cookie and hands back the raw
  `APIResponse`. It never throws on a non-2xx. That is deliberate: a client that
  throws on 4xx makes negative testing impossible, and on this platform the
  error responses are half the subject matter.
* **`clients/*.ts`** — one per service, plus typed `…Async` helpers for the
  common flows.
* **`fixtures/api.ts`** — authenticated and anonymous clients side by side, so
  "this endpoint is protected" is one line rather than a manual header dance.
* **`schemas/schemas.ts`** — Zod contracts, mostly `.strict()`, so a *new* field
  in a response is a reported change rather than a silent one.

### The flow the hybrid tests exercise

The platform keeps its session in a plain `token` cookie, and the admin screens
do nothing but read it. So a token minted over HTTP is indistinguishable from
one earned by filling the login form:

```ts
const token = await auth.loginAsAdminAsync();   // one HTTP call
await admin.loginWithTokenAsync(token);          // injected as a cookie
await admin.open();                              // lands on /admin/rooms
```

The login **form** is tested once, on purpose. Every other admin test starts from
an injected token — it removes a page load and a round-trip from each one, and
it stops a broken login form from failing twenty unrelated tests.

On top of that, state is pushed across the boundary in both directions:

| Direction | Scenario |
|---|---|
| API → UI | Create a room over HTTP, assert its row in the admin table |
| API → UI | Post a message over HTTP, assert it in the inbox and in the unread badge |
| API → UI | Delete a room over HTTP, reload, assert the row is gone |
| UI → API | Type a room into the admin form, assert the **room service** holds it |
| UI → API | Submit the public contact form, assert the **message service** holds it |
| API → API | Create a booking, assert the **report service** shows it |

That last row matters more than it looks: report is a different service reading
the same data, so it is the cheapest available proof that the two are wired
together.

---

## 🎯 Coverage

### Auth (`tests/api/auth.spec.ts`)
Token issuing, validation and endpoint protection. Includes the check that a
wrong password and an unknown username produce **identical** responses — correct
behaviour, and worth pinning, because a helpful error message here is a way to
enumerate accounts.

### Rooms (`tests/api/room.spec.ts`)
Public read, guarded write, full create/update/delete lifecycle, and validation
that reports every broken field at once rather than stopping at the first.

### Bookings (`tests/api/booking.spec.ts`)
Anonymous guest booking, double-booking refusal, partial-overlap refusal,
per-field validation, and the collection endpoint's refusal to answer without a
room filter. Also pins that dates come back **exactly** as sent — a service that
round-trips them through a timezone shifts a guest's stay by a day.

Every booking test creates its **own** room. The seeded rooms are shared with
everyone else using the instance, and overlapping stays are rejected outright,
so tests sharing a room would fail each other under parallelism.

### Messages (`tests/api/message.spec.ts`)
Contact form, inbox, read flag, deletion, and boundary validation either side of
the same rule — a 19-character body is refused, a 20-character body is accepted.
Testing only the middle of a range proves nothing about its edges.

### Branding & report (`tests/api/site.spec.ts`)
Public branding read with contract validation, guarded writes, `logoUrl`
validation, and the booking-to-report correlation described above.

### Hybrid (`tests/hybrid/*.spec.ts`)
Session sharing, room synchronisation both ways, message synchronisation both
ways, and the unread badge.

---

## 🐞 The eight defects

Each is marked `test.fail()` rather than deleted or weakened. The assertions
describe what the platform *should* do, so the day any of these is fixed the test
turns red and says so, instead of quietly agreeing with a bug.

### 1. Logout does not log anything out — `auth.spec.ts`
`POST /api/auth/logout` answers `{"success": true}` and the token keeps working.
Verified three times against fresh tokens: log in, confirm the token validates,
log out, and the same token still validates **and** still opens protected
endpoints. There is no way for a client to end a session.

### 2. An unknown token crashes the booking service — `auth.spec.ts`
A syntactically valid but unknown token produces `500` from
`GET /api/booking`. `/api/auth/validate` handles the same input correctly with a
`403`, so the fault is in how the booking service consumes the cookie.

### 3. The message service has no authentication at all — `message.spec.ts`
**The most serious finding here.** `/api/message` lists every enquiry and
`/api/message/{id}` returns the sender's name, e-mail address, phone number and
the full text of their message — with no token. The room and booking services in
the same platform do require one, so this is a missing guard on one service
rather than a platform-wide design decision.

### 4. A room created over HTTP is never offered to the public — `roomSync.spec.ts`
Commercially the most expensive one. Create a room, and `GET /api/room` returns
four rooms, the admin table lists four rooms — and the public page renders three
cards. The room cannot be booked by anyone, and nothing in the admin UI hints at
it.

Caching was the obvious explanation and it is wrong: a network trace of the page
load shows it calling `GET /api/room` on every visit, and the response it
receives contains the new room. The page fetches the full list and does not
render it. Watched across five reloads over a minute with a cache-busting query
string — three cards every time.

### 5. A branding update reports success and writes nothing — `site.spec.ts`
`PUT /api/branding` answers `200 {"success": true}` and discards the write.
A silent no-op behind a success status is worse than an error: nothing
downstream has any reason to retry or warn.

### 6. An unknown room id returns 500 instead of 404 — `room.spec.ts`
The body leaks the internal path (`"/room/9999"`), which incidentally confirms
the gateway prefix is stripped before the service sees it.

### 7. Room creation returns no id — `room.spec.ts`
`200 {"success": true}`, no `Location` header, no resource. The only way to learn
the id of what you just made is to re-read the whole collection and match on
name — which is exactly what `RoomClient.createRoomAsync` has to do. The booking
service, in the same platform, does it correctly.

### 8. A reversed date range is reported as a conflict — `booking.spec.ts`
A checkout before the checkin gets `409 "Failed to create booking"` — the same
status and the same message as a genuine double-booking. A client cannot tell
the two apart, yet "those dates are taken" and "you have the dates backwards"
need different messages on screen.

---

## 🧹 Working against a shared instance

This is a public sandbox. Other people are using it at the same moment, and it
appears to be reseeded periodically — the message ids and the seeded `logoUrl`
both changed mid-session. Three rules follow, and they shape most of the code:

**1. Every test cleans up after itself, and the undo is armed *before* the
write.** `fixtures/api.ts` provides a `janitor` that drains newest-first after
every test, pass or fail.

**2. Cleanup is registered by name, not by id.** Room and message creation do not
return one, so an undo written after the fact has nothing to hold onto.

**3. No test asserts on a total.** Row counts, message counts and unread counts
all belong to the whole instance, never to one test. Assertions are scoped to a
known id, or to a direction of travel (`toBeLessThan`), never to an exact total.

---

## 🔍 Four things that went wrong while writing this

Recorded because each one changed the code, and because the first is the kind of
mistake that ends up in a bug report to someone else's team.

**A defect that wasn't.** The branding write was first written up as a silent
no-op after a single write-then-read. Then the seeded `logoUrl` turned out to
have changed since the previous probe, which meant *something* had persisted, and
the whole conclusion was suspect. Cloudflare reports `cf-cache-status: DYNAMIC`
on the route, so CDN caching was ruled out too. The claim only went into the
suite once a marker written to `name` had been polled every five seconds for a
full minute — twelve reads, no change. The published finding is the one that
survived that; the first draft would have been wrong.

**Five leaked rooms.** The `test.fail` room-creation test registered its cleanup
*after* the first assertion — in a test expected to fail on that very assertion,
so it never ran. Five rooms accumulated on the shared instance before the pattern
was spotted. Hence rule 1 above: arm the undo before the write, always.

**Counting things this suite does not own.** Three tests compared totals — row
counts and unread counts — before and after an action. All three failed once the
suite ran in parallel, because another worker's data landed in between. One
failed with 19 before and 18 *after* creating a message. Hence rule 3.

**Reading 202 as if it meant 200.** Deletion assertions were written on the line
after the delete, and passed three runs in four. `202 Accepted` means the work is
queued, not done — the status code had been saying so all along. They are polled
now.

---

## 📊 Stability

Eight consecutive full runs, ~18 seconds each.

Seven were clean at 56/56. One run had a single failure on a booking test that
could not be reproduced in six targeted repeats, and which came back as a `500`
from the host. Concurrency was investigated and ruled out as the cause: 22 rooms
created four-at-a-time and then sequentially returned `200` every time.

Local `retries` is deliberately left at **0** so that host flakiness stays
visible rather than being quietly absorbed; CI uses 2. `RoomClient` retries
exactly once, and only on a `5xx` — never on a `4xx`, which would turn a real
validation failure into a confusing timeout.
