# Automation Exercise - Postman API Tests

This folder contains a complete Postman Collection and Environment for validating the APIs on [Automation Exercise](https://automationexercise.com/).

It covers all 14 official API testing scenarios organized in a structured, senior-level presentation.

---

## 📂 Folder Structure in Collection

### 1. **`1. Official API Scenarios`**
- Individual requests representing **API 1 to API 14** as defined on the website.
- Used for unit-level assertions of status codes (200 OK) and internal response codes (400, 404, 405, 201, 200).

### 2. **`2. E2E Integration Scenario`**
- A chained End-to-End integration test validating the entire catalogue search and user lifecycle sequentially.
- **Workflow Steps:**
  1. **E2E 1: GET Products List:** Retrieves all products and extracts the name of the first product dynamically, saving it in the environment variable `{{searchedProduct}}`.
  2. **E2E 2: POST Search Product:** Uses `{{searchedProduct}}` to query the search endpoint and validates that the correct item is found in the returned array.
  3. **E2E 3: POST Verify Login (Unregistered Check):** Attempts to log in with an unregistered email, asserting that it returns `404 User not found!`.
  4. **E2E 4: POST Create Account:** Generates a unique email dynamically in a **Pre-request Script** (`e2e_postman_timestamp@example.com`), registers the account, and saves it in `{{registerEmail}}`.
  5. **E2E 5: POST Verify Login (Registered Check):** Logs in with the newly registered user and asserts that the API returns `200 User exists!`.
  6. **E2E 6: GET User Details:** Fetches details of the registered user and verifies that the retrieved email and name match the registration values.
  7. **E2E 7: PUT Update User Account:** Updates the user's name to `"Updated E2E Postman User"` and phone to `"0987654321"`.
  8. **E2E 8: DELETE Account:** Deletes the user account and asserts that the API returns `200 Account deleted!`.
  9. **E2E 9: GET User Details (Deleted Check):** Attempts to fetch details of the deleted user, asserting that the API returns `404 User not found!` (inner response code `404`).

---

## ⚙️ Environment Variables

The environment file defines:
* `baseUrl`: `https://automationexercise.com`
* `email`: Default email for static login tests.
* `password`: Default password for static login tests.
* `registerEmail`: Dynamic placeholder updated during registration.
* `searchedProduct`: Dynamic placeholder updated during product list extraction.

---

## 🚀 How to Run in Postman UI

1. Open Postman.
2. Click **Import** in the top-left corner.
3. Drag and drop `AutomationExercise.postman_collection.json` and `AutomationExercise.postman_environment.json`.
4. Select **Automation Exercise - Local Env** from the active environment dropdown at the top right.
5. Right-click on the collection, select **Run collection**, choose your folders/requests, and click **Run Automation Exercise API**.

---

## 💻 How to Run via Newman (CLI)

### Option 1: Using local package scripts (Recommended)
1. Install dependencies locally:
   ```bash
   npm install
   ```
2. Run tests:
   ```bash
   npm run test
   ```
   *Or double-click the **`Run_API_Tests.bat`** file.*

### Option 2: Using global Newman
Ensure you have Newman installed globally:
```bash
npm install -g newman
```
To run the entire collection:
```bash
newman run AutomationExercise.postman_collection.json -e AutomationExercise.postman_environment.json --delay-request 1000
```
*(Recommended `--delay-request 1000` to prevent public server rate-limiting).*

