# WEare Social Network API Test Suite (Postman + Newman)

This project contains the automated API regression test suite for the **WEare Social Network** web application. It verifies REST endpoints for user authentication, administrator capabilities, posts, comments, friends, categories, and skills.

---

## 📂 Project Structure

```text
02-weare-social-network/
│
├── WEareSocialNetwork.postman_collection.json  # Postman test collection (91 scripts)
├── PlutoFinalProject.postman_environment.json  # Postman environment variables
├── Run_API_Tests.bat                           # Windows batch file to run tests via Newman
└── newman/                                     # Newman htmlextra generated HTML reports
```

---

## 🛠️ Key Refactoring Fixes

1. **Corrected Chai Assertion Syntaxes**:
   - Replaced invalid Chai syntax `.is.not.empty;` with standard, valid Chai assertions: `.to.not.be.empty;`.
   - Replaced invalid `.is.not.null;` checks with standard Chai assertions: `.to.not.be.null;`.
   - These errors previously caused tests to pass silently without actually asserting properties.
2. **Fixed Text Encoding Errors**:
   - Corrected encoding character issues (e.g. `he Regular user` to `The Regular user`) inside the test result name descriptions.

---

## 🚀 How to Run the Tests

### Option 1: CLI (Newman)
Ensure you have **Node.js** and **Newman** installed globally:
```bash
npm install -g newman newman-reporter-htmlextra
```
Navigate to this directory and run:
```bash
newman run WEareSocialNetwork.postman_collection.json -e PlutoFinalProject.postman_environment.json -r htmlextra
```
This command will execute the suite and generate a rich HTML execution report inside the `newman/` folder.

### Option 2: Windows Batch File
Double-click the **`Run_API_Tests.bat`** script to run the CLI tests automatically and output the html report.

### Option 3: Postman GUI
1. Import `WEareSocialNetwork.postman_collection.json` into Postman.
2. Import `PlutoFinalProject.postman_environment.json` as an environment.
3. Open **Collection Runner** and run the collection.
