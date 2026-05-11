import type { FormEvent } from 'react'
import FeedbackAlert from '../ui/FeedbackAlert'
import { Status } from '../../Types'
import type { Servico, ServicoRequest, ServicoUpdateRequest, StatusType } from '../../Types'
import type { FeedbackContent } from '../../utils/apiErrors'

interface ServicoFormProps {
  servicoId?: number
  initialData?: Servico | null
  feedback?: FeedbackContent | null
  feedbackVariant?: 'error' | 'success'
  onCancel?: () => void
  onSubmit: (payload: ServicoRequest | ServicoUpdateRequest, servicoId?: number) => Promise<void>
}

const ServicoForm = ({ servicoId, initialData, feedback, feedbackVariant = 'error', onCancel, onSubmit }: ServicoFormProps) => {
  const isEditing = Boolean(servicoId)

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    const formData = new FormData(event.currentTarget)
    const price = Number(formData.get('price') ?? 0)
    const duration = Number(formData.get('duration') ?? 0)
    const payload: ServicoRequest | ServicoUpdateRequest = {
      name: String(formData.get('name') ?? '').trim(),
      description: String(formData.get('description') ?? '').trim(),
      price: Number.isFinite(price) ? price : 0,
      duration: Number.isFinite(duration) ? duration : 0,
      status: Number(formData.get('status') ?? Status.Ativo) as StatusType,
    }

    await onSubmit(payload, servicoId)

    if (!servicoId) event.currentTarget.reset()
  }

  return (
    <form className="form-stack" onSubmit={handleSubmit}>
      <label>
        Nome do servico
        <input defaultValue={initialData?.name ?? ''} name="name" />
      </label>

      <label>
        Descricao
        <textarea
          defaultValue={initialData?.description ?? ''}
          name="description"
          rows={4}
        />
      </label>

      <label>
        Preco
        <input
          defaultValue={initialData?.price ?? ''}
          inputMode="decimal"
          name="price"
          step="0.01"
          type="text"
        />
      </label>

      <label>
        Duracao em minutos
        <input
          defaultValue={initialData?.duration ?? ''}
          inputMode="numeric"
          name="duration"
          type="text"
        />
      </label>

      <label>
        Status
        <select defaultValue={String(initialData?.status ?? Status.Ativo)} name="status">
          <option value={Status.Ativo}>Ativo</option>
          <option value={Status.Inativo}>Inativo</option>
        </select>
      </label>

      <FeedbackAlert content={feedback} variant={feedbackVariant} />

      <div className="form-actions">
        <button className="button button--primary" type="submit">
          {isEditing ? 'Atualizar servico' : 'Salvar servico'}
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

export default ServicoForm
