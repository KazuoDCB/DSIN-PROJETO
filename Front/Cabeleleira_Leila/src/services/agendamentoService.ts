import { apiFetch } from './api'
import type {
  Scheduling,
  SchedulingRequest,
  SchedulingUpdateRequest,
  OperationResult,
  SchedulingStatus,
} from '../Types'

export const getAgendamentos = async (page = 1, size = 100): Promise<OperationResult<Scheduling[]>> => {
  return apiFetch<Scheduling[]>(`/api/agendamentos?size=${size}&page=${page}`)
}

export const getAgendamentoById = async (id: number): Promise<OperationResult<Scheduling>> => {
  return apiFetch<Scheduling>(`/api/agendamentos/${id}`)
}

export const getAgendamentosByCliente = async (
  clienteId: number,
  dataInicio?: string,
  dataFim?: string,
): Promise<OperationResult<Scheduling[]>> => {
  const query = new URLSearchParams()
  if (dataInicio) query.set('dataInicio', dataInicio)
  if (dataFim) query.set('dataFim', dataFim)

  return apiFetch<Scheduling[]>(`/api/agendamentos/cliente/${clienteId}?${query.toString()}`)
}

export const createAgendamento = async (
  request: SchedulingRequest,
): Promise<OperationResult<Scheduling>> => {
  return apiFetch<Scheduling>('/api/agendamentos', {
    method: 'POST',
    body: JSON.stringify(request),
  })
}

export const updateAgendamento = async (
  agendamentoId: number,
  request: SchedulingUpdateRequest,
): Promise<OperationResult<Scheduling>> => {
  return apiFetch<Scheduling>(`/api/agendamentos/${agendamentoId}`, {
    method: 'PUT',
    body: JSON.stringify(request),
  })
}

export const adminUpdateAgendamento = async (
  agendamentoId: number,
  request: SchedulingUpdateRequest,
): Promise<OperationResult<Scheduling>> => {
  return apiFetch<Scheduling>(`/api/agendamentos/${agendamentoId}/admin`, {
    method: 'PUT',
    body: JSON.stringify(request),
  })
}

export const updateAgendamentoStatus = async (
  agendamentoId: number,
  status: SchedulingStatus,
): Promise<OperationResult<Scheduling>> => {
  return apiFetch<Scheduling>(`/api/agendamentos/${agendamentoId}/status`, {
    method: 'PATCH',
    body: JSON.stringify({ status }),
  })
}

export const deleteAgendamento = async (agendamentoId: number): Promise<OperationResult<null>> => {
  return apiFetch<null>(`/api/agendamentos/${agendamentoId}`, {
    method: 'DELETE',
  })
}

export const getSuggestion = async (
  clienteId: number,
  dataHora: string,
): Promise<OperationResult<Scheduling>> => {
  return apiFetch<Scheduling>(`/api/agendamentos/sugestao?clienteId=${clienteId}&dataHora=${encodeURIComponent(dataHora)}`)
}
