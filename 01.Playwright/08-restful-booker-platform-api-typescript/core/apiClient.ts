import { APIRequestContext, APIResponse } from '@playwright/test';

/**
 * Thin base for every service client.
 *
 * The platform authenticates with a token that travels as a `token` cookie, so
 * the auth header is attached here once rather than repeated in every call.
 * Nothing in this class asserts - clients hand the raw APIResponse back so the
 * specs can assert on the status code as well as the body. That is deliberate:
 * a client that throws on a non-2xx makes negative testing impossible, and this
 * platform's error responses are half the point of the suite.
 */
export class ApiClient {
  constructor(
    protected readonly request: APIRequestContext,
    protected token: string | null = null
  ) {}

  /** Swaps the credentials this client sends, without rebuilding it. */
  withToken(token: string | null): this {
    this.token = token;
    return this;
  }

  /**
   * The token travels as a cookie, not as an Authorization header.
   *
   * That is the platform's choice, not a preference of this suite, and it is
   * worth knowing because it is what makes the hybrid tests possible: a browser
   * context handed the same cookie is authenticated on exactly the same terms,
   * so a session opened over HTTP can be continued in a page without the login
   * form ever being filled in. A bearer header would not carry across that way.
   */
  protected authHeaders(): Record<string, string> {
    return this.token ? { Cookie: `token=${this.token}` } : {};
  }

  protected get(path: string, options: RequestOptions = {}): Promise<APIResponse> {
    return this.request.get(path, this.merge(options));
  }

  protected post(path: string, options: RequestOptions = {}): Promise<APIResponse> {
    return this.request.post(path, this.merge(options));
  }

  protected put(path: string, options: RequestOptions = {}): Promise<APIResponse> {
    return this.request.put(path, this.merge(options));
  }

  protected delete(path: string, options: RequestOptions = {}): Promise<APIResponse> {
    return this.request.delete(path, this.merge(options));
  }

  /**
   * Caller headers are spread last, so they win over the auth header.
   *
   * The order matters for the negative tests: a spec that wants to send a
   * malformed or expired token has to be able to override what the client would
   * otherwise attach, and reversing these two lines would silently discard the
   * value under test and pass for the wrong reason.
   */
  private merge(options: RequestOptions): RequestOptions {
    return {
      ...options,
      headers: { ...this.authHeaders(), ...(options.headers ?? {}) },
    };
  }
}

/**
 * A narrow local shape rather than Playwright's own options type. Only these
 * four fields are ever used here, and naming them keeps the client surface
 * readable - and stops a spec from reaching for `failOnStatusCode: true`, which
 * would defeat the point of returning raw responses.
 */
export interface RequestOptions {
  headers?: Record<string, string>;
  data?: unknown;
  params?: Record<string, string | number | boolean>;
  failOnStatusCode?: boolean;
}

/**
 * Reads the body as JSON without throwing when the service answers with an
 * empty payload. Several endpoints here reply 202 with no content at all, and
 * `response.json()` on an empty body is a parse error rather than a test
 * failure worth reading.
 */
export async function jsonOrNull<T = unknown>(response: APIResponse): Promise<T | null> {
  const text = await response.text();
  if (!text.trim()) return null;
  try {
    return JSON.parse(text) as T;
  } catch {
    return null;
  }
}
