import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { useAuthStore } from '@/stores/auth'
import * as authApi from '@/api/auth'
import type { User } from '@/types'

vi.mock('@/api/auth', () => ({ login: vi.fn(), fetchMe: vi.fn() }))

const demoUser: User = {
  username: 'demo',
  displayName: 'Demo User',
  email: 'demo@example.com',
  roles: ['User'],
}

describe('auth store', () => {
  beforeEach(() => {
    sessionStorage.clear()
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('starts signed out with no user', () => {
    const auth = useAuthStore()
    expect(auth.isAuthenticated).toBe(false)
    expect(auth.token).toBeNull()
    expect(auth.user).toBeNull()
  })

  it('stores the token and user after a successful login', async () => {
    vi.mocked(authApi.login).mockResolvedValue({ token: 'token-abc', user: demoUser })

    const auth = useAuthStore()
    await auth.login('demo', 'demo123')

    expect(authApi.login).toHaveBeenCalledWith('demo', 'demo123')
    expect(auth.isAuthenticated).toBe(true)
    expect(auth.token).toBe('token-abc')
    expect(auth.displayName).toBe('Demo User')
    expect(sessionStorage.getItem('app.token')).toBe('token-abc')
  })

  it('loads a persisted session from sessionStorage', () => {
    sessionStorage.setItem('app.token', 'persisted-token')
    sessionStorage.setItem('app.user', JSON.stringify(demoUser))
    setActivePinia(createPinia())

    const auth = useAuthStore()
    expect(auth.isAuthenticated).toBe(true)
    expect(auth.displayName).toBe('Demo User')
  })

  it('clears token, user and storage on clear()', async () => {
    vi.mocked(authApi.login).mockResolvedValue({ token: 'token-abc', user: demoUser })
    const auth = useAuthStore()
    await auth.login('demo', 'demo123')

    auth.clear()

    expect(auth.isAuthenticated).toBe(false)
    expect(auth.token).toBeNull()
    expect(sessionStorage.getItem('app.token')).toBeNull()
    expect(sessionStorage.getItem('app.user')).toBeNull()
  })
})
