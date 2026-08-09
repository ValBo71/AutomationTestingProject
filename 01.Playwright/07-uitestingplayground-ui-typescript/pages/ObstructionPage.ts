import { Page } from '@playwright/test';
import { BasePage } from '../core/basePage';
import { ObstructionSelectors } from '../selectors/Selectors';
import { Routes } from '../data/testData';

/**
 * Challenges where the element is present but something is in the way:
 * Click, Hidden Layers, Overlapped Element, Scrollbars, Scroll to Click
 * and Visibility.
 */
export class ObstructionPage extends BasePage {
  constructor(page: Page) {
    super(page);
  }

  // ----- /click -----

  async openClick() {
    await this.goto(Routes.click);
  }

  badButton() {
    return this.page.locator(ObstructionSelectors.badButton);
  }

  /**
   * The page listens for a real mousedown/mouseup pair and deliberately
   * ignores a synthetic element.click(). Playwright's click() dispatches
   * genuine input events, so it works where a JS-dispatched click would not.
   */
  async clickBadButton() {
    await this.badButton().click();
  }

  /** Demonstrates the failing approach, for contrast in the spec. */
  async dispatchDomClickOnBadButton() {
    await this.badButton().evaluate((el) => (el as HTMLElement).click());
  }

  async getBadButtonClassAsync(): Promise<string> {
    return (await this.badButton().getAttribute('class')) ?? '';
  }

  // ----- /hiddenlayers -----

  async openHiddenLayers() {
    await this.goto(Routes.hiddenLayers);
  }

  greenButton() {
    return this.page.locator(ObstructionSelectors.greenButton);
  }

  blueButton() {
    return this.page.locator(ObstructionSelectors.blueButton);
  }

  // ----- /overlapped -----

  async openOverlapped() {
    await this.goto(Routes.overlappedElement);
  }

  overlappedIdInput() {
    return this.page.locator(ObstructionSelectors.overlappedIdInput);
  }

  overlappedNameInput() {
    return this.page.locator(ObstructionSelectors.overlappedNameInput);
  }

  /**
   * The Name field sits under a grey overlay pinned inside a scrollable box.
   * scrollIntoViewIfNeeded is not enough - the overlay is absolutely
   * positioned against the outer container, so it does not move with the
   * input. The fix is to scroll the inner box until the input's own centre is
   * genuinely the topmost element there, verified with elementFromPoint.
   */
  async fillOverlappedName(value: string) {
    const input = this.overlappedNameInput();

    await input.evaluate((el) => {
      const scroller = el.closest('div[style*="overflow-y"]') as HTMLElement | null;
      if (!scroller) return;

      for (let offset = 0; offset <= scroller.scrollHeight; offset += 10) {
        scroller.scrollTop = offset;
        const box = el.getBoundingClientRect();
        const topMost = document.elementFromPoint(box.x + box.width / 2, box.y + box.height / 2);
        if (topMost === el) return;
      }
    });

    await input.fill(value);
  }

  // ----- /scrollbars -----

  async openScrollbars() {
    await this.goto(Routes.scrollbars);
  }

  hidingButton() {
    return this.page.locator(ObstructionSelectors.hidingButton);
  }

  // ----- /scrolltoclick -----

  async openScrollToClick() {
    await this.goto(Routes.scrollToClick);
  }

  scrollTarget(index: 1 | 2 | 3 | 4) {
    const selectors = {
      1: ObstructionSelectors.scrollTarget1,
      2: ObstructionSelectors.scrollTarget2,
      3: ObstructionSelectors.scrollTarget3,
      4: ObstructionSelectors.scrollTarget4,
    } as const;
    return this.page.locator(selectors[index]);
  }

  /** Target 4 only becomes visible while its row is hovered. */
  async hoverTargetRow() {
    await this.page.locator(ObstructionSelectors.hoverRow4).hover();
  }

  progressText() {
    return this.page.locator(ObstructionSelectors.progressText);
  }

  // ----- /visibility -----

  async openVisibility() {
    await this.goto(Routes.visibility);
  }

  async hideButtons() {
    await this.page.locator(ObstructionSelectors.hideButton).click();
  }

  button(name: keyof typeof ObstructionSelectors) {
    return this.page.locator(ObstructionSelectors[name]);
  }
}
