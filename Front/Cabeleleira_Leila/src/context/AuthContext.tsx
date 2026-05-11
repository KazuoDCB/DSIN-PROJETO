import { useEffect, useMemo, useState } from 'react'
import type { ReactNode } from 'react'
import { adminLogin, login as loginRequest, logout as logoutRequest } from '../services/authService'
import type { Cliente, LoginRequest, UserRoleType } from '../Types'
import { ApiError } from '../utils/apiErrors'
import { AuthContext } from './authContextDefinition'
import type { LoginScope } from './authContextDefinition'

const USER_STORAGE_KEY = 'dsin-user'
const TOKEN_STORAGE_KEY = 'dsin-auth-token'
const ROLE_STORAGE_KEY = 'dsin-auth-role'

const readStoredUser = (): { user: Cliente | null; token: string | null; role: UserRoleType | null } => {
  try {
    const rawUser = localStorage.getItem(USER_STORAGE_KEY)
    const rawToken = localStorage.getItem(TOKEN_STORAGE_KEY)
    const rawRole = localStorage.getItem(ROLE_STORAGE_KEY)
    return {
      user: rawUser ? JSON.parse(rawUser) : null,
      token: rawToken || null,
      role: rawRole ? (Number(rawRole) as UserRoleType) : null,
    }
  } catch {
    return { user: null, token: null, role: null }
  }
}

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [session, setSession] = useState(readStoredUser)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!error) return

    const timeoutId = window.setTimeout(() => setError(null), 1500)
    return () => window.clearTimeout(timeoutId)
  }, [error])

  const signIn = async (credentials: LoginRequest, scope: LoginScope) => {
    setLoading(true)
    setError(null)

    try {
      const response = scope === 'admin' ? await adminLogin(credentials) : await loginRequest(credentials)
      if (response.data?.accessToken && response.data.cliente) {
        const cliente = response.data.cliente
        localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(cliente))
        localStorage.setItem(TOKEN_STORAGE_KEY, response.data.accessToken)
        localStorage.setItem(ROLE_STORAGE_KEY, String(response.data.role))
        setSession({ user: cliente, token: response.data.accessToken, role: response.data.role })
        return
      }

      const errors = response.errors?.length
        ? response.errors
        : [{ property: 'Login', message: 'Falha ao fazer login.' }]
      const apiError = new ApiError(errors)
      setError(apiError.message)
      throw apiError
    } catch (error) {
      if (error instanceof ApiError) setError(error.message)
      throw error
    } finally {
      setLoading(false)
    }
  }

  const signOut = () => {
    logoutRequest()
    localStorage.removeItem(USER_STORAGE_KEY)
    localStorage.removeItem(TOKEN_STORAGE_KEY)
    localStorage.removeItem(ROLE_STORAGE_KEY)
    setSession({ user: null, token: null, role: null })
  }

  const value = useMemo(
    () => ({ user: session.user, token: session.token, role: session.role, loading, error, signIn, signOut }),
    [session, loading, error],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
