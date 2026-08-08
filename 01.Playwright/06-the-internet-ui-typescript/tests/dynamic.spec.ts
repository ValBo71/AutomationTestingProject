import { test, expect } from '@playwright/test';
import { DynamicPage } from '../pages/DynamicPage';
import { Expected } from '../data/testData';

test.describe('Waiting for elements', () => {
  let dynamicPage: DynamicPage;

  test.beforeEach(async ({ page }) => {
    dynamicPage = new DynamicPage(page);
  });

  test('Dynamic Loading 1: element exists but is hidden until loading finishes', async () => {
    await dynamicPage.openDynamicLoadingHidden();

    // The element is already in the DOM - only a visibility wait is meaningful.
    await expect(dynamicPage.finishText()).toBeHidden();

    await dynamicPage.startLoading();

    await expect(dynamicPage.finishText()).toBeVisible();
    await expect(dynamicPage.finishText()).toHaveText(Expected.dynamicLoadingText);
  });

  test('Dynamic Loading 2: element is not rendered until loading finishes', async () => {
    await dynamicPage.openDynamicLoadingRendered();

    // Here the element does not exist yet, so the wait is on attachment.
    await expect(dynamicPage.finishText()).toHaveCount(0);

    await dynamicPage.startLoading();

    await expect(dynamicPage.finishText()).toBeVisible();
    await expect(dynamicPage.finishText()).toHaveText(Expected.dynamicLoadingText);
  });

  test('Dynamic Controls: a checkbox can be removed and added back', async () => {
    await dynamicPage.openDynamicControls();

    await expect(dynamicPage.checkbox()).toBeVisible();

    await dynamicPage.toggleCheckbox();
    await expect(dynamicPage.message()).toHaveText(Expected.dynamicControlsGone);
    await expect(dynamicPage.checkbox()).toHaveCount(0);

    await dynamicPage.toggleCheckbox();
    await expect(dynamicPage.message()).toHaveText(Expected.dynamicControlsBack);
    await expect(dynamicPage.checkbox()).toBeVisible();
  });

  test('Dynamic Controls: a disabled input becomes editable', async () => {
    await dynamicPage.openDynamicControls();

    const input = dynamicPage.textInput();
    await expect(input).toBeDisabled();

    await dynamicPage.toggleInputEnabled();

    await expect(input).toBeEnabled();
    await expect(dynamicPage.message()).toHaveText(Expected.dynamicControlsEnabled);

    // Proving it is genuinely usable, not merely missing the disabled attribute.
    await input.fill('now editable');
    await expect(input).toHaveValue('now editable');
  });

  test('Slow Resources: the page still finishes loading and renders its heading', async () => {
    await dynamicPage.openSlowResources();

    // The challenge is a slow-loading asset; the page itself must still resolve.
    await expect(dynamicPage.heading()).toHaveText('Slow Resources');
  });
});

test.describe('Content that changes between loads', () => {
  let dynamicPage: DynamicPage;

  test.beforeEach(async ({ page }) => {
    dynamicPage = new DynamicPage(page);
  });

  test('Dynamic Content: the text differs between two page loads', async ({ page }) => {
    await dynamicPage.openDynamicContent();
    const firstLoad = await dynamicPage.getContentTextsAsync();
    expect(firstLoad.length).toBeGreaterThan(0);

    await page.reload();
    const secondLoad = await dynamicPage.getContentTextsAsync();

    // Two of the three rows are randomised on every load, so at least one of
    // them must differ. Asserting "all differ" would be flaky by design.
    const changed = firstLoad.some((text, index) => text !== secondLoad[index]);
    expect(changed).toBe(true);
  });

  test('Disappearing Elements: the menu has either 4 or 5 links', async () => {
    await dynamicPage.openDisappearingElements();

    const links = await dynamicPage.getNavLinkTextsAsync();

    // The "Gallery" entry appears at random, which is exactly the trap: a test
    // asserting a fixed count would fail intermittently for no real reason.
    expect(links.length).toBeGreaterThanOrEqual(4);
    expect(links.length).toBeLessThanOrEqual(5);
    expect(links).toContain('Home');
  });

  test('Shifting Content: the menu is still usable after the layout shifts', async () => {
    await dynamicPage.openShiftingContent();

    const items = dynamicPage.shiftingMenuItems();
    await expect(items.first()).toBeVisible();

    // The item count shifts between loads; the locator must not depend on it.
    expect(await items.count()).toBeGreaterThan(0);
  });

  test('Infinite Scroll: scrolling appends more content blocks', async ({ page }) => {
    await dynamicPage.openInfiniteScroll();

    const initialCount = await dynamicPage.scrollBlocks().count();

    const afterFirstScroll = await dynamicPage.scrollUntilMoreContentAsync(initialCount);
    expect(afterFirstScroll).toBeGreaterThan(initialCount);

    // A second round proves it keeps loading rather than appending only once.
    const afterSecondScroll = await dynamicPage.scrollUntilMoreContentAsync(afterFirstScroll);
    expect(afterSecondScroll).toBeGreaterThan(afterFirstScroll);
  });
});
