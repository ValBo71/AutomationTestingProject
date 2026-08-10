import { APIResponse } from '@playwright/test';
import { ApiClient } from '../core/apiClient';
import { Api } from '../data/endpoints';
import { MessagePayload } from '../data/testData';

export interface MessageSummary {
  id: number;
  name: string;
  subject: string;
  read: boolean;
}

export interface MessageDetail {
  messageid: number;
  name: string;
  email: string;
  phone: string;
  subject: string;
  description: string;
}

export class MessageClient extends ApiClient {
  /** The contact form is public; reading the inbox is not. */
  create(payload: Partial<MessagePayload>): Promise<APIResponse> {
    return this.post(Api.message.base, { data: payload });
  }

  list(): Promise<APIResponse> {
    return this.get(Api.message.base);
  }

  getById(id: number): Promise<APIResponse> {
    return this.get(Api.message.byId(id));
  }

  /** Named "count" but reports only the messages still marked unread. */
  unreadCount(): Promise<APIResponse> {
    return this.get(Api.message.unreadCount);
  }

  markRead(id: number): Promise<APIResponse> {
    return this.put(Api.message.read(id), { data: { read: true } });
  }

  remove(id: number): Promise<APIResponse> {
    return this.delete(Api.message.byId(id));
  }

  /** Name-based undo, registered before a message is posted. See RoomClient. */
  async removeBySubjectAsync(subject: string): Promise<void> {
    const messages = await this.listMessagesAsync();
    const match = messages.find((message) => message.subject === subject);
    if (match) await this.remove(match.id);
  }

  async listMessagesAsync(): Promise<MessageSummary[]> {
    const response = await this.list();
    const body = (await response.json()) as { messages: MessageSummary[] };
    return body.messages ?? [];
  }

  async unreadCountAsync(): Promise<number> {
    const response = await this.unreadCount();
    const body = (await response.json()) as { count: number };
    return body.count;
  }

  /**
   * Posts a message and resolves the id the service assigned it.
   *
   * Like room creation, the response is a bare `{"success": true}` with no id,
   * so the inbox is read back and matched on the unique subject that
   * buildMessage() generates.
   */
  async createMessageAsync(payload: MessagePayload): Promise<number> {
    const response = await this.create(payload);
    if (!response.ok()) {
      throw new Error(`Message creation failed with ${response.status()}: ${await response.text()}`);
    }

    const messages = await this.listMessagesAsync();
    const created = messages.find((message) => message.subject === payload.subject);
    if (!created) {
      throw new Error(`Message "${payload.subject}" was reported created but is not in the inbox.`);
    }
    return created.id;
  }
}
