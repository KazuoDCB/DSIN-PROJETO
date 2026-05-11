import type { ReactNode } from 'react'
import { NavLink } from 'react-router-dom'
import { useAuth } from '../../context/useAuth'
import Icon from '../ui/Icon'

interface AdminLayoutProps {
  children: ReactNode
}

const adminLinks = [
  { to: '/admin', label: 'Dashboard', icon: 'bar-chart' },
  { to: '/admin/agendamentos', label: 'Agenda', icon: 'calendar' },
  { to: '/admin/relatorios', label: 'Relatorios', icon: 'bar-chart' },
  { to: '/admin/clientes', label: 'Clientes', icon: 'users' },
  { to: '/admin/servicos', label: 'Servicos', icon: 'scissors' },
] as const

const AdminLayout = ({ children }: AdminLayoutProps) => {
  const { user, signOut } = useAuth()

  return (
    <div className="admin-shell">
      <aside className="admin-sidebar">
        <div className="admin-sidebar__brand">
          <strong>Leila</strong>
          <span>Painel Administrativo</span>
        </div>

        <nav className="admin-nav" aria-label="Navegacao administrativa">
          {adminLinks.map((item) => (
            <NavLink end={item.to === '/admin'} key={item.to} to={item.to}>
              <Icon name={item.icon} />
              {item.label}
            </NavLink>
          ))}
        </nav>

        <div className="admin-sidebar__account">
          <span>
            <Icon name="shield" />
            {user?.name ?? 'Administrador'}
          </span>
          <button className="button button--ghost" onClick={signOut} type="button">
            <Icon name="log-out" />
            Sair
          </button>
        </div>
      </aside>

      <main className="admin-content">{children}</main>
    </div>
  )
}

export default AdminLayout
