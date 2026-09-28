import { test, expect } from '@playwright/test';
import { LoginPage } from '../pages/LoginPage';
import { InventoryPage } from '../pages/InventoryPage';
import { TestData } from '../data/testData';

test.describe('Login functionality - Smoke Tests', () => {
  let loginPage: LoginPage;
  let inventoryPage: InventoryPage;

  test.beforeEach(async ({ page }) => {
    loginPage = new LoginPage(page);
    inventoryPage = new InventoryPage(page);
  });

  test('Valid user should login successfully', async ({ page }) => {
    // 1. Open https://www.saucedemo.com/
    await loginPage.open();

    // Verify the logo on the first page (Login)
    await expect(loginPage.loginLogo()).toBeVisible();
    await expect(loginPage.loginLogo()).toHaveText(TestData.expected.appTitle);

    // 2. Enter username
    // 3. Enter password
    // 4. Click the Login button
    await loginPage.login(
      TestData.credentials.standardUser,
      TestData.credentials.password
    );

    // 5. Validate successful login:
    // URL contains `/inventory.html`
    await expect(page).toHaveURL(/.*inventory\.html/);

    // Verify an element from the inventory page is present
    await expect(inventoryPage.inventoryContainer()).toBeVisible();

    // Verify the Swag Labs label
    await expect(inventoryPage.appLogo()).toBeVisible();
    await expect(inventoryPage.appLogo()).toHaveText(TestData.expected.appTitle);
  });

  test('Locked out user should see an error and stay on the login page', async ({ page }) => {
    await loginPage.open();

    await loginPage.login(
      TestData.credentials.lockedOutUser,
      TestData.credentials.password
    );

    // The error first: it is the page's answer to the click, so once it is shown the login attempt has
    // been handled. Asserted with toHaveText, which retries until the banner renders.
    await expect(loginPage.errorMessage()).toHaveText(TestData.expected.lockedOutError);

    // Then "stayed on the login page", stated positively. A "not on /inventory" check passes on its
    // first attempt - before any redirect could have started - so it proved nothing. The login URL and
    // the login form still being there do.
    await expect(page).toHaveURL(/saucedemo\.com\/$/);
    await expect(loginPage.loginButton()).toBeVisible();
    await expect(loginPage.usernameInput()).toBeVisible();
  });
});
