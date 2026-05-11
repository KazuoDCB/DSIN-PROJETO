import type { Servico } from './servico'

export const SchedulingStatus = {
  Agendado: 0,
  Confirmado: 1,
  EmAndamento: 2,
  Finalizado: 3,
  Cancelado: 4,
} as const

export type SchedulingStatus = (typeof SchedulingStatus)[keyof typeof SchedulingStatus]

export interface Scheduling {
  id: number
  clienteId: number
  clienteName: string
  dataHora: string
  status: SchedulingStatus
  totalValue: number
  totalDuration: number
  servicos: Servico[]
  createdAt: string
  updatedAt: string
}

export interface SchedulingStatusUpdateRequest {
  status: SchedulingStatus
}

export interface SchedulingRequest {
  clienteId: number
  dataHora: string
  servicoIds: number[]
}

export interface SchedulingUpdateRequest {
  dataHora: string
  status: SchedulingStatus
  servicoIds: number[]
}
