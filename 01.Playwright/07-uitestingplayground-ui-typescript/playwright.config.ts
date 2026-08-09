import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: 'html',
  timeout: 60_000,
  expect: { timeout: 10_000 },
  use: {
    baseURL: 'http://uitestingplayground.com',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    actionTimeout: 15_000,
    navigationTimeout: 30_000,
    acceptDownloads: true,
    /**
     * The site's TLS certificate is issued for *.azurewebsites.net and does not
     * cover uitestingplayground.com, so an HTTPS run fails the certificate
     * check outright. The baseURL above therefore uses http://, and this flag
     * keeps the suite working if anyone switches it to https://.
     */
    ignoreHTTPSErrors: true,
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
});
