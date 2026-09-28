import { Page } from '@playwright/test';
import { BasePage } from '../core/basePage';
import { MainPageSelectors } from '../selectors/MainPageSelectors';

/** The product list a user lands on after logging in. */
export class InventoryPage extends BasePage {
  constructor(page: Page) {
    super(page);
  }

  inventoryContainer() {
    return this.page.locator(MainPageSelectors.inventoryContainer).first();
  }

  appLogo() {
    return this.page.locator(MainPageSelectors.appLogo);
  }
}
