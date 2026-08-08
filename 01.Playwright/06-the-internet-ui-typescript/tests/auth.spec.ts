import { test, expect } from '@playwright/test';
import { AuthPage } from '../pages/AuthPage';
import { Credentials, Expected, Routes } from '../data/testData';

test.describe('Form Authentication', () => {
  let authPage: AuthPage;

  test.beforeEach(async ({ page }) => {
    authPage = new AuthPage(page);
    await authPage.openLogin();
  });

  test('Valid credentials reach the secure area', async ({ page }) => {
    await authPage.login(Credentials.formAuth.username, Credentials.formAuth.password);

    await expect(page).toHaveURL(/\/secure/);
    await expect(authPage.secureAreaHeading()).toHaveText('Secure Area');
    expect(await authPage.getFlashTextAsync()).toContain(Expected.loginSuccessFlash);
    await expect(authPage.logoutButton()).toBeVisible();
  });

  test('An unknown username is rejected and stays on the login page', async ({ page }) => {
    await authPage.login(Credentials.invalidUser.username, Credentials.invalidUser.password);

    await expect(page).toHaveURL(/\/login/);
    expect(await authPage.getFlashTextAsync()).toContain(Expected.loginInvalidUserFlash);
  });

  test('A valid username with the wrong password is rejected', async ({ page }) => {
    await authPage.login(Credentials.formAuth.username, 'definitely-not-the-password');

    await expect(page).toHaveURL(/\/login/);
    // The site distinguishes a bad password from a bad username, which is a
    // information-disclosure smell worth pinning down in a test.
    expect(await authPage.getFlashTextAsync()).toContain(Expected.loginInvalidPasswordFlash);
  });

  test('Logging out returns to the login page', async ({ page }) => {
    await authPage.login(Credentials.formAuth.username, Credentials.formAuth.password);
    await authPage.logout();

    await expect(page).toHaveURL(/\/login/);
    expect(await authPage.getFlashTextAsync()).toContain(Expected.logoutFlash);
  });
});

test.describe('Forgot Password', () => {
  /**
   * KNOWN DEFECT (server-side, verified 2026-08): submitting the form returns
   * HTTP 500 "Internal Server Error" instead of the confirmation page - the
   * deployed app has no working mailer. Reproduced on repeated runs, so this
   * is the application's behaviour rather than a flaky test.
   *
   * Marked as an expected failure rather than deleted or weakened: the suite
   * keeps asserting the correct behaviour, and Playwright will flag this as
   * "unexpected pass" the day the endpoint is fixed.
   */
  test.fail(
    'Submitting an e-mail shows the confirmation page',
    async ({ page }) => {
      const authPage = new AuthPage(page);
      await authPage.openForgotPassword();

      await authPage.requestPasswordReset('qa.tester@example.com');

      await expect(authPage.confirmationHeading()).toHaveText(Expected.forgotPasswordConfirmation);
    }
  );

  test('The form itself renders and accepts input', async ({ page }) => {
    const authPage = new AuthPage(page);
    await authPage.openForgotPassword();

    // Everything up to the broken submit is still worth covering.
    // This page titles itself with an <h2>, unlike the <h3> most examples use.
    await expect(page.locator('h2')).toHaveText('Forgot Password');
    const email = page.locator('#email');
    await email.fill('qa.tester@example.com');
    await expect(email).toHaveValue('qa.tester@example.com');
  });

  test('Submitting currently returns a 500 from the server', async ({ page }) => {
    const authPage = new AuthPage(page);
    await authPage.openForgotPassword();

    // Pins the current broken behaviour so a silent change is noticed.
    const [response] = await Promise.all([
      page.waitForResponse((r) => r.url().includes('forgot_password') && r.request().method() === 'POST'),
      authPage.requestPasswordReset('qa.tester@example.com'),
    ]);

    expect(response.status()).toBe(500);
  });
});

test.describe('HTTP authentication', () => {
  // Basic and Digest auth cannot be typed into the page - the credentials have
  // to be supplied at the browser-context level before the request is made.
  test('Basic Auth succeeds with credentials supplied by the context', async ({ browser }) => {
    const context = await browser.newContext({ httpCredentials: Credentials.basicAuth });
    const page = await context.newPage();

    const response = await page.goto(Routes.basicAuth);

    expect(response?.status()).toBe(200);
    await expect(page.locator('#content p')).toHaveText(Expected.basicAuthSuccess);

    await context.close();
  });

  test('Basic Auth is refused with a 401 when no credentials are sent', async ({ browser }) => {
    const context = await browser.newContext();
    const page = await context.newPage();

    const response = await page.goto(Routes.basicAuth);

    expect(response?.status()).toBe(401);

    await context.close();
  });

  test('Digest Auth succeeds with credentials supplied by the context', async ({ browser }) => {
    const context = await browser.newContext({ httpCredentials: Credentials.basicAuth });
    const page = await context.newPage();

    const response = await page.goto(Routes.digestAuth);

    expect(response?.status()).toBe(200);
    await expect(page.locator('#content p')).toHaveText(Expected.digestAuthSuccess);

    await context.close();
  });

  test('Secure File Download is protected by Basic Auth', async ({ browser }) => {
    const anonymousContext = await browser.newContext();
    const anonymousPage = await anonymousContext.newPage();
    const unauthorised = await anonymousPage.goto(Routes.secureFileDownload);
    expect(unauthorised?.status()).toBe(401);
    await anonymousContext.close();

    const authorisedContext = await browser.newContext({ httpCredentials: Credentials.basicAuth });
    const authorisedPage = await authorisedContext.newPage();
    const authorised = await authorisedPage.goto(Routes.secureFileDownload);
    expect(authorised?.status()).toBe(200);
    await expect(authorisedPage.locator('h3')).toHaveText('Secure File Downloader');
    await authorisedContext.close();
  });
});
