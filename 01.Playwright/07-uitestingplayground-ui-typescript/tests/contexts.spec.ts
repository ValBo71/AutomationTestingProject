import { test, expect } from '@playwright/test';
import * as fs from 'fs';
import * as os from 'os';
import * as path from 'path';
import { ContextPage } from '../pages/ContextPage';
import { ContextSelectors } from '../selectors/Selectors';
import { Routes, SecureBaseUrl, TestData } from '../data/testData';

test.describe('Frames', () => {
  let contextPage: ContextPage;

  test.beforeEach(async ({ page }) => {
    contextPage = new ContextPage(page);
    await contextPage.openFrames();
  });

  test('The outer and inner frames are told apart despite identical markup', async () => {
    await expect(contextPage.outerFrame().locator(ContextSelectors.frameLabel)).toHaveText(
      'Outer Frame (Level 1)'
    );
    await expect(contextPage.innerFrame().locator(ContextSelectors.frameLabel)).toHaveText(
      'Inner Frame (Level 2)'
    );
  });

  test('Each element-location strategy works inside the outer frame', async () => {
    const frame = contextPage.outerFrame();

    // Four different strategies against four buttons with no ids.
    await frame.locator(ContextSelectors.editButtonByData).click();
    await expect(frame.locator(ContextSelectors.frameResult)).toContainText('Edit');

    await frame.locator(ContextSelectors.submitButtonByText).click();
    await expect(frame.locator(ContextSelectors.frameResult)).toContainText('Submit');

    await frame.locator(ContextSelectors.clickMeButtonByName).click();
    await expect(frame.locator(ContextSelectors.frameResult)).toContainText('Click me');

    await frame.locator(ContextSelectors.primaryButtonByClass).click();
    await expect(frame.locator(ContextSelectors.frameResult)).toContainText('Primary');
  });

  test('Clicking inside the inner frame does not affect the outer one', async () => {
    const outer = contextPage.outerFrame();
    const inner = contextPage.innerFrame();

    await inner.locator(ContextSelectors.editButtonByData).click();

    await expect(inner.locator(ContextSelectors.frameResult)).toContainText('Edit');
    // The outer frame has its own identical #result, which must stay empty -
    // this is what proves the click was correctly scoped.
    await expect(outer.locator(ContextSelectors.frameResult).first()).toHaveText('');
  });
});

test.describe('Shadow DOM', () => {
  let contextPage: ContextPage;

  test.beforeEach(async ({ page }) => {
    contextPage = new ContextPage(page);
    await contextPage.openShadowDom();
  });

  test('A GUID is generated inside the shadow root', async () => {
    // Playwright pierces open shadow roots, so the field is addressed directly.
    await expect(contextPage.guidField()).toHaveValue('');

    await contextPage.generateGuid();

    const guid = await contextPage.getGuidAsync();
    expect(guid).toMatch(/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i);
  });

  test('Generating twice produces two different GUIDs', async () => {
    await contextPage.generateGuid();
    const first = await contextPage.getGuidAsync();

    await contextPage.generateGuid();
    const second = await contextPage.getGuidAsync();

    expect(second).not.toBe(first);
  });

  test('The copy button reads the value into the clipboard', async ({ context, page }) => {
    await context.grantPermissions(['clipboard-read', 'clipboard-write']);

    // Reloaded over https: navigator.clipboard is undefined in an insecure
    // context, and the suite's default baseURL is http because the site's
    // certificate is invalid.
    await page.goto(`${SecureBaseUrl}${Routes.shadowDom}`);

    await contextPage.generateGuid();
    const generated = await contextPage.getGuidAsync();

    await contextPage.copyGuid();

    const clipboard = await page.evaluate(() => navigator.clipboard.readText());
    expect(clipboard).toBe(generated);
  });
});

test.describe('File Upload', () => {
  test('A file is attached through the hidden input inside the iframe', async ({ page }) => {
    const contextPage = new ContextPage(page);

    // Built at runtime so no binary fixture is committed.
    const dir = fs.mkdtempSync(path.join(os.tmpdir(), 'utp-upload-'));
    const filePath = path.join(dir, TestData.upload.fileName);
    fs.writeFileSync(filePath, TestData.upload.fileContent, 'utf-8');

    await contextPage.openFileUpload();
    await contextPage.uploadFile(filePath);

    // The uploader lists the attached file once it is accepted.
    await expect(contextPage.uploadFrameBody()).toContainText(TestData.upload.fileName);

    fs.rmSync(dir, { recursive: true, force: true });
  });
});
