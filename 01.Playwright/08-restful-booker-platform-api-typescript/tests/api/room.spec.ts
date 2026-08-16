import { test, expect } from '../../fixtures/api';
import { buildRoom, Expected } from '../../data/testData';
import { assertMatchesSchema, RoomListSchema, RoomSchema } from '../../schemas/schemas';

/**
 * Rooms are split into reading, writing and known defects rather than kept in
 * one block, because the three have different requirements. The reading tests
 * are safe against the seeded rooms and need no cleanup at all. Every writing
 * test creates its own room and arms an undo before it does, since the instance
 * is public and this suite's workers run in parallel - a leaked room is visible
 * to everyone else using it, and there is no way to sweep one up afterwards
 * except by hand.
 */
test.describe('Rooms - reading', () => {
  test('The room list is returned to anyone, logged in or not', async ({ anonymousRooms }) => {
    const response = await anonymousRooms.list();

    expect(response.status()).toBe(200);
    const body = assertMatchesSchema(RoomListSchema, await response.json(), 'Room list');
    expect(body.rooms.length).toBeGreaterThan(0);
  });

  test('A single room can be fetched by its id', async ({ rooms }) => {
    // Reads whichever room happens to be first rather than creating one. The
    // point here is that the by-id route agrees with the collection route, and
    // any room proves that - so this test stays a pure read, with nothing to
    // clean up and nothing that another worker can trip over.
    const [first] = await rooms.listRoomsAsync();

    const response = await rooms.getById(first.roomid);

    expect(response.status()).toBe(200);
    const room = assertMatchesSchema(RoomSchema, await response.json(), 'Single room');
    expect(room).toMatchObject({ roomid: first.roomid, roomName: first.roomName });
  });
});

test.describe('Rooms - writing', () => {
  test('A room is created, appears in the list, and is deleted again', async ({ rooms, janitor }) => {
    const payload = buildRoom({ roomPrice: 249, type: 'Suite' });
    janitor.register(`room named ${payload.roomName}`, () =>
      rooms.removeByNameAsync(payload.roomName)
    );

    const created = await rooms.createRoomAsync(payload);

    expect(created).toMatchObject({
      roomName: payload.roomName,
      type: 'Suite',
      roomPrice: 249,
      accessible: payload.accessible,
    });
    expect(created.features).toEqual(expect.arrayContaining(payload.features));

    const removal = await rooms.remove(created.roomid);
    expect(removal.status()).toBe(202);

    // 202 is "accepted", not "done" - see the note in booking.spec.ts.
    await expect
      .poll(async () => (await rooms.listRoomsAsync()).map((room) => room.roomid), {
        message: 'the deleted room should stop being listed',
      })
      .not.toContain(created.roomid);
  });

  test('An update replaces the room and echoes the new state back', async ({ rooms, janitor }) => {
    const payload = buildRoom();
    janitor.register(`room named ${payload.roomName}`, () =>
      rooms.removeByNameAsync(payload.roomName)
    );
    const created = await rooms.createRoomAsync(payload);

    const response = await rooms.update(created.roomid, {
      roomName: created.roomName,
      type: 'Single',
      accessible: false,
      roomPrice: 99,
      features: ['WiFi'],
      description: 'Updated by the suite.',
      image: created.image,
    });

    // 202 Accepted, and unlike create this one does return the resource.
    expect(response.status()).toBe(202);
    const updated = assertMatchesSchema(RoomSchema, await response.json(), 'Updated room');
    expect(updated).toMatchObject({
      roomid: created.roomid,
      type: 'Single',
      accessible: false,
      roomPrice: 99,
    });
    expect(updated.features).toEqual(['WiFi']);
  });

  test('Creation is rejected when the name is blank and the price is below one', async ({ rooms }) => {
    const response = await rooms.create(buildRoom({ roomName: '', roomPrice: -5 }));

    expect(response.status()).toBe(400);
    const body = (await response.json()) as { errors: string[] };
    expect(body.errors).toEqual(
      expect.arrayContaining([Expected.roomNameRequired, Expected.roomPriceTooLow])
    );
  });

  test('Creation reports every invalid field at once, not just the first', async ({ rooms }) => {
    const response = await rooms.create({ roomName: '', roomPrice: 0 });

    expect(response.status()).toBe(400);
    const body = (await response.json()) as { errors: string[] };
    // A validator that stops at the first problem forces the caller to
    // round-trip once per mistake, so this is worth pinning down.
    expect(body.errors.length).toBeGreaterThan(1);
  });
});

test.describe('Rooms - known defects', () => {
  /**
   * Asking for a room that does not exist produces a stack-trace style 500
   * rather than a 404. The response even leaks the internal path ("/room/9999"),
   * confirming the gateway prefix is stripped before the service sees it.
   */
  test.fail('DEFECT: an unknown room id returns 500 instead of 404', async ({ rooms }) => {
    const response = await rooms.getById(999_999);

    expect(response.status()).toBe(404);
  });

  /**
   * Create answers 200 with `{"success": true}`. A creating endpoint should
   * answer 201 and hand back the new resource - or at least its id. As it
   * stands the caller has to re-read the whole collection and match on a name
   * to find out what it just made, which is what RoomClient.createRoomAsync
   * does. The booking service, in the same platform, does it correctly.
   */
  test.fail('DEFECT: room creation answers 200 with no id instead of 201 with the room', async ({
    rooms,
    janitor,
  }) => {
    const payload = buildRoom();

    /**
     * Registered before the room exists, not after.
     *
     * An earlier version of this test registered the undo *after* the first
     * assertion - which, in a test that is expected to fail on that very
     * assertion, never ran. Five rooms leaked onto the shared instance before
     * the pattern was spotted and had to be deleted by hand. Cleanup for a
     * write must be armed before the write, and by name, because this endpoint
     * returns no id to clean up with.
     */
    janitor.register(`room named ${payload.roomName}`, () =>
      rooms.removeByNameAsync(payload.roomName)
    );

    const response = await rooms.create(payload);

    expect(response.status()).toBe(201);
    const body = (await response.json()) as { roomid?: number };
    expect(body.roomid).toBeGreaterThan(0);
  });
});
