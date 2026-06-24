import { expect, test } from '@playwright/test';

test.describe('Dashboard', () => {
  test('root redirects to dashboard and shows seeded ownership changes', async ({ page }) => {
    await page.goto('/');
    await expect(page).toHaveURL(/\/dashboard$/);
    await expect(page.getByRole('heading', { name: /Dashboard/i })).toBeVisible();

    // Seeded data: Maisonette changed hands; Carmen Attard is the current owner.
    await expect(page.getByText('Maisonette').first()).toBeVisible();
    await expect(page.getByText('Carmen Attard').first()).toBeVisible();
  });

  test('shows a Sold At (USD) column with a value or N/A fallback', async ({ page }) => {
    await page.goto('/dashboard');
    await expect(page.getByRole('columnheader', { name: /Sold At \(USD\)/i })).toBeVisible();
    // Either a converted USD figure or the documented N/A fallback must render.
    const usdCells = page.locator('td');
    await expect(usdCells.filter({ hasText: /\$|N\/A/ }).first()).toBeVisible();
  });
});
