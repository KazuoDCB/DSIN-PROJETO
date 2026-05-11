import Icon from '../ui/Icon'

interface FeatureCardProps {
  icon: 'sparkle' | 'clock' | 'scissors'
  title: string
  description: string
}

const FeatureCard = ({ icon, title, description }: FeatureCardProps) => (
  <article className="feature-card">
    <span className="feature-card__icon">
      <Icon name={icon} size={28} />
    </span>
    <h2>{title}</h2>
    <p>{description}</p>
  </article>
)

export default FeatureCard
