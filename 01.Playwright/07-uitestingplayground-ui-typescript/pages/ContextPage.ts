import { FrameLocator, Page } from '@playwright/test';
import { BasePage } from '../core/basePage';
import { ContextSelectors } from '../selectors/Selectors';
import { Routes } from '../data/testData';

/**
 * Challenges that need a context switch before the element is reachable:
 * Frames (nested), Shadow DOM and File Upload (uploader inside an iframe).
 */
export class ContextPage extends BasePage {
  constructor(page: Page) {
    super(page);
  }

  // ----- /frames -----

  async openFrames() {
    await this.goto(Routes.frames);
  }

  outerFrame(): FrameLocator {
    return this.page.frameLocator(ContextSelectors.outerFrame);
  }

  /**
   * The inner frame is nested inside the outer one and holds *identical*
   * button markup, so a page-level locator would match the outer frame's
   * copy. The chained frameLocator is what disambiguates them.
   */
  innerFrame(): FrameLocator {
    return this.outerFrame().frameLocator(ContextSelectors.innerFrame);
  }

  // ----- /shadowdom -----

  async openShadowDom() {
    await this.goto(Routes.shadowDom);
  }

  /**
   * The generator's controls live in an open shadow root. Playwright pierces
   * open roots automatically, so these read like ordinary locators - in
   * Selenium the same steps would need an explicit getShadowRoot() hop.
   */
  guidField() {
    return this.page.locator(ContextSelectors.guidEditField);
  }

  async generateGuid() {
    await this.page.locator(ContextSelectors.guidGenerateButton).click();
  }

  async copyGuid() {
    await this.page.locator(ContextSelectors.guidCopyButton).click();
  }

  async getGuidAsync(): Promise<string> {
    return this.guidField().inputValue();
  }

  // ----- /upload -----

  async openFileUpload() {
    await this.goto(Routes.fileUpload);
  }

  uploadFrame(): FrameLocator {
    return this.page.frameLocator(ContextSelectors.uploadFrame);
  }

  /**
   * The <input type="file"> is hidden behind a styled "Browse files" control.
   * setInputFiles works on hidden inputs, so there is no need to click the
   * visible button or simulate a drag-and-drop.
   */
  async uploadFile(filePath: string) {
    await this.uploadFrame().locator(ContextSelectors.uploadFileInput).setInputFiles(filePath);
  }

  uploadFrameBody() {
    return this.uploadFrame().locator('body');
  }
}
