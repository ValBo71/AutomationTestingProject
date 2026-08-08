# the-internet — Playwright + TypeScript UI Automation

End-to-end UI automation covering **all 44 challenges** on
[the-internet.herokuapp.com](https://the-internet.herokuapp.com/), the reference
practice site for hard-to-automate web elements.

Unlike a straightforward e-commerce flow, every page here exists to break a
naive test: randomised DOM ids, elements that appear only after a delay, content
inside iframes and shadow roots, native dialogs, HTTP auth, and images that are
"visible" while being broken.

**62 tests, 9 spec files, all green.** Verified over four consecutive full runs
to rule out flakiness on the randomised challenges.

---

## 🚀 Running

```bash
npm install
npx playwright install chromium

npm test            # headless (default)
npm run test:headed # watch a scenario in a real browser
npm run report      # open the HTML report
npm run typecheck   # tsc --noEmit
```

The suite runs against the live public site — no local setup or seed data.

---

## 📂 Structure

```text
06-the-internet-ui-typescript/
├── core/basePage.ts        # shared navigation + the flash-banner helper
├── data/testData.ts        # every route and expected string in one place
├── selectors/              # selector maps, grouped by challenge area
├── pages/                  # page objects (one per challenge group)
├── tests/                  # 9 spec files
└── playwright.config.ts
```

Page objects hold the *interaction* logic (how to defeat a challenge); specs
hold only intent and assertions.

---

## 🎯 Coverage — all 44 challenges

| # | Challenge | Spec | What makes it non-trivial |
|---|---|---|---|
| 1 | A/B Testing | `forms` | Variation chosen by cookie — assert the known set, not one value |
| 2 | Add/Remove Elements | `forms` | Indices shift after each delete |
| 3 | Basic Auth | `auth` | Credentials must be set on the browser context, not typed |
| 4 | Broken Images | `dom` | A broken image is still "visible" — only `naturalWidth` catches it |
| 5 | Challenging DOM | `dom` | Button ids are regenerated on every load |
| 6 | Checkboxes | `forms` | One box ships pre-checked — `setChecked` proves real state change |
| 7 | Context Menu | `interactions` | Right-click raises a native dialog |
| 8 | Digest Authentication | `auth` | Same context-level credential handling as Basic |
| 9 | Disappearing Elements | `dynamic` | A nav item appears at random — fixed counts would be flaky |
| 10 | Drag and Drop | `interactions` | HTML5 DnD — needs manual `DragEvent` dispatch |
| 11 | Dropdown | `forms` | Placeholder option is disabled |
| 12 | Dynamic Content | `dynamic` | Text changes per load — compare across two loads |
| 13 | Dynamic Controls | `dynamic` | **The id moves from the wrapper onto the input after re-add** |
| 14 | Dynamic Loading | `dynamic` | Hidden vs. not-rendered — two different waits |
| 15 | Entry Ad | `notifications` | Modal on load |
| 16 | Exit Intent | `notifications` | Fires on `mouseleave` at the top of the viewport |
| 17 | File Download | `navigation` | Capture the `download` event |
| 18 | File Upload | `navigation` | Fixture written at runtime, not committed |
| 19 | Floating Menu | `interactions` | Must stay in viewport after scroll |
| 20 | Forgot Password | `auth` | ⚠️ Server returns HTTP 500 — see *Known defects* |
| 21 | Form Authentication | `auth` | Valid / bad user / bad password / logout |
| 22 | Frames | `frames` | Cross the iframe boundary |
| 23 | Geolocation | `navigation` | Permission granted + coordinates pinned for determinism |
| 24 | Horizontal Slider | `forms` | `fill()` skips the change event; clicking jumps the thumb |
| 25 | Hovers | `interactions` | Captions exist but are hidden until hover |
| 26 | Infinite Scroll | `dynamic` | Scroll until content grows, not scroll-once |
| 27 | Inputs | `forms` | Arrow-key increments |
| 28 | JQuery UI Menus | `interactions` | Submenu only reachable via hover |
| 29 | JavaScript Alerts | `notifications` | alert / confirm / prompt, accept and dismiss |
| 30 | JavaScript onload error | `navigation` | Invisible in UI — needs a `pageerror` listener attached first |
| 31 | Key Presses | `forms` | Page echoes the key name |
| 32 | Large & Deep DOM | `dom` | 50-level nesting + a large table |
| 33 | Multiple Windows | `navigation` | Await the popup *before* clicking |
| 34 | Nested Frames | `frames` | Frame within a frame |
| 35 | Notification Messages | `notifications` | Random message; must enter via `/notification_message` |
| 36 | Redirect Link | `navigation` | Follows to the status-codes page |
| 37 | Secure File Download | `auth` | 401 without credentials, 200 with |
| 38 | Shadow DOM | `dom` | Playwright pierces open shadow roots automatically |
| 39 | Shifting Content | `dynamic` | Layout shifts a few pixels per load |
| 40 | Slow Resources | `dynamic` | Page must still resolve |
| 41 | Sortable Data Tables | `dom` | **tablesorter rewrites rows asynchronously** — poll, don't read immediately |
| 42 | Status Codes | `navigation` | Parametrised over 200 / 301 / 404 / 500 |
| 43 | Typos | `dom` | Typo injected at random — assert one of two known states |
| 44 | WYSIWYG Editor | `frames` | ⚠️ Editor is read-only — see *Known defects* |

---

## ⚠️ Known defects (site-side, not test bugs)

Two challenges cannot currently pass because of problems in the target site
itself. Rather than deleting the coverage or weakening the assertion until it
goes green, both are marked `test.fail()` — the suite keeps asserting the
*correct* behaviour, and Playwright will report an **unexpected pass** the day
either is fixed.

**Forgot Password — HTTP 500.** Submitting the form returns
`Internal Server Error` instead of the confirmation page; the deployed app has
no working mailer. Reproduced on repeated runs, so it is behaviour, not flake.
A companion test pins the current 500 so a silent change is noticed.

**WYSIWYG Editor — read-only.** TinyMCE renders with
`class="mce-content-readonly"` and `contenteditable="false"`, reporting
*"TinyMCE is in read-only mode because you have no more editor loads available
this month."* The site owner's TinyMCE cloud quota is exhausted, so typing is
impossible for any client. Reading the editor's content across the iframe
boundary — the actual skill this challenge tests — is still covered and passes.

---

## 🧭 Notes on approach

* **Assertions over randomness.** Several pages randomise their output. Those
  tests assert membership of a known set or compare two loads, rather than
  pinning a value that would fail intermittently for no real reason.
* **No hard-coded sleeps.** Waiting is expressed through Playwright's
  auto-waiting assertions, `expect.poll`, or `waitForFunction`.
* **Locators chosen for stability.** Where the site regenerates ids
  (Challenging DOM) or moves them between elements (Dynamic Controls), the
  locators deliberately avoid them — with a comment explaining why.
