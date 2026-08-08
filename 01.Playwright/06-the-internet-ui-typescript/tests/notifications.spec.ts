import { test, expect } from '@playwright/test';
import { NotificationPage } from '../pages/NotificationPage';
import { Expected } from '../data/testData';

test.describe('JavaScript dialogs', () => {
  let notificationPage: NotificationPage;

  test.beforeEach(async ({ page }) => {
    notificationPage = new NotificationPage(page);
    await notificationPage.openJavaScriptAlerts();
  });

  test('JS Alert: the alert is accepted and the result is reported', async ({ page }) => {
    let message = '';
    page.once('dialog', async (dialog) => {
      message = dialog.message();
      expect(dialog.type()).toBe('alert');
      await dialog.accept();
    });

    await notificationPage.clickAlertButton();

    expect(message).toBe(Expected.jsAlertText);
    await expect(notificationPage.result()).toHaveText(Expected.jsAlertResult);
  });

  test('JS Confirm: accepting reports Ok', async ({ page }) => {
    page.once('dialog', async (dialog) => {
      expect(dialog.type()).toBe('confirm');
      await dialog.accept();
    });

    await notificationPage.clickConfirmButton();

    await expect(notificationPage.result()).toHaveText(Expected.jsConfirmOkResult);
  });

  test('JS Confirm: dismissing reports Cancel', async ({ page }) => {
    page.once('dialog', async (dialog) => {
      await dialog.dismiss();
    });

    await notificationPage.clickConfirmButton();

    await expect(notificationPage.result()).toHaveText(Expected.jsConfirmCancelResult);
  });

  test('JS Prompt: the typed text is echoed back', async ({ page }) => {
    const typedText = 'Playwright was here';

    page.once('dialog', async (dialog) => {
      expect(dialog.type()).toBe('prompt');
      await dialog.accept(typedText);
    });

    await notificationPage.clickPromptButton();

    await expect(notificationPage.result()).toHaveText(`You entered: ${typedText}`);
  });

  test('JS Prompt: dismissing reports null', async ({ page }) => {
    page.once('dialog', async (dialog) => {
      await dialog.dismiss();
    });

    await notificationPage.clickPromptButton();

    await expect(notificationPage.result()).toHaveText('You entered: null');
  });
});

test.describe('Modals and banners', () => {
  let notificationPage: NotificationPage;

  test.beforeEach(async ({ page }) => {
    notificationPage = new NotificationPage(page);
  });

  test('Entry Ad: the modal appears on load and can be dismissed', async () => {
    await notificationPage.openEntryAd();

    await expect(notificationPage.modal()).toBeVisible();
    await expect(notificationPage.modalTitle()).toHaveText(Expected.entryAdModalTitle);

    await notificationPage.closeModal();

    await expect(notificationPage.modal()).toBeHidden();
  });

  test('Exit Intent: the modal appears when the pointer leaves the viewport', async () => {
    await notificationPage.openExitIntent();

    // The ouibounce wrapper is present from the start but kept off-screen,
    // so "hidden" - not "absent" - is the correct initial assertion.
    await expect(notificationPage.exitIntentModal()).toBeHidden();

    await notificationPage.triggerExitIntent();

    await expect(notificationPage.exitIntentModal()).toBeVisible();
    await expect(notificationPage.modalTitle()).toHaveText(Expected.entryAdModalTitle);
  });

  test('Notification Message: each load shows one of the known messages', async ({ page }) => {
    // Entry is through /notification_message, which sets the flash and
    // redirects; loading the _rendered page directly leaves the banner empty.
    await notificationPage.openNotificationMessage();
    await expect(page).toHaveURL(/notification_message_rendered/);

    const message = await notificationPage.getFlashTextAsync();

    // The message is picked at random, so the assertion is on the known set.
    // Note the site's own typo in "unsuccesful" - kept verbatim on purpose.
    expect(Expected.notificationMessages).toContain(message);

    await notificationPage.requestNewMessage();

    const nextMessage = await notificationPage.getFlashTextAsync();
    expect(Expected.notificationMessages).toContain(nextMessage);
  });
});
