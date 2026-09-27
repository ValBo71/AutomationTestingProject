import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: 'html',
  // The site is a free Heroku app and occasionally responds slowly, so the
  // per-test budget is deliberately more generous than the 30s default.
  timeout: 60_000,
  expect: { timeout: 10_000 },
  use: {
    baseURL: 'https://the-internet.herokuapp.com',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    actionTimeout: 15_000,
    navigationTimeout: 30_000,
    // Unlike the saucedemo project, this suite runs headless by default even
    // locally - with 60-odd tests, a slowed-down headed run is impractical.
    // Use `npm run test:headed` when you actually need to watch a scenario.
    acceptDownloads: true,
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
});
