import { Page } from '@playwright/test';
import { BasePage } from '../core/basePage';
import { InteractionSelectors } from '../selectors/InteractionSelectors';
import { Routes } from '../data/testData';

/** Covers Drag and Drop, Context Menu, Hovers, JQuery UI Menus and Floating Menu. */
export class InteractionPage extends BasePage {
  constructor(page: Page) {
    super(page);
  }

  // ----- /drag_and_drop -----

  async openDragAndDrop() {
    await this.goto(Routes.dragAndDrop);
  }

  columnA() {
    return this.page.locator(InteractionSelectors.columnA);
  }

  columnB() {
    return this.page.locator(InteractionSelectors.columnB);
  }

  /**
   * The page uses the HTML5 drag-and-drop API, which Playwright's dragTo()
   * does not reliably trigger here. Dispatching the events manually is the
   * documented workaround and is what makes this challenge non-trivial.
   */
  async dragColumnAOntoB() {
    await this.page.evaluate(
      ([sourceSelector, targetSelector]) => {
        const source = document.querySelector(sourceSelector);
        const target = document.querySelector(targetSelector);
        if (!source || !target) {
          throw new Error('Drag and drop columns were not found on the page.');
        }

        const dataTransfer = new DataTransfer();
        source.dispatchEvent(new DragEvent('dragstart', { bubbles: true, dataTransfer }));
        target.dispatchEvent(new DragEvent('dragenter', { bubbles: true, dataTransfer }));
        target.dispatchEvent(new DragEvent('dragover', { bubbles: true, dataTransfer }));
        target.dispatchEvent(new DragEvent('drop', { bubbles: true, dataTransfer }));
        source.dispatchEvent(new DragEvent('dragend', { bubbles: true, dataTransfer }));
      },
      [InteractionSelectors.columnA, InteractionSelectors.columnB]
    );
  }

  async getColumnHeaderAsync(columnSelector: string): Promise<string> {
    return (
      await this.page.locator(`${columnSelector} ${InteractionSelectors.columnHeader}`).innerText()
    ).trim();
  }

  // ----- /context_menu -----

  async openContextMenu() {
    await this.goto(Routes.contextMenu);
  }

  hotSpot() {
    return this.page.locator(InteractionSelectors.hotSpot);
  }

  async rightClickHotSpot() {
    await this.hotSpot().click({ button: 'right' });
  }

  // ----- /hovers -----

  async openHovers() {
    await this.goto(Routes.hovers);
  }

  figures() {
    return this.page.locator(InteractionSelectors.figures);
  }

  figureCaption(index: number) {
    return this.figures().nth(index).locator(InteractionSelectors.figureCaption);
  }

  async hoverOverFigure(index: number) {
    await this.figures().nth(index).hover();
  }

  // ----- /jqueryui/menu -----

  async openJqueryUiMenu() {
    await this.goto(Routes.jqueryUiMenu);
  }

  enabledMenuItem() {
    return this.page.locator(InteractionSelectors.enabledMenuItem);
  }

  downloadsMenuItem() {
    return this.page.locator(InteractionSelectors.downloadsMenuItem);
  }

  /** "Enabled" opens a submenu on hover, and only then is "Downloads" reachable. */
  async openDownloadsSubmenu() {
    await this.enabledMenuItem().hover();
    await this.downloadsMenuItem().waitFor({ state: 'visible' });
  }

  // ----- /floating_menu -----

  async openFloatingMenu() {
    await this.goto(Routes.floatingMenu);
  }

  floatingMenu() {
    return this.page.locator(InteractionSelectors.floatingMenu);
  }

  async scrollToBottom() {
    await this.page.evaluate(() => window.scrollTo(0, document.body.scrollHeight));
  }
}
