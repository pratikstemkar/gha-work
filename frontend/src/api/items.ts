import { apiFetch } from './client'
import type { Item } from '@/types'

export function fetchItems() {
  return apiFetch<Item[]>('/api/items')
}

export function createItem(title: string) {
  return apiFetch<Item>('/api/items', {
    method: 'POST',
    body: JSON.stringify({ title }),
  })
}

export function setItemDone(id: number, done: boolean) {
  return apiFetch<Item>(`/api/items/${id}`, {
    method: 'PATCH',
    body: JSON.stringify({ done }),
  })
}

export function deleteItem(id: number) {
  return apiFetch<void>(`/api/items/${id}`, { method: 'DELETE' })
}
