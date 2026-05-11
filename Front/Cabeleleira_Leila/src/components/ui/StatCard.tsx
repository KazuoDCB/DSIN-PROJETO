import Icon from './Icon'

type StatIcon = 'calendar' | 'currency' | 'users' | 'bar-chart' | 'scissors'

interface StatCardProps {
  icon: StatIcon
  label: string
  value: string | number
  meta?: string
  tone?: 'gold' | 'green' | 'blue' | 'purple'
}

const StatCard = ({ icon, label, value, meta, tone = 'gold' }: StatCardProps) => (
  <article className="stat-card">
    <div className="stat-card__header">
      <div className={`stat-card__icon stat-card__icon--${tone}`}>
        <Icon name={icon} size={20} />
      </div>
      {meta ? <span className="stat-card__meta">{meta}</span> : null}
    </div>
    <div className="stat-card__body">
      <strong>{value}</strong>
      <p>{label}</p>
    </div>
  </article>
)

export default StatCard
