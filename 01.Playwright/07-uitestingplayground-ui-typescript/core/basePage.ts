import { Page } from '@playwright/test';

/**
 * Shared behaviour for every page object in this suite.
 *
 * Each challenge lives at its own route and follows the same layout: an <h3>
 * title, an explanation, and a "Playground" section holding the elements under
 * test. Several pages report the outcome of an action in a #opstatus div.
 */
export class BasePage {
  protected page: Page;

  constructor(page: Page) {
    this.page = page;
  }

  /** Navigates to a path relative to the baseURL configured in playwright.config.ts. */
  async goto(path: string) {
    await this.page.goto(path);
  }

  /** The <h3> heading every challenge page renders. */
  heading() {
    return this.page.locator('h3');
  }

  /** The status line several challenges use to confirm an action succeeded. */
  operationStatus() {
    return this.page.locator('#opstatus');
  }
}
