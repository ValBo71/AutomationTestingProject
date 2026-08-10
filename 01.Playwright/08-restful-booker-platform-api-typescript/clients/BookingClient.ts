import { APIResponse } from '@playwright/test';
import { ApiClient } from '../core/apiClient';
import { Api } from '../data/endpoints';
import { BookingPayload } from '../data/testData';

export interface Booking {
  bookingid: number;
  roomid: number;
  firstname: string;
  lastname: string;
  depositpaid: boolean;
  bookingdates: { checkin: string; checkout: string };
}

export class BookingClient extends ApiClient {
  /** Reading bookings requires both a room filter and an authenticated caller. */
  listForRoom(roomId: number): Promise<APIResponse> {
    return this.get(Api.booking.byRoom(roomId));
  }

  /** Deliberately omits the roomid, to prove the service demands it. */
  listWithoutRoomFilter(): Promise<APIResponse> {
    return this.get(Api.booking.base);
  }

  getById(id: number): Promise<APIResponse> {
    return this.get(Api.booking.byId(id));
  }

  /** Creating a booking is public - a guest is not logged in when they book. */
  create(payload: Partial<BookingPayload>): Promise<APIResponse> {
    return this.post(Api.booking.base, { data: payload });
  }

  update(id: number, payload: Partial<BookingPayload>): Promise<APIResponse> {
    return this.put(Api.booking.byId(id), { data: payload });
  }

  remove(id: number): Promise<APIResponse> {
    return this.delete(Api.booking.byId(id));
  }

  async createBookingAsync(payload: BookingPayload): Promise<Booking> {
    const response = await this.create(payload);
    if (response.status() !== 201) {
      throw new Error(`Booking creation failed with ${response.status()}: ${await response.text()}`);
    }
    return (await response.json()) as Booking;
  }

  async listBookingsAsync(roomId: number): Promise<Booking[]> {
    const response = await this.listForRoom(roomId);
    const body = (await response.json()) as { bookings: Booking[] };
    return body.bookings ?? [];
  }
}
