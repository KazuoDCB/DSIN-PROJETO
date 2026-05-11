import { apiFetch, saveAuthToken } from './api'
import type { LoginRequest, LoginResponse, OperationResult } from '../Types'

export const login = async (request: LoginRequest): Promise<OperationResult<LoginResponse>> => {
  const response = await apiFetch<LoginResponse>('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify(request),
  })

  if (response.data?.accessToken) {
    saveAuthToken(response.data.accessToken)
  }

  return response
}

export const adminLogin = async (request: LoginRequest): Promise<OperationResult<LoginResponse>> => {
  const response = await apiFetch<LoginResponse>('/api/auth/admin/login', {
    method: 'POST',
    body: JSON.stringify(request),
  })

  if (response.data?.accessToken) {
    saveAuthToken(response.data.accessToken)
  }

  return response
}

export const logout = () => {
  saveAuthToken(null)
}
