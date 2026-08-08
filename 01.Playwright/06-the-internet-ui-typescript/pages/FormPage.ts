import { Page } from '@playwright/test';
import { BasePage } from '../core/basePage';
import { FormSelectors } from '../selectors/FormSelectors';
import { Routes } from '../data/testData';

/** Covers Checkboxes, Dropdown, Inputs, Add/Remove Elements, Horizontal Slider, Key Presses. */
export class FormPage extends BasePage {
  constructor(page: Page) {
    super(page);
  }

  // ----- /checkboxes -----

  async openCheckboxes() {
    await this.goto(Routes.checkboxes);
  }

  checkboxes() {
    return this.page.locator(FormSelectors.checkboxes);
  }

  /**
   * Checkbox 2 ships pre-checked, so a test that only calls check() would pass
   * without proving anything. setChecked() makes the intended end state explicit.
   */
  async setCheckboxState(index: number, checked: boolean) {
    await this.checkboxes().nth(index).setChecked(checked);
  }

  // ----- /dropdown -----

  async openDropdown() {
    await this.goto(Routes.dropdown);
  }

  dropdown() {
    return this.page.locator(FormSelectors.dropdown);
  }

  async selectOption(value: string) {
    await this.dropdown().selectOption(value);
  }

  // ----- /inputs -----

  async openInputs() {
    await this.goto(Routes.inputs);
  }

  numberInput() {
    return this.page.locator(FormSelectors.numberInput);
  }

  // ----- /add_remove_elements/ -----

  async openAddRemoveElements() {
    await this.goto(Routes.addRemoveElements);
  }

  addedElements() {
    return this.page.locator(FormSelectors.addedElements);
  }

  async addElements(count: number) {
    const addButton = this.page.locator(FormSelectors.addElementButton);
    for (let i = 0; i < count; i++) {
      await addButton.click();
    }
  }

  /** Always removes the first remaining button, since indices shift after each delete. */
  async removeElements(count: number) {
    for (let i = 0; i < count; i++) {
      await this.addedElements().first().click();
    }
  }

  // ----- /horizontal_slider -----

  async openHorizontalSlider() {
    await this.goto(Routes.horizontalSlider);
  }

  slider() {
    return this.page.locator(FormSelectors.slider);
  }

  sliderValue() {
    return this.page.locator(FormSelectors.sliderValue);
  }

  /**
   * Drives the range input with arrow keys rather than fill(). fill() would set
   * the value without firing the onchange handler the page listens to, so the
   * displayed value would never update - the exact trap this challenge sets.
   *
   * focus() rather than click(): clicking a range input jumps the thumb to
   * wherever the pointer landed, so the arrow presses would start counting
   * from an unpredictable position instead of from the current value.
   */
  async moveSliderTo(target: number) {
    const slider = this.slider();
    await slider.focus();

    const current = Number(await slider.inputValue());
    const steps = Math.round((target - current) / 0.5);
    const key = steps >= 0 ? 'ArrowRight' : 'ArrowLeft';

    for (let i = 0; i < Math.abs(steps); i++) {
      await slider.press(key);
    }
  }

  // ----- /key_presses -----

  async openKeyPresses() {
    await this.goto(Routes.keyPresses);
  }

  keyPressTarget() {
    return this.page.locator(FormSelectors.keyPressTarget);
  }

  keyPressResult() {
    return this.page.locator(FormSelectors.keyPressResult);
  }

  async pressKey(key: string) {
    await this.keyPressTarget().press(key);
  }
}
