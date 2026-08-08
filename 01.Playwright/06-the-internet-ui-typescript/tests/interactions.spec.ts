import { test, expect } from '@playwright/test';
import { InteractionPage } from '../pages/InteractionPage';
import { InteractionSelectors } from '../selectors/InteractionSelectors';
import { Expected } from '../data/testData';

test.describe('Mouse interactions', () => {
  let interactionPage: InteractionPage;

  test.beforeEach(async ({ page }) => {
    interactionPage = new InteractionPage(page);
  });

  test('Drag and Drop: columns A and B swap places', async () => {
    await interactionPage.openDragAndDrop();

    expect(await interactionPage.getColumnHeaderAsync(InteractionSelectors.columnA)).toBe('A');
    expect(await interactionPage.getColumnHeaderAsync(InteractionSelectors.columnB)).toBe('B');

    await interactionPage.dragColumnAOntoB();

    // The elements keep their ids; it is their contents that swap.
    expect(await interactionPage.getColumnHeaderAsync(InteractionSelectors.columnA)).toBe('B');
    expect(await interactionPage.getColumnHeaderAsync(InteractionSelectors.columnB)).toBe('A');
  });

  test('Context Menu: right-clicking the hot spot raises a JS alert', async ({ page }) => {
    await interactionPage.openContextMenu();

    let alertText = '';
    page.once('dialog', async (dialog) => {
      alertText = dialog.message();
      await dialog.accept();
    });

    await interactionPage.rightClickHotSpot();

    await expect.poll(() => alertText).toBe(Expected.contextMenuAlert);
  });

  test('Hovers: hovering an avatar reveals its caption', async () => {
    await interactionPage.openHovers();

    await expect(interactionPage.figures()).toHaveCount(3);

    // Captions exist in the DOM but are hidden until hover, so visibility -
    // not presence - is the meaningful assertion.
    const firstCaption = interactionPage.figureCaption(0);
    await expect(firstCaption).toBeHidden();

    await interactionPage.hoverOverFigure(0);

    await expect(firstCaption).toBeVisible();
    await expect(firstCaption).toContainText('name: user1');
  });

  test('JQuery UI Menus: a submenu opens on hover and its items become clickable', async () => {
    await interactionPage.openJqueryUiMenu();

    await expect(interactionPage.downloadsMenuItem()).toBeHidden();

    await interactionPage.openDownloadsSubmenu();

    await expect(interactionPage.downloadsMenuItem()).toBeVisible();
  });

  test('Floating Menu: the menu stays visible after scrolling down', async () => {
    await interactionPage.openFloatingMenu();

    const menu = interactionPage.floatingMenu();
    await expect(menu).toBeVisible();

    await interactionPage.scrollToBottom();

    // The whole point of a floating menu: it must survive the scroll.
    await expect(menu).toBeInViewport();
  });
});
