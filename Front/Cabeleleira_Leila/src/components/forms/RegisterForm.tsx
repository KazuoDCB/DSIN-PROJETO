import { useState } from 'react'
import type { FormEvent } from 'react'
import FeedbackAlert from '../ui/FeedbackAlert'
import type { ClienteRequest } from '../../Types'
import { useAutoDismiss } from '../../hooks/useAutoDismiss'
import { toFeedbackContent } from '../../utils/apiErrors'
import type { FeedbackContent } from '../../utils/apiErrors'

interface RegisterFormProps {
  onCancel: () => void
  onSubmit: (payload: ClienteRequest) => Promise<void>
}

const RegisterForm = ({ onCancel, onSubmit }: RegisterFormProps) => {
  const [loading, setLoading] = useState(false)
  const [message, setMessage] = useState<FeedbackContent | null>(null)

  useAutoDismiss(message, () => setMessage(''))

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setMessage(null)
    const form = event.currentTarget
    const formData = new FormData(form)
    const password = String(formData.get('password') ?? '')

    setLoading(true)

    try {
      await onSubmit({
        name: String(formData.get('name') ?? '').trim(),
        number: String(formData.get('number') ?? '').trim(),
        email: String(formData.get('email') ?? '').trim(),
        password,
      })
      form.reset()
    } catch (error) {
      setMessage(toFeedbackContent(error, 'Nao foi possivel criar a conta.'))
    } finally {
      setLoading(false)
    }
  }

  return (
    <form className="form-stack" onSubmit={handleSubmit}>
      <label>
        Nome
        <input autoComplete="name" name="name" />
      </label>

      <label>
        Telefone
        <input autoComplete="tel" inputMode="tel" name="number" />
      </label>

      <label>
        Email
        <input autoComplete="email" inputMode="email" name="email" type="text" />
      </label>

      <label>
        Senha
        <input autoComplete="new-password" name="password" type="password" />
      </label>

      <FeedbackAlert content={message} />

      <button className="button button--primary button--full" disabled={loading} type="submit">
        {loading ? 'Cadastrando...' : 'Cadastrar'}
      </button>

      <button className="auth-switch auth-switch--button" onClick={onCancel} type="button">
        Ja possui uma conta? Entrar
      </button>
    </form>
  )
}

export default RegisterForm
