import { useEffect, useState } from 'react'
import ServiceCard from '../components/marketing/ServiceCard'
import EmptyState from '../components/ui/EmptyState'
import { getServicos } from '../services/servicoService'
import { Status, type Servico } from '../Types'

const PublicServicosPage = () => {
  const [servicos, setServicos] = useState<Servico[]>([])
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    let mounted = true

    getServicos(1, 100)
      .then((response) => {
        const activeServicos = response.data?.filter((servico) => servico.status === Status.Ativo) ?? []
        if (mounted) setServicos(activeServicos)
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
    <section className="public-section public-section--top">
      <div className="section-title">
        <h1>Nossos Servicos</h1>
        <p>Uma experiencia completa de beleza com profissionais preparados e produtos de alta qualidade.</p>
      </div>
      {isLoading ? (
        <EmptyState icon="bar-chart" title="Carregando servicos..." />
      ) : servicos.length === 0 ? (
        <EmptyState icon="bar-chart" title="Nenhum servico ativo encontrado" />
      ) : (
        <div className="service-grid">
          {servicos.map((servico) => (
            <ServiceCard key={servico.id} servico={servico} />
          ))}
        </div>
      )}
    </section>
  )
}

export default PublicServicosPage
