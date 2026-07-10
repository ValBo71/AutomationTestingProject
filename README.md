# Automation Testing Project

[![Playwright UI Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-dotnet-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-dotnet-tests.yml)
[![Playwright API Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-api-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-api-tests.yml)
[![Expand Testing API Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-expandtesting-api-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-expandtesting-api-tests.yml)
[![Playwright TS UI Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-ts-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-ts-tests.yml)
[![Performance Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/performance-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/performance-tests.yml)

A professional repository containing automated testing solutions.

## 🚀 CI/CD Pipelines (GitHub Actions)

This repository includes professional continuous integration (CI) pipelines implemented via GitHub Actions to validate both UI and API test suites automatically.

### 1. UI Test Pipeline (Playwright .NET Tests)
* **Target Project:** `01.Playwright/03-automationexercise-ui-csharp/AutomationExercise.sln`
* **Trigger Events:**
  * Every `push` to the `main` branch.
  * Every `pull_request` targeting the `main` branch.
  * Scheduled run: **Every day at 07:00 AM Bulgarian time** (04:00 UTC).
  * Manual execution via the GitHub UI (`workflow_dispatch`).
* **Features:** Env-override for Headless mode, system dependency auto-install, TRX test results export, failure screenshot capture, Allure results archiving, and a ready-to-run email dispatch script to send diagnostics directly to `vbogdanov.test@abv.bg`.

### 2. API Test Pipeline (Playwright API Tests)
* **Target Project:** `01.Playwright/04-automationexercise-api-csharp/AutomationExercise.ApiTests.sln`
* **Trigger Events:**
  * Every `push` to the `main` branch.
  * Every `pull_request` targeting the `main` branch.
  * Scheduled run: **Every day at 07:00 AM Bulgarian time** (04:00 UTC).
  * Manual execution via the GitHub UI (`workflow_dispatch`).
* **Features:** Lightweight running without full browser binary downloads (uses Playwright API Request Context), TRX results export, Allure results archiving, and a ready-to-run email dispatch script to send diagnostics directly to `vbogdanov.test@abv.bg`.

### 3. Expand Testing API Test Pipeline (Playwright Notes API Tests)
* **Target Project:** `01.Playwright/05-practice-expandtesting-apo-csharp/ApiTests.sln`
* **Trigger Events:**
  * Every `push` to the `main` branch.
  * Every `pull_request` targeting the `main` branch.
  * Scheduled run: **Every day at 07:00 AM Bulgarian time** (04:00 UTC).
  * Manual execution via the GitHub UI (`workflow_dispatch`).
* **Features:** Uses Playwright API Request Context, TRX results export, Allure results archiving, and email body preparation template.

### 4. TypeScript UI Test Pipeline (Playwright TS Tests)
* **Target Project:** `01.Playwright/02-typescript-playwright/TS-Playwright-Project`
* **Trigger Events:**
  * Every `push` to the `main` branch.
  * Every `pull_request` targeting the `main` branch.
  * Scheduled run: **Every day at 07:00 AM Bulgarian time** (04:00 UTC).
  * Manual execution via the GitHub UI (`workflow_dispatch`).
* **Features:** OS dependencies auto-install, Node.js environment configuration, and Playwright HTML test report archiving.

### 5. Performance & Load Test Pipeline (JMeter & k6)
* **Target Project:** `02-Performance-Tests/`
* **Trigger Events:**
  * Every `push` to the `main` branch.
  * Every `pull_request` targeting the `main` branch.
  * Scheduled run: **Every day at 07:00 AM Bulgarian time** (04:00 UTC).
  * Manual execution via the GitHub UI (`workflow_dispatch`).
* **Features:** Automatic Java configuration, Apache JMeter installation, Grafana k6 setup, and execution of concurrent thread/VU load smoke tests.

---

## 📂 Projects in this Repository

### 1. C# API Testing (Swagger Petstore)
* **Path:** [01.Playwright/01-api-testing](file:///E:/Programing/My_project/GitHub/AutomationTestingProject/01.Playwright/01-api-testing)
* **Goal:** Playwright API tests in C# validating the Swagger Petstore API endpoints, complete with Postman collections.

### 2. TypeScript + Playwright Project
* **Path:** [01.Playwright/02-typescript-playwright](file:///E:/Programing/My_project/GitHub/AutomationTestingProject/01.Playwright/02-typescript-playwright)
* **Goal:** TypeScript-based automation project using the Playwright framework with standard Page Object Model patterns.

### 3. C# UI Automation (Automation Exercise)
* **Path:** [01.Playwright/03-automationexercise-ui-csharp](file:///E:/Programing/My_project/GitHub/AutomationTestingProject/01.Playwright/03-automationexercise-ui-csharp)
* **Goal:** UI automation of [Automation Exercise](https://automationexercise.com/), achieving 100% test coverage of all 26 official test cases.

### 4. C# API Automation (Automation Exercise)
* **Path:** [01.Playwright/04-automationexercise-api-csharp](file:///E:/Programing/My_project/GitHub/AutomationTestingProject/01.Playwright/04-automationexercise-api-csharp)
* **Goal:** Backend API automation using Playwright `IAPIRequestContext` to validate web endpoints.

### 5. C# API Automation (Expand Testing Notes API)
* **Path:** [01.Playwright/05-practice-expandtesting-apo-csharp](file:///E:/Programing/My_project/GitHub/AutomationTestingProject/01.Playwright/05-practice-expandtesting-apo-csharp)
* **Goal:** Backend API automation using Playwright `IAPIRequestContext` to validate Expand Testing Notes API endpoints, executing 21 comprehensive test scenarios.

### 6. Performance & Load Testing (JMeter & k6)
* **Path:** [02-Performance-Tests](file:///E:/Programing/My_project/GitHub/AutomationTestingProject/02-Performance-Tests)
* **Goal:** API load and performance test suites built with **Apache JMeter** (.jmx Test Plans) and **Grafana k6** (JavaScript scripts) validating target endpoints under simulated concurrent workloads.