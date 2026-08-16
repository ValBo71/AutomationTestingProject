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

test.describe('Branding - known defects', () => {
  /**
   * The update endpoint answers 200 {"success": true} and writes nothing.
   *
   * Established carefully, because the first reading of it was wrong. A single
   * write-then-read looked like a stale cache, and Cloudflare reports
   * cf-cache-status: DYNAMIC on this route, so caching was ruled out. The
   * settled evidence is a marker written to `name`, followed by a GET every
   * five seconds for a full minute: twelve reads, no change, on a response that
   * had already claimed success.
   *
   * A silent no-op behind a success status is the worst of both worlds. An
   * error at least tells the caller to retry or to warn someone; this tells
   * every client that the save worked.
   *
   * No cleanup is registered here, and that is not an oversight - the defect
   * being documented is precisely that nothing is ever written.
   */
  test.fail('DEFECT: an accepted branding update reports success but changes nothing', async ({
    site,
  }) => {
    const original = (await (await site.getBranding()).json()) as Branding;
    const marker = `Probe ${Date.now()}`;

    const response = await site.updateBranding({ ...original, name: marker });
    expect(response.status()).toBe(200);
    expect(await response.json()).toMatchObject({ success: true });

    await expect
      .poll(
        async () => ((await (await site.getBranding()).json()) as Branding).name,
        { timeout: 15_000, message: 'the update reported success, so it should become visible' }
      )
      .toBe(marker);
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
