<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { fetchMe } from '@/api/auth'
import { useAuthStore } from '@/stores/auth'
import { ApiError } from '@/api/client'
import type { User } from '@/types'

const auth = useAuthStore()
const router = useRouter()

const profile = ref<User | null>(auth.user)
const error = ref('')
const loading = ref(true)

onMounted(async () => {
  try {
    const user = await fetchMe()
    profile.value = user
    auth.setUser(user)
  } catch (err) {
    error.value = err instanceof ApiError ? err.message : 'Unable to load your profile.'
  } finally {
    loading.value = false
  }
})

function logout() {
  auth.clear()
  router.push({ name: 'login' })
}
</script>

<template>
  <div>
    <h1 class="text-2xl font-semibold text-slate-900">Profile</h1>

    <p v-if="loading" class="mt-4 text-sm text-slate-500">Loading…</p>
    <p v-else-if="error" class="mt-4 rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">{{ error }}</p>

    <div v-else-if="profile" class="mt-6 max-w-lg rounded-xl bg-white p-6 shadow-sm ring-1 ring-slate-200">
      <dl class="space-y-4">
        <div>
          <dt class="text-sm font-medium text-slate-500">Display name</dt>
          <dd class="mt-1 text-base text-slate-900">{{ profile.displayName }}</dd>
        </div>
        <div>
          <dt class="text-sm font-medium text-slate-500">Username</dt>
          <dd class="mt-1 text-base text-slate-900">{{ profile.username }}</dd>
        </div>
        <div>
          <dt class="text-sm font-medium text-slate-500">Email</dt>
          <dd class="mt-1 text-base text-slate-900">{{ profile.email }}</dd>
        </div>
        <div>
          <dt class="text-sm font-medium text-slate-500">Roles</dt>
          <dd class="mt-1 flex flex-wrap gap-2">
            <span
              v-for="role in profile.roles"
              :key="role"
              class="rounded-full bg-slate-100 px-2.5 py-0.5 text-xs font-medium text-slate-600"
            >
              {{ role }}
            </span>
          </dd>
        </div>
      </dl>

      <button
        type="button"
        class="mt-6 rounded-lg bg-slate-900 px-4 py-2 text-sm font-semibold text-white transition hover:bg-slate-800"
        @click="logout"
      >
        Log out
      </button>
    </div>
  </div>
</template>
