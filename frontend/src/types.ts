export interface User {
  username: string
  displayName: string
  email: string
  roles: string[]
}

export interface LoginResponse {
  token: string
  user: User
}

export interface Item {
  id: number
  title: string
  done: boolean
  createdAt: string
}

export interface Dashboard {
  greeting: string
  user: User
  totalItems: number
  completedItems: number
  pendingItems: number
}
