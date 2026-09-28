import { Page } from '@playwright/test';
import { BasePage } from '../core/basePage';
import { LoginSelectors } from '../selectors/LoginSelectors';

export class LoginPage extends BasePage {
  constructor(page: Page) {
    super(page);
  }

  async open() {
    await this.page.goto('/');
  }

  async login(username: string, password: string) {
    await this.usernameInput().fill(username);
    await this.page.locator(LoginSelectors.passwordInput).fill(password);
    await this.loginButton().click();
  }

  /**
   * Locators rather than read-once values, so tests assert with `expect(...)` and get Playwright's
   * auto-retry: the error banner is rendered after the click, and an `innerText()` read straight away
   * could run before it appears.
   */
  loginLogo() {
    return this.page.locator(LoginSelectors.loginLogo);
  }

  usernameInput() {
    return this.page.locator(LoginSelectors.usernameInput);
  }

  loginButton() {
    return this.page.locator(LoginSelectors.loginButton);
  }

  errorMessage() {
    return this.page.locator(LoginSelectors.errorMessage);
  }
}
