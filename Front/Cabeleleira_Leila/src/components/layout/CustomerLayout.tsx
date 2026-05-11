import type { ReactNode } from 'react'
import { Link, NavLink } from 'react-router-dom'
import { useAuth } from '../../context/useAuth'
import Icon from '../ui/Icon'

interface CustomerLayoutProps {
  children: ReactNode
}

const CustomerLayout = ({ children }: CustomerLayoutProps) => {
  const { user, signOut } = useAuth()

  return (
    <div className="customer-shell">
      <header className="customer-header">
        <Link className="brand" to="/minha-agenda">
          Leila
        </Link>
        <nav className="customer-nav" aria-label="Area do cliente">
          <NavLink to="/minha-agenda">Minha agenda</NavLink>
          <NavLink to="/minha-agenda/servicos">Servicos</NavLink>
        </nav>
        <div className="customer-header__account">
          <span>{user?.name}</span>
          <button className="button button--ghost" onClick={signOut} type="button">
            <Icon name="log-out" />
            Sair
          </button>
        </div>
      </header>
      <main className="customer-content">{children}</main>
    </div>
  )
}

export default CustomerLayout
