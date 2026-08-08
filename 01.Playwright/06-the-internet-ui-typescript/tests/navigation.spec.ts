import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as os from 'os';
import * as path from 'path';
import { NavigationPage } from '../pages/NavigationPage';
import { Expected, Routes, TestData } from '../data/testData';

test.describe('Windows and redirects', () => {
  let navigationPage: NavigationPage;

  test.beforeEach(async ({ page }) => {
    navigationPage = new NavigationPage(page);
  });

  test('Multiple Windows: the new tab is captured and both pages stay usable', async ({ page }) => {
    await navigationPage.openMultipleWindows();

    const newPage = await navigationPage.openNewWindowAsync();

    await expect(newPage.locator('h3')).toHaveText(Expected.newWindowHeading);
    // The original page must remain untouched and still addressable.
    await expect(page.locator('h3')).toHaveText('Opening a new window');

    await newPage.close();
  });

  test('Redirect Link: following the link lands on the status codes page', async ({ page }) => {
    await navigationPage.openRedirector();

    await navigationPage.followRedirect();

    await expect(page).toHaveURL(new RegExp(`${Routes.statusCodes}$`));
    await expect(page.locator('h3')).toHaveText('Status Codes');
  });

  for (const code of TestData.statusCodes) {
    test(`Status Codes: /status_codes/${code} returns HTTP ${code}`, async ({ page }) => {
      const navigation = new NavigationPage(page);
      const response = await page.goto(`${Routes.statusCodes}/${code}`);

      // The site serves each code with a rendered body rather than a bare
      // header, so even the 301 is reported as-is instead of being followed.
      expect(response?.status()).toBe(code);
      await expect(navigation.heading()).toHaveText('Status Codes');
    });
  }
});

test.describe('Files', () => {
  test('File Upload: a file is uploaded and its name is shown back', async ({ page }) => {
    const navigationPage = new NavigationPage(page);

    // Build the file at runtime so the repo carries no binary fixture.
    const uploadDir = fs.mkdtempSync(path.join(os.tmpdir(), 'pw-upload-'));
    const filePath = path.join(uploadDir, TestData.uploadFileName);
    fs.writeFileSync(filePath, TestData.uploadFileContent, 'utf-8');

    await navigationPage.openFileUpload();
    await navigationPage.uploadFile(filePath);

    await expect(navigationPage.heading()).toHaveText(Expected.uploadSuccessHeading);
    await expect(navigationPage.uploadedFiles()).toHaveText(TestData.uploadFileName);

    fs.rmSync(uploadDir, { recursive: true, force: true });
  });

  test('File Download: a file is downloaded and arrives with content', async ({ page }) => {
    const navigationPage = new NavigationPage(page);
    await navigationPage.openFileDownload();

    const links = navigationPage.downloadLinks();
    expect(await links.count()).toBeGreaterThan(0);

    const [download] = await Promise.all([
      page.waitForEvent('download'),
      links.first().click(),
    ]);

    const savedPath = await download.path();
    expect(savedPath).toBeTruthy();
    expect(download.suggestedFilename().length).toBeGreaterThan(0);
  });
});

test.describe('Browser permissions', () => {
  test('Geolocation: a mocked position is reported back by the page', async ({ browser }) => {
    // Real geolocation would prompt and be non-deterministic; granting the
    // permission and pinning coordinates makes the test repeatable.
    const context = await browser.newContext({
      permissions: ['geolocation'],
      geolocation: { latitude: 42.6977, longitude: 23.3219 }, // Sofia
    });
    const page = await context.newPage();
    const navigationPage = new NavigationPage(page);

    await navigationPage.openGeolocation();
    await navigationPage.requestLocation();

    await expect(navigationPage.latitude()).toContainText('42.69');
    await expect(navigationPage.longitude()).toContainText('23.32');

    await context.close();
  });
});

test.describe('Page-level errors', () => {
  test('JavaScript onload error: the page reports a console error', async ({ page }) => {
    const pageErrors: string[] = [];
    page.on('pageerror', (error) => pageErrors.push(error.message));

    await page.goto(Routes.javaScriptOnloadError);

    // The challenge is that the failure is invisible in the UI - only a
    // listener attached before navigation catches it.
    await expect.poll(() => pageErrors.length).toBeGreaterThan(0);
    expect(pageErrors.join(' ')).toContain('Cannot read properties of undefined');
  });
});
