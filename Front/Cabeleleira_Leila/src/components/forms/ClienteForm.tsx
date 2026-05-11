import type { FormEvent } from 'react'
import FeedbackAlert from '../ui/FeedbackAlert'
import { Status, UserRole } from '../../Types'
import type { Cliente, ClienteRequest, ClienteUpdateRequest, StatusType } from '../../Types'
import type { FeedbackContent } from '../../utils/apiErrors'

interface ClienteFormProps {
  clienteId?: number
  initialData?: Cliente | null
  feedback?: FeedbackContent | null
  feedbackVariant?: 'error' | 'success'
  onCancel?: () => void
  onSubmit: (payload: ClienteRequest | ClienteUpdateRequest, clienteId?: number) => Promise<void>
}

const ClienteForm = ({ clienteId, initialData, feedback, feedbackVariant = 'error', onCancel, onSubmit }: ClienteFormProps) => {
  const isEditing = Boolean(clienteId)

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    const formData = new FormData(event.currentTarget)

    if (clienteId) {
      await onSubmit(
        {
          name: String(formData.get('name') ?? '').trim(),
          number: String(formData.get('number') ?? '').trim(),
          email: String(formData.get('email') ?? '').trim(),
          status: Number(formData.get('status') ?? Status.Ativo) as StatusType,
          role: initialData?.role ?? UserRole.Cliente,
        },
        clienteId,
      )
      return
    }

    await onSubmit({
      name: String(formData.get('name') ?? '').trim(),
      number: String(formData.get('number') ?? '').trim(),
      email: String(formData.get('email') ?? '').trim(),
      password: String(formData.get('password') ?? ''),
    })

    event.currentTarget.reset()
  }

  return (
    <form className="form-stack" onSubmit={handleSubmit}>
      <label>
        Nome
        <input defaultValue={initialData?.name ?? ''} name="name" />
      </label>

      <label>
        Telefone
        <input defaultValue={initialData?.number ?? ''} inputMode="tel" name="number" />
      </label>

      <label>
        Email
        <input defaultValue={initialData?.email ?? ''} inputMode="email" name="email" type="text" />
      </label>

      {isEditing ? null : (
        <label>
          Senha
          <input name="password" type="password" />
        </label>
      )}

      {isEditing ? (
        <label>
          Status
          <select defaultValue={String(initialData?.status ?? Status.Ativo)} name="status">
            <option value={Status.Ativo}>Ativo</option>
            <option value={Status.Inativo}>Inativo</option>
          </select>
        </label>
      ) : null}

      <FeedbackAlert content={feedback} variant={feedbackVariant} />

      <div className="form-actions">
        <button className="button button--primary" type="submit">
          {isEditing ? 'Atualizar cliente' : 'Criar cliente'}
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

export default ClienteForm
