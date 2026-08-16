import { test, expect } from '../../fixtures/api';
import { AdminPage } from '../../pages/AdminPage';
import { FrontPage } from '../../pages/FrontPage';
import { buildRoom, uniqueRoomName } from '../../data/testData';

/**
 * Both directions of the same boundary: state created over HTTP must show up in
 * the browser, and state created in the browser must be visible over HTTP.
 *
 * Seeding through the API is not a shortcut for its own sake. It removes the
 * eight form interactions that would otherwise stand between the test and the
 * thing it actually wants to check, so when this fails it is because the room
 * did not render - not because a dropdown moved.
 */
test.describe('Room state crosses the API/UI boundary', () => {
  test('A room created over HTTP appears in the admin table', async ({
    page,
    rooms,
    adminToken,
    janitor,
  }) => {
    const payload = buildRoom({ roomPrice: 313 });
    janitor.register(`room named ${payload.roomName}`, () =>
      rooms.removeByNameAsync(payload.roomName)
    );
    const created = await rooms.createRoomAsync(payload);

    const admin = new AdminPage(page);
    await admin.loginWithTokenAsync(adminToken);
    await admin.open();

    await expect(admin.roomRow(created.roomid)).toBeVisible();
    await expect(admin.roomNameCell(payload.roomName)).toHaveText(payload.roomName);
  });

  test('The public page shows the seeded rooms', async ({ page }) => {
    const front = new FrontPage(page);
    await front.open();

    await expect(front.roomCards()).toHaveCount(3);
  });

  /**
   * A room that exists everywhere else is missing from the page guests use.
   *
   * The room is created over HTTP, GET /api/room returns four rooms, and the
   * admin table lists all four - but the public page renders three cards.
   *
   * The obvious explanation was caching, and it is wrong. A network trace of
   * the page load shows it calling GET /api/room on every visit, and the
   * response it receives contains the new room. The page fetches the full list
   * and then does not render it. Watched across five reloads over a minute with
   * a cache-busting query string: three cards every time.
   *
   * Commercially this is the most expensive defect here - a room the hotel has
   * published cannot be booked by anyone, and nothing in the admin UI hints at
   * it, because the admin table shows the room quite happily.
   */
  test.fail('DEFECT: a room created over HTTP is never offered to the public', async ({
    page,
    rooms,
    janitor,
  }) => {
    const payload = buildRoom({ roomPrice: 411, type: 'Suite' });
    janitor.register(`room named ${payload.roomName}`, () =>
      rooms.removeByNameAsync(payload.roomName)
    );
    await rooms.createRoomAsync(payload);

    const front = new FrontPage(page);
    await front.open();

    // Matched on price, because the public cards are titled by type - a room
    // named "104" renders as "Suite", so the name is not on screen at all.
    await expect(front.cardForPrice(payload.roomPrice)).toHaveCount(1);
  });

  test('A room created in the browser is visible over HTTP', async ({
    page,
    rooms,
    adminToken,
    janitor,
  }) => {
    const roomName = uniqueRoomName();
    // Armed before the UI writes anything, for the same reason the API tests
    // arm theirs early: an assertion that throws must not leak a room.
    janitor.register(`room named ${roomName}`, () => rooms.removeByNameAsync(roomName));

    const admin = new AdminPage(page);
    await admin.loginWithTokenAsync(adminToken);
    await admin.open();

    await admin.createRoomAsync(roomName, 262, 'Twin');

    /**
     * Asserted on this room's own cell rather than on the row count. Counting
     * rows failed as soon as the suite ran in parallel - another worker's room
     * landed between the two reads and the total was one higher than expected.
     * A shared instance never lets a test own a total.
     */
    await expect(admin.roomNameCell(roomName)).toBeVisible();

    // The assertion that matters: the API - a different service from the one
    // that rendered the table - agrees the room exists and holds what was typed.
    const overHttp = (await rooms.listRoomsAsync()).find((room) => room.roomName === roomName);
    expect(overHttp, 'the room typed into the form should be readable over HTTP').toBeDefined();
    expect(overHttp).toMatchObject({ roomName, roomPrice: 262, type: 'Twin' });
  });

  test('A room deleted over HTTP disappears from a table already on screen', async ({
    page,
    rooms,
    adminToken,
    janitor,
  }) => {
    const payload = buildRoom();
    janitor.register(`room named ${payload.roomName}`, () =>
      rooms.removeByNameAsync(payload.roomName)
    );
    const created = await rooms.createRoomAsync(payload);

    const admin = new AdminPage(page);
    await admin.loginWithTokenAsync(adminToken);
    await admin.open();
    await expect(admin.roomRow(created.roomid)).toBeVisible();

    await rooms.remove(created.roomid);
    await page.reload();

    await expect(admin.roomRow(created.roomid)).toHaveCount(0);
  });
});
