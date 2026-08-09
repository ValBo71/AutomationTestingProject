import { Page } from '@playwright/test';
import { BasePage } from '../core/basePage';
import { WaitSelectors } from '../selectors/Selectors';
import { Routes } from '../data/testData';

/**
 * Timing challenges: AJAX Data, Client Side Delay, Load Delay, Auto Wait,
 * Progress Bar, Disabled Input and Animated Button.
 */
export class WaitPage extends BasePage {
  constructor(page: Page) {
    super(page);
  }

  // ----- /ajax and /clientdelay -----

  async openAjaxData() {
    await this.goto(Routes.ajaxData);
  }

  async openClientSideDelay() {
    await this.goto(Routes.clientSideDelay);
  }

  triggerButton() {
    return this.page.locator(WaitSelectors.triggerButton);
  }

  loadedLabel() {
    return this.page.locator(WaitSelectors.loadedLabel);
  }

  async trigger() {
    await this.triggerButton().click();
  }

  // ----- /loaddelay -----

  async openLoadDelay() {
    await this.goto(Routes.loadDelay);
  }

  delayedButton() {
    return this.page.locator(WaitSelectors.delayedButton);
  }

  // ----- /autowait -----

  async openAutoWait() {
    await this.goto(Routes.autoWait);
  }

  target() {
    return this.page.locator(WaitSelectors.target);
  }

  /**
   * Configures the target element and schedules it to become interactable
   * after a delay. Playwright's built-in actionability checks then do the
   * waiting - no explicit sleep required, which is the lesson of this page.
   */
  async configureTarget(options: {
    elementType?: string;
    visible?: boolean;
    enabled?: boolean;
    onTop?: boolean;
    delaySelector?: string;
  }) {
    const {
      elementType = 'button',
      visible = true,
      enabled = true,
      onTop = true,
      delaySelector = WaitSelectors.applyAfter3Seconds,
    } = options;

    await this.page.locator(WaitSelectors.elementTypeSelect).selectOption(elementType);
    await this.page.locator(WaitSelectors.visibleCheckbox).setChecked(visible);
    await this.page.locator(WaitSelectors.enabledCheckbox).setChecked(enabled);
    await this.page.locator(WaitSelectors.onTopCheckbox).setChecked(onTop);

    await this.page.locator(delaySelector).click();
  }

  // ----- /progressbar -----

  async openProgressBar() {
    await this.goto(Routes.progressBar);
  }

  progressBar() {
    return this.page.locator(WaitSelectors.progressBar);
  }

  async startProgress() {
    await this.page.locator(WaitSelectors.startButton).click();
  }

  async stopProgress() {
    await this.page.locator(WaitSelectors.stopButton).click();
  }

  async getProgressValueAsync(): Promise<number> {
    const text = (await this.progressBar().innerText()).replace('%', '').trim();
    return Number(text);
  }

  /**
   * Stops the bar as close to the requested value as possible. Polling the
   * DOM in a loop is the whole point: a fixed sleep would overshoot on a fast
   * machine and undershoot on a slow one.
   */
  async stopAtAsync(targetPercent: number) {
    await this.startProgress();
    await this.page.waitForFunction(
      ([selector, target]) => {
        const bar = document.querySelector(selector as string);
        if (!bar) return false;
        return Number((bar.textContent ?? '').replace('%', '').trim()) >= (target as number);
      },
      [WaitSelectors.progressBar, targetPercent] as const,
      { timeout: 40_000 }
    );
    await this.stopProgress();
  }

  // ----- /disabledinput -----

  async openDisabledInput() {
    await this.goto(Routes.disabledInput);
  }

  disabledInputField() {
    return this.page.locator(WaitSelectors.disabledInputField);
  }

  async requestEnable() {
    await this.page.locator(WaitSelectors.enableButton).click();
  }

  // ----- /animation -----

  async openAnimatedButton() {
    await this.goto(Routes.animatedButton);
  }

  movingTarget() {
    return this.page.locator(WaitSelectors.movingTarget);
  }

  async startAnimation() {
    await this.page.locator(WaitSelectors.startAnimationButton).click();
  }
}
