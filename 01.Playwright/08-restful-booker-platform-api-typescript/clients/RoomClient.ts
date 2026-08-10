import { APIResponse } from '@playwright/test';
import { ApiClient } from '../core/apiClient';
import { Api } from '../data/endpoints';
import { RoomPayload } from '../data/testData';

export interface Room {
  roomid: number;
  roomName: string;
  type: string;
  accessible: boolean;
  roomPrice: number;
  features: string[];
  description: string;
  image: string;
}

export class RoomClient extends ApiClient {
  list(): Promise<APIResponse> {
    return this.get(Api.room.base);
  }

  getById(id: number): Promise<APIResponse> {
    return this.get(Api.room.byId(id));
  }

  create(payload: Partial<RoomPayload>): Promise<APIResponse> {
    return this.post(Api.room.base, { data: payload });
  }

  update(id: number, payload: Partial<Room>): Promise<APIResponse> {
    return this.put(Api.room.byId(id), { data: { roomid: id, ...payload } });
  }

  remove(id: number): Promise<APIResponse> {
    return this.delete(Api.room.byId(id));
  }

  /**
   * Deletes a room found by its name, and says nothing if there is none.
   *
   * This exists so cleanup can be registered *before* the room is created:
   * the create endpoint does not return an id, so an undo written after the
   * fact has nothing to hold on to, and an assertion that throws in between
   * would leak the room onto the shared instance. Registering a name-based
   * undo up front removes that window entirely.
   */
  async removeByNameAsync(roomName: string): Promise<void> {
    const rooms = await this.listRoomsAsync();
    const match = rooms.find((room) => room.roomName === roomName);
    if (match) await this.remove(match.roomid);
  }

  async listRoomsAsync(): Promise<Room[]> {
    const response = await this.list();
    const body = (await response.json()) as { rooms: Room[] };
    return body.rooms;
  }

  /**
   * Creates a room and returns it.
   *
   * The create endpoint answers `{"success": true}` rather than echoing the
   * new resource, and sends no Location header either, so the only way to learn
   * the generated id is to read the collection back and match on the name -
   * which is exactly why buildRoom() generates a unique one.
   */
  async createRoomAsync(payload: RoomPayload): Promise<Room> {
    let response = await this.create(payload);

    /**
     * One retry, and only for a 5xx.
     *
     * The host returned a single 500 "An unexpected error occurred" during a
     * full run. Concurrency was the obvious suspect and it was checked: 22
     * rooms created four-at-a-time and then sequentially, every one a 200. So
     * this is an occasional hiccup from a free-tier host, not a race worth
     * designing around, and one retry is enough to stop it derailing a run.
     *
     * Deliberately narrow. A 4xx is the service answering correctly and must
     * never be retried - that would turn a real validation failure into a
     * confusing timeout.
     */
    if (response.status() >= 500) {
      response = await this.create(payload);
    }

    if (!response.ok()) {
      throw new Error(`Room creation failed with ${response.status()}: ${await response.text()}`);
    }

    const rooms = await this.listRoomsAsync();
    const created = rooms.find((room) => room.roomName === payload.roomName);
    if (!created) {
      throw new Error(`Room "${payload.roomName}" was reported created but is not in the list.`);
    }
    return created;
  }
}
