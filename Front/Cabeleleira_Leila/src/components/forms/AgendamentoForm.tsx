import { useState } from 'react'
import type { FormEvent } from 'react'
import { SchedulingStatus } from '../../Types/agendamento'
import FeedbackAlert from '../ui/FeedbackAlert'
import type {
  Cliente,
  Scheduling,
  SchedulingRequest,
  SchedulingStatus as SchedulingStatusType,
  SchedulingUpdateRequest,
  Servico,
} from '../../Types'
import { formatCurrency, formatDateTimeLocal } from '../../utils/formatters'
import type { FeedbackContent } from '../../utils/apiErrors'

interface AgendamentoFormProps {
  agendamentoId?: number
  allowStatusEdit?: boolean
  clientes: Cliente[]
  defaultClienteId?: number
  feedback?: FeedbackContent | null
  feedbackVariant?: 'error' | 'success'
  initialData?: Scheduling | null
  servicos: Servico[]
  onCancel?: () => void
  onSubmit: (payload: SchedulingRequest | SchedulingUpdateRequest, agendamentoId?: number) => Promise<void>
}

const AgendamentoForm = ({
  agendamentoId,
  allowStatusEdit = false,
  clientes,
  defaultClienteId,
  feedback,
  feedbackVariant = 'error',
  initialData,
  servicos,
  onCancel,
  onSubmit,
}: AgendamentoFormProps) => {
  const isEditing = Boolean(agendamentoId)
  const [selectedTotal, setSelectedTotal] = useState(() =>
    (initialData?.servicos ?? []).reduce((total, servico) => total + servico.price, 0),
  )

  const updateSelectedTotal = (form: HTMLFormElement) => {
    const formData = new FormData(form)
    const selectedIds = formData.getAll('servicoIds').map((value) => Number(value))
    const total = servicos
      .filter((servico) => selectedIds.includes(servico.id))
      .reduce((sum, servico) => sum + servico.price, 0)

    setSelectedTotal(total)
  }

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    const form = event.currentTarget
    const formData = new FormData(form)
    const servicoIds = formData.getAll('servicoIds').map((value) => Number(value))
    const dataHora = String(formData.get('dataHora') ?? '')

    if (agendamentoId) {
      await onSubmit(
        {
          dataHora,
          status: Number(formData.get('status') ?? SchedulingStatus.Agendado) as SchedulingStatusType,
          servicoIds,
        },
        agendamentoId,
      )
      return
    }

    await onSubmit({
      clienteId: Number(formData.get('clienteId') ?? 0),
      dataHora,
      servicoIds,
    })
    form.reset()
    setSelectedTotal(0)
  }

  return (
    <form
      className="form-stack"
      onChange={(event) => {
        if (event.target instanceof HTMLInputElement && event.target.name === 'servicoIds') {
          updateSelectedTotal(event.currentTarget)
        }
      }}
      onSubmit={handleSubmit}
    >
      <label>
        Cliente
        <select defaultValue={String(initialData?.clienteId ?? defaultClienteId ?? clientes[0]?.id ?? 0)} disabled={isEditing} name="clienteId">
          <option value={0}>Selecione um cliente</option>
          {clientes.map((cliente) => (
            <option key={cliente.id} value={cliente.id}>
              {cliente.name}
            </option>
          ))}
        </select>
      </label>

      <label>
        Data e hora
        <input defaultValue={initialData ? formatDateTimeLocal(initialData.dataHora) : ''} name="dataHora" type="datetime-local" />
      </label>

      {isEditing && allowStatusEdit ? (
        <label>
          Status
          <select defaultValue={String(initialData?.status ?? SchedulingStatus.Agendado)} name="status">
            <option value={SchedulingStatus.Agendado}>Agendado</option>
            <option value={SchedulingStatus.Confirmado}>Confirmado</option>
            <option value={SchedulingStatus.EmAndamento}>Em andamento</option>
            <option value={SchedulingStatus.Finalizado}>Finalizado</option>
            <option value={SchedulingStatus.Cancelado}>Cancelado</option>
          </select>
        </label>
      ) : null}

      <fieldset className="checkbox-group">
        <legend>Servicos</legend>
        {servicos.map((servico) => (
          <label className="checkbox-item" key={servico.id}>
            <input
              defaultChecked={initialData?.servicos.some((item) => item.id === servico.id) ?? false}
              name="servicoIds"
              type="checkbox"
              value={servico.id}
            />
            <span>{servico.name}</span>
            <small>{formatCurrency(servico.price)}</small>
          </label>
        ))}
      </fieldset>

      <p className="form-help">Total selecionado: {formatCurrency(selectedTotal)}</p>
      <FeedbackAlert content={feedback} variant={feedbackVariant} />

      <div className="form-actions">
        <button className="button button--primary" type="submit">
          {isEditing ? 'Atualizar agendamento' : 'Criar agendamento'}
        </button>
        {isEditing && onCancel ? (
          <button className="button button--ghost" onClick={onCancel} type="button">
            Cancelar
          </button>
        ) : null}
      </div>
    </form>
  )
}

export default AgendamentoForm
