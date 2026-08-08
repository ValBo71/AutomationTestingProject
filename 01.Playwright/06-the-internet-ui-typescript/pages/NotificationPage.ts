import { Page } from '@playwright/test';
import { BasePage } from '../core/basePage';
import { NotificationSelectors } from '../selectors/InteractionSelectors';
import { Routes } from '../data/testData';

/** Covers JavaScript Alerts, Entry Ad, Exit Intent and Notification Messages. */
export class NotificationPage extends BasePage {
  constructor(page: Page) {
    super(page);
  }

  // ----- /javascript_alerts -----

  async openJavaScriptAlerts() {
    await this.goto(Routes.javaScriptAlerts);
  }

  result() {
    return this.page.locator(NotificationSelectors.result);
  }

  async clickAlertButton() {
    await this.page.locator(NotificationSelectors.alertButton).click();
  }

  async clickConfirmButton() {
    await this.page.locator(NotificationSelectors.confirmButton).click();
  }

  async clickPromptButton() {
    await this.page.locator(NotificationSelectors.promptButton).click();
  }

  // ----- /entry_ad -----

  async openEntryAd() {
    await this.goto(Routes.entryAd);
  }

  modal() {
    return this.page.locator(NotificationSelectors.modal);
  }

  modalTitle() {
    return this.page.locator(NotificationSelectors.modalTitle);
  }

  async closeModal() {
    await this.page.locator(NotificationSelectors.modalClose).click();
  }

  // ----- /exit_intent -----

  async openExitIntent() {
    await this.goto(Routes.exitIntent);
  }

  exitIntentModal() {
    return this.page.locator(NotificationSelectors.exitIntentModal);
  }

  /**
   * The page uses the ouibounce library, which listens for a mouseleave on
   * documentElement near the top of the viewport - the gesture of a user
   * heading for the address bar. Real mouse movement alone does not leave the
   * viewport in a headless run, so the event is dispatched explicitly with the
   * clientY that ouibounce checks for.
   */
  async triggerExitIntent() {
    await this.page.mouse.move(400, 300);
    await this.page.mouse.move(400, 0);
    await this.page.evaluate(() => {
      document.documentElement.dispatchEvent(
        new MouseEvent('mouseleave', { bubbles: true, clientY: 0 })
      );
    });
  }

  // ----- /notification_message_rendered -----

  async openNotificationMessage() {
    await this.goto(Routes.notificationMessage);
  }

  async requestNewMessage() {
    await this.page.locator(NotificationSelectors.newMessageLink).click();
  }
}
