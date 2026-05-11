import { useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import EmptyState from '../components/ui/EmptyState'
import FeedbackAlert from '../components/ui/FeedbackAlert'
import Icon from '../components/ui/Icon'
import PageHeader from '../components/ui/PageHeader'
import StatCard from '../components/ui/StatCard'
import { useAutoDismiss } from '../hooks/useAutoDismiss'
import { getAgendamentos } from '../services/agendamentoService'
import { getClientes } from '../services/clienteService'
import { Status } from '../Types'
import type { Cliente, Scheduling } from '../Types'
import { SchedulingStatus } from '../Types/agendamento'
import { toFeedbackContent } from '../utils/apiErrors'
import type { FeedbackContent } from '../utils/apiErrors'
import { formatCurrency, formatDate, formatTime, getStatusTone, statusLabel } from '../utils/formatters'
import { normalizeSchedulings } from '../utils/scheduling'

const DashboardPage = () => {
  const [agendamentos, setAgendamentos] = useState<Scheduling[]>([])
  const [clientes, setClientes] = useState<Cliente[]>([])
  const [feedback, setFeedback] = useState<FeedbackContent | null>(null)

  useAutoDismiss(feedback, () => setFeedback(null))

  useEffect(() => {
    const loadDashboard = async () => {
      try {
        const [agendamentosResponse, clientesResponse] = await Promise.all([getAgendamentos(1, 100), getClientes(1, 100)])
        setAgendamentos(normalizeSchedulings(agendamentosResponse.data ?? []))
        setClientes(clientesResponse.data ?? [])
      } catch (error) {
        setFeedback(toFeedbackContent(error))
      }
    }

    loadDashboard()
  }, [])

  const today = useMemo(() => new Date(), [])
  const todayAgendamentos = useMemo(
    () =>
      agendamentos.filter((agendamento) => {
        const date = new Date(agendamento.dataHora)
        return (
          date.getDate() === today.getDate() &&
          date.getMonth() === today.getMonth() &&
          date.getFullYear() === today.getFullYear()
        )
      }),
    [agendamentos, today],
  )

  const todayRevenue = todayAgendamentos
    .filter((agendamento) => agendamento.status !== SchedulingStatus.Cancelado)
    .reduce((total, agendamento) => total + agendamento.totalValue, 0)

  const finishedToday = todayAgendamentos.filter((agendamento) => agendamento.status === SchedulingStatus.Finalizado).length
  const completionRate = todayAgendamentos.length === 0 ? 0 : Math.round((finishedToday / todayAgendamentos.length) * 100)
  const activeClients = clientes.filter((cliente) => cliente.status === Status.Ativo).length
  const recentAgendamentos = agendamentos.slice(0, 6)

  return (
    <div className="admin-page">
      <PageHeader subtitle="Resumo do dia" title="Dashboard" />

      <FeedbackAlert content={feedback} />

      <section className="stats-grid">
        <StatCard icon="calendar" label="Agendamentos" meta="Hoje" value={todayAgendamentos.length} />
        <StatCard icon="currency" label="Faturamento" meta="Hoje" tone="green" value={formatCurrency(todayRevenue)} />
        <StatCard icon="users" label="Clientes ativos" meta="30 dias" tone="blue" value={activeClients} />
        <StatCard icon="bar-chart" label="Conclusao" meta="Hoje" tone="purple" value={`${completionRate}%`} />
      </section>

      <section className="panel">
        <header className="panel__header">
          <h2>Agendamentos recentes</h2>
          <Link className="text-link" to="/admin/agendamentos">
            Ver todos
            <Icon name="chevron-right" />
          </Link>
        </header>

        {recentAgendamentos.length === 0 ? (
          <EmptyState icon="calendar" title="Nenhum agendamento encontrado" />
        ) : (
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Cliente</th>
                  <th>Data</th>
                  <th>Hora</th>
                  <th>Valor</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                {recentAgendamentos.map((agendamento) => (
                  <tr key={agendamento.id}>
                    <td>{agendamento.clienteName || `Cliente ${agendamento.clienteId}`}</td>
                    <td>{formatDate(agendamento.dataHora)}</td>
                    <td>{formatTime(agendamento.dataHora)}</td>
                    <td>{formatCurrency(agendamento.totalValue)}</td>
                    <td>
                      <span className={`status-pill status-pill--${getStatusTone(agendamento.status)}`}>
                        {statusLabel[agendamento.status]}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </div>
  )
}

export default DashboardPage
