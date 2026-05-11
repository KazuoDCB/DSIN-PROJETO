import { apiFetch } from './api'
import type { Cliente, ClienteRequest, ClienteUpdateRequest, OperationResult } from '../Types'

export const getClientes = async (page = 1, size = 50): Promise<OperationResult<Cliente[]>> => {
  return apiFetch<Cliente[]>(`/api/clientes?size=${size}&page=${page}`)
}

export const getClienteById = async (id: number): Promise<OperationResult<Cliente>> => {
  return apiFetch<Cliente>(`/api/clientes/${id}`)
}

export const createCliente = async (request: ClienteRequest): Promise<OperationResult<Cliente>> => {
  return apiFetch<Cliente>('/api/clientes', {
    method: 'POST',
    body: JSON.stringify(request),
  })
}

export const updateCliente = async (
  id: number,
  request: ClienteUpdateRequest,
): Promise<OperationResult<Cliente>> => {
  return apiFetch<Cliente>(`/api/clientes/${id}`, {
    method: 'PUT',
    body: JSON.stringify(request),
  })
}

export const deleteCliente = async (id: number): Promise<OperationResult<null>> => {
  return apiFetch<null>(`/api/clientes/${id}`, {
    method: 'DELETE',
  })
}
