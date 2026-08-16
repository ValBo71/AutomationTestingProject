import { Page, expect } from '@playwright/test';
import { AdminLogin, AdminMessages, AdminRooms } from '../selectors/UiSelectors';
import { AdminCredentials } from '../data/testData';
import { Ui } from '../data/endpoints';

/**
 * The admin screens behind the login: the room table and the message inbox.
 *
 * Deliberately thin. It exposes locators rather than assertions, so the tests
 * keep their own expectations where a reader can see them, and the only methods
 * that do more than one thing are the two login paths and the create-room form -
 * sequences that would otherwise be copied into every test that needs them.
 */
export class AdminPage {
  constructor(private readonly page: Page) {}

  async open() {
    await this.page.goto(Ui.admin);
  }

  async openMessages() {
    await this.page.goto(Ui.adminMessages);
  }

  /** The slow path: type the credentials into the form like a person would. */
  async loginThroughFormAsync() {
    await this.open();
    await this.page.locator(AdminLogin.username).fill(AdminCredentials.username);
    await this.page.locator(AdminLogin.password).fill(AdminCredentials.password);
    await this.page.getByRole('button', { name: 'Login' }).click();
    await this.expectLoggedIn();
  }

  /**
   * The fast path: hand the browser a token that was obtained over HTTP.
   *
   * The platform keeps its session in a plain `token` cookie, and the admin
   * screens do nothing more than read it, so a token minted through
   * /api/auth/login is indistinguishable from one earned by filling the form.
   * Every admin test that is not itself about logging in should start this way -
   * it removes a page load and a round-trip from each one, and it stops a
   * broken login form from failing twenty unrelated tests.
   */
  async loginWithTokenAsync(token: string) {
    await this.page.context().addCookies([
      {
        name: 'token',
        value: token,
        domain: 'automationintesting.online',
        path: '/',
      },
    ]);
  }

  async expectLoggedIn() {
    // Landing on /admin/rooms is the app's own signal that the token was taken.
    await expect(this.page).toHaveURL(/\/admin\/rooms/);
    await expect(this.roomRows().first()).toBeVisible();
  }

  roomRows() {
    return this.page.locator(AdminRooms.row);
  }

  roomRow(roomId: number) {
    return this.page.locator(AdminRooms.rowById(roomId));
  }

  roomNameCell(roomName: string) {
    return this.page.locator(AdminRooms.nameCell(roomName));
  }

  /** The navigation badge doubles as the unread-message counter. */
  messagesNavLink() {
    return this.page.getByRole('link', { name: /Messages/ });
  }

  /**
   * The number lives in its own span, which is rendered only after the count
   * request comes back. Reading the link's text too early yields a bare
   * "Messages" and a count of zero - which is how this first went wrong, with
   * a test that failed claiming the badge was empty when it simply had not
   * arrived yet. Callers should poll this rather than read it once.
   */
  unreadBadge() {
    return this.messagesNavLink().locator('.badge');
  }

  async unreadBadgeCountAsync(): Promise<number> {
    const text = (await this.unreadBadge().innerText()).trim();
    const parsed = Number(text);
    return Number.isNaN(parsed) ? 0 : parsed;
  }

  messageRows() {
    return this.page.locator(AdminMessages.anyRow);
  }

  async createRoomAsync(name: string, price: number, type = 'Double') {
    const form = AdminRooms.createForm;
    await this.page.locator(form.name).fill(name);
    await this.page.locator(form.type).selectOption(type);
    await this.page.locator(form.price).fill(String(price));
    await this.page.locator(form.wifi).check();
    await this.page.locator(form.submit).click();
  }
}
