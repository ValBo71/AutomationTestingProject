import { test, expect } from '../../fixtures/api';
import { AdminPage } from '../../pages/AdminPage';
import { FrontPage } from '../../pages/FrontPage';
import { buildMessage } from '../../data/testData';

/**
 * The message equivalent of roomSync: state written on one side of the boundary
 * has to be visible from the other.
 *
 * The contact form is the one place in the platform where a real user writes
 * data anonymously, so the first test types it in properly rather than seeding
 * it - the form's own validation and wiring is part of what is under test. The
 * two after it seed over HTTP, because what they check is the admin rendering,
 * not the typing.
 */
test.describe('Messages cross the API/UI boundary', () => {
  test('A message sent through the contact form is readable over HTTP', async ({
    page,
    messages,
    janitor,
  }) => {
    const payload = buildMessage({ name: 'Contact Form Guest' });
    janitor.register(`message "${payload.subject}"`, () =>
      messages.removeBySubjectAsync(payload.subject)
    );

    const front = new FrontPage(page);
    await front.open();
    await front.submitContactFormAsync(payload);

    /**
     * Polled rather than read once: the form posts asynchronously, and the only
     * on-screen confirmation is a banner. Asking the API directly is both a
     * stronger check and a faster one - it proves the data reached the service,
     * not merely that the page said so.
     */
    await expect
      .poll(
        async () =>
          (await messages.listMessagesAsync()).some(
            (message) => message.subject === payload.subject
          ),
        { message: 'the contact form submission should reach the message service' }
      )
      .toBe(true);

    const id = (await messages.listMessagesAsync()).find(
      (message) => message.subject === payload.subject
    )!.id;
    const detail = await (await messages.getById(id)).json();

    expect(detail).toMatchObject({
      name: payload.name,
      email: payload.email,
      phone: payload.phone,
      subject: payload.subject,
      description: payload.description,
    });
  });

  test('A message posted over HTTP shows up in the admin inbox', async ({
    page,
    messages,
    adminToken,
    janitor,
  }) => {
    const payload = buildMessage({ name: 'Seeded Over Http' });
    janitor.register(`message "${payload.subject}"`, () =>
      messages.removeBySubjectAsync(payload.subject)
    );
    await messages.createMessageAsync(payload);

    const admin = new AdminPage(page);
    await admin.loginWithTokenAsync(adminToken);
    await admin.openMessages();

    await expect(page.getByText(payload.subject)).toBeVisible();
    await expect(admin.messageRows().first()).toBeVisible();
  });

  test('The navigation badge counts unread messages and drops when one is read', async ({
    page,
    messages,
    adminToken,
    janitor,
  }) => {
    const payload = buildMessage();
    janitor.register(`message "${payload.subject}"`, () =>
      messages.removeBySubjectAsync(payload.subject)
    );
    const id = await messages.createMessageAsync(payload);

    const admin = new AdminPage(page);
    await admin.loginWithTokenAsync(adminToken);
    await admin.open();

    await expect(admin.unreadBadge()).toBeVisible();
    const withUnread = await admin.unreadBadgeCountAsync();
    expect(withUnread).toBeGreaterThan(0);

    await messages.markRead(id);
    await page.reload();

    /**
     * Compared as "lower than before" rather than "exactly one lower". The
     * instance is public and this suite's own workers run in parallel, so the
     * badge is never a number this test owns outright - only its direction of
     * travel is safe to assert.
     */
    await expect
      .poll(() => admin.unreadBadgeCountAsync(), {
        message: 'marking a message read should lower the unread badge',
      })
      .toBeLessThan(withUnread);
  });
});
