import { test, expect } from '../../fixtures/api';
import { AdminCredentials, Expected } from '../../data/testData';
import { assertMatchesSchema, TokenSchema, ValidationSchema } from '../../schemas/schemas';

test.describe('Auth - issuing tokens', () => {
  test('Valid credentials return a token', async ({ auth }) => {
    const response = await auth.login(AdminCredentials.username, AdminCredentials.password);

    expect(response.status()).toBe(200);
    const body = assertMatchesSchema(TokenSchema, await response.json(), 'Login response');
    expect(body.token).not.toHaveLength(0);
  });

  test('A wrong password is rejected with 401', async ({ auth }) => {
    const response = await auth.login(AdminCredentials.username, 'definitely-not-the-password');

    expect(response.status()).toBe(401);
    expect(await response.json()).toMatchObject({ error: Expected.invalidCredentials });
  });

  test('An unknown username is rejected the same way as a wrong password', async ({ auth }) => {
    const response = await auth.login('no-such-user', AdminCredentials.password);

    // Identical response for both cases, which is the correct behaviour - a
    // different message would tell an attacker which usernames exist.
    expect(response.status()).toBe(401);
    expect(await response.json()).toMatchObject({ error: Expected.invalidCredentials });
  });

  test('Two logins issue two different tokens', async ({ auth }) => {
    const first = await auth.loginAsAdminAsync();
    const second = await auth.loginAsAdminAsync();

    expect(second).not.toBe(first);
  });
});

test.describe('Auth - validating tokens', () => {
  test('A freshly issued token validates', async ({ auth, adminToken }) => {
    const response = await auth.validate(adminToken);

    expect(response.status()).toBe(200);
    const body = assertMatchesSchema(ValidationSchema, await response.json(), 'Validate response');
    expect(body.valid).toBe(true);
  });

  test('A made-up token is refused with 403', async ({ auth }) => {
    const response = await auth.validate('not-a-real-token-at-all');

    expect(response.status()).toBe(403);
    expect(await response.json()).toMatchObject({ error: Expected.invalidToken });
  });
});

test.describe('Auth - protecting endpoints', () => {
  test('Reading bookings without a token is refused', async ({ anonymousBookings }) => {
    const response = await anonymousBookings.listForRoom(1);

    expect(response.status()).toBe(401);
  });

  test('Creating a room without a token is refused', async ({ anonymousRooms }) => {
    const response = await anonymousRooms.create({ roomName: '999', roomPrice: 100 });

    expect(response.status()).toBe(401);
  });

  test('Deleting without a token answers 403, not the 401 a read gets', async ({
    anonymousRooms,
    anonymousBookings,
  }) => {
    // Same missing credential, two different statuses depending on the verb.
    // Asserted as-is rather than normalised, because a client written against
    // "401 means log in again" will silently mishandle every delete.
    expect((await anonymousRooms.remove(1)).status()).toBe(403);
    expect((await anonymousBookings.remove(1)).status()).toBe(403);
  });
});

test.describe('Auth - known defects', () => {
  /**
   * Logout reports success but does not invalidate anything.
   *
   * Verified by hand before it was written up: log in, confirm the token
   * validates, log out, and the same token still validates and still opens
   * protected endpoints. Repeated three times against fresh tokens.
   *
   * Marked test.fail() rather than deleted or weakened. The assertions below
   * describe what a session ought to do, so the day the platform starts
   * revoking tokens this test turns red and says so, instead of quietly
   * agreeing with a bug.
   */
  test.fail(
    'DEFECT: a token survives logout and still opens protected endpoints',
    async ({ auth, request }) => {
      const token = await auth.loginAsAdminAsync();
      expect((await auth.validate(token)).status()).toBe(200);

      const logout = await auth.logout(token);
      expect(logout.status()).toBe(200);
      expect(await logout.json()).toMatchObject({ success: true });

      // Both of these should now fail. Neither does.
      const validation = await auth.validate(token);
      expect(validation.status()).toBe(403);

      const protectedRead = await request.get('/api/booking?roomid=1', {
        headers: { Cookie: `token=${token}` },
      });
      expect(protectedRead.status()).toBe(401);
    }
  );

  /**
   * A syntactically valid but unknown token makes the booking service throw
   * instead of rejecting the caller. /api/auth/validate handles the same input
   * correctly with a 403, so the fault is in how the booking service consumes
   * the cookie, not in the token format.
   */
  test.fail('DEFECT: an unknown token makes the booking service return 500', async ({ request }) => {
    const response = await request.get('/api/booking?roomid=1', {
      headers: { Cookie: 'token=totally-made-up-token' },
    });

    expect([401, 403]).toContain(response.status());
  });
});
