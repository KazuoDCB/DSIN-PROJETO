import type { FeedbackContent } from '../../utils/apiErrors'

interface FeedbackAlertProps {
  content?: FeedbackContent | null
  variant?: 'error' | 'success'
}

const FeedbackAlert = ({ content, variant = 'error' }: FeedbackAlertProps) => {
  if (!content || (Array.isArray(content) && content.length === 0)) return null

  return (
    <div className={`alert alert--${variant}`} role={variant === 'error' ? 'alert' : 'status'}>
      {Array.isArray(content) ? (
        <ul className="alert__list">
          {content.map((error, index) => (
            <li key={`${error.property}-${index}`}>
              {error.property ? `${error.property}: ` : ''}
              {error.message}
            </li>
          ))}
        </ul>
      ) : (
        content
      )}
    </div>
  )
}

export default FeedbackAlert
