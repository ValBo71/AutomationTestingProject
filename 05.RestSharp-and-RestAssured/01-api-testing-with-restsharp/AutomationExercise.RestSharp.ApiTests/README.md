# API Automation Test Framework (RestSharp + C# + NUnit)

This repository contains a robust, highly maintainable, and scalable API automation testing framework built using **C#**, **.NET 8**, **RestSharp**, **NUnit**, and **Allure Reports** targeting the API of [Automation Exercise](https://automationexercise.com).

---

## 📂 Project Structure

```text
AutomationExercise.RestSharp.ApiTests
├── Base
│   └── BaseApiTest.cs         # Base setup/teardown, RestClient initialization
├── Clients
│   ├── ApiClient.cs           # Wrapper around RestClient mapping to Allure attachments
│   ├── ProductsApiClient.cs   # Products endpoint actions
│   ├── BrandsApiClient.cs     # Brands endpoint actions
│   └── AccountApiClient.cs    # Account lifecycle & login verification actions
├── Config
│   ├── appsettings.json       # Base URL, credentials, timeout settings
│   ├── allureConfig.json      # Allure report configuration
│   └── TestSettings.cs        # Strongly-typed configuration binder
├── Constants
│   ├── ApiEndpoints.cs        # Endpoint routing constants
│   └── ExpectedMessages.cs    # Shared message validation strings
├── Helpers
│   ├── AllureHelper.cs        # Request/Response formatters for Allure
│   ├── JsonHelper.cs          # Serialization configurations
│   ├── RandomDataGenerator.cs # Random generator for dynamic test users
│   └── ResponseHelper.cs      # Custom response assertions and deserializers
├── Models
│   ├── Requests               # Strongly-typed request payloads
│   └── Responses              # Strongly-typed response payloads
├── Tests
│   ├── ProductsApiTests.cs    # GET products list, POST products list
│   ├── BrandsApiTests.cs      # GET brands list, PUT brands list
│   ├── SearchProductApiTests.cs # Search product (valid vs missing query)
│   ├── LoginApiTests.cs       # Login validations (positive & negative)
│   └── AccountApiTests.cs     # Full User account registration & lifecycle CRUD
└── AutomationExercise.RestSharp.ApiTests.csproj
```

---

## ⚙️ Key Technical Features

1. **Client-Based Architecture**: Separation of concerns. RestSharp clients (`ProductsApiClient`, `BrandsApiClient`, `AccountApiClient`) execute API requests, returning `RestResponse` objects to test methods.
2. **Form Parameters Handling**: The target API accepts standard form parameters (`application/x-www-form-urlencoded`) for POST, PUT, and DELETE requests. The framework handles this via custom dictionaries mapped to the `AddParameter` RestSharp utility.
3. **Robust Cleanup Hook**: In `AccountApiTests.cs`, any test that registers a user account stores the email inside a cleanup variable. The `[TearDown]` lifecycle hook deletes the created user to keep database states consistent.
4. **Allure Integration**: All HTTP requests, endpoints, methods, headers, payload inputs, and responses are formatted and attached to the Allure execution lifecycle. On failure, error message and stack trace details are attached automatically in `BaseApiTest.cs`.

---

## 🚀 Running the Tests Locally

### 1. Build the Project
Open a command prompt in the project root (`AutomationExercise.RestSharp.ApiTests`) and build:
```bash
dotnet build
```

### 2. Execute Tests
Run all test suites using the .NET Test Runner:
```bash
dotnet test
```

---

## 📊 Allure Report Generation

Once `dotnet test` completes, raw execution data is saved inside the `allure-results` folder. 

To generate and view the report:

1. **Install Allure CLI** (if not installed):
   - **Windows** (via Scoop):
     ```bash
     scoop install allure
     ```
   - **Windows** (via npm):
     ```bash
     npm install -g allure-commandline --save-dev
     ```
2. **Generate the Report**:
   ```bash
   allure generate allure-results --clean -o allure-report
   ```
3. **Open the Report**:
   ```bash
   allure open allure-report
   ```

---

## ☁️ CI/CD Pipeline Steps

This project is fully CI/CD ready. Integrate it into your GitHub Actions, GitLab CI, or Jenkins pipelines using these stages:

```yaml
# Example pipeline stages
stages:
  - restore
  - build
  - test
  - report

# Restore dependencies
dotnet restore

# Compile project
dotnet build --configuration Release --no-restore

# Run tests
dotnet test --configuration Release --no-build --logger "Console;verbosity=detailed"

# Generate report (Allure)
allure generate bin/Release/net8.0/allure-results --clean -o allure-report
```
