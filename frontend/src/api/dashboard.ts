import { apiFetch } from './client'
import type { Dashboard } from '@/types'

export function fetchDashboard() {
  return apiFetch<Dashboard>('/api/dashboard')
}
