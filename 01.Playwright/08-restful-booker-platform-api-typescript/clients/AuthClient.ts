import { APIResponse } from '@playwright/test';
import { ApiClient } from '../core/apiClient';
import { Api } from '../data/endpoints';
import { AdminCredentials } from '../data/testData';

export class AuthClient extends ApiClient {
  login(username: string, password: string): Promise<APIResponse> {
    return this.post(Api.auth.login, { data: { username, password } });
  }

  validate(token: string): Promise<APIResponse> {
    return this.post(Api.auth.validate, { data: { token } });
  }

  logout(token: string): Promise<APIResponse> {
    return this.post(Api.auth.logout, { data: { token } });
  }

  /**
   * Logs in as the published admin account and returns the raw token.
   * Throws rather than returning null: every authenticated test depends on
   * this, so a silent failure here would surface as a confusing 401 later.
   */
  async loginAsAdminAsync(): Promise<string> {
    const response = await this.login(AdminCredentials.username, AdminCredentials.password);
    if (!response.ok()) {
      throw new Error(`Admin login failed with ${response.status()}: ${await response.text()}`);
    }
    const body = (await response.json()) as { token?: string };
    if (!body.token) {
      throw new Error(`Admin login returned no token: ${JSON.stringify(body)}`);
    }
    return body.token;
  }
}
