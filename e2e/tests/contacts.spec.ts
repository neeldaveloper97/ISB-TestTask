import { expect, test } from '@playwright/test';

test.describe('Contacts', () => {
  test('lists seeded contacts', async ({ page }) => {
    await page.goto('/contacts');
    await expect(page.getByRole('heading', { name: 'Contacts' })).toBeVisible();
    await expect(page.getByText('Carmen Attard')).toBeVisible();
    await expect(page.getByText('Joe Borg')).toBeVisible();
  });

  test('contact detail shows properties owned past and present', async ({ page }) => {
    await page.goto('/contacts');
    await page.getByText('Carmen Attard').click();

    await expect(page.getByRole('heading', { name: 'Carmen Attard' })).toBeVisible();
    await expect(page.getByRole('heading', { name: /Properties Owned/i })).toBeVisible();
    // Carmen is the current owner of the Maisonette per the seed data.
    await expect(page.getByText('Maisonette')).toBeVisible();
  });

  test('creates a new contact', async ({ page }) => {
    const last = `Tester${Date.now()}`;
    await page.goto('/contacts/new');

    await page.getByLabel('First name').fill('Pat');
    await page.getByLabel('Last name').fill(last);
    await page.getByLabel('Email').fill(`pat.${Date.now()}@example.com`);
    await page.getByRole('button', { name: 'Create' }).click();

    await expect(page.getByRole('heading', { name: `Pat ${last}` })).toBeVisible();
  });
});
