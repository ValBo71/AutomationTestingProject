import { test, expect } from '../../fixtures/api';
import { Branding } from '../../clients/SiteClient';
import { buildBooking, buildRoom } from '../../data/testData';
import { assertMatchesSchema, BrandingSchema, ReportSchema } from '../../schemas/schemas';

/**
 * Branding and report are grouped as "site" because they are what the platform
 * exposes about itself rather than about its rooms and bookings.
 *
 * They make an instructive pair on authentication. Branding is readable by
 * anyone - reasonable, since the public page renders it - but writable only with
 * a token. The report needs a token for both, and gets it right. Between them
 * they show the platform does understand the distinction, which is what makes
 * the unguarded message service look like an oversight rather than a policy.
 */
test.describe('Branding', () => {
  test('Branding is public and complete', async ({ request }) => {
    const response = await request.get('/api/branding');

    expect(response.status()).toBe(200);
    const branding = assertMatchesSchema(BrandingSchema, await response.json(), 'Branding');
    expect(branding.name).not.toHaveLength(0);
    expect(branding.map.latitude).toBeGreaterThan(-90);
    expect(branding.map.latitude).toBeLessThan(90);
  });

  test('Changing branding requires a token', async ({ request }) => {
    const response = await request.put('/api/branding', { data: { name: 'Should not apply' } });

    expect(response.status()).toBe(401);
  });

  test('logoUrl is validated as an absolute URL, so a relative path is refused', async ({ site }) => {
    const original = (await (await site.getBranding()).json()) as Branding;

    const response = await site.updateBranding({ ...original, logoUrl: '/images/rbp-logo.jpg' });

    // Worth pinning because the platform has shipped a relative path in this
    // field before, which made the settings screen impossible to save: the
    // form loaded a value the save endpoint would not take back.
    expect(response.status()).toBe(400);
    const body = (await response.json()) as { errorMessage?: string };
    expect(body.errorMessage).toContain('logoUrl');
  });
});

test.describe('Report', () => {
  test('The report is protected and matches its contract', async ({ site, request }) => {
    expect((await request.get('/api/report')).status()).toBe(401);

    const response = await site.getReport();
    expect(response.status()).toBe(200);
    assertMatchesSchema(ReportSchema, await response.json(), 'Report');
  });

  test('A booking made through the API turns up in the report', async ({
    rooms,
    bookings,
    site,
    janitor,
  }) => {
    const roomPayload = buildRoom();
    janitor.register(`room named ${roomPayload.roomName}`, () =>
      rooms.removeByNameAsync(roomPayload.roomName)
    );
    const room = await rooms.createRoomAsync(roomPayload);

    const payload = buildBooking(room.roomid, { firstname: 'Report', lastname: 'Probe' });
    const booking = await bookings.createBookingAsync(payload);
    janitor.register(`booking ${booking.bookingid}`, () => bookings.remove(booking.bookingid));

    // The report is a separate service reading the same data, so this is the
    // cheapest available check that the two are actually wired together.
    const entries = await site.reportEntriesAsync();
    const entry = entries.find(
      (item) => item.start === payload.bookingdates.checkin && item.title.includes('Report Probe')
    );

    expect(entry, 'the new booking should appear in the report').toBeDefined();
    expect(entry!.title).toContain(room.roomName);
    expect(entry!.end).toBe(payload.bookingdates.checkout);
  });
});
