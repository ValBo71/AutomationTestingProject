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
   * The page uses the HTML5 drag-and-drop API. A real pointer drag through
   * dragTo() drives it - verified against the live page - so no events are
   * dispatched by hand. An earlier version faked the drag with synthetic
   * DragEvents, which proved only that the page's JavaScript handler works,
   * not that a user dragging the column gets the same result.
   */
  async dragColumnAOntoB() {
    await this.columnA().dragTo(this.columnB());
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

  pdfMenuItem() {
    return this.page.locator(InteractionSelectors.pdfDownload);
  }

  /** Downloads opens its own submenu on hover, one level below Enabled. */
  async openPdfItem() {
    await this.openDownloadsSubmenu();
    await this.downloadsMenuItem().hover();
    await this.pdfMenuItem().waitFor({ state: 'visible' });
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
