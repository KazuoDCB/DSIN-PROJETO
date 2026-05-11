import { apiFetch } from './api'
import type { Servico, ServicoRequest, ServicoUpdateRequest, OperationResult } from '../Types'

export const getServicos = async (page = 1, size = 50): Promise<OperationResult<Servico[]>> => {
  return apiFetch<Servico[]>(`/api/servicos?size=${size}&page=${page}`)
}

export const getServicoById = async (id: number): Promise<OperationResult<Servico>> => {
  return apiFetch<Servico>(`/api/servicos/${id}`)
}

export const createServico = async (request: ServicoRequest): Promise<OperationResult<Servico>> => {
  return apiFetch<Servico>('/api/servicos', {
    method: 'POST',
    body: JSON.stringify(request),
  })
}

export const updateServico = async (
  id: number,
  request: ServicoUpdateRequest,
): Promise<OperationResult<Servico>> => {
  return apiFetch<Servico>(`/api/servicos/${id}`, {
    method: 'PUT',
    body: JSON.stringify(request),
  })
}

export const deleteServico = async (id: number): Promise<OperationResult<null>> => {
  return apiFetch<null>(`/api/servicos/${id}`, {
    method: 'DELETE',
  })
}
