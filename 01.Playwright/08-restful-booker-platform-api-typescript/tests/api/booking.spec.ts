import { test, expect } from '../../fixtures/api';
import { Room } from '../../clients/RoomClient';
import { buildBooking, buildRoom, Expected, futureStay } from '../../data/testData';
import { assertMatchesSchema, BookingListSchema, BookingSchema } from '../../schemas/schemas';

/**
 * Bookings are tested against a room this suite creates rather than against the
 * sandbox's seeded rooms. Two reasons: the seeded rooms are shared with everyone
 * else using the instance, and the service rejects overlapping stays outright,
 * so tests that share a room would fail each other with a 409 under parallelism.
 */
test.describe('Bookings', () => {
  let room: Room;

  test.beforeEach(async ({ rooms, janitor }) => {
    const payload = buildRoom();
    // Armed by name before the room exists - see RoomClient.removeByNameAsync.
    janitor.register(`room named ${payload.roomName}`, () =>
      rooms.removeByNameAsync(payload.roomName)
    );
    room = await rooms.createRoomAsync(payload);
  });

  test('A guest can book without being logged in', async ({
    anonymousBookings,
    bookings,
    janitor,
  }) => {
    const payload = buildBooking(room.roomid);

    // Deliberately the anonymous client: a guest booking a room has no account.
    const response = await anonymousBookings.create(payload);

    expect(response.status()).toBe(201);
    const booking = assertMatchesSchema(BookingSchema, await response.json(), 'Created booking');
    janitor.register(`booking ${booking.bookingid}`, () => bookings.remove(booking.bookingid));

    expect(booking).toMatchObject({
      roomid: room.roomid,
      firstname: payload.firstname,
      lastname: payload.lastname,
      depositpaid: payload.depositpaid,
      bookingdates: payload.bookingdates,
    });
  });

  test('A new booking shows up when the room is queried', async ({ bookings, janitor }) => {
    const created = await bookings.createBookingAsync(buildBooking(room.roomid));
    janitor.register(`booking ${created.bookingid}`, () => bookings.remove(created.bookingid));

    const response = await bookings.listForRoom(room.roomid);

    expect(response.status()).toBe(200);
    const body = assertMatchesSchema(BookingListSchema, await response.json(), 'Booking list');
    expect(body.bookings.map((booking) => booking.bookingid)).toContain(created.bookingid);
  });

  test('A booking is removed and stops appearing in the room', async ({ bookings }) => {
    const created = await bookings.createBookingAsync(buildBooking(room.roomid));

    const removal = await bookings.remove(created.bookingid);
    expect(removal.status()).toBe(202);

    /**
     * Polled, because 202 means "accepted", not "done".
     *
     * Reading the list immediately after the delete passed three runs out of
     * four and then failed - the booking was still listed. The status code was
     * saying so all along: a service that answers 202 is telling the caller the
     * work is queued, so asserting on the next line asserts against a race the
     * API never promised to win.
     */
    await expect
      .poll(async () => (await bookings.listBookingsAsync(room.roomid)).map((b) => b.bookingid), {
        message: 'the deleted booking should stop being listed',
      })
      .not.toContain(created.bookingid);
  });

  test('The response echoes the dates exactly as sent, with no timezone drift', async ({
    bookings,
    janitor,
  }) => {
    const dates = futureStay(3);
    const created = await bookings.createBookingAsync(
      buildBooking(room.roomid, { bookingdates: dates })
    );
    janitor.register(`booking ${created.bookingid}`, () => bookings.remove(created.bookingid));

    // Worth pinning: a service that parses these into timestamps and formats
    // them back in a different zone shifts a stay by a day, and the bug only
    // shows for guests in certain offsets.
    expect(created.bookingdates.checkin).toBe(dates.checkin);
    expect(created.bookingdates.checkout).toBe(dates.checkout);
  });
});

test.describe('Bookings - refusals', () => {
  let room: Room;

  test.beforeEach(async ({ rooms, janitor }) => {
    const payload = buildRoom();
    // Armed by name before the room exists - see RoomClient.removeByNameAsync.
    janitor.register(`room named ${payload.roomName}`, () =>
      rooms.removeByNameAsync(payload.roomName)
    );
    room = await rooms.createRoomAsync(payload);
  });

  test('The same room cannot be booked twice for the same nights', async ({ bookings, janitor }) => {
    const dates = futureStay(2);
    const first = await bookings.createBookingAsync(
      buildBooking(room.roomid, { bookingdates: dates })
    );
    janitor.register(`booking ${first.bookingid}`, () => bookings.remove(first.bookingid));

    const clash = await bookings.create(buildBooking(room.roomid, { bookingdates: dates }));

    expect(clash.status()).toBe(409);
    expect(await clash.json()).toMatchObject({ error: Expected.bookingCreateFailed });
  });

  test('An overlapping stay is refused even when the dates are not identical', async ({
    bookings,
    janitor,
  }) => {
    const dates = futureStay(4);
    const first = await bookings.createBookingAsync(
      buildBooking(room.roomid, { bookingdates: dates })
    );
    janitor.register(`booking ${first.bookingid}`, () => bookings.remove(first.bookingid));

    // Starts one day into the existing stay and runs past its end.
    const overlapStart = shiftDate(dates.checkin, 1);
    const overlapEnd = shiftDate(dates.checkout, 2);
    const overlap = await bookings.create(
      buildBooking(room.roomid, { bookingdates: { checkin: overlapStart, checkout: overlapEnd } })
    );

    expect(overlap.status()).toBe(409);
  });

  test('A booking with no first or last name is rejected field by field', async ({ bookings }) => {
    const payload = buildBooking(room.roomid);
    const response = await bookings.create({ ...payload, firstname: '', lastname: '' });

    expect(response.status()).toBe(400);
    const body = (await response.json()) as { errors: string[] };
    expect(body.errors).toEqual(
      expect.arrayContaining([Expected.blankFirstname, Expected.blankLastname])
    );
  });

  test('Listing every booking without naming a room is refused', async ({ bookings }) => {
    const response = await bookings.listWithoutRoomFilter();

    expect(response.status()).toBe(400);
    expect(await response.json()).toMatchObject({ error: Expected.bookingRoomIdRequired });
  });
});

test.describe('Bookings - known defects', () => {
  /**
   * A stay that ends before it starts is a malformed request, not a clash with
   * another guest, so it belongs in the 400 family. The service answers 409
   * "Failed to create booking" - the same status and the same message it uses
   * for a genuine double-booking.
   *
   * The practical cost is that a client cannot tell the two apart: "those dates
   * are taken, pick others" and "you have the dates backwards" need different
   * messages in the UI, and this response supports neither.
   */
  test.fail('DEFECT: a reversed date range is reported as a conflict, not a bad request', async ({
    rooms,
    bookings,
    janitor,
  }) => {
    const roomPayload = buildRoom();
    janitor.register(`room named ${roomPayload.roomName}`, () =>
      rooms.removeByNameAsync(roomPayload.roomName)
    );
    const room = await rooms.createRoomAsync(roomPayload);

    const stay = futureStay(5);
    const response = await bookings.create(
      buildBooking(room.roomid, {
        bookingdates: { checkin: stay.checkout, checkout: stay.checkin },
      })
    );

    expect(response.status()).toBe(400);
  });
});

/**
 * Adds days to a yyyy-mm-dd string and returns the same format.
 *
 * The trailing Z is not decoration - drop it and this function is wrong by a
 * day on most machines. JavaScript parses a bare date ("2027-01-10") as UTC,
 * but a date *and time* without a zone ("2027-01-10T00:00:00") as local, so the
 * UTC calendar day underneath is not the one that was written, and every
 * setUTCDate call after it lands one day off. Measured here, in UTC+2:
 *
 *   '2027-01-10'            + 1 day -> 2027-01-11   correct
 *   '2027-01-10T00:00:00'   + 1 day -> 2027-01-10   silently wrong
 *   '2027-01-10T00:00:00Z'  + 1 day -> 2027-01-11   correct
 *
 * Worth spelling out because the failure it causes is the worst kind: the
 * overlap test would still create two bookings and still get its 409, so it
 * would pass while checking something other than what it claims - and the
 * suite would only notice west of Greenwich, where the drift goes the other
 * way and the stays stop overlapping.
 */
function shiftDate(date: string, days: number): string {
  const shifted = new Date(`${date}T00:00:00Z`);
  shifted.setUTCDate(shifted.getUTCDate() + days);
  return shifted.toISOString().slice(0, 10);
}
