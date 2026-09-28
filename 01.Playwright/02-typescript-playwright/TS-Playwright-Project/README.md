# TS-Playwright-Project

This is an automation project built with Playwright and TypeScript, based on the Page Object Model (POM) pattern.
It targets [saucedemo.com](https://www.saucedemo.com), which ships fixed demo users, so the tests create no data.

## Project Structure

The project follows these best practices:
- **POM**: There is a separate Page class for each page (in `pages/`: `LoginPage`, `InventoryPage`). Page classes
  hold actions and expose locators; they contain no assertions.
- **Selectors**: Centralized and stored in separate files (in `selectors/`).
- **Test Data**: Centralized and stored in external files (in `data/`).
- **Assertions**: Playwright `expect` is used only within the tests themselves, always as web-first assertions
  (`toHaveText`, `toBeVisible`, `toHaveURL`) so they retry until the page catches up.

## Installation

After initializing the project, install the required dependencies:

```bash
npm install
npx playwright install chromium
```

## Running Tests

Tests run **headless** by default, locally and in CI:

```bash
npm test
```

To watch them in a browser window, optionally slowed down (milliseconds per action):

```bash
npm run test:headed
SLOW_MO=500 npm run test:headed
```

Type-check the project (CI runs this before the tests, since Playwright itself only transpiles TypeScript):

```bash
npm run typecheck
```

## CI

The `Playwright TypeScript UI Tests` workflow runs on every push, pull request and nightly. In CI a failed test is
retried up to twice, but a test that only passes on a retry is reported as flaky and **fails the run**
(`failOnFlakyTests`), so flakiness is visible instead of hidden behind a green build.
