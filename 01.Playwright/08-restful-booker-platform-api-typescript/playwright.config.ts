import { defineConfig, devices } from '@playwright/test';

/**
 * The suite runs against a public, shared sandbox. Two consequences shape this
 * config:
 *
 *  - Every test cleans up what it creates (see fixtures/api.ts), because other
 *    people are using the same instance at the same time.
 *  - Workers are capped. The booking service rejects overlapping stays for the
 *    same room with a 409, and hammering a free-tier host from many workers
 *    produces flakiness that has nothing to do with the code under test.
 */
export default defineConfig({
  testDir: './tests',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 2 : 4,
  reporter: 'html',
  timeout: 60_000,
  expect: { timeout: 10_000 },
  use: {
    baseURL: 'https://automationintesting.online',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    actionTimeout: 15_000,
    navigationTimeout: 30_000,
    extraHTTPHeaders: {
      Accept: 'application/json',
    },
  },
  /**
   * Two projects, split by whether a browser is needed at all.
   *
   * The api project carries no `use.browserName` and matches only tests/api, so
   * those 44 tests never start Chromium. That is worth the extra config: it
   * takes seconds off every local run, and in CI it means the API suite can run
   * without `playwright install`, which is the slowest step in the pipeline.
   */
  projects: [
    {
      /** Pure API tests - no browser is launched at all. */
      name: 'api',
      testMatch: /api\/.*\.spec\.ts/,
    },
    {
      /** Tests that drive the UI, usually seeded through the API first. */
      name: 'hybrid',
      testMatch: /hybrid\/.*\.spec\.ts/,
      use: { ...devices['Desktop Chrome'] },
    },
  ],
});
