import type { Status } from './status'

export interface Servico {
  id: number
  name: string
  description: string
  price: number
  duration: number
  status: Status
  createdAt: string
  updatedAt: string
}

export interface ServicoRequest {
  name: string
  description: string
  price: number
  duration: number
  status: Status
}

export interface ServicoUpdateRequest {
  name: string
  description: string
  price: number
  duration: number
  status: Status
}
