import { z } from 'zod';

/**
 * Contract definitions for the responses this suite depends on.
 *
 * These are not a restatement of the assertions in the specs. A functional test
 * asks "is the price 175?"; a contract test asks "is roomPrice still a number,
 * and is roomid still called roomid?". The second question is the one that
 * catches a backend change before it reaches anyone's client code, and it costs
 * almost nothing to ask once the shapes are written down.
 *
 * `.strict()` is used where the shape is fully known, so a *new* field is a
 * reported change rather than a silent one.
 */

export const RoomSchema = z
  .object({
    roomid: z.number().int().positive(),
    roomName: z.string().min(1),
    type: z.string().min(1),
    accessible: z.boolean(),
    roomPrice: z.number().int().min(1),
    features: z.array(z.string()),
    description: z.string(),
    image: z.string(),
  })
  .strict();

export const RoomListSchema = z.object({ rooms: z.array(RoomSchema) }).strict();

export const BookingSchema = z
  .object({
    bookingid: z.number().int().positive(),
    roomid: z.number().int().positive(),
    firstname: z.string().min(1),
    lastname: z.string().min(1),
    depositpaid: z.boolean(),
    bookingdates: z
      .object({
        /** ISO calendar dates, not timestamps - the service returns yyyy-mm-dd. */
        checkin: z.string().regex(/^\d{4}-\d{2}-\d{2}$/),
        checkout: z.string().regex(/^\d{4}-\d{2}-\d{2}$/),
      })
      .strict(),
  })
  .strict();

export const BookingListSchema = z.object({ bookings: z.array(BookingSchema) }).strict();

export const MessageSummarySchema = z
  .object({
    id: z.number().int().positive(),
    name: z.string(),
    subject: z.string(),
    read: z.boolean(),
  })
  .strict();

export const MessageListSchema = z.object({ messages: z.array(MessageSummarySchema) }).strict();

/**
 * Note that `email` is a plain string here, not z.string().email().
 *
 * The contract is that the field exists and is a string; whether the service
 * rejects a malformed address is a behaviour, and it is tested as one in
 * message.spec.ts. Encoding the rule in both places would mean that a single
 * bad address - posted by anyone else using this public instance - fails every
 * test that so much as reads a message, pointing at a contract break that is
 * not one. A schema should describe the shape, and leave the rules to the
 * tests written for them.
 */
export const MessageDetailSchema = z
  .object({
    messageid: z.number().int().positive(),
    name: z.string(),
    email: z.string(),
    phone: z.string(),
    subject: z.string(),
    description: z.string(),
  })
  .strict();

export const UnreadCountSchema = z.object({ count: z.number().int().min(0) }).strict();

export const TokenSchema = z.object({ token: z.string().min(1) }).strict();

export const ValidationSchema = z.object({ valid: z.boolean() }).strict();

/**
 * Branding is not marked strict: it is the one payload the admin UI edits
 * freely, and an extra optional field there is a feature, not a contract break.
 */
export const BrandingSchema = z.object({
  name: z.string(),
  description: z.string(),
  logoUrl: z.string(),
  directions: z.string(),
  map: z.object({ latitude: z.number(), longitude: z.number() }),
  contact: z.object({
    name: z.string(),
    phone: z.string(),
    email: z.string(),
  }),
  address: z.object({
    line1: z.string(),
    line2: z.string(),
    postTown: z.string(),
    county: z.string(),
    postCode: z.string(),
  }),
});

export const ReportSchema = z.object({
  report: z.array(
    z
      .object({
        title: z.string(),
        start: z.string().regex(/^\d{4}-\d{2}-\d{2}$/),
        end: z.string().regex(/^\d{4}-\d{2}-\d{2}$/),
      })
      .strict()
  ),
});

/**
 * Turns a Zod failure into something readable in a test report. The default
 * message is a nested JSON dump that tells you nothing at a glance.
 */
export function assertMatchesSchema<T>(schema: z.ZodType<T>, payload: unknown, label: string): T {
  const result = schema.safeParse(payload);
  if (!result.success) {
    const problems = result.error.issues
      .map((issue) => `  - ${issue.path.join('.') || '(root)'}: ${issue.message}`)
      .join('\n');
    throw new Error(`${label} does not match its contract:\n${problems}`);
  }
  return result.data;
}
