<script setup lang="ts">
import { RouterLink, RouterView, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const router = useRouter()

const navigation = [
  { name: 'Dashboard', to: { name: 'dashboard' } },
  { name: 'Items', to: { name: 'items' } },
  { name: 'Profile', to: { name: 'profile' } },
]

function logout() {
  auth.clear()
  router.push({ name: 'login' })
}
</script>

<template>
  <div class="min-h-screen bg-slate-100 text-slate-900">
    <header class="border-b border-slate-200 bg-white">
      <div class="mx-auto flex max-w-5xl items-center justify-between gap-4 px-6 py-4">
        <div class="flex items-center gap-8">
          <span class="text-lg font-semibold text-slate-900">App</span>
          <nav class="flex items-center gap-1">
            <RouterLink
              v-for="link in navigation"
              :key="link.name"
              :to="link.to"
              class="rounded-md px-3 py-1.5 text-sm font-medium text-slate-600 transition hover:bg-slate-100 hover:text-slate-900"
              active-class="bg-slate-900 text-white hover:bg-slate-900 hover:text-white"
            >
              {{ link.name }}
            </RouterLink>
          </nav>
        </div>

        <div class="flex items-center gap-3">
          <span class="hidden text-sm text-slate-500 sm:inline">{{ auth.displayName }}</span>
          <button
            type="button"
            class="rounded-md border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-700 transition hover:bg-slate-100"
            @click="logout"
          >
            Log out
          </button>
        </div>
      </div>
    </header>

    <main class="mx-auto max-w-5xl px-6 py-8">
      <RouterView />
    </main>
  </div>
</template>
