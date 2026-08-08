import { Page } from '@playwright/test';
import { BasePage } from '../core/basePage';
import { DomSelectors } from '../selectors/DynamicSelectors';
import { Routes } from '../data/testData';

/**
 * Covers the DOM-structure challenges: Challenging DOM, Large & Deep DOM,
 * Sortable Data Tables, Shadow DOM, Broken Images and Typos.
 */
export class DomPage extends BasePage {
  constructor(page: Page) {
    super(page);
  }

  // ----- /challenging_dom -----

  async openChallengingDom() {
    await this.goto(Routes.challengingDom);
  }

  /**
   * The three buttons carry randomly regenerated UUID ids, so any test that
   * locked onto an id would break on the next page load. Position is the only
   * stable handle here - that is the whole point of this challenge.
   */
  challengeButtons() {
    return this.page.locator(DomSelectors.challengeButtons);
  }

  challengeTableRows() {
    return this.page.locator(DomSelectors.challengeTableRows);
  }

  canvas() {
    return this.page.locator(DomSelectors.canvas);
  }

  async getCellTextAsync(row: number, column: number): Promise<string> {
    return (
      await this.challengeTableRows().nth(row).locator('td').nth(column).innerText()
    ).trim();
  }

  // ----- /large -----

  async openLargeAndDeepDom() {
    await this.goto(Routes.largeAndDeepDom);
  }

  deepestNoSiblingsElement() {
    return this.page.locator(DomSelectors.deepestNoSiblings);
  }

  largeTable() {
    return this.page.locator(DomSelectors.siblingTable);
  }

  // ----- /tables -----

  async openSortableTables() {
    await this.goto(Routes.sortableDataTables);
  }

  table1() {
    return this.page.locator(DomSelectors.table1);
  }

  /** Reads one column out of a table so the spec can assert the sort order. */
  async getColumnValuesAsync(tableSelector: string, columnIndex: number): Promise<string[]> {
    return this.page
      .locator(`${tableSelector} ${DomSelectors.tableRows} td:nth-child(${columnIndex + 1})`)
      .allInnerTexts();
  }

  async sortByColumn(tableSelector: string, columnIndex: number) {
    await this.page
      .locator(`${tableSelector} ${DomSelectors.tableHeaders}`)
      .nth(columnIndex)
      .click();
  }

  // ----- /shadowdom -----

  async openShadowDom() {
    await this.goto(Routes.shadowDom);
  }

  /**
   * Playwright pierces open shadow roots automatically, so a plain text
   * locator reaches the slotted content without any special traversal.
   */
  shadowHost() {
    return this.page.locator(DomSelectors.shadowHost);
  }

  // ----- /broken_images -----

  async openBrokenImages() {
    await this.goto(Routes.brokenImages);
  }

  images() {
    return this.page.locator(DomSelectors.images);
  }

  /**
   * A broken <img> still exists in the DOM and is "visible", so visibility
   * assertions prove nothing. naturalWidth === 0 is what actually identifies
   * an image the browser failed to load.
   */
  async getBrokenImageCountAsync(): Promise<number> {
    return this.page.locator(DomSelectors.images).evaluateAll(
      (images) => images.filter((img) => (img as HTMLImageElement).naturalWidth === 0).length
    );
  }

  // ----- /typos -----

  async openTypos() {
    await this.goto(Routes.typos);
  }

  typoParagraph() {
    return this.page.locator(DomSelectors.typoParagraph);
  }
}
