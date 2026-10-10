import { beforeEach, describe, expect, it } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import type { RouteLocationNormalized } from 'vue-router'
import { authGuard } from '@/router'
import { useAuthStore } from '@/stores/auth'

function route(overrides: Partial<RouteLocationNormalized>): RouteLocationNormalized {
  return { fullPath: '/', path: '/', meta: {}, name: undefined, ...overrides } as unknown as RouteLocationNormalized
}

describe('authGuard', () => {
  beforeEach(() => {
    sessionStorage.clear()
    setActivePinia(createPinia())
  })

  it('sends anonymous users to login, remembering where they were headed', () => {
    const result = authGuard(route({ fullPath: '/items', meta: {} }))
    expect(result).toEqual({ name: 'login', query: { redirect: '/items' } })
  })

  it('lets anonymous users see public routes', () => {
    expect(authGuard(route({ name: 'login', meta: { public: true } }))).toBeUndefined()
  })

  it('lets signed-in users through to protected routes', () => {
    useAuthStore().token = 'token-123'
    expect(authGuard(route({ name: 'items', meta: {} }))).toBeUndefined()
  })

  it('bounces signed-in users away from the login page', () => {
    useAuthStore().token = 'token-123'
    expect(authGuard(route({ name: 'login', meta: { public: true } }))).toEqual({ name: 'dashboard' })
  })
})
