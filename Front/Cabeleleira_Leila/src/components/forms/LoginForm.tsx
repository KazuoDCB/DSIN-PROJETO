import { useState } from 'react'
import type { FormEvent } from 'react'
import type { ReactNode } from 'react'
import FeedbackAlert from '../ui/FeedbackAlert'
import type { LoginRequest } from '../../Types'
import { useAutoDismiss } from '../../hooks/useAutoDismiss'
import { toFeedbackContent } from '../../utils/apiErrors'
import type { FeedbackContent } from '../../utils/apiErrors'

interface LoginFormProps {
  children?: ReactNode
  error?: FeedbackContent | null
  onSubmit: (credentials: LoginRequest) => Promise<void>
}

const LoginForm = ({ children, error, onSubmit }: LoginFormProps) => {
  const [loading, setLoading] = useState(false)
  const [message, setMessage] = useState<FeedbackContent | null>(null)

  useAutoDismiss(message, () => setMessage(''))

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setLoading(true)
    setMessage(null)
    const formData = new FormData(event.currentTarget)

    try {
      await onSubmit({
        email: String(formData.get('email') ?? '').trim(),
        password: String(formData.get('password') ?? ''),
      } satisfies LoginRequest)
    } catch (submitError) {
      setMessage(toFeedbackContent(submitError, 'Falha no login.'))
    } finally {
      setLoading(false)
    }
  }

  return (
    <form className="form-stack" onSubmit={handleSubmit}>
      <label>
        Email
        <input
          autoComplete="email"
          inputMode="email"
          name="email"
          placeholder="seu@email.com"
          type="text"
        />
      </label>

      <label>
        Senha
        <input
          autoComplete="current-password"
          name="password"
          placeholder="********"
          type="password"
        />
      </label>

      <FeedbackAlert content={message || error} />

      <button className="button button--primary button--full" disabled={loading} type="submit">
        {loading ? 'Entrando...' : 'Entrar'}
      </button>

      {children}
    </form>
  )
}

export default LoginForm
