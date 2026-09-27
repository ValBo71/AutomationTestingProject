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
   cd 01.Playwright/05-practice-expandtesting-api-csharp/ApiTests
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

## 🧪 Automated Test Scenarios (Total: 23)

This framework executes 23 distinct automated checks covering positive, negative, parameterized, and integration flows:

### 👤 Users Module
1. `RegisterUser_WithValidData_ShouldCreateAccount` - Positive user registration.
2. `RegisterUser_WithDuplicateEmail_ShouldReturnConflict` - A second registration with the same email is refused with 409 Conflict (the first one is asserted to succeed).
3. `RegisterUser_WithoutName_ShouldReturnBadRequest` - Missing required fields registration error (400 Bad Request).
4. `LoginUser_WithValidCredentials_ShouldReturnToken` - Successful login and token retrieval.
5. `LoginUser_WithInvalidCredentials_ShouldReturnUnauthorized` - Login with an email no account has (401, "Incorrect email address or password").
6. `GetProfile_WithValidToken_ShouldReturnProfile` - Retrieve profile info for authenticated user.
7. `GetProfile_WithoutToken_ShouldReturnUnauthorized` - Profile access without token (401 Unauthorized).
8. `UpdateProfile_WithValidData_ShouldUpdateProfile` - Update user names, phone, and company details.
9. `ChangePassword_WithValidCredentials_ShouldChangePassword` - Changes the password, then proves the new one logs in and the old one is refused.
10. `ChangePassword_WithInvalidCurrentPassword_ShouldReturnBadRequest` - Password update with incorrect current credentials.
11. `Logout_WithValidToken_ShouldInvalidateToken` - Logs out, then proves the same token is refused (401).
12. `DeleteAccount_WithValidToken_ShouldRemoveAccount` - Deletes the account, then proves it can no longer log in.

### 📝 Notes Module
13. `NoteCRUDLifecycle_ShouldSucceed` - Full CRUD lifecycle integration test (Create -> Get by ID -> Update Details PUT -> read back -> Complete Status PATCH -> read back -> Delete Note -> Verify 404).
14-16. `CreateNote_WithValidCategories_ShouldCreateNote` - Parameterized checks for note creation under `Home`, `Work`, and `Personal` categories.
17. `CreateNote_WithInvalidCategory_ShouldReturnBadRequest` - Invalid category format validation (400 Bad Request).
18. `CreateNote_WithMissingTitle_ShouldReturnBadRequest` - Empty note title validation.
19. `CreateNote_WithoutToken_ShouldReturnUnauthorized` - Create note attempt without token (401 Unauthorized).
20. `GetAllNotes_ShouldReturnList` - A new user's list holds exactly the two notes it created, with their content.
21. `GetNoteById_WithNonExistentId_ShouldReturnNotFound` - Note fetch with invalid/non-existent ID (400/404).
22. `GetNoteById_WithInvalidIdFormat_ShouldReturnBadRequest` - Note fetch with malformed ID string.
23. `DeleteNote_WithNonExistentId_ShouldReturnNotFound` - Note deletion with non-existent ID.

### 🧹 Test accounts on the public sandbox
Every test that creates an account hands it to `BaseApiTest` the moment it is registered, and the base
`TearDown` logs in afresh and deletes it - whether the test passed, failed, cleared its token or changed
its password. If that deletion fails, the test is marked with a warning naming the account, so a leak is
visible in the results rather than silent.

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
