# TS-Playwright-Project

This is an automation project built with Playwright and TypeScript, based on the Page Object Model (POM) pattern.

## Project Structure

The project follows these best practices:
- **POM**: There is a separate Page class for each page (in `pages/`), containing only methods (actions).
- **Selectors**: Centralized and stored in separate files (in `selectors/`).
- **Test Data**: Centralized and stored in external files (in `data/`).
- **Assertions**: Playwright `expect` is used only within the tests themselves.

## Installation

After initializing the project, install the required dependencies:

```bash
npm install
npx playwright install
```

## Running Tests

To run all tests in **headless** mode:

```bash
npx playwright test
```

To run all tests in **headed** mode:

```bash
npx playwright test --headed
```

Or you can use the NPM scripts:

```bash
npm test
npm run test:headed
```
