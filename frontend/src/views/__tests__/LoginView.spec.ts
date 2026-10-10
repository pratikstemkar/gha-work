import { beforeEach, describe, expect, it, vi } from 'vitest'
import { flushPromises, mount } from '@vue/test-utils'
import { createPinia } from 'pinia'
import LoginView from '@/views/LoginView.vue'
import { ApiError } from '@/api/client'

const { push } = vi.hoisted(() => ({ push: vi.fn() }))
const authApi = vi.hoisted(() => ({ login: vi.fn(), fetchMe: vi.fn() }))

vi.mock('vue-router', () => ({
  useRouter: () => ({ push }),
  useRoute: () => ({ query: {} }),
}))
vi.mock('@/api/auth', () => authApi)

function mountLogin() {
  return mount(LoginView, { global: { plugins: [createPinia()] } })
}

describe('LoginView', () => {
  beforeEach(() => {
    sessionStorage.clear()
    vi.clearAllMocks()
  })

  it('logs in and redirects to the dashboard', async () => {
    authApi.login.mockResolvedValue({
      token: 'token-abc',
      user: { username: 'demo', displayName: 'Demo User', email: 'demo@example.com', roles: ['User'] },
    })

    const wrapper = mountLogin()
    await wrapper.find('#username').setValue('demo')
    await wrapper.find('#password').setValue('demo123')
    await wrapper.find('form').trigger('submit')
    await flushPromises()

    expect(authApi.login).toHaveBeenCalledWith('demo', 'demo123')
    expect(push).toHaveBeenCalledWith({ name: 'dashboard' })
  })

  it('shows an error when the credentials are rejected', async () => {
    authApi.login.mockRejectedValue(new ApiError(401, 'Invalid username or password.'))

    const wrapper = mountLogin()
    await wrapper.find('#username').setValue('demo')
    await wrapper.find('#password').setValue('nope')
    await wrapper.find('form').trigger('submit')
    await flushPromises()

    expect(wrapper.text()).toContain('Invalid username or password.')
    expect(push).not.toHaveBeenCalled()
  })

  it('fills in the demo credentials', async () => {
    const wrapper = mountLogin()
    await wrapper.find('button[type="button"]').trigger('click')

    expect((wrapper.find('#username').element as HTMLInputElement).value).toBe('demo')
    expect((wrapper.find('#password').element as HTMLInputElement).value).toBe('demo123')
  })
})
