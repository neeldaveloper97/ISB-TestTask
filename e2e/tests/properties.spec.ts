import { expect, test } from '@playwright/test';

test.describe('Properties', () => {
  test('lists seeded properties', async ({ page }) => {
    await page.goto('/properties');
    await expect(page.getByRole('heading', { name: 'Properties' })).toBeVisible();
    await expect(page.getByText('Maisonette')).toBeVisible();
    await expect(page.getByText('Penthouse')).toBeVisible();
  });

  test('filters the list by name', async ({ page }) => {
    await page.goto('/properties');
    await page.getByLabel('Name').fill('Penthouse');
    await page.getByRole('button', { name: 'Search' }).click();

    await expect(page.getByText('Penthouse')).toBeVisible();
    await expect(page.getByText('Maisonette')).toHaveCount(0);
  });

  test('creates a new property and lands on its detail page', async ({ page }) => {
    const unique = `Test Villa ${Date.now()}`;
    await page.goto('/properties/new');

    await page.getByLabel('Name').fill(unique);
    await page.getByLabel('Address').fill('99 Test Street');
    await page.getByLabel('Price').fill('275000');
    await page.getByRole('button', { name: 'Create' }).click();

    await expect(page.getByRole('heading', { name: unique })).toBeVisible();
    // Creating a property registers an initial price-history record.
    await expect(page.getByRole('heading', { name: 'Price History' })).toBeVisible();
  });
});
