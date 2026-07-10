# Swagger Petstore API Testing (JUnit 5 + Rest Assured)

This project is a professional API test automation framework for the [Swagger Petstore v2 API](https://petstore.swagger.io/).

It is converted from a Postman collection and fully implements **Java 17**, **Rest Assured**, **JUnit 5**, and **Allure Reports** under a highly decoupled API Client wrapper pattern.

---

## 📂 Project Architecture

```text
02-petstore-rest-assured
├── pom.xml                        # Maven dependency build configurations
├── README.md                      # Project documentation
├── .gitignore                     # Git ignore file
├── src
│   ├── main
│   │   ├── java
│   │   │   ├── clients            # API Client Wrappers (GET, POST, PUT, DELETE)
│   │   │   │   ├── ApiClient.java
│   │   │   │   ├── PetApiClient.java
│   │   │   │   ├── StoreApiClient.java
│   │   │   │   └── UserApiClient.java
│   │   │   ├── config             # JSON Configuration readers
│   │   │   │   └── ConfigReader.java
│   │   │   ├── constants          # HTTP Headers, Endpoints and expected messages
│   │   │   │   ├── ApiEndpoints.java
│   │   │   │   ├── ExpectedMessages.java
│   │   │   │   └── HttpHeaders.java
│   │   │   ├── helpers            # Dynamic generators and runtime variables state managers
│   │   │   │   ├── RandomDataGenerator.java
│   │   │   │   └── RuntimeVariables.java
│   │   │   └── models             # DTO (Data Transfer Objects) mapping JSON requests/responses
│   │   │       ├── requests
│   │   │       │   ├── PetRequest.java
│   │   │       │   ├── UserRequest.java
│   │   │       │   └── OrderRequest.java
│   │   │       └── responses
│   │   │           └── ApiResponseMessage.java
│   │   └── resources
│   │       ├── appsettings.json         # Global settings (baseUrl, timeout, auth)
│   │       ├── collectionVariables.json # Static variables imported from Postman
│   │       └── logback-test.xml         # SLF4J log mapping configurations
│   └── test
│       ├── java
│       │   ├── base
│       │   │   └── BaseApiTest.java     # Base class instantiating API clients
│       │   └── tests
│       │       ├── pets
│       │       │   └── PetTests.java        # Pet unit tests (CRUD)
│       │       ├── store
│       │       │   └── StoreTests.java      # Store unit tests
│       │       ├── users
│       │       │   └── UserTests.java       # User unit tests (CRUD)
│       │       └── e2e
│       │           └── E2EBookingTests.java # Chained Booking E2E Integration Suite
│       └── resources
│           ├── allure.properties        # Allure path properties
│           └── testdata
│               └── test-data.json       # Additional test data resources
```

---

## ⚙️ Configuration Files

1. **`appsettings.json`**
   - Configures the target `baseUrl` and core timeouts.
2. **`collectionVariables.json`**
   - Holds static fallback collection variables imported from Postman.
3. **`RuntimeVariables.java`**
   - Provides thread-safe, runtime memory storage for dynamically generated parameters (e.g. `petId`, `username`, `orderId`) passed between integration test steps.

---

## 🚀 How to Run Tests Locally

Execute the Maven test suite using your command line:
```bash
mvn clean test
```

---

## 📊 Generating Allure Reports

After running the tests, compile and launch the interactive Allure report portal:
```bash
allure generate target/allure-results --clean -o target/allure-report
allure open target/allure-report
```
*Note: Requires Allure CLI installed on the local system.*
