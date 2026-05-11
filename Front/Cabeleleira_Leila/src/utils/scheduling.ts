import type { Scheduling } from '../Types'

export const normalizeScheduling = (agendamento: Scheduling): Scheduling => ({
  ...agendamento,
  clienteName: agendamento.clienteName ?? '',
  servicos: agendamento.servicos ?? [],
  totalDuration: agendamento.totalDuration ?? 0,
  totalValue: agendamento.totalValue ?? 0,
})

export const normalizeSchedulings = (agendamentos: Scheduling[]) => agendamentos.map(normalizeScheduling)
