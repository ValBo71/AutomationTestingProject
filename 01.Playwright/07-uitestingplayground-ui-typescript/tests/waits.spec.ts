import { test, expect } from '@playwright/test';
import { WaitPage } from '../pages/WaitPage';
import { Expected, TestData } from '../data/testData';

test.describe('Server and client delays', () => {
  let waitPage: WaitPage;

  test.beforeEach(async ({ page }) => {
    waitPage = new WaitPage(page);
  });

  test('AJAX Data: the label appears after the server responds', async () => {
    // The page stalls for ~15s on purpose, so this one spec gets a longer budget.
    test.setTimeout(60_000);
    await waitPage.openAjaxData();

    await waitPage.trigger();

    await expect(waitPage.loadedLabel()).toBeVisible({
      timeout: TestData.slowChallengeTimeout,
    });
    await expect(waitPage.loadedLabel()).toContainText(Expected.ajaxSuccessText);
  });

  test('Client Side Delay: the label appears after the client-side work finishes', async () => {
    test.setTimeout(60_000);
    await waitPage.openClientSideDelay();

    await waitPage.trigger();

    // Same shape as the AJAX case, but the delay is a busy loop in the browser
    // rather than a slow response - no network event to wait on.
    await expect(waitPage.loadedLabel()).toBeVisible({
      timeout: TestData.slowChallengeTimeout,
    });
    await expect(waitPage.loadedLabel()).toContainText(Expected.clientDelaySuccessText);
  });

  test('Load Delay: the button is present once the slow page has loaded', async () => {
    test.setTimeout(60_000);
    await waitPage.openLoadDelay();

    // goto() already waits for the load event, so no extra waiting is needed.
    await expect(waitPage.delayedButton()).toBeVisible();
    await waitPage.delayedButton().click();
  });
});

test.describe('Elements that become interactable later', () => {
  let waitPage: WaitPage;

  test.beforeEach(async ({ page }) => {
    waitPage = new WaitPage(page);
  });

  test('Auto Wait: clicking succeeds once the target becomes visible and enabled', async () => {
    await waitPage.openAutoWait();

    await waitPage.configureTarget({ visible: true, enabled: true, onTop: true });

    // No explicit sleep: Playwright's actionability checks hold the click until
    // the element is visible, enabled, stable and not covered.
    await waitPage.target().click();

    await expect(waitPage.operationStatus()).toContainText(Expected.autoWaitSuccess);
  });

  test('Auto Wait: a target scheduled 5 seconds out is still clicked without a sleep', async () => {
    await waitPage.openAutoWait();

    await waitPage.configureTarget({ delaySelector: '#applyButton5' });

    await waitPage.target().click({ timeout: 20_000 });

    await expect(waitPage.operationStatus()).toContainText(Expected.autoWaitSuccess);
  });

  test('Disabled Input: the field is disabled, then re-enabled after the delay', async () => {
    await waitPage.openDisabledInput();

    // The field starts enabled - the button disables it first and only
    // restores it a few seconds later, which is the trap: a test that clicks
    // and types straight away hits the disabled window.
    await expect(waitPage.disabledInputField()).toBeEnabled();

    await waitPage.requestEnable();

    await expect(waitPage.disabledInputField()).toBeDisabled();
    await expect(waitPage.operationStatus()).toContainText(Expected.disabledInputDisabled);

    await expect(waitPage.disabledInputField()).toBeEnabled({ timeout: 20_000 });
    await expect(waitPage.operationStatus()).toContainText(Expected.disabledInputEnabled);

    // Proving it is genuinely usable, not merely missing the disabled attribute.
    await waitPage.disabledInputField().fill('now editable');
    await expect(waitPage.disabledInputField()).toHaveValue('now editable');
  });

  test('Animated Button: the click only registers after the animation settles', async () => {
    await waitPage.openAnimatedButton();

    await waitPage.startAnimation();

    // Playwright waits for the element to be "stable" (two consecutive frames
    // at the same position) before clicking, which is what makes this pass.
    await waitPage.movingTarget().click();

    await expect(waitPage.operationStatus()).toContainText(Expected.animationSuccess);
  });

  test('Progress Bar: the bar is stopped close to the requested value', async () => {
    test.setTimeout(90_000);
    await waitPage.openProgressBar();

    await waitPage.stopAtAsync(75);

    const value = await waitPage.getProgressValueAsync();

    // A small overshoot is unavoidable - the bar keeps ticking between the
    // poll and the click - so the assertion allows a tolerance instead of
    // demanding an exact 75.
    expect(value).toBeGreaterThanOrEqual(75);
    expect(value).toBeLessThanOrEqual(80);
  });
});
