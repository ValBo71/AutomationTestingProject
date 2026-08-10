import { APIResponse } from '@playwright/test';
import { ApiClient } from '../core/apiClient';
import { Api } from '../data/endpoints';

export interface Branding {
  name: string;
  map: { latitude: number; longitude: number };
  logoUrl: string;
  description: string;
  directions: string;
  contact: { name: string; phone: string; email: string };
  address: {
    line1: string;
    line2: string;
    postTown: string;
    county: string;
    postCode: string;
  };
}

export interface ReportEntry {
  title: string;
  start: string;
  end: string;
}

/**
 * Branding and report share a client because both are small and read-mostly:
 * four methods between them, no shared state, and no lifecycle to manage.
 *
 * Kept together deliberately rather than split for symmetry with the other
 * services. A class per endpoint is a cost, not a virtue, when the class would
 * hold two methods and nothing else. The line worth splitting on is behaviour,
 * not tidiness - if branding grows a real edit lifecycle, or report starts
 * taking filters and date ranges, they stop being the same kind of thing and
 * should separate then.
 */
export class SiteClient extends ApiClient {
  getBranding(): Promise<APIResponse> {
    return this.get(Api.branding);
  }

  updateBranding(payload: Partial<Branding>): Promise<APIResponse> {
    return this.put(Api.branding, { data: payload });
  }

  getReport(): Promise<APIResponse> {
    return this.get(Api.report);
  }

  async reportEntriesAsync(): Promise<ReportEntry[]> {
    const response = await this.getReport();
    const body = (await response.json()) as { report: ReportEntry[] };
    return body.report ?? [];
  }
}
