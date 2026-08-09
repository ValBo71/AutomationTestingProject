import { test, expect } from '@playwright/test';
import { ObstructionPage } from '../pages/ObstructionPage';
import { Expected } from '../data/testData';

test.describe('Blocked and hidden targets', () => {
  let obstructionPage: ObstructionPage;

  test.beforeEach(async ({ page }) => {
    obstructionPage = new ObstructionPage(page);
  });

  test('Click: a real mouse click registers where a DOM click does not', async () => {
    await obstructionPage.openClick();

    // The page ignores element.click() on purpose.
    await obstructionPage.dispatchDomClickOnBadButton();
    expect(await obstructionPage.getBadButtonClassAsync()).not.toContain(
      Expected.clickSuccessClass
    );

    // Playwright dispatches genuine input events, so this one lands.
    await obstructionPage.clickBadButton();
    await expect(obstructionPage.badButton()).toHaveClass(
      new RegExp(Expected.clickSuccessClass)
    );
  });

  test('Hidden Layers: the green button cannot be clicked twice', async () => {
    await obstructionPage.openHiddenLayers();

    await obstructionPage.greenButton().click();

    // A transparent layer is drawn over the green button after the first
    // click, so a second click is intercepted rather than reaching it.
    await expect(async () => {
      await obstructionPage.greenButton().click({ timeout: 3_000 });
    }).rejects.toThrow();
  });

  test('Overlapped Element: the input is reachable after scrolling it clear', async () => {
    await obstructionPage.openOverlapped();

    // The Id field is not covered and behaves normally.
    await obstructionPage.overlappedIdInput().fill('12345');
    await expect(obstructionPage.overlappedIdInput()).toHaveValue('12345');

    // The Name field sits under a fixed overlay until it is scrolled into view.
    await obstructionPage.fillOverlappedName('QA Tester');
    await expect(obstructionPage.overlappedNameInput()).toHaveValue('QA Tester');
  });

  test('Scrollbars: the button deep inside a scrollable box is clickable', async () => {
    await obstructionPage.openScrollbars();

    // Playwright scrolls the element into view automatically before clicking.
    await obstructionPage.hidingButton().click();
    await expect(obstructionPage.hidingButton()).toBeVisible();
  });

  test('Scroll to Click: all four targets are reachable', async () => {
    await obstructionPage.openScrollToClick();

    // 1: far down the page. 2: inside a two-axis scrollable box.
    // 3: nested scroll containers. 4: only visible while its row is hovered.
    await obstructionPage.scrollTarget(1).click();
    await obstructionPage.scrollTarget(2).click();
    await obstructionPage.scrollTarget(3).click();

    await obstructionPage.hoverTargetRow();
    await obstructionPage.scrollTarget(4).click();

    await expect(obstructionPage.progressText()).toHaveText(Expected.scrollToClickComplete);
  });
});

test.describe('Visibility', () => {
  let obstructionPage: ObstructionPage;

  test.beforeEach(async ({ page }) => {
    obstructionPage = new ObstructionPage(page);
    await obstructionPage.openVisibility();
  });

  test('All eight buttons start visible', async () => {
    /**
     * The Hide button is counted here deliberately, and it is easy to miss.
     *
     * The challenge describes eight buttons: seven targets plus the control
     * that hides them. An earlier version of this test looped over the seven
     * targets only, so the title claimed eight while the assertion covered
     * seven - the kind of quiet gap that makes a suite report more coverage
     * than it has. Verified against the live page, which renders #hideButton
     * alongside the seven.
     */
    for (const name of [
      'hideButton',
      'removedButton',
      'zeroWidthButton',
      'overlappedButton',
      'transparentButton',
      'invisibleButton',
      'notDisplayedButton',
      'offscreenButton',
    ] as const) {
      await expect(obstructionPage.button(name)).toBeVisible();
    }
  });

  test('Each hiding technique produces a different observable state', async () => {
    await obstructionPage.hideButtons();

    // Removed from the DOM entirely.
    await expect(obstructionPage.button('removedButton')).toHaveCount(0);

    // Still in the DOM, but Playwright reports them as hidden - zero size,
    // visibility:hidden and display:none all collapse to "not visible".
    await expect(obstructionPage.button('zeroWidthButton')).toBeHidden();
    await expect(obstructionPage.button('invisibleButton')).toBeHidden();
    await expect(obstructionPage.button('notDisplayedButton')).toBeHidden();

    // opacity:0 is the awkward one: the element still occupies layout space,
    // so it counts as visible and only the computed style gives it away.
    await expect(obstructionPage.button('transparentButton')).toBeVisible();
    await expect(obstructionPage.button('transparentButton')).toHaveCSS('opacity', '0');

    // Moved off-screen but still rendered - also "visible" to the engine.
    await expect(obstructionPage.button('offscreenButton')).toBeVisible();
    await expect(obstructionPage.button('offscreenButton')).not.toBeInViewport();

    // Covered by another element: visible, but a click would be intercepted.
    await expect(obstructionPage.button('overlappedButton')).toBeVisible();
  });
});
