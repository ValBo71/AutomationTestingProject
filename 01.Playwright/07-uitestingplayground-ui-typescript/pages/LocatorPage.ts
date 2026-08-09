import { Page } from '@playwright/test';
import { BasePage } from '../core/basePage';
import { LocatorSelectors } from '../selectors/Selectors';
import { Routes } from '../data/testData';

/**
 * Challenges about picking a locator that survives the page's tricks:
 * Dynamic ID, Class Attribute, Non-Breaking Space, Text Input, Verify Text
 * and CSS Selectors.
 */
export class LocatorPage extends BasePage {
  constructor(page: Page) {
    super(page);
  }

  // ----- /dynamicid -----

  async openDynamicId() {
    await this.goto(Routes.dynamicId);
  }

  /** Located by class, never by id - the id is a fresh GUID on every load. */
  dynamicIdButton() {
    return this.page.locator(LocatorSelectors.dynamicIdButton);
  }

  // ----- /classattr -----

  async openClassAttribute() {
    await this.goto(Routes.classAttribute);
  }

  /**
   * All three buttons share "btn" and "btn-test"; only the colour class is
   * unique. An exact class match (`[@class='btn-primary']`) finds nothing,
   * which is the trap the page is built around.
   */
  classAttributeButton() {
    return this.page.locator(LocatorSelectors.classAttributeButton);
  }

  // ----- /nbsp -----

  async openNonBreakingSpace() {
    await this.goto(Routes.nonBreakingSpace);
  }

  nbspButton() {
    return this.page.locator(LocatorSelectors.nbspButton);
  }

  /** Returns the caption with the non-breaking space still intact. */
  async getNbspRawTextAsync(): Promise<string> {
    return this.nbspButton().evaluate((el) => el.textContent ?? '');
  }

  // ----- /textinput -----

  async openTextInput() {
    await this.goto(Routes.textInput);
  }

  newButtonNameInput() {
    return this.page.locator(LocatorSelectors.newButtonNameInput);
  }

  updatingButton() {
    return this.page.locator(LocatorSelectors.updatingButton);
  }

  async renameButton(newName: string) {
    await this.newButtonNameInput().fill(newName);
    await this.updatingButton().click();
  }

  // ----- /verifytext -----

  async openVerifyText() {
    await this.goto(Routes.verifyText);
  }

  verifyTextBadge() {
    return this.page.locator(LocatorSelectors.verifyTextBadge);
  }

  /** The raw DOM text, padding and line breaks included. */
  async getRawTextAsync(): Promise<string> {
    return this.verifyTextBadge().evaluate((el) => el.textContent ?? '');
  }

  // ----- /cssselectors -----

  async openCssSelectors() {
    await this.goto(Routes.cssSelectors);
  }
}
