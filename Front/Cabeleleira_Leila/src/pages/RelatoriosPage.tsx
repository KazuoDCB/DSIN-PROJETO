import { useEffect, useMemo, useState } from 'react'
import EmptyState from '../components/ui/EmptyState'
import FeedbackAlert from '../components/ui/FeedbackAlert'
import PageHeader from '../components/ui/PageHeader'
import StatCard from '../components/ui/StatCard'
import { useAutoDismiss } from '../hooks/useAutoDismiss'
import { getAgendamentos } from '../services/agendamentoService'
import type { Scheduling } from '../Types'
import { SchedulingStatus } from '../Types/agendamento'
import { toFeedbackContent } from '../utils/apiErrors'
import type { FeedbackContent } from '../utils/apiErrors'
import { formatCurrency, statusLabel } from '../utils/formatters'
import { normalizeSchedulings } from '../utils/scheduling'

type Period = 'semana' | 'mes'

const isInsidePeriod = (date: Date, period: Period) => {
  const now = new Date()
  const start = new Date(now)

  if (period === 'semana') {
    start.setDate(now.getDate() - 7)
  } else {
    start.setMonth(now.getMonth() - 1)
  }

  return date >= start && date <= now
}

const RelatoriosPage = () => {
  const [agendamentos, setAgendamentos] = useState<Scheduling[]>([])
  const [feedback, setFeedback] = useState<FeedbackContent | null>(null)
  const [period, setPeriod] = useState<Period>('semana')

  useAutoDismiss(feedback, () => setFeedback(null))

  useEffect(() => {
    const loadReports = async () => {
      try {
        const response = await getAgendamentos(1, 300)
        setAgendamentos(normalizeSchedulings(response.data ?? []))
      } catch (error) {
        setFeedback(toFeedbackContent(error))
      }
    }

    loadReports()
  }, [])

  const filteredAgendamentos = useMemo(
    () => agendamentos.filter((agendamento) => isInsidePeriod(new Date(agendamento.dataHora), period)),
    [agendamentos, period],
  )

  const validAgendamentos = filteredAgendamentos.filter((agendamento) => agendamento.status !== SchedulingStatus.Cancelado)
  const totalRevenue = validAgendamentos.reduce((total, agendamento) => total + agendamento.totalValue, 0)
  const averageTicket = validAgendamentos.length === 0 ? 0 : totalRevenue / validAgendamentos.length
  const finished = filteredAgendamentos.filter((agendamento) => agendamento.status === SchedulingStatus.Finalizado).length

  const statusDistribution = Object.values(SchedulingStatus)
    .filter((status) => typeof status === 'number')
    .map((status) => ({
      status,
      total: filteredAgendamentos.filter((agendamento) => agendamento.status === status).length,
    }))

  const serviceRanking = filteredAgendamentos
    .flatMap((agendamento) => agendamento.servicos)
    .reduce<Record<string, number>>((accumulator, servico) => {
      accumulator[servico.name] = (accumulator[servico.name] ?? 0) + 1
      return accumulator
    }, {})

  const sortedServices = Object.entries(serviceRanking)
    .sort((left, right) => right[1] - left[1])
    .slice(0, 5)

  return (
    <div className="admin-page">
      <PageHeader
        action={
          <div className="segmented-control">
            <button className={period === 'semana' ? 'is-active' : ''} onClick={() => setPeriod('semana')} type="button">
              Semana
            </button>
            <button className={period === 'mes' ? 'is-active' : ''} onClick={() => setPeriod('mes')} type="button">
              Mes
            </button>
          </div>
        }
        subtitle="Acompanhe o desempenho do salao"
        title="Relatorios"
      />

      <FeedbackAlert content={feedback} />

      <section className="stats-grid">
        <StatCard icon="scissors" label="Atendimentos" meta={period} value={finished} />
        <StatCard icon="currency" label="Faturamento total" meta={period} tone="green" value={formatCurrency(totalRevenue)} />
        <StatCard icon="currency" label="Ticket medio" meta={period} tone="blue" value={formatCurrency(averageTicket)} />
        <StatCard icon="calendar" label="Total agendamentos" meta={period} tone="purple" value={filteredAgendamentos.length} />
      </section>

      <div className="reports-grid">
        <section className="panel report-panel">
          <h2>Faturamento por dia</h2>
          {filteredAgendamentos.length === 0 ? (
            <EmptyState icon="bar-chart" title="Sem dados para este periodo" />
          ) : (
            <div className="simple-bars">
              {validAgendamentos.slice(0, 8).map((agendamento) => (
                <span key={agendamento.id} style={{ '--bar': `${Math.max(8, agendamento.totalValue)}px` } as React.CSSProperties}>
                  {formatCurrency(agendamento.totalValue)}
                </span>
              ))}
            </div>
          )}
        </section>

        <section className="panel report-panel">
          <h2>Distribuicao por status</h2>
          {filteredAgendamentos.length === 0 ? (
            <EmptyState icon="bar-chart" title="Sem dados para este periodo" />
          ) : (
            <ul className="report-list">
              {statusDistribution.map((item) => (
                <li key={item.status}>
                  <span>{statusLabel[item.status]}</span>
                  <strong>{item.total}</strong>
                </li>
              ))}
            </ul>
          )}
        </section>
      </div>

      <section className="panel report-panel">
        <h2>Servicos mais vendidos</h2>
        {sortedServices.length === 0 ? (
          <EmptyState icon="bar-chart" title="Sem dados para este periodo" />
        ) : (
          <ul className="report-list">
            {sortedServices.map(([name, total]) => (
              <li key={name}>
                <span>{name}</span>
                <strong>{total}</strong>
              </li>
            ))}
          </ul>
        )}
      </section>
    </div>
  )
}

export default RelatoriosPage
