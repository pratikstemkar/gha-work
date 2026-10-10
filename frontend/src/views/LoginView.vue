<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { ApiError } from '@/api/client'

const auth = useAuthStore()
const router = useRouter()
const route = useRoute()

const username = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

async function submit() {
  error.value = ''
  loading.value = true
  try {
    await auth.login(username.value.trim(), password.value)
    const redirect = route.query.redirect
    await router.push(typeof redirect === 'string' ? redirect : { name: 'dashboard' })
  } catch (err) {
    error.value = err instanceof ApiError ? err.message : 'Unable to reach the server.'
  } finally {
    loading.value = false
  }
}

function useDemo() {
  username.value = 'demo'
  password.value = 'demo123'
}
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-slate-100 px-4">
    <div class="w-full max-w-sm rounded-2xl bg-white p-8 shadow-sm ring-1 ring-slate-200">
      <div class="mb-6 text-center">
        <h1 class="text-2xl font-semibold text-slate-900">Sign in</h1>
        <p class="mt-1 text-sm text-slate-500">Welcome back to App</p>
      </div>

      <form class="space-y-4" @submit.prevent="submit">
        <div>
          <label for="username" class="mb-1 block text-sm font-medium text-slate-700">Username</label>
          <input
            id="username"
            v-model="username"
            type="text"
            autocomplete="username"
            required
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none transition focus:border-slate-900 focus:ring-2 focus:ring-slate-900/10"
          />
        </div>

        <div>
          <label for="password" class="mb-1 block text-sm font-medium text-slate-700">Password</label>
          <input
            id="password"
            v-model="password"
            type="password"
            autocomplete="current-password"
            required
            class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none transition focus:border-slate-900 focus:ring-2 focus:ring-slate-900/10"
          />
        </div>

        <p v-if="error" class="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700">{{ error }}</p>

        <button
          type="submit"
          :disabled="loading"
          class="w-full rounded-lg bg-slate-900 px-4 py-2 text-sm font-semibold text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-60"
        >
          {{ loading ? 'Signing in…' : 'Sign in' }}
        </button>
      </form>

      <div class="mt-6 rounded-lg bg-slate-50 px-4 py-3 text-sm text-slate-600">
        <p class="font-medium text-slate-700">Demo account</p>
        <p class="mt-1">Username <code class="rounded bg-white px-1">demo</code> · Password <code class="rounded bg-white px-1">demo123</code></p>
        <button type="button" class="mt-2 text-sm font-medium text-slate-900 underline" @click="useDemo">
          Fill demo credentials
        </button>
      </div>
    </div>
  </div>
</template>
