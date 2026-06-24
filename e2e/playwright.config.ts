import { defineConfig, devices } from '@playwright/test';

const UI_URL = process.env['UI_URL'] || 'http://localhost:4200';
const API_URL = process.env['API_URL'] || 'http://localhost:5000';

/**
 * Runs the E2E suite against a live stack.
 * By default Playwright will start both the API and the Angular dev server.
 * Set PW_NO_SERVER=1 to run against an already-running stack instead.
 */
const startServers = !process.env['PW_NO_SERVER'];

export default defineConfig({
  testDir: './tests',
  timeout: 30_000,
  expect: { timeout: 10_000 },
  fullyParallel: false,
  retries: process.env['CI'] ? 1 : 0,
  reporter: [['list'], ['html', { open: 'never' }]],
  use: {
    baseURL: UI_URL,
    trace: 'on-first-retry',
    screenshot: 'only-on-failure'
  },
  projects: [
    { name: 'chromium', use: { ...devices['Desktop Chrome'] } }
  ],
  webServer: startServers
    ? [
        {
          command: 'dotnet run --project ../PropertyInventory.API',
          url: `${API_URL}/swagger/index.html`,
          timeout: 120_000,
          reuseExistingServer: true
        },
        {
          command: 'npm --prefix ../property-inventory-ui start',
          url: UI_URL,
          timeout: 120_000,
          reuseExistingServer: true
        }
      ]
    : undefined
});
