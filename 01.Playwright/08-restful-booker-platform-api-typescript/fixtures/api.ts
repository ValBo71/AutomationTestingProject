import { test as base, expect } from '@playwright/test';
import { AuthClient } from '../clients/AuthClient';
import { BookingClient } from '../clients/BookingClient';
import { MessageClient } from '../clients/MessageClient';
import { RoomClient } from '../clients/RoomClient';
import { SiteClient } from '../clients/SiteClient';

/**
 * Undo stack for anything a test writes to the sandbox.
 *
 * This instance is public and shared, so leaving rooms, bookings and messages
 * behind is not a tidiness problem - it is pollution other people have to work
 * around, and it eventually breaks this suite too, because a leaked booking
 * occupies dates and a leaked room shows up in list assertions.
 *
 * Deletions run newest-first so a booking is removed before the room it hangs
 * off, and a failure to clean up is reported without failing the test that
 * already passed - the test's own verdict should not be rewritten by the
 * janitor.
 */
export class Janitor {
  private readonly undo: Array<{ label: string; run: () => Promise<unknown> }> = [];

  register(label: string, run: () => Promise<unknown>): void {
    this.undo.push({ label, run });
  }

  async runAsync(): Promise<void> {
    for (const item of this.undo.reverse()) {
      try {
        await item.run();
      } catch (error) {
        console.warn(`[janitor] could not clean up ${item.label}: ${String(error)}`);
      }
    }
    this.undo.length = 0;
  }
}

interface ApiFixtures {
  /** A valid admin token, obtained once per test. */
  adminToken: string;
  /** Authenticated clients - the common case. */
  auth: AuthClient;
  rooms: RoomClient;
  bookings: BookingClient;
  messages: MessageClient;
  site: SiteClient;
  /** Unauthenticated counterparts, for proving endpoints are protected. */
  anonymousRooms: RoomClient;
  anonymousBookings: BookingClient;
  anonymousMessages: MessageClient;
  janitor: Janitor;
}

export const test = base.extend<ApiFixtures>({
  /**
   * A fresh token per test, not one shared across the worker.
   *
   * Worker scope would be cheaper - one login instead of one per test - and it
   * would work, because this platform's logout does not actually revoke
   * anything (see the defect documented in tests/api/auth.spec.ts), so a token
   * minted early stays valid for the whole run.
   *
   * That is exactly why it is not done. Building the suite on top of a defect
   * means the day the platform starts revoking tokens properly, tests unrelated
   * to auth begin failing in ways that point nowhere near the cause. A login is
   * one cheap request; test independence is worth more than saving it.
   */
  adminToken: async ({ request }, use) => {
    const token = await new AuthClient(request).loginAsAdminAsync();
    await use(token);
  },

  auth: async ({ request }, use) => {
    await use(new AuthClient(request));
  },

  rooms: async ({ request, adminToken }, use) => {
    await use(new RoomClient(request, adminToken));
  },

  bookings: async ({ request, adminToken }, use) => {
    await use(new BookingClient(request, adminToken));
  },

  messages: async ({ request, adminToken }, use) => {
    await use(new MessageClient(request, adminToken));
  },

  site: async ({ request, adminToken }, use) => {
    await use(new SiteClient(request, adminToken));
  },

  /**
   * The unauthenticated clients are fixtures in their own right rather than
   * something a spec builds inline. It makes "this endpoint is protected" a
   * one-line test, and more importantly it makes the intent unmistakable: a
   * reader sees `anonymousRooms` and knows the missing token is the subject of
   * the test, not an oversight in its setup.
   */
  anonymousRooms: async ({ request }, use) => {
    await use(new RoomClient(request));
  },

  anonymousBookings: async ({ request }, use) => {
    await use(new BookingClient(request));
  },

  anonymousMessages: async ({ request }, use) => {
    await use(new MessageClient(request));
  },

  /**
   * Declared last on purpose. Fixtures tear down in reverse order of setup, so
   * the janitor is unwound before the clients it needs are disposed of.
   */
  janitor: async ({}, use) => {
    const janitor = new Janitor();
    await use(janitor);
    await janitor.runAsync();
  },
});

export { expect };
