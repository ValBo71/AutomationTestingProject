# Automation Testing Portfolio

[![Playwright UI Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-dotnet-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-dotnet-tests.yml)
[![Playwright API Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-api-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-api-tests.yml)
[![Expand Testing API Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-expandtesting-api-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-expandtesting-api-tests.yml)
[![Playwright TS UI Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-ts-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-ts-tests.yml)
[![Performance Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/performance-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/performance-tests.yml)

A professional repository containing automated testing solutions. It demonstrates modern web, API, and performance testing frameworks along with continuous integration (CI) pipelines using industry-standard tools and design patterns.

---

## 📂 Project Overview

This repository acts as a showcase of senior-level test automation concepts, structured into multiple sub-projects that target different layers of web applications. It serves to highlight maintainability, clean code patterns, robust error handling, and automation architecture suitable for production deployment.

### Projects Table

| Project | Tech Stack | CI/CD | Purpose |
| :--- | :--- | :--- | :--- |
| **Automation Exercise UI** | C#, Playwright, NUnit, Allure | Yes | UI automation using Page Object Model (POM) pattern. |
| **Automation Exercise API** | C#, Playwright API, NUnit | Yes | API automation tests for public REST endpoints. |
| **Expand Testing Notes API** | C#, Playwright API, NUnit | Yes | CRUD and authentication-focused API test scenarios. |
| **TypeScript Playwright UI** | TypeScript, Playwright | Yes | UI automation examples using Playwright TypeScript. |
| **Performance Tests** | JMeter, k6 | Yes | Performance and load smoke testing suitable for CI pipelines. |

---

## 📁 Repository Structure

### 1. C# UI Automation (Automation Exercise)
* **Path:** [01.Playwright/03-automationexercise-ui-csharp](./01.Playwright/03-automationexercise-ui-csharp)
* **Description:** UI automation suite for [Automation Exercise](https://automationexercise.com/), covering all 26 official test cases. Utilizes the Page Object Model (POM) pattern, custom ad-blocking rules, and overload-reload mitigations to keep test runs clean and reliable on public staging websites.

### 2. C# API Automation (Automation Exercise)
* **Path:** [01.Playwright/04-automationexercise-api-csharp](./01.Playwright/04-automationexercise-api-csharp)
* **Description:** Backend API validation using Playwright `IAPIRequestContext` to perform lightweight, browserless HTTP assertions against REST endpoints.

### 3. C# API Automation (Expand Testing Notes API)
* **Path:** [01.Playwright/05-practice-expandtesting-api-csharp](./01.Playwright/05-practice-expandtesting-api-csharp)
* **Description:** API test suite covering authentication, user registration, and a complete note CRUD lifecycle (21 test scenarios) against the Notes API. Features DTO models, custom token authorization headers, and JSON serialization helpers.

### 4. TypeScript UI Automation (Playwright TS)
* **Path:** [01.Playwright/02-typescript-playwright](./01.Playwright/02-typescript-playwright)
* **Description:** UI automation project using Playwright in TypeScript, demonstrating selectors centralization, test data parametrization, and Page Object Model patterns.

### 5. Performance & Load Testing (JMeter & k6)
* **Path:** [02-Performance-Tests](./02-Performance-Tests)
* **Description:** API load validation scripts implemented in both **Apache JMeter** (.jmx plans) and **Grafana k6** (JavaScript scripts) verifying HTTP endpoints under simulated concurrent user traffic.

### 6. C# API Testing (Swagger Petstore)
* **Path:** [01.Playwright/01-api-testing](./01.Playwright/01-api-testing)
* **Description:** Simple Playwright API verification tests in C# validating the Swagger Petstore API endpoints.

---

## 🧠 Senior QA Automation Concepts Demonstrated

* **Page Object Model (POM):** Clean separation of UI selectors/actions from test verification logic.
* **API Testing Isolation:** Browserless API test execution using Playwright's native HTTP client (`IAPIRequestContext`), minimizing pipeline resource usage.
* **Dynamic Test Data:** Parameterized tests and run-time data generation (using random email generators and mock utilities) preventing shared-state test pollution.
* **CI/CD Optimization:** Automated workflow runs configured for headless execution, automated browser installations, dependency caching, and clean execution steps.
* **Node 24 Forcing:** Use of `FORCE_JAVASCRIPT_ACTIONS_TO_NODE24: true` across all pipelines to ensure compatibility with modern GitHub runners.
* **Test Failure Diagnostics:** Archiving failed test traces, logs, and screenshots within CI artifacts to ensure fast debugging.
* **Performance Smoke Tests:** Lightweight load tests embedded directly inside CI/CD to detect performance regressions early.
* **Ad & Tracker Blocking:** Context-level request routing in Playwright to intercept and abort third-party analytics and advertisements, shortening test runs.

---

## 📋 Test Strategy

* **UI Regression & Smoke Testing:** Verifies end-to-end critical user flows (cart checkout, signup, login, catalog navigation). Focuses on visual consistency and page state transitions.
* **API Validation:** Validates API contracts, status codes, correct serialization of JSON parameters (e.g. lowercase boolean formatting), conflict states (409 Conflict), and unauthorized access protection.
* **Performance Validation in CI:** Runs low-load smoke performance scripts in the CI runner. This validates that the performance test scripts themselves remain functional, compile successfully, and that the APIs meet basic SLA thresholds under light load.
* **Resiliency against Public Environments:** Public demo websites can suffer from network latency and database lockups. The C# UI framework implements custom retry loops, wait conditions, and automatic page-reload logic upon transient network failures.

---

## 🚀 How to Run the Tests

### UI and API Tests (C# .NET)
1. Navigate to the desired project folder:
   * **UI Tests:** `cd 01.Playwright/03-automationexercise-ui-csharp`
   * **API Tests:** `cd 01.Playwright/04-automationexercise-api-csharp`
   * **Notes API:** `cd 01.Playwright/05-practice-expandtesting-api-csharp`
2. Build the project:
   ```bash
   dotnet build
   ```
3. Run tests locally:
   ```bash
   dotnet test
   ```

### TypeScript Tests
1. Navigate to: `cd 01.Playwright/02-typescript-playwright/TS-Playwright-Project`
2. Install dependencies:
   ```bash
   npm install
   npx playwright install
   ```
3. Run tests:
   ```bash
   npx playwright test
   ```

### Performance Tests
* **k6:**
  ```bash
  cd 02-Performance-Tests/02-K6
  k6 run --vus 1 --iterations 1 k6_performance_test.js
  ```
* **JMeter:** Open `02-Performance-Tests/01-JMeter/01-Automation-Exercise/AutomationExercise_Performance_Test.jmx` via the JMeter GUI or run in CLI mode:
  ```bash
  jmeter -n -t AutomationExercise_Performance_Test.jmx -Jusers=2 -Jrampup=2 -Jduration=10 -l results.jtl
  ```

---

## 📊 Artifacts and Reports

Every pipeline run produces rich diagnostic evidence available for download:
* **TRX Files:** Standard XML-formatted unit test execution results (easily parsed by CI systems).
* **Allure Reports:** Interactive HTML reports containing detailed step-by-step logs, durations, and system environment info.
* **Traces & Screenshots:** Playwright trace zip files showing timeline, network logs, and screenshots of every single step leading to test failure.
* **JMeter & k6 Summaries:** Raw execution statistics, response times percentiles, and SLA threshold verification results.

---

## ⚙️ GitHub Repository Configuration (Manual Steps)

To maximize the discoverability and presentation of this portfolio on GitHub, configure the following settings in your repository UI:

* **Description:**
  > QA Automation portfolio with Playwright, C#, NUnit, TypeScript, API testing, performance testing, Allure reporting and GitHub Actions CI/CD.
* **Topics:**
  `playwright`, `csharp`, `nunit`, `qa-automation`, `sdet`, `api-testing`, `ui-testing`, `github-actions`, `allure-report`, `jmeter`, `k6`, `typescript`, `performance-testing`