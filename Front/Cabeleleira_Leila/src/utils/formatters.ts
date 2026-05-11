import { SchedulingStatus } from '../Types/agendamento'
import type { SchedulingStatus as SchedulingStatusType } from '../Types'

export const currencyFormatter = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

export const dateFormatter = new Intl.DateTimeFormat('pt-BR', {
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
})

export const timeFormatter = new Intl.DateTimeFormat('pt-BR', {
  hour: '2-digit',
  minute: '2-digit',
})

export const weekdayDateFormatter = new Intl.DateTimeFormat('pt-BR', {
  weekday: 'long',
  day: 'numeric',
  month: 'long',
  year: 'numeric',
})

export const formatCurrency = (value: number) => currencyFormatter.format(value)

export const formatDate = (value: string | Date) => dateFormatter.format(new Date(value))

export const formatTime = (value: string | Date) => timeFormatter.format(new Date(value))

export const formatDateTime = (value: string | Date) => {
  const date = new Date(value)
  return `${formatDate(date)} as ${formatTime(date)}`
}

export const statusLabel: Record<SchedulingStatusType, string> = {
  [SchedulingStatus.Agendado]: 'Agendado',
  [SchedulingStatus.Confirmado]: 'Confirmado',
  [SchedulingStatus.EmAndamento]: 'Em andamento',
  [SchedulingStatus.Finalizado]: 'Finalizado',
  [SchedulingStatus.Cancelado]: 'Cancelado',
}

export const getStatusTone = (status: SchedulingStatusType) => {
  if (status === SchedulingStatus.Finalizado) return 'success'
  if (status === SchedulingStatus.Cancelado) return 'danger'
  if (status === SchedulingStatus.Confirmado) return 'info'
  if (status === SchedulingStatus.EmAndamento) return 'warning'
  return 'neutral'
}

export const formatDateTimeLocal = (dateString: string) => {
  const date = new Date(dateString)
  if (Number.isNaN(date.getTime())) return ''

  const pad = (value: number) => String(value).padStart(2, '0')
  const year = date.getFullYear()
  const month = pad(date.getMonth() + 1)
  const day = pad(date.getDate())
  const hours = pad(date.getHours())
  const minutes = pad(date.getMinutes())

  return `${year}-${month}-${day}T${hours}:${minutes}`
}
