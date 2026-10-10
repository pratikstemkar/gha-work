import { expect, test } from '@playwright/test'

test('anonymous visitors are redirected to the login page', async ({ page }) => {
  await page.goto('/')

  await expect(page).toHaveURL(/\/login/)
  await expect(page.getByRole('heading', { name: 'Sign in' })).toBeVisible()
})

test('a user can sign in and add an item', async ({ page }) => {
  await page.goto('/login')

  await page.getByLabel('Username').fill('demo')
  await page.getByLabel('Password').fill('demo123')
  await page.getByRole('button', { name: 'Sign in' }).click()

  await expect(page.getByRole('heading', { name: 'Dashboard' })).toBeVisible()
  await expect(page.getByText('Welcome back, Demo User!')).toBeVisible()

  await page.getByRole('link', { name: 'Items' }).click()
  await expect(page.getByRole('heading', { name: 'Items' })).toBeVisible()

  const title = `E2E item ${Date.now()}`
  await page.getByPlaceholder('Add an item…').fill(title)
  await page.getByRole('button', { name: 'Add' }).click()

  await expect(page.getByText(title)).toBeVisible()
})
