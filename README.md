# Automation Testing Project

[![Playwright UI Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-dotnet-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-dotnet-tests.yml)
[![Playwright API Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-api-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-api-tests.yml)

A professional repository containing automated testing solutions.

## 🚀 CI/CD Pipelines (GitHub Actions)

This repository includes professional continuous integration (CI) pipelines implemented via GitHub Actions to validate both UI and API test suites automatically.

### 1. UI Test Pipeline (Playwright .NET Tests)
* **Target Project:** `01.Playwright/3.AutomationАtАutomationexercise.com/AutomationExercise.sln`
* **Trigger Events:**
  * Every `push` to the `main` branch.
  * Every `pull_request` targeting the `main` branch.
  * Scheduled run: **Every day at 07:00 AM Bulgarian time** (04:00 UTC).
  * Manual execution via the GitHub UI (`workflow_dispatch`).
* **Features:** Env-override for Headless mode, system dependency auto-install, TRX test results export, failure screenshot capture, Allure results archiving, and instant email reports with test failure diagnostics sent to `vbogdanov.test@abv.bg`.

### 2. API Test Pipeline (Playwright API Tests)
* **Target Project:** `01.Playwright/4.APITestingОnАutomationexercise.com/AutomationExercise.ApiTests.sln`
* **Trigger Events:**
  * Every `push` to the `main` branch.
  * Every `pull_request` targeting the `main` branch.
  * Scheduled run: **Every day at 07:30 AM Bulgarian time** (04:30 UTC).
  * Manual execution via the GitHub UI (`workflow_dispatch`).
* **Features:** Lightweight running without full browser binary downloads (uses Playwright API Request Context), TRX results export, Allure results archiving, and instant email reports with test failure diagnostics sent to `vbogdanov.test@abv.bg`.

### 📊 Email Notifications & Failure Diagnostics
Both pipelines are configured to parse test execution results (.trx files) using a Python utility. After each run, a summary email containing execution statistics and detailed logs for failed tests is dispatched to `vbogdanov.test@abv.bg`.

### QA Automation / SDET Portfolio Showcase
This setup demonstrates:
* **CI/CD Best Practices:** Infrastructure-as-code automation using YAML workflows.
* **Hermetic & Headless Execution:** Separating local debugging configs from high-performance headless CI runs.
* **Proactive Monitoring:** Direct notification system containing exact errors to minimize MTTR (Mean Time to Resolution).

---

## 📂 Projects in this Repository

### 1. Playwright + C# UI Test Suite
* **Path:** [01.Playwright/3.AutomationАtАutomationexercise.com](file:///E:/Programing/My_project/GitHub/AutomationTestingProject/01.Playwright/3.Automation%D0%90t%D0%90utomationexercise.com)
* **Goal:** UI automation of [Automation Exercise](https://automationexercise.com/), achieving 100% test coverage of all 26 official test cases.
* See the [project README](file:///E:/Programing/My_project/GitHub/AutomationTestingProject/01.Playwright/3.Automation%D0%90t%D0%90utomationexercise.com/README.md) for details.

### 2. Playwright + C# API Test Suite
* **Path:** [01.Playwright/4.APITestingОnАutomationexercise.com](file:///E:/Programing/My_project/GitHub/AutomationTestingProject/01.Playwright/4.APITesting%D0%9En%D0%90utomationexercise.com)
* **Goal:** Backend API automation using Playwright `IAPIRequestContext` to validate web endpoints.