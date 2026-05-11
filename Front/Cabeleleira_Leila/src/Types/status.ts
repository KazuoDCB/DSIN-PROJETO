export const Status = {
  Inativo: 0,
  Ativo: 1,
} as const

export type Status = (typeof Status)[keyof typeof Status]

export const statusText: Record<Status, string> = {
  [Status.Inativo]: 'Inativo',
  [Status.Ativo]: 'Ativo',
}
