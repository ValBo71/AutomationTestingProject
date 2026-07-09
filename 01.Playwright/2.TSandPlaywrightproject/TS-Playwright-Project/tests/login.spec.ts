import { test, expect } from '@playwright/test';
import { LoginPage } from '../pages/LoginPage';
import { TestData } from '../data/testData';
import { LoginSelectors } from '../selectors/LoginSelectors';
import { MainPageSelectors } from '../selectors/MainPageSelectors';

test.describe('Login functionality - Smoke Tests', () => {
  let loginPage: LoginPage;

  test.beforeEach(async ({ page }) => {
    loginPage = new LoginPage(page);
  });

  test('Valid user should login successfully', async ({ page }) => {
    // 1. Open https://www.saucedemo.com/
    await loginPage.open();

    // Verify the logo on the first page (Login)
    const loginLogo = page.locator(LoginSelectors.loginLogo);
    await expect(loginLogo).toBeVisible();
    await expect(loginLogo).toHaveText('Swag Labs');

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
    const inventoryContainer = page.locator(LoginSelectors.inventoryContainer).first();
    await expect(inventoryContainer).toBeVisible();

    // Verify the Swag Labs label
    const appLogo = page.locator(MainPageSelectors.appLogo);
    await expect(appLogo).toBeVisible();
    await expect(appLogo).toHaveText('Swag Labs');

    // Keeps the browser open after the test finishes (Playwright Inspector)
    await page.pause();
  });
});
