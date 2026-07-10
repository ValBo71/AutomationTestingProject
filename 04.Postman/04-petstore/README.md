# Swagger Petstore - Dynamic Postman API Tests

This folder contains a complete Postman Collection and Environment for validating the [Swagger Petstore v2 API](https://petstore.swagger.io/).

It features **Pre-request Scripts** on all critical creation endpoints to dynamically generate non-repeating data (users, pets, order IDs, and photo URLs) to ensure tests can be executed repeatedly without causing database key conflicts.

---

## 📂 Files Included

1. **`SwaggerPetstore.postman_collection.json`**
   - The Postman collection containing all Unit APIs and a complete E2E Booking & User Lifecycle integration flow.
   - All dynamic variables are managed via `pm.collectionVariables.set` and stored inside **Collection Variables**.
2. **`SwaggerPetstore.postman_environment.json`**
   - The Postman Environment file containing the `baseUrl` variable.

---

## 📂 Folder Structure in Collection

### 1. **`1. Pet Operations`**
- Unit requests: Add a pet, Find by ID, Update pet, and Delete pet.
- **Dynamic Pre-request Script (Add a pet):**
  - Generates a random `unitPetId` (between 100,000 and 1,100,000).
  - Generates a custom name: `unit_doggie_{{unitPetId}}`.
  - Generates a unique photo URL: `https://example.com/photos/unit_dog_{{unitPetId}}.jpg`.
  - Saves all parameters in collection variables (`unitPetId`, `unitPetName`, `unitPhotoUrl`).

### 2. **`2. Store Operations`**
- Unit requests: Find inventory, Place an order, Find order by ID, and Delete order by ID.
- **Dynamic Pre-request Script (Place an order):**
  - Generates a random `unitOrderId` (between 10,000 and 110,000) and saves it to collection variables (`unitOrderId`).

### 3. **`3. User Operations`**
- Unit requests: Create user, Get by username, Update user, Delete user, Login, and Logout.
- **Dynamic Pre-request Script (Create user):**
  - Generates a unique `unitUsername` using a timestamp suffix (`unit_user_timestamp`).
  - Generates a unique `unitEmail` (`unit_email_timestamp@example.com`).
  - Generates a unique `unitPhone` using a timestamp substring.
  - Saves all parameters to collection variables (`unitUsername`, `unitEmail`, `unitPhone`).

### 4. **`4. E2E Integration Scenario`**
- Chained End-to-End integration test simulating a complete booking flow and user registration/cleanup lifecycle.
- **Dynamic Pre-request Scripts:**
  - **E2E 1: POST Create User:** Generates a unique `username`, `e2eEmail`, and `e2ePhone` via timestamp suffix.
  - **E2E 3: POST Add Pet:** Generates a unique `petId`, `petName`, and `photoUrl` (e.g. `Buddy_petId`).
  - **E2E 5: POST Place Order for Pet:** Generates a unique `orderId`.

---

## ⚙️ Variables Configurations

* **Environment Variable:**
  - `baseUrl`: `https://petstore.swagger.io`
* **Collection Variables:**
  - `username` / `e2eEmail` / `e2ePhone` / `petId` / `petName` / `photoUrl` / `orderId`: Dynamic placeholders for the E2E flow.
  - `unitUsername` / `unitPetId` / `unitPhotoUrl` / `unitOrderId` / `unitEmail` / `unitPassword` / `unitFirstName` / `unitLastName` / `unitPhone`: Dynamic placeholders for the Unit requests.

---

## 🚀 How to Run in Postman UI

1. Open Postman.
2. Click **Import** in the top-left corner.
3. Import both `SwaggerPetstore.postman_collection.json` and `SwaggerPetstore.postman_environment.json`.
4. Select **Swagger Petstore - Env** from the active environment dropdown at the top right.
5. Open the Collection Runner, choose your folder (e.g., `4. E2E Integration Scenario`), and click **Run Swagger Petstore API**.

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
newman run SwaggerPetstore.postman_collection.json -e SwaggerPetstore.postman_environment.json --delay-request 1000
```

