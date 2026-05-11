import { Link } from 'react-router-dom'
import Icon from '../components/ui/Icon'

const NotFoundPage = () => (
  <main className="not-found-page">
    <section className="auth-card">
      <span className="eyebrow">404</span>
      <h1>Pagina nao encontrada</h1>
      <p>A pagina que voce esta procurando nao existe ou foi movida.</p>
      <Link className="button button--primary" to="/">
        Voltar ao inicio
        <Icon name="arrow-right" />
      </Link>
    </section>
  </main>
)

export default NotFoundPage
