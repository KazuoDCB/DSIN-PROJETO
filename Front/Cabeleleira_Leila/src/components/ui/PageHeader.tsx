import type { ReactNode } from 'react'

interface PageHeaderProps {
  title: string
  subtitle: string
  action?: ReactNode
}

const PageHeader = ({ title, subtitle, action }: PageHeaderProps) => (
  <header className="page-header">
    <div>
      <h1>{title}</h1>
      <p>{subtitle}</p>
    </div>
    {action ? <div className="page-header__action">{action}</div> : null}
  </header>
)

export default PageHeader
