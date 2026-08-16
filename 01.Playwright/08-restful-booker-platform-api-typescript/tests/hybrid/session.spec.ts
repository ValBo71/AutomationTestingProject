import { test, expect } from '../../fixtures/api';
import { AdminPage } from '../../pages/AdminPage';

/**
 * The reason this suite exists in Playwright rather than in a pure HTTP client:
 * the API and the browser can share one session.
 *
 * Four tests, and the order they are written in is the argument they make. The
 * form is exercised once, so it is covered. The token path is shown to reach the
 * same place. Then two negatives - no cookie, and a made-up one - establish that
 * the second test proved something, rather than merely finding an app that lets
 * anyone through. Without those two, every other admin test in this suite would
 * be resting on an unchecked assumption.
 */
test.describe('Session', () => {
  test('The login form works, and is tested exactly once', async ({ page }) => {
    const admin = new AdminPage(page);

    await admin.loginThroughFormAsync();

    await expect(page).toHaveURL(/\/admin\/rooms/);
  });

  test('A token minted over HTTP logs the browser in without touching the form', async ({
    page,
    adminToken,
  }) => {
    const admin = new AdminPage(page);

    // No credentials are typed here. The token came from /api/auth/login.
    await admin.loginWithTokenAsync(adminToken);
    await admin.open();

    await admin.expectLoggedIn();
  });

  test('Without the cookie the same URL shows the login form instead', async ({ page }) => {
    const admin = new AdminPage(page);

    await admin.open();

    // The control for the test above: proof that the redirect is driven by the
    // token and not by the app simply letting anyone in.
    await expect(page.locator('#username')).toBeVisible();
    await expect(page).not.toHaveURL(/\/admin\/rooms/);
  });

  test('A token that never existed does not open the admin screens', async ({ page }) => {
    const admin = new AdminPage(page);

    await admin.loginWithTokenAsync('not-a-real-token');
    await admin.open();

    await expect(page.locator('#username')).toBeVisible();
  });
});
