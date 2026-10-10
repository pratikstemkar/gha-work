<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { fetchDashboard } from '@/api/dashboard'
import { ApiError } from '@/api/client'
import type { Dashboard } from '@/types'

const dashboard = ref<Dashboard | null>(null)
const error = ref('')
const loading = ref(true)

onMounted(async () => {
  try {
    dashboard.value = await fetchDashboard()
  } catch (err) {
    error.value = err instanceof ApiError ? err.message : 'Unable to load the dashboard.'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div>
    <h1 class="text-2xl font-semibold text-slate-900">Dashboard</h1>

    <p v-if="loading" class="mt-4 text-sm text-slate-500">Loading…</p>
    <p v-else-if="error" class="mt-4 rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">{{ error }}</p>

    <template v-else-if="dashboard">
      <p class="mt-1 text-sm text-slate-500">{{ dashboard.greeting }}</p>

      <div class="mt-6 grid gap-4 sm:grid-cols-3">
        <div class="rounded-xl bg-white p-5 shadow-sm ring-1 ring-slate-200">
          <p class="text-sm font-medium text-slate-500">Total items</p>
          <p class="mt-2 text-3xl font-semibold text-slate-900">{{ dashboard.totalItems }}</p>
        </div>
        <div class="rounded-xl bg-white p-5 shadow-sm ring-1 ring-slate-200">
          <p class="text-sm font-medium text-slate-500">Completed</p>
          <p class="mt-2 text-3xl font-semibold text-emerald-600">{{ dashboard.completedItems }}</p>
        </div>
        <div class="rounded-xl bg-white p-5 shadow-sm ring-1 ring-slate-200">
          <p class="text-sm font-medium text-slate-500">Pending</p>
          <p class="mt-2 text-3xl font-semibold text-amber-600">{{ dashboard.pendingItems }}</p>
        </div>
      </div>

      <div class="mt-6 rounded-xl bg-white p-5 shadow-sm ring-1 ring-slate-200">
        <p class="text-sm font-medium text-slate-500">Signed in as</p>
        <p class="mt-1 text-base text-slate-900">
          {{ dashboard.user.displayName }}
          <span class="text-slate-400">({{ dashboard.user.username }})</span>
        </p>
        <div class="mt-2 flex flex-wrap gap-2">
          <span
            v-for="role in dashboard.user.roles"
            :key="role"
            class="rounded-full bg-slate-100 px-2.5 py-0.5 text-xs font-medium text-slate-600"
          >
            {{ role }}
          </span>
        </div>
      </div>
    </template>
  </div>
</template>
