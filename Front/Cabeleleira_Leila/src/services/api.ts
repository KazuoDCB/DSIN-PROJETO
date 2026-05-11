import type { ErrorMessage, OperationResult } from '../Types'
import { ApiError } from '../utils/apiErrors'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5165'
const AUTH_TOKEN_KEY = 'dsin-auth-token'
const UNEXPECTED_ERROR_MESSAGE = 'Erro inesperado. Tente novamente em alguns instantes.'
const CONNECTION_ERROR_MESSAGE = 'Nao foi possivel concluir a acao. Verifique sua conexao e tente novamente.'

export const getAuthToken = (): string | null => localStorage.getItem(AUTH_TOKEN_KEY)

export const saveAuthToken = (token: string | null) => {
  if (token) {
    localStorage.setItem(AUTH_TOKEN_KEY, token)
    return
  }

  localStorage.removeItem(AUTH_TOKEN_KEY)
}

const isErrorMessage = (value: unknown): value is ErrorMessage => {
  if (!value || typeof value !== 'object') return false

  return 'message' in value || 'Message' in value || 'property' in value || 'Property' in value
}

const normalizeErrorMessage = (value: unknown): ErrorMessage | null => {
  if (!isErrorMessage(value)) return null

  const rawError = value as Partial<ErrorMessage> & {
    Message?: unknown
    Property?: unknown
  }
  const property = typeof rawError.property === 'string'
    ? rawError.property
    : typeof rawError.Property === 'string'
      ? rawError.Property
      : 'Erro'
  const message = typeof rawError.message === 'string'
    ? rawError.message
    : typeof rawError.Message === 'string'
      ? rawError.Message
      : ''

  if (!message.trim()) return null

  return {
    property,
    message: toFriendlyMessage(message),
  }
}

const readMessage = (payload: unknown): string | null => {
  if (!payload || typeof payload !== 'object' || !('message' in payload)) return null

  const message = payload.message
  return typeof message === 'string' ? message : null
}

const isTechnicalMessage = (message: string): boolean => {
  const normalizedMessage = message.toLowerCase()
  const technicalTerms = [
    'api',
    'exception',
    'failed to fetch',
    'fetch',
    'networkerror',
    'stack',
    'status code',
    'timestamp',
    'trace',
    'typeerror',
  ]

  return technicalTerms.some((term) => normalizedMessage.includes(term))
}

const toFriendlyMessage = (message: string | null | undefined, fallback = UNEXPECTED_ERROR_MESSAGE): string => {
  if (!message) return fallback

  const trimmedMessage = message.trim()
  if (!trimmedMessage || isTechnicalMessage(trimmedMessage)) return fallback

  return trimmedMessage
}

const parseApiErrors = (payload: unknown): ErrorMessage[] => {
  if (!payload) {
    return [{ property: 'Erro', message: UNEXPECTED_ERROR_MESSAGE }]
  }

  if (typeof payload === 'object') {
    const rawPayload = payload as { errors?: unknown; Errors?: unknown }
    const rawErrors = Array.isArray(rawPayload.errors)
      ? rawPayload.errors
      : Array.isArray(rawPayload.Errors)
        ? rawPayload.Errors
        : null

    const errors = rawErrors
      ?.map(normalizeErrorMessage)
      .filter((error): error is ErrorMessage => Boolean(error))
      ?? []

    if (errors.length > 0) return errors
  }

  return [{ property: 'Erro', message: toFriendlyMessage(readMessage(payload)) }]
}

export async function apiFetch<T>(path: string, options: RequestInit = {}): Promise<OperationResult<T>> {
  const headers = new Headers(options.headers)

  if (!(options.body instanceof FormData)) {
    headers.set('Content-Type', 'application/json')
  }

  const token = getAuthToken()
  if (token) {
    headers.set('Authorization', `Bearer ${token}`)
  }

  let response: Response

  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      ...options,
      headers,
    })
  } catch {
    throw new Error(CONNECTION_ERROR_MESSAGE)
  }

  const body = await response.text()
  let payload: unknown

  try {
    payload = body ? JSON.parse(body) : null
  } catch {
    throw new Error(UNEXPECTED_ERROR_MESSAGE)
  }

  if (!response.ok) {
    const errors = parseApiErrors(payload)
    throw new ApiError(errors)
  }

  return payload as OperationResult<T>
}
