import { useState } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import LoginForm from '../components/forms/LoginForm'
import RegisterForm from '../components/forms/RegisterForm'
import type { LoginScope } from '../context/authContextDefinition'
import { useAuth } from '../context/useAuth'
import { createCliente } from '../services/clienteService'
import type { ClienteRequest, LoginRequest } from '../Types'

interface RouteState {
  from?: {
    pathname?: string
  }
}

interface LoginPageProps {
  scope: LoginScope
}

const LoginPage = ({ scope }: LoginPageProps) => {
  const navigate = useNavigate()
  const location = useLocation()
  const { signIn, error } = useAuth()
  const [isRegistering, setIsRegistering] = useState(false)
  const state = location.state as RouteState | null
  const isAdmin = scope === 'admin'
  const from = state?.from?.pathname || (isAdmin ? '/admin' : '/minha-agenda')

  const handleLogin = async (credentials: LoginRequest) => {
    await signIn(credentials, scope)
    navigate(from, { replace: true })
  }

  const handleRegister = async (payload: ClienteRequest) => {
    await createCliente(payload)
    await signIn({ email: payload.email, password: payload.password }, 'cliente')
    navigate('/minha-agenda', { replace: true })
  }

  const title = isRegistering ? 'Cadastro do cliente' : isAdmin ? 'Login administrativo' : 'Login do cliente'
  const description = isRegistering
    ? 'Crie sua conta para agendar suas idas ao salao e acompanhar seus horarios.'
    : isAdmin
      ? 'Acesse dashboards, agenda e relatorios com seu usuario administrativo.'
      : 'Entre para agendar suas idas ao salao e acompanhar seus horarios.'

  return (
    <section className="auth-page">
      <article className="auth-card">
        <span className="eyebrow">{isAdmin ? 'Administrativo Leila' : 'Cliente Leila'}</span>
        <h1>{title}</h1>
        <p>{description}</p>

        {isRegistering ? (
          <RegisterForm onCancel={() => setIsRegistering(false)} onSubmit={handleRegister} />
        ) : (
          <LoginForm error={error} onSubmit={handleLogin}>
            {!isAdmin ? (
              <button className="auth-switch" onClick={() => setIsRegistering(true)} type="button">
                Ainda não possui uma conta ? Cadastre-se
              </button>
            ) : null}
          </LoginForm>
        )}
      </article>
    </section>
  )
}

export default LoginPage
