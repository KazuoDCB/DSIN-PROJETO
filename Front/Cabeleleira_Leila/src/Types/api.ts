export interface ErrorMessage {
  property: string
  message: string
}

export interface OperationResult<T = null> {
  success: boolean
  statusCode: number
  errors: ErrorMessage[]
  data: T | null
}
