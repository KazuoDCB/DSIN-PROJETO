import Icon from './Icon'

interface EmptyStateProps {
  icon?: 'calendar' | 'users' | 'bar-chart'
  title: string
  description?: string
}

const EmptyState = ({ icon = 'calendar', title, description }: EmptyStateProps) => (
  <div className="empty-state">
    <Icon name={icon} size={36} />
    <strong>{title}</strong>
    {description ? <p>{description}</p> : null}
  </div>
)

export default EmptyState
