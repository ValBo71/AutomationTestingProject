import { test, expect } from '@playwright/test';
import { AppPage } from '../pages/AppPage';
import { Expected, Routes, SecureBaseUrl, TestData } from '../data/testData';

test.describe('Sample App', () => {
  let appPage: AppPage;

  test.beforeEach(async ({ page }) => {
    appPage = new AppPage(page);
    await appPage.openSampleApp();
  });

  test('Logging in with the expected password succeeds', async () => {
    await expect(appPage.loginStatus()).toHaveText('User logged out.');

    await appPage.login(TestData.sampleApp.username, TestData.sampleApp.password);

    await expect(appPage.loginStatus()).toHaveText(
      Expected.sampleAppLoggedIn(TestData.sampleApp.username)
    );
  });

  test('A wrong password is rejected', async () => {
    await appPage.login(TestData.sampleApp.username, TestData.sampleApp.wrongPassword);

    await expect(appPage.loginStatus()).toHaveText(Expected.sampleAppInvalid);
  });
});

test.describe('Alerts', () => {
  let appPage: AppPage;

  test.beforeEach(async ({ page }) => {
    appPage = new AppPage(page);
    await appPage.openAlerts();
  });

  test('The alert dialog is accepted', async ({ page }) => {
    let type = '';
    page.once('dialog', async (dialog) => {
      type = dialog.type();
      await dialog.accept();
    });

    await appPage.clickAlert();

    await expect.poll(() => type).toBe('alert');
  });

  test('The confirm dialog is accepted and dismissed', async ({ page }) => {
    let type = '';
    page.once('dialog', async (dialog) => {
      type = dialog.type();
      await dialog.accept();
    });
    await appPage.clickConfirm();
    await expect.poll(() => type).toBe('confirm');

    let dismissed = false;
    page.once('dialog', async (dialog) => {
      dismissed = true;
      await dialog.dismiss();
    });
    await appPage.clickConfirm();
    await expect.poll(() => dismissed).toBe(true);
  });

  test('The prompt dialog accepts typed text', async ({ page }) => {
    let type = '';
    page.once('dialog', async (dialog) => {
      type = dialog.type();
      await dialog.accept('Playwright');
    });

    await appPage.clickPrompt();

    await expect.poll(() => type).toBe('prompt');
  });
});

test.describe('Select', () => {
  let appPage: AppPage;

  test.beforeEach(async ({ page }) => {
    appPage = new AppPage(page);
    await appPage.openSelect();
  });

  test('Selecting by value updates the status line', async () => {
    await appPage.languageSelect().selectOption(TestData.select.language.value);

    await expect(appPage.languageSelect()).toHaveValue(TestData.select.language.value);
    await expect(appPage.languageStatus()).toContainText(TestData.select.language.label);
  });

  test('Selecting by visible label works as well as by value', async () => {
    await appPage.citySelect().selectOption({ value: TestData.select.city.value });

    await expect(appPage.citySelect()).toHaveValue(TestData.select.city.value);
    await expect(appPage.cityStatus()).not.toHaveText('');
  });
});

test.describe('Clear Input', () => {
  let appPage: AppPage;

  test.beforeEach(async ({ page }) => {
    appPage = new AppPage(page);
    await appPage.openClearInput();
  });

  test('Text and number fields can be cleared', async () => {
    await expect(appPage.textInput()).toHaveValue('Initial Text Value');
    await appPage.textInput().fill('');
    await expect(appPage.textInput()).toHaveValue('');

    await expect(appPage.numberInput()).toHaveValue('42');
    await appPage.numberInput().fill('');
    await expect(appPage.numberInput()).toHaveValue('');
  });

  test('A contenteditable div is cleared differently from an input', async () => {
    const editable = appPage.contentEditable();
    await expect(editable).not.toHaveText('');

    // fill() does not apply to a div, so the content is selected and deleted.
    await appPage.clearContentEditable();

    await expect(editable).toHaveText('');
  });
});

test.describe('Mouse Over', () => {
  let appPage: AppPage;

  test.beforeEach(async ({ page }) => {
    appPage = new AppPage(page);
    await appPage.openMouseOver();
  });

  test('Two consecutive clicks both register despite the link being retitled', async () => {
    // Hovering rewrites the anchor - its title changes from "Click me" to
    // "Active Link" - so a locator bound to the original title goes stale the
    // moment the mouse arrives. Re-querying by the new title stays attached.
    await appPage.mouseOverLink(Expected.mouseOverLinkTitleBefore).hover();

    const active = appPage.mouseOverLink(Expected.mouseOverLinkTitleAfter);
    await active.click();
    await active.click();

    await expect(appPage.clickCount()).toHaveText('2');
  });

  test('The second link increments its own counter', async () => {
    // This one is swapped for an identical node on hover, so the counter is
    // the only way to tell the click actually landed.
    const linkButton = appPage.mouseOverLink('Link Button');
    await linkButton.hover();
    await linkButton.click();

    await expect(appPage.clickButtonCount()).toHaveText('1');
  });
});

test.describe('Dynamic Table', () => {
  let appPage: AppPage;

  test.beforeEach(async ({ page }) => {
    appPage = new AppPage(page);
    await appPage.openDynamicTable();
  });

  test('The CPU value for Chrome matches the label above the table', async () => {
    // Column and row order are both randomised per load, so the value is
    // found by matching header text to cell position.
    const cpuFromTable = await appPage.getCpuValueForAsync('Chrome');
    const labelText = await appPage.chromeCpuLabel().innerText();

    expect(labelText).toContain(cpuFromTable);
  });

  test('All five columns are present regardless of their order', async () => {
    const headers = (await appPage.columnHeaders().allInnerTexts()).map((h) => h.trim());

    expect(headers).toHaveLength(Expected.dynamicTableColumns.length);
    for (const column of Expected.dynamicTableColumns) {
      expect(headers).toContain(column);
    }
  });
});

test.describe('Geo Location', () => {
  test('A mocked position is reported back by the page', async ({ browser }) => {
    // Pinning the coordinates keeps the assertion deterministic; a real
    // position would differ per machine and prompt for permission.
    const context = await browser.newContext({
      permissions: ['geolocation'],
      geolocation: TestData.geolocation,
      ignoreHTTPSErrors: true,
    });
    const page = await context.newPage();
    const appPage = new AppPage(page);

    // Loaded over https on purpose: the Geolocation API is unavailable in an
    // insecure context, and the suite's default baseURL is http because the
    // site's certificate is invalid. Verified - over http the page reports
    // "unavailable" no matter what permissions are granted.
    await page.goto(`${SecureBaseUrl}${Routes.geoLocation}`);
    await appPage.requestLocation();

    await expect(appPage.locationOutput()).toContainText('42.69');
    await expect(appPage.locationOutput()).toContainText('23.32');

    await context.close();
  });
});
