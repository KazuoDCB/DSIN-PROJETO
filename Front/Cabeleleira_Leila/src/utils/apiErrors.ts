import type { ErrorMessage } from '../Types'

export type FeedbackContent = string | ErrorMessage[]

const FALLBACK_ERROR: ErrorMessage = {
  property: 'Erro',
  message: 'Erro inesperado. Tente novamente em alguns instantes.',
}

export class ApiError extends Error {
  errors: ErrorMessage[]

  constructor(errors: ErrorMessage[]) {
    const validErrors = errors.length > 0 ? errors : [FALLBACK_ERROR]
    super(validErrors.map((error) => error.message).join(' '))
    this.name = 'ApiError'
    this.errors = validErrors
  }
}

export const toFeedbackContent = (error: unknown, fallback = FALLBACK_ERROR.message): FeedbackContent => {
  if (error instanceof ApiError) return error.errors
  if (error instanceof Error && error.message) return error.message

  return fallback
}
