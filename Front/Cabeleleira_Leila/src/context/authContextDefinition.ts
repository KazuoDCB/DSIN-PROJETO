import { createContext } from 'react'
import type { Cliente, LoginRequest, UserRoleType } from '../Types'

export type LoginScope = 'cliente' | 'admin'

export interface AuthContextValue {
  user: Cliente | null
  token: string | null
  role: UserRoleType | null
  loading: boolean
  error: string | null
  signIn: (credentials: LoginRequest, scope: LoginScope) => Promise<void>
  signOut: () => void
}

export const AuthContext = createContext<AuthContextValue | null>(null)
