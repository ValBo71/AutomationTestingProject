/**
 * Credentials, payload builders and the expected strings the services return.
 *
 * Everything that gets written to the sandbox is built here, and everything
 * built here is unique per call. That matters more than usual: the instance is
 * public, several people may be running against it at the same moment, and the
 * booking service rejects overlapping stays for the same room outright.
 */

/** Published on the site's own landing page - these are not secrets. */
export const AdminCredentials = {
  username: 'admin',
  password: 'password',
} as const;

/**
 * Counter feeding the unique-value helpers.
 *
 * The counter alone is not enough: Playwright runs each worker in its own
 * process, so every worker would start from 1 and hand out the same values.
 * The suffix is therefore three separate parts, never truncated:
 *
 *  - a run tag from the clock (base 36), which separates one run from the next
 *    - including a colleague's run against the same public instance;
 *  - the worker index Playwright assigns (TEST_WORKER_INDEX, unique per worker
 *    within a run, restarts included), which separates workers;
 *  - the counter, which separates calls within a worker.
 *
 * The letters between the parts matter. Without them worker 1 / call 12 and
 * worker 11 / call 2 both read "112", and an earlier version that sliced a fixed
 * number of digits off a mixed seed lost the worker part altogether - two
 * workers could get the same room name, and cleanup by name would then delete
 * the other worker's room.
 */
let sequence = 0;
const runTag = Date.now().toString(36).slice(-5);
const workerTag = process.env.TEST_WORKER_INDEX ?? String(process.pid);

function nextSuffix(): string {
  sequence += 1;
  return `${runTag}w${workerTag}n${sequence}`;
}

/**
 * A room name unique across workers and runs.
 *
 * Letters and digits only, because the admin table builds element ids out of
 * the value it renders - a room called 101 becomes `#roomName101` - so the name
 * ends up inside a selector.
 */
export function uniqueRoomName(): string {
  return nextSuffix();
}

/**
 * A stay far enough ahead that it cannot overlap the sandbox's seeded data.
 * Each call walks further into the future, so repeated calls never clash.
 *
 * Two years out is not arbitrary. The booking service refuses an overlapping
 * stay for the same room with a 409, and this instance is public - other people
 * demo it by booking dates near today. Pushing the whole suite well past any
 * plausible manual booking removes a class of failure that would otherwise look
 * like a bug in the code under test.
 */
export function futureStay(nights = 2): { checkin: string; checkout: string } {
  sequence += 1;
  const start = new Date();
  start.setUTCFullYear(start.getUTCFullYear() + 2);
  start.setUTCDate(start.getUTCDate() + sequence * (nights + 1));

  const end = new Date(start);
  end.setUTCDate(end.getUTCDate() + nights);

  return { checkin: isoDate(start), checkout: isoDate(end) };
}

function isoDate(date: Date): string {
  return date.toISOString().slice(0, 10);
}

export interface RoomPayload {
  roomName: string;
  type: string;
  accessible: boolean;
  roomPrice: number;
  features: string[];
  description: string;
  image: string;
}

export function buildRoom(overrides: Partial<RoomPayload> = {}): RoomPayload {
  return {
    roomName: uniqueRoomName(),
    type: 'Double',
    accessible: true,
    roomPrice: 175,
    features: ['WiFi', 'TV', 'Safe'],
    description: 'Created by the Playwright API suite.',
    image: '/images/room2.jpg',
    ...overrides,
  };
}

export interface BookingPayload {
  roomid: number;
  firstname: string;
  lastname: string;
  depositpaid: boolean;
  email: string;
  phone: string;
  bookingdates: { checkin: string; checkout: string };
}

export function buildBooking(roomId: number, overrides: Partial<BookingPayload> = {}): BookingPayload {
  return {
    roomid: roomId,
    firstname: 'Playwright',
    lastname: 'Suite',
    depositpaid: true,
    email: `pw.suite.${nextSuffix()}@example.com`,
    /** The validator insists on 11-21 characters, so this is deliberately long. */
    phone: '01234567890123',
    bookingdates: futureStay(),
    ...overrides,
  };
}

export interface MessagePayload {
  name: string;
  email: string;
  phone: string;
  subject: string;
  description: string;
}

export function buildMessage(overrides: Partial<MessagePayload> = {}): MessagePayload {
  return {
    name: 'Playwright Suite',
    email: `pw.suite.${nextSuffix()}@example.com`,
    phone: '01234567890123',
    subject: `Automated probe ${nextSuffix()}`,
    /** Must be 20-2000 characters, so a short greeting would be rejected. */
    description: 'Sent by the Playwright API suite while exercising the message service.',
    ...overrides,
  };
}

/**
 * Validation text the services return verbatim. Captured from live responses
 * rather than copied from documentation - several of these differ from what
 * the published API description claims.
 */
export const Expected = {
  invalidCredentials: 'Invalid credentials',
  invalidToken: 'Invalid token',
  bookingRoomIdRequired: 'Room ID is required',
  bookingCreateFailed: 'Failed to create booking',
  blankFirstname: 'Firstname should not be blank',
  blankLastname: 'Lastname should not be blank',
  roomNameRequired: 'Room name must be set',
  roomPriceTooLow: 'must be greater than or equal to 1',
  messageNameBlank: 'Name may not be blank',
  messageEmailMalformed: 'must be a well-formed email address',
  messagePhoneLength: 'Phone must be between 11 and 21 characters.',
  messageSubjectLength: 'Subject must be between 5 and 100 characters.',
  messageBodyLength: 'Message must be between 20 and 2000 characters.',
} as const;
