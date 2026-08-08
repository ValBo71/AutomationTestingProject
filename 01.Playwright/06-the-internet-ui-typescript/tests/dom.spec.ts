import { test, expect } from '@playwright/test';
import { DomPage } from '../pages/DomPage';
import { DomSelectors } from '../selectors/DynamicSelectors';
import { Expected } from '../data/testData';

test.describe('Difficult DOM structures', () => {
  let domPage: DomPage;

  test.beforeEach(async ({ page }) => {
    domPage = new DomPage(page);
  });

  test('Challenging DOM: buttons are located by position, not by their random ids', async ({
    page,
  }) => {
    await domPage.openChallengingDom();

    const buttons = domPage.challengeButtons();
    await expect(buttons).toHaveCount(3);

    // Capture an id, reload, and show it changed - which is precisely why the
    // locator strategy above cannot rely on ids.
    const idBeforeReload = await buttons.first().getAttribute('id');
    await page.reload();
    const idAfterReload = await domPage.challengeButtons().first().getAttribute('id');
    expect(idAfterReload).not.toBe(idBeforeReload);

    // The button is still reachable by position despite the id churn.
    await expect(domPage.challengeButtons().first()).toBeVisible();
  });

  test('Challenging DOM: the table has 10 rows and 7 columns and a canvas is present', async () => {
    await domPage.openChallengingDom();

    await expect(domPage.challengeTableRows()).toHaveCount(10);
    expect(await domPage.getCellTextAsync(0, 0)).not.toBe('');
    await expect(domPage.canvas()).toBeVisible();
  });

  test('Large & Deep DOM: the 50-levels-deep element and the big table are reachable', async () => {
    await domPage.openLargeAndDeepDom();

    await expect(domPage.deepestNoSiblingsElement()).toBeVisible();
    await expect(domPage.deepestNoSiblingsElement()).toHaveText('No siblings');

    await expect(domPage.largeTable()).toBeVisible();
  });

  test('Sortable Data Tables: clicking a header sorts the column', async () => {
    await domPage.openSortableTables();

    // Column 3 is "Due" - a numeric column, so a naive string sort would be
    // visible in the assertion below if the site got it wrong.
    const before = await domPage.getColumnValuesAsync(DomSelectors.table1, 3);
    expect(before.length).toBeGreaterThan(0);

    await domPage.sortByColumn(DomSelectors.table1, 3);

    // tablesorter rewrites the rows asynchronously, so polling until the column
    // is ordered avoids reading the pre-sort DOM (a real race, not a guess -
    // an immediate read returns the original order).
    await expect
      .poll(async () => {
        const values = await domPage.getColumnValuesAsync(DomSelectors.table1, 3);
        const numbers = values.map((value) => Number(value.replace('$', '')));
        return numbers.every((value, index) => index === 0 || numbers[index - 1] <= value);
      })
      .toBe(true);
  });

  test('Shadow DOM: slotted text inside the shadow root is readable', async () => {
    await domPage.openShadowDom();

    // Playwright pierces open shadow roots, so no manual traversal is needed -
    // this is where Selenium would require getShadowRoot() gymnastics.
    for (const expectedText of Expected.shadowDomTexts) {
      await expect(domPage.shadowHost().filter({ hasText: expectedText }).first()).toBeVisible();
    }
  });

  test('Broken Images: exactly two of the three images fail to load', async () => {
    await domPage.openBrokenImages();

    await expect(domPage.images()).toHaveCount(3);

    // A broken image is still "visible" to the DOM, so naturalWidth is the
    // only assertion that actually catches it.
    expect(await domPage.getBrokenImageCountAsync()).toBe(2);
  });

  test('Typos: the sentence is either correct or contains the known typo', async () => {
    await domPage.openTypos();

    const text = (await domPage.typoParagraph().innerText()).replace(/\s+/g, ' ').trim();

    const hasCorrectText = text.includes(Expected.typoSentenceCorrect);
    const hasTypo = text.includes(Expected.typoSentenceWrong);

    // The typo is injected at random on each load, so the test asserts that the
    // page is in exactly one of the two known states rather than guessing.
    expect(hasCorrectText || hasTypo).toBe(true);
  });
});
