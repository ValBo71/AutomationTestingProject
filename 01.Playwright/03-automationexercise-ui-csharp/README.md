# Automation Exercise - Playwright & NUnit Test Suite

[![Playwright .NET Tests](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-dotnet-tests.yml/badge.svg)](https://github.com/ValBo71/AutomationTestingProject/actions/workflows/playwright-dotnet-tests.yml)

This repository contains a maintainable Playwright test framework using the Page Object Model targeting [Automation Exercise](https://automationexercise.com/), covering all 26 official test scenarios. 


The tests are written in C# utilizing **Playwright** for fast and reliable browser automation and **NUnit** as the test framework.

---

## 🛠️ Technology Stack & Features

* **Language/Framework:** C# (.NET 9.0)
* **Automation Library:** Playwright (Headed/Headless mode support)
* **Test runner:** NUnit 3.14
* **Design Pattern:** Page Object Model (POM) for clean, maintainable, and reusable page interactions
* **Reporting:** Allure Reports integration with detailed step-by-step logs and screenshots on failure
* **Resiliency Features:** navigation is retried when the site answers with its "heavy load (queue full)" page
  (`Infrastructure/SiteAvailability.cs`), and ad, consent-overlay and tracker hosts are blocked by host name
  (`Drivers/PlaywrightDriver.cs`).
* **`[Retry(2)]` on every test:** the target is a busy public demo site, so a test gets up to two more attempts.
  A retry can hide a genuinely flaky test, which is why a failed attempt keeps its own screenshot and trace
  (`TestResults/traces/<test>_attempt<n>.zip`) instead of being overwritten by the next one.
* **No accounts left behind:** every email a test generates is tracked; `BaseTest.TearDown` deletes any account
  still on the site through the site's API and confirms it is gone, including when a test fails part-way.
  A leftover is reported as a test warning with its email and one-off password.

---

## 📂 Project Structure

```text
03-automationexercise-ui-csharp/
│
├── AutomationExercise.sln           # Visual Studio Solution File
├── .gitignore                       # Git ignore rules (bin, obj, TestResults, allure, etc. are ignored)
├── README.md                        # Project documentation
│
└── AutomationExercise.Tests/        # Main Test Project
    ├── Base/
    │   └── BaseTest.cs              # Base setup/teardown (browser initialization, cookies, cleanup)
    ├── Config/
    │   ├── appsettings.json         # Browser, Headless, and SlowMo configurations
    │   └── allureConfig.json        # Allure results folder paths
    ├── Drivers/
    │   └── PlaywrightDriver.cs      # Browser context, block ads/analytics trackers setup
    ├── Helpers/
    │   ├── AllureHelper.cs          # Helper to wrap Playwright steps into Allure steps
    │   ├── RandomDataGenerator.cs   # Unique emails/names; every email is tracked for cleanup
    │   └── ScreenshotHelper.cs      # Failure screenshots into TestResults/Screenshots
    ├── Infrastructure/
    │   ├── AccountCleanup.cs        # Deletes leftover test accounts through the API
    │   ├── SiteAvailability.cs      # Navigation retry when the site reports heavy load
    │   └── TestLog.cs               # Test log output
    ├── Pages/                       # Page Object classes (POM)
    │   ├── HomePage.cs
    │   ├── LoginPage.cs
    │   ├── SignupPage.cs
    │   └── ...
    ├── Selectors/                   # Centralized selectors to prevent duplication
    │   └── CommonSelectors.cs
    ├── TestData/                    # Static/dynamic mock test data templates
    └── Tests/                       # Test suites grouped by feature
        ├── AccountTests.cs
        ├── CartTests.cs
        ├── CheckoutTests.cs
        ├── ProductTests.cs
        └── ...
```

---

## ⚙️ Configuration

Configurations can be changed in [appsettings.json](./AutomationExercise.Tests/Config/appsettings.json):

```json
{
  "TestSettings": {
    "BaseUrl": "https://automationexercise.com",
    "Browser": "Chromium",
    "Headless": false,
    "TimeoutSeconds": 30,
    "SlowMoMs": 500
  }
}
```

* **`Headless`**: Set to `true` to run tests silently in the background, or `false` to see the browser window.
* **`SlowMoMs`**: Adds a delay in milliseconds between actions. Set to `500` to easily observe button clicks and typing actions visually.
* **`TimeoutSeconds`**: Default timeout for Playwright actions and waits.

CI overrides `Headless` and `SlowMoMs` with the `HEADLESS` and `SLOW_MO_MS` environment variables.

---

## 🚀 Running the Tests

Ensure you have the [.NET 9.0 SDK](https://dotnet.microsoft.com/download) installed.

### 1. Restore & Build the project
```bash
dotnet build
```

### 2. Run the complete suite
```bash
dotnet test
```

### 3. Run specific test case(s)
To run a specific test by name:
```bash
dotnet test --filter "Name~PlaceOrder_RegisterWhileCheckout"
```

---

## 📊 Allure Reporting

The test suite generates Allure-compatible XML/JSON results automatically on execution.

### Prerequisites
Install the Allure command-line tool globally via npm:
```bash
npm install -g allure-commandline
```

### Generate & View Report

1. Navigate to the compilation output folder:
   ```bash
   cd AutomationExercise.Tests/bin/Debug/net9.0
   ```

2. Compile the XML results into an HTML report:
   ```bash
   allure generate allure-results --clean -o allure-report
   ```

3. Start a local server to view the dashboard in your browser:
   ```bash
   allure open allure-report
   ```
