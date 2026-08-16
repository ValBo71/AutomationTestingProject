import { Page } from '@playwright/test';
import { FrontPage as Selectors } from '../selectors/UiSelectors';
import { MessagePayload } from '../data/testData';
import { Ui } from '../data/endpoints';

/**
 * The public page a guest sees: the room cards and the contact form.
 *
 * Worth remembering that this is the only page in the platform that an
 * unauthenticated visitor can act on, which makes it the surface where a defect
 * costs the hotel money directly - see the room-card defect recorded in
 * tests/hybrid/roomSync.spec.ts.
 */
export class FrontPage {
  constructor(private readonly page: Page) {}

  async open() {
    await this.page.goto(Ui.home);
  }

  roomCards() {
    return this.page.locator(Selectors.roomCard);
  }

  /**
   * The public cards are titled by room *type*, not by room number - a room
   * named "104" appears as "Double". Worth knowing before writing an assertion
   * that looks for the name a test just created.
   */
  cardTitles() {
    return this.page.locator(`${Selectors.roomCard} ${Selectors.cardTitle}`);
  }

  cardForPrice(price: number) {
    return this.roomCards().filter({ hasText: `£${price}` });
  }

  async submitContactFormAsync(payload: MessagePayload) {
    const contact = Selectors.contact;
    await this.page.locator(contact.name).fill(payload.name);
    await this.page.locator(contact.email).fill(payload.email);
    await this.page.locator(contact.phone).fill(payload.phone);
    await this.page.locator(contact.subject).fill(payload.subject);
    await this.page.locator(contact.description).fill(payload.description);
    await this.page.getByRole('button', { name: /Submit/i }).click();
  }
}
