import type { Status } from './status'
import type { UserRole } from './auth'

export interface Cliente {
  id: number
  name: string
  number: string
  email: string
  status: Status
  role: UserRole
  createdAt: string
  updatedAt: string
}

export interface ClienteRequest {
  name: string
  number: string
  email: string
  password: string
}

export interface ClienteUpdateRequest {
  name: string
  number: string
  email: string
  status: Status
  role: UserRole
}
