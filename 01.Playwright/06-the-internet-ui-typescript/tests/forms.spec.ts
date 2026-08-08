import { test, expect } from '@playwright/test';
import { FormPage } from '../pages/FormPage';
import { Expected, TestData } from '../data/testData';

test.describe('Forms and simple inputs', () => {
  let formPage: FormPage;

  test.beforeEach(async ({ page }) => {
    formPage = new FormPage(page);
  });

  test('Checkboxes: both boxes can be driven to an explicit state', async () => {
    await formPage.openCheckboxes();

    const checkboxes = formPage.checkboxes();
    await expect(checkboxes).toHaveCount(2);

    // The page ships with box 1 unchecked and box 2 checked.
    await expect(checkboxes.nth(0)).not.toBeChecked();
    await expect(checkboxes.nth(1)).toBeChecked();

    // Flip both, so the assertion proves a real state change either way.
    await formPage.setCheckboxState(0, true);
    await formPage.setCheckboxState(1, false);

    await expect(checkboxes.nth(0)).toBeChecked();
    await expect(checkboxes.nth(1)).not.toBeChecked();
  });

  test('Dropdown: an option can be selected and reports the correct value', async () => {
    await formPage.openDropdown();

    const dropdown = formPage.dropdown();
    // The placeholder option is disabled, so nothing is selected initially.
    await expect(dropdown).toHaveValue('');

    await formPage.selectOption('2');
    await expect(dropdown).toHaveValue('2');
    await expect(dropdown.locator('option:checked')).toHaveText('Option 2');
  });

  test('Inputs: the number field accepts digits and arrow-key increments', async () => {
    await formPage.openInputs();

    const input = formPage.numberInput();
    await input.fill(String(TestData.numberInput));
    await expect(input).toHaveValue(String(TestData.numberInput));

    await input.press('ArrowUp');
    await expect(input).toHaveValue(String(TestData.numberInput + 1));

    await input.press('ArrowDown');
    await expect(input).toHaveValue(String(TestData.numberInput));
  });

  test('Add/Remove Elements: elements are added and then removed again', async () => {
    await formPage.openAddRemoveElements();

    await expect(formPage.addedElements()).toHaveCount(0);

    await formPage.addElements(TestData.elementsToAdd);
    await expect(formPage.addedElements()).toHaveCount(TestData.elementsToAdd);

    await formPage.removeElements(TestData.elementsToAdd);
    await expect(formPage.addedElements()).toHaveCount(0);
  });

  test('Horizontal Slider: arrow keys move the slider and update the readout', async () => {
    await formPage.openHorizontalSlider();

    await expect(formPage.sliderValue()).toHaveText('0');

    await formPage.moveSliderTo(Number(TestData.sliderTarget));

    await expect(formPage.slider()).toHaveValue(TestData.sliderTarget);
    await expect(formPage.sliderValue()).toHaveText(TestData.sliderTarget);
  });

  test('Key Presses: the page reports which key was pressed', async () => {
    await formPage.openKeyPresses();

    await formPage.pressKey(TestData.keyToPress.key);

    await expect(formPage.keyPressResult()).toHaveText(TestData.keyToPress.expectedLabel);
  });
});

test.describe('Authentication forms', () => {
  test('A/B Test: the page renders one of the two known variations', async ({ page }) => {
    const formPage = new FormPage(page);
    await formPage.goto('/abtest');

    // Which variation is served is decided by a cookie, so the test asserts
    // membership of the known set rather than one fixed heading.
    const heading = await formPage.heading().innerText();
    expect(Expected.abTestHeadings).toContain(heading.trim());
  });
});
