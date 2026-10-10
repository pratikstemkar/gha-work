import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { login as loginRequest } from '@/api/auth'
import type { User } from '@/types'

const TOKEN_KEY = 'app.token'
const USER_KEY = 'app.user'

function readStoredUser(): User | null {
  const raw = sessionStorage.getItem(USER_KEY)
  if (!raw) {
    return null
  }
  try {
    return JSON.parse(raw) as User
  } catch {
    return null
  }
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(sessionStorage.getItem(TOKEN_KEY))
  const user = ref<User | null>(readStoredUser())

  const isAuthenticated = computed(() => Boolean(token.value))
  const displayName = computed(() => user.value?.displayName ?? user.value?.username ?? '')

  function persist() {
    if (token.value) {
      sessionStorage.setItem(TOKEN_KEY, token.value)
    } else {
      sessionStorage.removeItem(TOKEN_KEY)
    }
    if (user.value) {
      sessionStorage.setItem(USER_KEY, JSON.stringify(user.value))
    } else {
      sessionStorage.removeItem(USER_KEY)
    }
  }

  async function login(username: string, password: string) {
    const result = await loginRequest(username, password)
    token.value = result.token
    user.value = result.user
    persist()
  }

  function setUser(value: User) {
    user.value = value
    persist()
  }

  function clear() {
    token.value = null
    user.value = null
    persist()
  }

  return { token, user, isAuthenticated, displayName, login, setUser, clear }
})
