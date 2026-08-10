# Automation Testing Portfolio

[![Playwright UI Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-dotnet-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-dotnet-tests.yml)
[![Playwright API Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-api-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-api-tests.yml)
[![Expand Testing API Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-expandtesting-api-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-expandtesting-api-tests.yml)
[![Playwright TS UI Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-ts-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-ts-tests.yml)
[![the-internet UI Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-the-internet-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-the-internet-tests.yml)
[![UI Test Automation Playground](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-uitestingplayground-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-uitestingplayground-tests.yml)
[![Performance Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/performance-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/performance-tests.yml)

Test automation across five stacks — **C#, TypeScript, Java, Postman/Newman and
JMeter/k6** — covering UI, API, contract, GUI-image and performance testing,
with GitHub Actions pipelines on the suites that run unattended.

Roughly **250 tests** in total. Every suite runs against a public demo system, so
anyone can clone it and get the same result.

---

## 📂 What is in here

```text
01.Playwright/     UI, API and hybrid suites - C# and TypeScript
02-Performance-Tests/  JMeter plans and k6 scripts
03-Sikuli/         GUI automation by image recognition
04.Postman/        Newman-runnable collections
05.RestSharp-and-RestAssured/  C# and Java API frameworks
```

### Playwright suites

| # | Project | Stack | Tests | CI | Target |
|:--|:--|:--|:--|:--|:--|
| 01 | [api-testing](./01.Playwright/01-api-testing) | C#, NUnit | 1 | — | Swagger Petstore. A deliberately raw, inline starting point, kept as the "before" half of a before/after comparison with 04. |
| 02 | [typescript-playwright](./01.Playwright/02-typescript-playwright) | TypeScript | 2 | ✅ | Small POM reference project — page classes, centralised selectors, external test data. |
| 03 | [automationexercise-ui-csharp](./01.Playwright/03-automationexercise-ui-csharp) | C#, NUnit, Allure | 26 | ✅ | All 26 official [Automation Exercise](https://automationexercise.com/) test cases. POM, ad/tracker blocking, retry-on-transient-failure. |
| 04 | [automationexercise-api-csharp](./01.Playwright/04-automationexercise-api-csharp) | C#, NUnit | 11 | ✅ | The same site's REST API, browserless via `IAPIRequestContext`. |
| 05 | [practice-expandtesting-api-csharp](./01.Playwright/05-practice-expandtesting-api-csharp) | C#, NUnit, Allure | 18 | ✅ | Notes API — registration, auth and a full note CRUD lifecycle. DTO models, token headers, JSON helpers. |
| 06 | [the-internet-ui-typescript](./01.Playwright/06-the-internet-ui-typescript) | TypeScript | 62 | ✅ | All **44 challenges** on the-internet — iframes, shadow roots, native dialogs, HTTP auth, broken images. |
| 07 | [uitestingplayground-ui-typescript](./01.Playwright/07-uitestingplayground-ui-typescript) | TypeScript | 43 | ✅ | All **29 challenges** on UI Test Automation Playground — randomised ids, overlays, elements that lie about their state. |
| 08 | [restful-booker-platform-api-typescript](./01.Playwright/08-restful-booker-platform-api-typescript) | TypeScript, Zod | 56 | — | Six-service booking platform. API tests, Zod contract validation, and hybrid tests that share one session between HTTP and the browser. |

### Performance

| Project | Tool | CI | Target |
|:--|:--|:--|:--|
| [01-Automation-Exercise](./02-Performance-Tests/01-JMeter/01-Automation-Exercise) | JMeter | ✅ | Parameterised load plan (`-Jusers`, `-Jrampup`, `-Jduration`) |
| [02-BlazeDemo](./02-Performance-Tests/01-JMeter/02-BlazeDemo) | JMeter | ✅ | 20-user booking flow |
| [01-Automation-Exercise](./02-Performance-Tests/02-K6/01-Automation-Exercise) | k6 | ✅ | HTTP smoke load with SLA thresholds |
| [02-QuickPizza](./02-Performance-Tests/02-K6/02-QuickPizza) | k6 | ✅ | Grafana's QuickPizza reference app |

### Other frameworks

| Project | Stack | Tests | Target |
|:--|:--|:--|:--|
| [Postman collections](./04.Postman) | Postman, Newman, JS | 4 suites | iMX Approval Book (CSV-driven XML regression), WEare social network (91 scripts), Automation Exercise (chained E2E signup), Petstore (pre-request data generation) |
| [api-testing-with-restsharp](./05.RestSharp-and-RestAssured/01-api-testing-with-restsharp) | C#, RestSharp, .NET 8, Allure | 14 | Client-based architecture, dynamic form parameters, teardown data cleanup |
| [petstore-rest-assured](./05.RestSharp-and-RestAssured/02-petstore-rest-assured) | Java 17, RestAssured, JUnit 5, Allure | 19 | Decoupled client wrappers, JSON config, thread-safe runtime state, Jackson DTOs |
| [Sikuli GUI automation](./03-Sikuli) | SikuliX, Jython | 1 flow | Image-recognition automation of a full Jira Sandbox signup and bug-reporting flow, including temporary e-mail retrieval |

---

## 🐞 Defects found in the systems under test

Test suites are usually judged by whether they pass. These found real problems in
the applications they point at, and each one is held open as a failing test
rather than deleted or weakened — so the day it is fixed, the suite says so.

**restful-booker-platform** (8 documented defects, see
[its README](./01.Playwright/08-restful-booker-platform-api-typescript)):

* **The message service has no authentication at all.** `/api/message/{id}`
  returns a guest's name, e-mail address, phone number and full enquiry text with
  no token. Other services on the same platform do require one.
* **A room created through the API is never offered to the public.** The page
  requests the full room list, receives the new room, and renders the old three.
  Caching was ruled out with a network trace and five cache-busted reloads.
* **Logout does not end a session.** The endpoint reports success; the token
  keeps working, verified three times against fresh tokens.
* Plus a `500` on an unknown token, a `500` instead of `404`, a creation endpoint
  that returns no id, a branding update that reports success and writes nothing,
  and a reversed date range reported as a booking conflict.

**the-internet**: two server-side defects confirmed and marked as expected
failures — `/forgot_password` returns HTTP 500, and the TinyMCE editor is
read-only.

---

## 🧠 Practices used across the suites

* **Page Object Model** with selectors kept in separate files from actions, so a
  markup change touches one file.
* **Browserless API testing** through Playwright's HTTP client, which keeps API
  pipelines off the browser install entirely.
* **Contract testing** with Zod schemas, mostly `strict`, so a *new* field in a
  response is a reported change rather than a silent one.
* **Session sharing between HTTP and the browser** — logging in once over the API
  and injecting the token, so the login form is tested once instead of before
  every admin test.
* **No hard-coded sleeps.** Waiting is expressed with auto-retrying assertions,
  `expect.poll` or `waitForFunction`.
* **Cleanup armed before the write, not after**, so an assertion that throws
  cannot leak data into a shared public environment.
* **No assertions on totals** in suites that share an environment with other
  users — assertions are scoped to a known id or to a direction of change.
* **Failure diagnostics in CI**: traces, screenshots, videos and TRX/Allure
  reports uploaded as artifacts on every run.

---

## 🚀 Running the suites

### C# (.NET)

```bash
cd 01.Playwright/03-automationexercise-ui-csharp   # or 04-…, 05-…
dotnet build
dotnet test
```

### TypeScript (Playwright)

```bash
cd 01.Playwright/06-the-internet-ui-typescript     # or 02-…, 07-…, 08-…
npm install
npx playwright install chromium
npm test
```

### Java (RestAssured)

```bash
cd 05.RestSharp-and-RestAssured/02-petstore-rest-assured
mvn clean test
```

### Postman (Newman)

```bash
cd 04.Postman/04-petstore
npm install
npm run test
```

### Performance

```bash
# k6
cd 02-Performance-Tests/02-K6/01-Automation-Exercise
k6 run --vus 1 --iterations 1 k6_performance_test.js

# JMeter
jmeter -n -t AutomationExercise_Performance_Test.jmx \
       -Jusers=2 -Jrampup=2 -Jduration=10 -l results.jtl
```

---

## 📊 Artifacts

Every pipeline run publishes downloadable evidence: Playwright traces (timeline,
network log and a screenshot of every step), HTML and Allure reports, TRX result
files, and k6/JMeter response-time percentiles against SLA thresholds.
