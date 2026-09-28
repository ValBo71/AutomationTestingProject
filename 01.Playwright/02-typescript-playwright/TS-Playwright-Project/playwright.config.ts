import { defineConfig, devices } from '@playwright/test';

/**
 * Headless by default, everywhere - a machine without a display (CI, WSL, SSH, Docker) runs the suite
 * as-is. To watch it: `npm run test:headed`, and add SLOW_MO=500 (milliseconds per action) to slow it
 * down. Slow motion used to be on for every local run at 1000 ms, which made a local run far slower
 * than CI and would eventually have pushed longer tests past the 30 s timeout on developer machines only.
 */
const slowMo = Number(process.env.SLOW_MO ?? 0);

export default defineConfig({
  testDir: './tests',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  // Retries absorb a genuine network blip on the public demo site, but a test that only passes on a
  // retry is flaky, and the run should say so: in CI a flaky test fails the run instead of hiding in
  // the HTML report.
  failOnFlakyTests: !!process.env.CI,
  workers: process.env.CI ? 1 : undefined,
  reporter: 'html',
  use: {
    baseURL: 'https://www.saucedemo.com',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    headless: true,
    launchOptions: {
      slowMo: slowMo > 0 ? slowMo : undefined,
    },
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
});
