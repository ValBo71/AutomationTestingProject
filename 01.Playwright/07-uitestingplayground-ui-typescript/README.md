# UI Test Automation Playground — Playwright + TypeScript

UI automation covering **all 29 challenges** on
[uitestingplayground.com](http://uitestingplayground.com/) — a site where every
page is a deliberate trap built to break naive automation.

Where `06-the-internet-ui-typescript` covers breadth (44 different element
types), this suite goes after depth: randomised ids, elements that lie about
their state, overlays that swallow clicks, and DOM nodes replaced under the
test's feet.

**43 tests, 5 spec files, all green.** Verified over four consecutive full runs
(26.1s, 28.1s, 29.8s, 21.6s).

> The sources for this suite are kept outside the public repository; this file
> documents the work and the findings.

---

## ⚠️ The site's certificate is broken, and it changes what can be tested

`uitestingplayground.com` serves a certificate issued for `*.azurewebsites.net`,
so an HTTPS request fails the certificate check outright. The suite therefore
uses `http://` as its `baseURL` and sets `ignoreHTTPSErrors: true`.

That has one consequence that is easy to lose an afternoon to: **the Geolocation
and Clipboard APIs are only available in a secure context, and plain `http://`
is not one.** Verified directly rather than assumed — over http the geolocation
page reports `unavailable` no matter what permissions are granted, and
`navigator.clipboard` is `undefined`.

The two specs that need those APIs load the site over `https://` instead, where
`ignoreHTTPSErrors` lets the invalid certificate through while still giving the
page a secure origin. Same site, same test, different security context.

---

## 📂 Structure

```text
07-uitestingplayground-ui-typescript/
├── core/basePage.ts        # navigation + the shared #opstatus helper
├── data/testData.ts        # routes, expected strings, tuning values
├── selectors/Selectors.ts  # selector maps grouped by challenge type
├── pages/                  # page objects - one per challenge family
└── tests/                  # 5 spec files
```

---

## 🎯 Coverage — all 29 challenges

### Locator strategy (`locators.spec.ts`)
| Challenge | The trap |
|---|---|
| Dynamic ID | The id is a fresh GUID on every load — the test proves it changes, then locates by class |
| Class Attribute | Three buttons share `btn` and `btn-test`; `[class="btn-primary"]` matches nothing |
| Non-Breaking Space | The caption uses U+00A0 — visually identical to a space, so a plain-space locator fails |
| Text Input | Button caption is rewritten from an input value |
| Verify Text | DOM text is padded with newlines the screen never shows |
| CSS Selectors | id / class / attribute / adjacent-sibling, plus five different hiding techniques |

### Waiting (`waits.spec.ts`)
| Challenge | The trap |
|---|---|
| AJAX Data | ~15s server delay |
| Client Side Delay | ~15s busy loop — no network event to wait on |
| Load Delay | The button does not exist until the slow page finishes loading |
| Auto Wait | Target becomes visible/enabled after 3s and 5s — no sleeps used |
| Disabled Input | **Starts enabled**; the button disables it first and re-enables it seconds later |
| Animated Button | The click only counts once the element stops moving |
| Progress Bar | Stopped at a target value by polling, not by sleeping |

### Obstructions (`obstructions.spec.ts`)
| Challenge | The trap |
|---|---|
| Click | Ignores `element.click()` — the spec shows the DOM click failing, then a real click working |
| Hidden Layers | A transparent layer covers the button after the first click |
| Overlapped Element | Input sits under an absolutely-positioned overlay inside a scrollable box |
| Scrollbars | Button far inside a scrollable container |
| Scroll to Click | Four targets: page scroll, two-axis box, nested scrollers, hover-revealed |
| Visibility | Eight buttons hidden eight different ways — each with a different observable state |

### Context switching (`contexts.spec.ts`)
| Challenge | The trap |
|---|---|
| Frames | An outer frame and a frame nested inside it with **identical** markup |
| Shadow DOM | GUID generator whose controls live in an open shadow root |
| File Upload | React drag-and-drop uploader inside an iframe, with a hidden file input |

### Application behaviour (`app.spec.ts`)
| Challenge | The trap |
|---|---|
| Sample App | **Input ids are random GUIDs** — only the `name` attribute is stable |
| Alerts | alert / confirm / prompt, accepted and dismissed |
| Select | Selection by value, with the status line as proof |
| Clear Input | Inputs clear with `fill('')`; a contenteditable div needs select-all + delete |
| Mouse Over | Hovering retitles the anchor from "Click me" to "Active Link" — the classic stale-element trap |
| Dynamic Table | Column **and** row order randomised per load — the value is found by matching header text to cell position |
| Geo Location | Mocked coordinates, loaded over https for a secure context |

---

## 🔍 Three places where the published HTML lies

Every selector in this suite was read from the **live DOM**, not from the site's
documentation or its static markup. That was not caution for its own sake —
three challenges would otherwise have produced tests that pass against a stale
assumption and fail for everyone else:

**Sample App.** The published markup shows `id="username"`. The deployed page
generates **random GUID ids** on every load. Only the `name` attribute is stable.

**Class Attribute.** The colour classes differ from the published HTML — `class2`
renders as `btn-success`, not `btn-warning`. A test asserting the documented
colour fails against the real site.

**CSS Selectors.** Neither input carries an id at all. The markup contains a
duplicated `id` attribute, which the browser discards.

---

## 🧭 Notes on approach

* **No hard-coded sleeps.** Waiting is expressed through Playwright's
  auto-waiting assertions, `expect.poll` or `waitForFunction` — including on the
  two challenges that stall for a full fifteen seconds.
* **The overlapped-element solution scrolls until the element is genuinely
  topmost**, verified with `document.elementFromPoint`, rather than assuming
  `scrollIntoViewIfNeeded` is enough. It is not: the overlay is positioned
  against the outer container, so it does not move with the input.
* **The non-breaking space caught the author too.** The first version of
  `testData.ts` contained a pasted U+00A0 that looked exactly like a space, so
  the assertion compared the character to itself and the test failed for the
  wrong reason — the very confusion the `/nbsp` page exists to teach. It is now
  built with `String.fromCharCode(0x00a0)`, so neither a formatter nor a careless
  edit can silently turn it back.
