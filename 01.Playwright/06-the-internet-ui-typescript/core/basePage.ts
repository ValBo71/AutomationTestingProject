import { Page } from '@playwright/test';

/**
 * Shared behaviour for every page object in this suite.
 *
 * Each challenge on the-internet lives at its own path, so the base class owns
 * the navigation and the two header lookups that every example page shares.
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

  /** The <h3> heading every example page renders above its content. */
  heading() {
    return this.page.locator('h3');
  }

  /** The green/red banner the site uses to report the outcome of an action. */
  flashMessage() {
    return this.page.locator('#flash');
  }

  /**
   * The flash banner keeps a trailing "x" close button in its text node, and
   * the site pads it with newlines - both are noise for an assertion.
   */
  async getFlashTextAsync(): Promise<string> {
    const raw = await this.flashMessage().innerText();
    return raw.replace('×', '').trim();
  }
}
