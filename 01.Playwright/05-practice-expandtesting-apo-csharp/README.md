# Expand Testing - Notes API Automation (C# + Playwright + NUnit)

This project contains a professional API automation framework written in C# using NUnit and Playwright to validate the **Notes API** from `practice.expandtesting.com`.

---

## 🛠️ Project Structure
```text
ApiTests
├── Base
│   └── BaseApiTest.cs
├── Clients
│   ├── ApiClient.cs
│   └── UsersApiClient.cs
│   └── NotesApiClient.cs
├── Config
│   ├── appsettings.json
│   └── allureConfig.json
├── Constants
│   ├── ApiEndpoints.cs
│   └── ExpectedMessages.cs
├── Helpers
│   ├── AllureHelper.cs
│   ├── JsonHelper.cs
│   ├── RandomDataGenerator.cs
│   └── ResponseHelper.cs
├── Models
│   └── Responses
│       ├── GenericResponse.cs
│       ├── LoginResponse.cs
│       ├── ProfileResponse.cs
│       ├── NoteResponse.cs
│       └── NotesListResponse.cs
└── Tests
    ├── UsersApiTests.cs
    └── NotesApiTests.cs
```

---

## 🚀 Running Tests Locally

### Prerequisites
* .NET SDK (9.0 or 8.0)
* Allure command-line tool (optional, for report generation)

### Steps
1. Navigate to the project folder:
   ```bash
   cd 01.Playwright/05-practice-expandtesting-apo-csharp/ApiTests
   ```
2. Build the project:
   ```bash
   dotnet build
   ```
3. Run the tests:
   ```bash
   dotnet test
   ```

### 📊 Generate Allure Reports
1. Install Allure command line tool (if not already installed).
2. Generate the report:
   ```bash
   allure generate bin/Debug/net9.0/allure-results --clean -o allure-report
   ```
3. Open the report:
   ```bash
   allure open allure-report
   ```

---

## 📡 List of Automated Endpoints

| Tag | HTTP Method | Endpoint | Description |
|---|---|---|---|
| **Users** | `POST` | `/users/register` | Register a new user account |
| **Users** | `POST` | `/users/login` | Log in as an existing user & get access token |
| **Users** | `GET` | `/users/profile` | Retrieve user profile information |
| **Users** | `PATCH` | `/users/profile` | Update user profile information |
| **Users** | `DELETE` | `/users/logout` | Log out the user |
| **Users** | `DELETE` | `/users/delete-account` | Delete user account |
| **Notes** | `POST` | `/notes` | Create a new note |
| **Notes** | `GET` | `/notes` | Get all notes for authenticated user |
| **Notes** | `GET` | `/notes/{id}` | Get a note by ID |
| **Notes** | `PUT` | `/notes/{id}` | Update an existing note |
| **Notes** | `PATCH` | `/notes/{id}` | Update completed status of a note |
| **Notes** | `DELETE` | `/notes/{id}` | Delete a note by ID |

---

## ⚠️ Endpoints that Cannot be Automated (Limitations)
* **`/users/forgot-password` / `/users/verify-reset-password-token` / `/users/reset-password`:** 
  These endpoints require retrieving a password reset link/token sent via a real email address. In a headless automated test context, there is no email access configured to intercept the email link dynamically, so these flows are skipped for automation.
