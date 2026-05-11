import type { Cliente } from './cliente'

export const UserRole = {
  Cliente: 0,
  Admin: 1,
} as const

export type UserRole = (typeof UserRole)[keyof typeof UserRole]

export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  accessToken: string
  expiresAt: string
  role: UserRole
  cliente: Cliente
}
