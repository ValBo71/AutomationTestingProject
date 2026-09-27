import { test as base, expect, APIResponse } from '@playwright/test';
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
 *
 * "Reported" covers both ways a cleanup can fail: an undo that throws, and an
 * undo whose request the service refused. The clients never throw on a 4xx or
 * 5xx, so an undo returns its APIResponse and the janitor inspects it - without
 * that, a delete answered with 401 or 500 would leak silently. A 404 is not a
 * failure: the item is already gone, usually because the test deleted it itself.
 */
export class Janitor {
  private readonly undo: Array<{ label: string; run: () => Promise<unknown> }> = [];

  register(label: string, run: () => Promise<unknown>): void {
    this.undo.push({ label, run });
  }

  async runAsync(): Promise<void> {
    for (const item of this.undo.reverse()) {
      try {
        const result = await item.run();
        if (isApiResponse(result) && !result.ok() && result.status() !== 404) {
          console.warn(
            `[janitor] could not clean up ${item.label}: the service answered ${result.status()} ${await result.text()}`
          );
        }
      } catch (error) {
        console.warn(`[janitor] could not clean up ${item.label}: ${String(error)}`);
      }
    }
    this.undo.length = 0;
  }
}

function isApiResponse(value: unknown): value is APIResponse {
  return (
    typeof value === 'object' &&
    value !== null &&
    typeof (value as APIResponse).ok === 'function' &&
    typeof (value as APIResponse).status === 'function'
  );
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
   * Depends on `request` on purpose, although it never uses it directly.
   *
   * The undo closures call clients that send their requests through `request`,
   * so the janitor has to drain before that context is disposed. Playwright
   * tears a fixture down before the fixtures it depends on - not in reverse
   * order of declaration, and not in reverse order of how a test happens to list
   * them - so this dependency is what guarantees the order. Without it, a test
   * that asked for `({ janitor, rooms })` would run its cleanup on a closed
   * context.
   */
  janitor: async ({ request }, use) => {
    const janitor = new Janitor();
    await use(janitor);
    await janitor.runAsync();
  },
});

/**
 * Marks the running test as an expected failure - but only for the outcome it
 * documents.
 *
 * A plain `test.fail()` turns *any* failure into a pass: the documented defect,
 * but also a worse regression, a 500, or a broken setup step, all reported as
 * "known defect, still there". So a defect test first observes the platform,
 * then calls this:
 *
 *  - `documented`: the platform shows exactly the defect described. The test is
 *    expected to fail and does.
 *  - `fixed`: the platform now does the right thing. The test is still marked
 *    expected-to-fail, its assertions pass, and Playwright reports it as an
 *    unexpected pass - the signal to retire the defect entry.
 *  - neither: something else happened. The test runs as a normal test and fails
 *    for real, which is what a new problem deserves.
 */
export function markKnownDefect(outcome: { documented: boolean; fixed: boolean }, description: string): void {
  test.fail(outcome.documented || outcome.fixed, description);
}

export { expect };
