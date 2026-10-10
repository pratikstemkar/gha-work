import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { ApiError, apiFetch } from '@/api/client'
import { useAuthStore } from '@/stores/auth'

function jsonResponse(body: unknown, status = 200) {
  return new Response(JSON.stringify(body), {
    status,
    headers: { 'Content-Type': 'application/json' },
  })
}

describe('apiFetch', () => {
  beforeEach(() => {
    sessionStorage.clear()
    setActivePinia(createPinia())
  })

  afterEach(() => {
    vi.unstubAllGlobals()
    vi.restoreAllMocks()
  })

  it('returns the parsed JSON body', async () => {
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(jsonResponse({ id: 1, title: 'Alpha' })))

    const result = await apiFetch<{ id: number; title: string }>('/api/items')

    expect(result).toEqual({ id: 1, title: 'Alpha' })
  })

  it('attaches the bearer token when signed in', async () => {
    const fetchMock = vi.fn().mockResolvedValue(jsonResponse([]))
    vi.stubGlobal('fetch', fetchMock)

    const auth = useAuthStore()
    auth.token = 'token-123'

    await apiFetch('/api/items')

    const [, options] = fetchMock.mock.calls[0] as [string, RequestInit]
    expect(new Headers(options.headers).get('Authorization')).toBe('Bearer token-123')
  })

  it('sets a JSON content type when a body is present', async () => {
    const fetchMock = vi.fn().mockResolvedValue(jsonResponse({ id: 1 }))
    vi.stubGlobal('fetch', fetchMock)

    await apiFetch('/api/items', { method: 'POST', body: JSON.stringify({ title: 'Alpha' }) })

    const [, options] = fetchMock.mock.calls[0] as [string, RequestInit]
    expect(new Headers(options.headers).get('Content-Type')).toBe('application/json')
  })

  it('clears the session and throws on 401', async () => {
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(new Response('', { status: 401 })))

    const auth = useAuthStore()
    auth.token = 'token-123'

    await expect(apiFetch('/api/items')).rejects.toBeInstanceOf(ApiError)
    expect(auth.isAuthenticated).toBe(false)
    expect(auth.token).toBeNull()
  })

  it('surfaces the server error message', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue(jsonResponse({ message: 'Title is required.' }, 400)),
    )

    await expect(apiFetch('/api/items', { method: 'POST', body: '{}' })).rejects.toThrow(
      'Title is required.',
    )
  })

  it('returns undefined for 204 responses', async () => {
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(new Response(null, { status: 204 })))

    await expect(apiFetch('/api/items/1', { method: 'DELETE' })).resolves.toBeUndefined()
  })
})
