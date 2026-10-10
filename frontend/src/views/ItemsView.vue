<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { createItem, deleteItem, fetchItems, setItemDone } from '@/api/items'
import { ApiError } from '@/api/client'
import type { Item } from '@/types'

const items = ref<Item[]>([])
const newTitle = ref('')
const error = ref('')
const loading = ref(true)
const saving = ref(false)

onMounted(load)

async function load() {
  loading.value = true
  try {
    items.value = await fetchItems()
  } catch (err) {
    error.value = err instanceof ApiError ? err.message : 'Unable to load items.'
  } finally {
    loading.value = false
  }
}

async function add() {
  const title = newTitle.value.trim()
  if (!title) {
    return
  }

  saving.value = true
  error.value = ''
  try {
    const item = await createItem(title)
    items.value = [item, ...items.value]
    newTitle.value = ''
  } catch (err) {
    error.value = err instanceof ApiError ? err.message : 'Unable to add the item.'
  } finally {
    saving.value = false
  }
}

async function toggle(item: Item) {
  error.value = ''
  try {
    const updated = await setItemDone(item.id, !item.done)
    items.value = items.value.map((i) => (i.id === updated.id ? updated : i))
  } catch (err) {
    error.value = err instanceof ApiError ? err.message : 'Unable to update the item.'
  }
}

async function remove(item: Item) {
  error.value = ''
  try {
    await deleteItem(item.id)
    items.value = items.value.filter((i) => i.id !== item.id)
  } catch (err) {
    error.value = err instanceof ApiError ? err.message : 'Unable to delete the item.'
  }
}
</script>

<template>
  <div>
    <h1 class="text-2xl font-semibold text-slate-900">Items</h1>
    <p class="mt-1 text-sm text-slate-500">Stored in memory on the API. Resets when the server restarts.</p>

    <form class="mt-6 flex gap-2" @submit.prevent="add">
      <input
        v-model="newTitle"
        type="text"
        placeholder="Add an item…"
        class="flex-1 rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none transition focus:border-slate-900 focus:ring-2 focus:ring-slate-900/10"
      />
      <button
        type="submit"
        :disabled="saving || !newTitle.trim()"
        class="rounded-lg bg-slate-900 px-4 py-2 text-sm font-semibold text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-60"
      >
        Add
      </button>
    </form>

    <p v-if="error" class="mt-4 rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">{{ error }}</p>

    <p v-if="loading" class="mt-6 text-sm text-slate-500">Loading…</p>

    <p v-else-if="items.length === 0" class="mt-6 rounded-xl bg-white p-6 text-center text-sm text-slate-500 shadow-sm ring-1 ring-slate-200">
      No items yet. Add one above.
    </p>

    <ul v-else class="mt-6 space-y-2">
      <li
        v-for="item in items"
        :key="item.id"
        class="flex items-center gap-3 rounded-xl bg-white px-4 py-3 shadow-sm ring-1 ring-slate-200"
      >
        <input
          type="checkbox"
          :checked="item.done"
          class="h-4 w-4 shrink-0 cursor-pointer accent-slate-900"
          @change="toggle(item)"
        />
        <span class="flex-1 text-sm" :class="item.done ? 'text-slate-400 line-through' : 'text-slate-900'">
          {{ item.title }}
        </span>
        <button
          type="button"
          class="text-sm font-medium text-red-600 transition hover:text-red-700"
          @click="remove(item)"
        >
          Delete
        </button>
      </li>
    </ul>
  </div>
</template>
