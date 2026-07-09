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
    await this.page.locator(LoginSelectors.usernameInput).fill(username);
    await this.page.locator(LoginSelectors.passwordInput).fill(password);
    await this.page.locator(LoginSelectors.loginButton).click();
  }
}
