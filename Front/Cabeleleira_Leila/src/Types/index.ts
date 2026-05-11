export type { ErrorMessage, OperationResult } from './api'
export { UserRole } from './auth'
export type { LoginRequest, LoginResponse, UserRole as UserRoleType } from './auth'
export type { Cliente, ClienteRequest, ClienteUpdateRequest } from './cliente'
export type {
  Scheduling,
  SchedulingRequest,
  SchedulingStatus,
  SchedulingStatusUpdateRequest,
  SchedulingUpdateRequest,
} from './agendamento'
export type { Servico, ServicoRequest, ServicoUpdateRequest } from './servico'
export { Status, statusText } from './status'
export type { Status as StatusType } from './status'
