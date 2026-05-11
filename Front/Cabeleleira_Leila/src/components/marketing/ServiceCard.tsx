import type { Servico } from '../../Types'
import { formatCurrency } from '../../utils/formatters'

interface ServiceCardProps {
  servico: Servico
}

const ServiceCard = ({ servico }: ServiceCardProps) => (
  <article className="service-card">
    <h2>{servico.name}</h2>
    <p>{servico.description}</p>
    <div className="service-card__meta">
      <strong>{formatCurrency(servico.price)}</strong>
      <span>{servico.duration} min</span>
    </div>
  </article>
)

export default ServiceCard
