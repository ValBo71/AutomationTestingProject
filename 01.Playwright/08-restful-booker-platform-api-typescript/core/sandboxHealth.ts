import { APIRequestContext } from '@playwright/test';
import { Api } from '../data/endpoints';

/**
 * What the suite needs to know about the sandbox before it writes to it.
 *
 * The platform is several services behind one gateway, and the gateway has
 * been seen failing on its own: every route that carries an id in the path
 * (`/api/room/{id}`, `/api/booking/{id}`, `/api/message/{id}`) answers a bare
 * 500 for any verb, while the collection routes keep working (first seen
 * 2026-10-24).
 *
 * In that state a test can still create a room, a booking or a message - and
 * can no longer read it back, change it or delete it. Every run then leaves
 * data behind in an instance other people share, and the janitor can do
 * nothing about it. So the state is detected once per worker, and the tests
 * that depend on those routes skip with this reason instead of failing on
 * someone else's outage and polluting the sandbox while they do.
 */
export interface SandboxHealth {
  /** True when the by-id routes of two independent services both answer 500. */
  byIdRoutesDown: boolean;
  /** One line for the skip reason and the CI annotation. */
  detail: string;
}

export const BY_ID_OUTAGE_REASON =
  'Sandbox outage: the by-id routes answer 500, so data cannot be read back or cleaned up';

/**
 * Reads one existing room and one existing message by id, without a token.
 *
 * Ids are taken from the collection routes rather than assumed: an unknown
 * room id answers 500 on a healthy instance too (known defect #5), so probing
 * a made-up id would report an outage that is not there. Two services have to
 * fail together before it is called an outage - one failing alone is a defect
 * in that service, and the tests should fail on it.
 */
export async function probeSandboxAsync(request: APIRequestContext): Promise<SandboxHealth> {
  const healthy: SandboxHealth = { byIdRoutesDown: false, detail: 'by-id routes answer' };

  const roomsResponse = await request.get(Api.room.base);
  const messagesResponse = await request.get(Api.message.base);
  if (!roomsResponse.ok() || !messagesResponse.ok()) return healthy;

  const rooms = ((await roomsResponse.json()) as { rooms?: Array<{ roomid: number }> }).rooms ?? [];
  const messages = ((await messagesResponse.json()) as { messages?: Array<{ id: number }> }).messages ?? [];
  if (rooms.length === 0 || messages.length === 0) return healthy;

  const room = await request.get(Api.room.byId(rooms[0].roomid));
  const message = await request.get(Api.message.byId(messages[0].id));
  if (room.status() !== 500 || message.status() !== 500) return healthy;

  return {
    byIdRoutesDown: true,
    detail: `GET ${Api.room.byId(rooms[0].roomid)} and GET ${Api.message.byId(messages[0].id)} both answered 500`,
  };
}
