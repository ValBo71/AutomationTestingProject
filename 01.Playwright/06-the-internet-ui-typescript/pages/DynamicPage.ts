import { Page } from '@playwright/test';
import { BasePage } from '../core/basePage';
import { DynamicSelectors } from '../selectors/DynamicSelectors';
import { Routes } from '../data/testData';

/**
 * Covers the timing-sensitive challenges: Dynamic Loading, Dynamic Controls,
 * Dynamic Content, Disappearing Elements, Shifting Content, Slow Resources
 * and Infinite Scroll.
 */
export class DynamicPage extends BasePage {
  constructor(page: Page) {
    super(page);
  }

  // ----- /dynamic_loading/1 and /2 -----

  async openDynamicLoadingHidden() {
    await this.goto(Routes.dynamicLoadingHidden);
  }

  async openDynamicLoadingRendered() {
    await this.goto(Routes.dynamicLoadingRendered);
  }

  finishText() {
    return this.page.locator(DynamicSelectors.finishText);
  }

  async startLoading() {
    await this.page.locator(DynamicSelectors.startButton).click();
  }

  // ----- /dynamic_controls -----

  async openDynamicControls() {
    await this.goto(Routes.dynamicControls);
  }

  checkbox() {
    return this.page.locator(DynamicSelectors.checkbox);
  }

  textInput() {
    return this.page.locator(DynamicSelectors.textInput);
  }

  message() {
    return this.page.locator(DynamicSelectors.message);
  }

  async toggleCheckbox() {
    await this.page.locator(DynamicSelectors.removeAddButton).click();
  }

  async toggleInputEnabled() {
    await this.page.locator(DynamicSelectors.enableDisableButton).click();
  }

  // ----- /dynamic_content -----

  async openDynamicContent() {
    await this.goto(Routes.dynamicContent);
  }

  contentRows() {
    return this.page.locator(DynamicSelectors.contentRows);
  }

  /** Grabs the visible paragraph text so two page loads can be compared. */
  async getContentTextsAsync(): Promise<string[]> {
    return this.page.locator(DynamicSelectors.contentText).allInnerTexts();
  }

  // ----- /disappearing_elements -----

  async openDisappearingElements() {
    await this.goto(Routes.disappearingElements);
  }

  navLinks() {
    return this.page.locator(DynamicSelectors.navLinks);
  }

  async getNavLinkTextsAsync(): Promise<string[]> {
    return this.navLinks().allInnerTexts();
  }

  // ----- /shifting_content/menu_element -----

  async openShiftingContent() {
    await this.goto(Routes.shiftingContent);
  }

  shiftingMenuItems() {
    return this.page.locator(DynamicSelectors.shiftingMenu);
  }

  // ----- /slow -----

  async openSlowResources() {
    await this.goto(Routes.slowResources);
  }

  // ----- /infinite_scroll -----

  async openInfiniteScroll() {
    await this.goto(Routes.infiniteScroll);
  }

  scrollBlocks() {
    return this.page.locator(DynamicSelectors.scrollBlocks);
  }

  async scrollToBottom() {
    await this.page.evaluate(() => window.scrollTo(0, document.body.scrollHeight));
  }

  /**
   * Scrolls repeatedly until more blocks appear, instead of scrolling once and
   * hoping. jscroll only fetches when the viewport is near the bottom, and a
   * single scroll can land just short of that threshold - which made a
   * scroll-once version of this fail roughly one run in three.
   */
  async scrollUntilMoreContentAsync(previousCount: number, maxScrolls = 10): Promise<number> {
    for (let attempt = 0; attempt < maxScrolls; attempt++) {
      await this.scrollToBottom();

      try {
        await this.page.waitForFunction(
          ([selector, count]) => document.querySelectorAll(selector as string).length > (count as number),
          [DynamicSelectors.scrollBlocks, previousCount] as const,
          { timeout: 3000 }
        );
        return this.scrollBlocks().count();
      } catch {
        // Not loaded yet - scroll again on the next iteration.
      }
    }

    throw new Error(
      `Infinite scroll did not load more than ${previousCount} blocks after ${maxScrolls} scrolls.`
    );
  }
}
