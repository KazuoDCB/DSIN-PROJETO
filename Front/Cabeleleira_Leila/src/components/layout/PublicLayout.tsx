import type { ReactNode } from 'react'
import { Link, NavLink } from 'react-router-dom'
import Icon from '../ui/Icon'

interface PublicLayoutProps {
  children: ReactNode
}

const PublicLayout = ({ children }: PublicLayoutProps) => (
  <div className="public-shell">
    <header className="public-header">
      <Link className="brand" to="/">
        Leila
      </Link>
      <nav className="public-nav" aria-label="Navegacao principal">
        <NavLink to="/servicos">Servicos</NavLink>
        <NavLink to="/sobre">Sobre</NavLink>
        <a href="#contato">Contato</a>
      </nav>
      <Link className="button button--primary" to="/login">
        <Icon name="log-in" />
        Entrar
      </Link>
    </header>

    {children}

    <footer className="public-footer" id="contato">
      <div className="public-footer__grid">
        <section>
          <Link className="brand" to="/">
            Leila
          </Link>
          <p>Transformando vidas atraves da beleza ha mais de 15 anos.</p>
        </section>
        <section>
          <h2>Contato</h2>
          <p>(11) 99999-9999</p>
          <p>Rua da Beleza, 123 - Sao Paulo</p>
        </section>
        <section>
          <h2>Horario</h2>
          <p>Seg - Sex: 9h as 20h</p>
          <p>Sabado: 9h as 18h</p>
          <p>Domingo: Fechado</p>
        </section>
        <section>
          <h2>Links rapidos</h2>
          <Link to="/login">Login cliente</Link>
          <Link to="/admin/login">Login administrativo</Link>
          <Link to="/servicos">Servicos</Link>
        </section>
      </div>
      <p className="public-footer__copy">2026 Salao Leila. Todos os direitos reservados.</p>
    </footer>
  </div>
)

export default PublicLayout
