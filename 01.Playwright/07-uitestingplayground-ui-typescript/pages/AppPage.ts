import { Page } from '@playwright/test';
import { BasePage } from '../core/basePage';
import { AppSelectors } from '../selectors/Selectors';
import { Routes } from '../data/testData';

/**
 * The remaining interactive challenges: Sample App, Alerts, Select,
 * Clear Input, Mouse Over, Dynamic Table and Geo Location.
 */
export class AppPage extends BasePage {
  constructor(page: Page) {
    super(page);
  }

  // ----- /sampleapp -----

  async openSampleApp() {
    await this.goto(Routes.sampleApp);
  }

  loginStatus() {
    return this.page.locator(AppSelectors.loginStatus);
  }

  async login(username: string, password: string) {
    await this.page.locator(AppSelectors.username).fill(username);
    await this.page.locator(AppSelectors.password).fill(password);
    await this.page.locator(AppSelectors.loginButton).click();
  }

  // ----- /alerts -----

  async openAlerts() {
    await this.goto(Routes.alerts);
  }

  async clickAlert() {
    await this.page.locator(AppSelectors.alertButton).click();
  }

  async clickConfirm() {
    await this.page.locator(AppSelectors.confirmButton).click();
  }

  async clickPrompt() {
    await this.page.locator(AppSelectors.promptButton).click();
  }

  // ----- /select -----

  async openSelect() {
    await this.goto(Routes.select);
  }

  languageSelect() {
    return this.page.locator(AppSelectors.languageSelect);
  }

  languageStatus() {
    return this.page.locator(AppSelectors.languageStatus);
  }

  citySelect() {
    return this.page.locator(AppSelectors.citySelect);
  }

  cityStatus() {
    return this.page.locator(AppSelectors.cityStatus);
  }

  // ----- /clearinput -----

  async openClearInput() {
    await this.goto(Routes.clearInput);
  }

  clearTargets() {
    return this.page.locator(AppSelectors.clearTargets);
  }

  textInput() {
    return this.page.locator(AppSelectors.clearTextInput);
  }

  numberInput() {
    return this.page.locator(AppSelectors.clearNumberInput);
  }

  contentEditable() {
    return this.page.locator(AppSelectors.clearContentEditable);
  }

  /**
   * fill() only works on form controls, so a contenteditable div has to be
   * emptied the way a user would: select everything, then delete.
   */
  async clearContentEditable() {
    const editable = this.contentEditable();
    await editable.click();
    await this.page.keyboard.press('ControlOrMeta+a');
    await this.page.keyboard.press('Delete');
  }

  // ----- /mouseover -----

  async openMouseOver() {
    await this.goto(Routes.mouseOver);
  }

  /**
   * Hovering replaces the anchor in the DOM, so a locator captured before the
   * hover points at a detached node - the classic stale-element trap. Because
   * Playwright locators resolve lazily on each use, re-querying by title after
   * the hover is enough to stay attached to the live element.
   */
  mouseOverLink(title: string) {
    return this.page.locator(`a[title="${title}"]`);
  }

  clickCount() {
    return this.page.locator(AppSelectors.clickCount);
  }

  clickButtonCount() {
    return this.page.locator(AppSelectors.clickButtonCount);
  }

  // ----- /dynamictable -----

  async openDynamicTable() {
    await this.goto(Routes.dynamicTable);
  }

  columnHeaders() {
    return this.page.locator(AppSelectors.columnHeader);
  }

  chromeCpuLabel() {
    return this.page.locator(AppSelectors.chromeCpuLabel);
  }

  /**
   * Both the column order and the row order are randomised on every load, so
   * the value has to be found by matching header text to cell position rather
   * than by fixed coordinates.
   */
  async getCpuValueForAsync(processName: string): Promise<string> {
    const headers = await this.columnHeaders().allInnerTexts();
    const cpuColumnIndex = headers.findIndex((h) => h.trim() === 'CPU');
    if (cpuColumnIndex === -1) {
      throw new Error(`No "CPU" column found. Headers were: ${headers.join(', ')}`);
    }

    const rows = this.page.locator(AppSelectors.tableRow);
    const rowCount = await rows.count();

    for (let i = 0; i < rowCount; i++) {
      const cells = rows.nth(i).locator(AppSelectors.tableCell);
      if ((await cells.count()) === 0) continue;

      const values = await cells.allInnerTexts();
      const nameColumnIndex = headers.findIndex((h) => h.trim() === 'Name');
      if (values[nameColumnIndex]?.trim() === processName) {
        return values[cpuColumnIndex].trim();
      }
    }

    throw new Error(`Process "${processName}" was not found in the table.`);
  }

  // ----- /geolocation -----

  async openGeoLocation() {
    await this.goto(Routes.geoLocation);
  }

  async requestLocation() {
    await this.page.locator(AppSelectors.requestLocationButton).click();
  }

  locationOutput() {
    return this.page.locator(AppSelectors.locationOutput);
  }
}
