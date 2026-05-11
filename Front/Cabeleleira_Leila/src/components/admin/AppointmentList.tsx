import type { Scheduling } from '../../Types'
import { SchedulingStatus } from '../../Types/agendamento'
import { formatCurrency, formatTime, getStatusTone, statusLabel } from '../../utils/formatters'
import EmptyState from '../ui/EmptyState'
import Icon from '../ui/Icon'

interface AppointmentListProps {
  agendamentos: Scheduling[]
  onDelete?: (agendamento: Scheduling) => void
  onEdit?: (agendamento: Scheduling) => void
}

const AppointmentList = ({ agendamentos, onDelete, onEdit }: AppointmentListProps) => {
  if (agendamentos.length === 0) {
    return <EmptyState description="Selecione outra data ou crie um novo horario." title="Nenhum agendamento para este dia" />
  }

  return (
    <div className="appointment-list">
      {agendamentos.map((agendamento) => (
        <article className="appointment-item" key={agendamento.id}>
          <div>
            <time>{formatTime(agendamento.dataHora)}</time>
            <strong>{agendamento.clienteName || `Cliente ${agendamento.clienteId}`}</strong>
            <p>{agendamento.servicos.map((servico) => servico.name).join(', ') || 'Servicos nao informados'}</p>
          </div>
          <div className="appointment-item__side">
            <span className={`status-pill status-pill--${getStatusTone(agendamento.status)}`}>
              {statusLabel[agendamento.status]}
            </span>
            <strong>{formatCurrency(agendamento.totalValue)}</strong>
            <div className="appointment-item__actions">
              {onEdit ? (
                <button aria-label="Editar agendamento" className="icon-button" onClick={() => onEdit(agendamento)} type="button">
                  <Icon name="edit" />
                </button>
              ) : null}
              {onDelete && agendamento.status !== SchedulingStatus.Finalizado ? (
                <button
                  aria-label="Excluir agendamento"
                  className="icon-button icon-button--danger"
                  onClick={() => onDelete(agendamento)}
                  type="button"
                >
                  <Icon name="trash" />
                </button>
              ) : null}
            </div>
          </div>
        </article>
      ))}
    </div>
  )
}

export default AppointmentList
