import { Page } from '@playwright/test';
import { BasePage } from '../core/basePage';
import { AuthSelectors } from '../selectors/FormSelectors';
import { Routes } from '../data/testData';

/** Covers Form Authentication, Forgot Password, Basic Auth and Digest Auth. */
export class AuthPage extends BasePage {
  constructor(page: Page) {
    super(page);
  }

  // ----- /login -----

  async openLogin() {
    await this.goto(Routes.formAuthentication);
  }

  async login(username: string, password: string) {
    await this.page.locator(AuthSelectors.username).fill(username);
    await this.page.locator(AuthSelectors.password).fill(password);
    await this.page.locator(AuthSelectors.submitButton).click();
  }

  logoutButton() {
    return this.page.locator(AuthSelectors.logoutButton);
  }

  async logout() {
    await this.logoutButton().click();
  }

  secureAreaHeading() {
    return this.page.locator(AuthSelectors.secureAreaHeading);
  }

  // ----- /forgot_password -----

  async openForgotPassword() {
    await this.goto(Routes.forgotPassword);
  }

  async requestPasswordReset(email: string) {
    await this.page.locator(AuthSelectors.email).fill(email);
    await this.page.locator(AuthSelectors.forgotPasswordSubmit).click();
  }

  confirmationHeading() {
    return this.page.locator(AuthSelectors.confirmationHeading);
  }
}
