import { Page } from '@playwright/test';
import { BasePage } from '../core/basePage';
import { NavigationSelectors } from '../selectors/NavigationSelectors';
import { Routes } from '../data/testData';

/**
 * Covers Multiple Windows, Redirect Link, Status Codes, File Upload,
 * File Download and Geolocation.
 */
export class NavigationPage extends BasePage {
  constructor(page: Page) {
    super(page);
  }

  // ----- /windows -----

  async openMultipleWindows() {
    await this.goto(Routes.multipleWindows);
  }

  /**
   * Waits for the popup event before clicking, otherwise the new page can open
   * faster than the listener is attached and the wait would hang.
   */
  async openNewWindowAsync(): Promise<Page> {
    const [popup] = await Promise.all([
      this.page.context().waitForEvent('page'),
      this.page.locator(NavigationSelectors.newWindowLink).click(),
    ]);
    await popup.waitForLoadState();
    return popup;
  }

  // ----- /redirector -----

  async openRedirector() {
    await this.goto(Routes.redirectLink);
  }

  async followRedirect() {
    await this.page.locator(NavigationSelectors.redirectLink).click();
  }

  // ----- /status_codes -----

  async openStatusCodes() {
    await this.goto(Routes.statusCodes);
  }

  async openStatusCode(code: number) {
    await this.goto(`${Routes.statusCodes}/${code}`);
  }

  // ----- /upload -----

  async openFileUpload() {
    await this.goto(Routes.fileUpload);
  }

  async uploadFile(filePath: string) {
    await this.page.locator(NavigationSelectors.fileInput).setInputFiles(filePath);
    await this.page.locator(NavigationSelectors.uploadSubmit).click();
  }

  uploadedFiles() {
    return this.page.locator(NavigationSelectors.uploadedFiles);
  }

  // ----- /download -----

  async openFileDownload() {
    await this.goto(Routes.fileDownload);
  }

  downloadLinks() {
    return this.page.locator(NavigationSelectors.downloadLinks);
  }

  // ----- /geolocation -----

  async openGeolocation() {
    await this.goto(Routes.geolocation);
  }

  async requestLocation() {
    await this.page.locator(NavigationSelectors.whereAmIButton).click();
  }

  latitude() {
    return this.page.locator(NavigationSelectors.latitude);
  }

  longitude() {
    return this.page.locator(NavigationSelectors.longitude);
  }
}
