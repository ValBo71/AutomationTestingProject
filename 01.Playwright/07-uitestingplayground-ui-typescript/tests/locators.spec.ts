import { test, expect } from '@playwright/test';
import { LocatorPage } from '../pages/LocatorPage';
import { LocatorSelectors } from '../selectors/Selectors';
import { Expected, TestData } from '../data/testData';

test.describe('Locator strategy challenges', () => {
  let locatorPage: LocatorPage;

  test.beforeEach(async ({ page }) => {
    locatorPage = new LocatorPage(page);
  });

  test('Dynamic ID: the button is found by class because its id changes each load', async ({
    page,
  }) => {
    await locatorPage.openDynamicId();

    const firstId = await locatorPage.dynamicIdButton().getAttribute('id');
    expect(firstId).toBeTruthy();

    await page.reload();
    const secondId = await locatorPage.dynamicIdButton().getAttribute('id');

    // Proves why the id is unusable as a locator.
    expect(secondId).not.toBe(firstId);

    // The class-based locator still resolves after the id churn.
    await locatorPage.dynamicIdButton().click();
    await expect(locatorPage.dynamicIdButton()).toBeVisible();
  });

  test('Class Attribute: only a partial class match finds the right button', async ({ page }) => {
    await locatorPage.openClassAttribute();

    // All three buttons share "btn" and "btn-test"; an exact-class match on
    // "btn-primary" alone would find nothing.
    await expect(page.locator(LocatorSelectors.allClassButtons)).toHaveCount(3);
    await expect(page.locator('button[class="btn-primary"]')).toHaveCount(0);

    let alertText = '';
    page.once('dialog', async (dialog) => {
      alertText = dialog.message();
      await dialog.accept();
    });

    await locatorPage.classAttributeButton().click();

    await expect.poll(() => alertText).toBe(Expected.classAttributeAlert);
  });

  test('Non-Breaking Space: the caption contains U+00A0, not a plain space', async () => {
    await locatorPage.openNonBreakingSpace();

    const raw = await locatorPage.getNbspRawTextAsync();

    /**
     * Expected.nbspButtonRaw is built with String.fromCharCode(0x00a0) rather
     * than written as a literal, and that detail is the whole test.
     *
     * The first version of this file compared a pasted U+00A0 against the one
     * on the page. It looked correct, read correctly, and asserted nothing -
     * the character was being compared to itself. Constructing the expectation
     * means no formatter, editor or careless paste can silently turn it back
     * into an ordinary space and leave a test that passes without checking.
     */
    // The raw caption holds a non-breaking space...
    expect(raw).toBe(Expected.nbspButtonRaw);
    // ...so a locator written with an ordinary space would never match it,
    // even though the two are indistinguishable on screen.
    expect(raw).not.toBe(Expected.nbspButtonNormalised);

    // Playwright normalises whitespace when matching text, so the readable
    // form succeeds where an exact DOM-text comparison fails.
    await expect(locatorPage.nbspButton()).toHaveText(Expected.nbspButtonNormalised);
  });

  test('Text Input: the button caption changes to the typed value', async () => {
    await locatorPage.openTextInput();

    await locatorPage.renameButton(TestData.textInput.newButtonName);

    await expect(locatorPage.updatingButton()).toHaveText(TestData.textInput.newButtonName);
  });

  test('Verify Text: the DOM text carries whitespace the screen does not show', async () => {
    await locatorPage.openVerifyText();

    const raw = await locatorPage.getRawTextAsync();

    // The DOM text is wrapped in newlines and indentation the screen never
    // shows, so a raw equality check against the visible caption fails.
    expect(raw).not.toBe(Expected.verifyTextNormalised);
    expect(raw).toMatch(/\n/);

    // Trimming is enough here - the padding is only at the ends.
    expect(raw.trim()).toBe(Expected.verifyTextNormalised);

    // toHaveText normalises whitespace itself, which is why it matches the
    // caption as a human reads it.
    await expect(locatorPage.verifyTextBadge()).toHaveText(Expected.verifyTextNormalised);
  });
});

test.describe('CSS Selectors', () => {
  let locatorPage: LocatorPage;

  test.beforeEach(async ({ page }) => {
    locatorPage = new LocatorPage(page);
    await locatorPage.openCssSelectors();
  });

  test('Elements are reachable by id, class, attribute and combinator', async ({ page }) => {
    await expect(page.locator(LocatorSelectors.primaryButton)).toBeVisible();
    await expect(page.locator(LocatorSelectors.highlightedClassButton)).toBeVisible();
    await expect(page.locator(LocatorSelectors.usernameInput)).toBeVisible();
    await expect(page.locator(LocatorSelectors.linkByHref)).toBeVisible();

    // Adjacent-sibling combinator.
    await expect(page.locator(LocatorSelectors.adjacentParagraph)).toHaveCount(1);
  });

  test('Each hiding technique is correctly reported as not visible', async ({ page }) => {
    await expect(page.locator(LocatorSelectors.visibleButton)).toBeVisible();

    // display:none, visibility:hidden and opacity:0 are all "hidden" to
    // Playwright, but for different underlying reasons.
    await expect(page.locator(LocatorSelectors.hiddenDisplayButton)).toBeHidden();
    await expect(page.locator(LocatorSelectors.hiddenVisibilityButton)).toBeHidden();

    // opacity:0 is a special case - the element still occupies space and is
    // considered visible by Playwright, so only the style reveals it.
    await expect(page.locator(LocatorSelectors.hiddenOpacityButton)).toHaveCSS('opacity', '0');
  });
});
