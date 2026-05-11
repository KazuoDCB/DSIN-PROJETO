import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import FeatureCard from '../components/marketing/FeatureCard'
import ServiceCard from '../components/marketing/ServiceCard'
import EmptyState from '../components/ui/EmptyState'
import Icon from '../components/ui/Icon'
import { featureCards } from '../data/publicContent'
import { getServicos } from '../services/servicoService'
import { Status, type Servico } from '../Types'

const HomePage = () => {
  const [servicos, setServicos] = useState<Servico[]>([])
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    let mounted = true

    getServicos(1, 4)
      .then((response) => {
        if (mounted) setServicos((response.data ?? []).filter((servico) => servico.status === Status.Ativo))
      })
      .catch(() => {
        if (mounted) setServicos([])
      })
      .finally(() => {
        if (mounted) setIsLoading(false)
      })

    return () => {
      mounted = false
    }
  }, [])

  return (
    <>
      <section className="hero-section">
        <div className="hero-section__content">
          <span className="eyebrow">Salao Leila</span>
          <h1>Beleza que entende o seu tempo</h1>
          <p>Agendamento simples, atendimento cuidadoso e uma experiencia feita para voce se renovar.</p>
          <div className="hero-section__actions">
            <Link className="button button--primary" to="/login">
              <Icon name="calendar" />
              Agendar horario
            </Link>
            <Link className="button button--outline" to="/servicos">
              Nossos servicos
            </Link>
          </div>
        </div>
      </section>

      <section className="public-section" id="servicos">
        <div className="section-title">
          <h2>Nossos Servicos</h2>
          <p>Tratamentos completos para corte, cor, finalizacao e saude capilar.</p>
        </div>
        {isLoading ? (
          <EmptyState icon="bar-chart" title="Carregando servicos..." />
        ) : servicos.length === 0 ? (
          <EmptyState icon="bar-chart" title="Nenhum servico ativo encontrado" />
        ) : (
          <div className="service-grid service-grid--compact">
            {servicos.map((servico) => (
              <ServiceCard key={servico.id} servico={servico} />
            ))}
          </div>
        )}
      </section>

      <section className="public-section public-section--muted">
        <div className="section-title">
          <h2>Por que escolher o Salao Leila?</h2>
        </div>
        <div className="feature-grid">
          {featureCards.map((feature) => (
            <FeatureCard
              description={feature.description}
              icon={feature.icon}
              key={feature.title}
              title={feature.title}
            />
          ))}
        </div>
      </section>

      <section className="cta-section">
        <h2>Pronto para se renovar?</h2>
        <p>Agende seu horario agora e descubra uma nova versao de si mesmo.</p>
        <Link className="button button--primary" to="/login">
          Agendar Agora
          <Icon name="arrow-right" />
        </Link>
      </section>
    </>
  )
}

export default HomePage
