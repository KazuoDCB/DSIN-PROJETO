import { useEffect, useMemo, useState } from 'react'
import AppointmentList from '../components/admin/AppointmentList'
import CalendarPanel from '../components/admin/CalendarPanel'
import AgendamentoForm from '../components/forms/AgendamentoForm'
import ConfirmModal from '../components/ui/ConfirmModal'
import Icon from '../components/ui/Icon'
import PageHeader from '../components/ui/PageHeader'
import { useAutoDismiss } from '../hooks/useAutoDismiss'
import {
  adminUpdateAgendamento,
  createAgendamento,
  deleteAgendamento,
  getAgendamentos,
} from '../services/agendamentoService'
import { getClientes } from '../services/clienteService'
import { getServicos } from '../services/servicoService'
import { Status } from '../Types'
import type { Cliente, Scheduling, SchedulingRequest, SchedulingUpdateRequest, Servico } from '../Types'
import { toFeedbackContent } from '../utils/apiErrors'
import type { FeedbackContent } from '../utils/apiErrors'
import { sameDay } from '../utils/calendar'
import { weekdayDateFormatter } from '../utils/formatters'
import { normalizeScheduling, normalizeSchedulings } from '../utils/scheduling'

const AgendamentosPage = () => {
  const [agendamentos, setAgendamentos] = useState<Scheduling[]>([])
  const [clientes, setClientes] = useState<Cliente[]>([])
  const [editingAgendamentoId, setEditingAgendamentoId] = useState<number | undefined>()
  const [feedback, setFeedback] = useState<FeedbackContent | null>(null)
  const [feedbackVariant, setFeedbackVariant] = useState<'error' | 'success'>('error')
  const [month, setMonth] = useState(() => new Date())
  const [selectedDate, setSelectedDate] = useState(() => new Date())
  const [servicos, setServicos] = useState<Servico[]>([])
  const [agendamentoToDelete, setAgendamentoToDelete] = useState<Scheduling | null>(null)

  const editingAgendamento = useMemo(
    () => agendamentos.find((agendamento) => agendamento.id === editingAgendamentoId) ?? null,
    [agendamentos, editingAgendamentoId],
  )

  const selectedDateAgendamentos = useMemo(
    () => agendamentos.filter((agendamento) => sameDay(new Date(agendamento.dataHora), selectedDate)),
    [agendamentos, selectedDate],
  )

  const datesWithAppointments = useMemo(
    () => agendamentos.map((agendamento) => new Date(agendamento.dataHora)),
    [agendamentos],
  )

  useAutoDismiss(feedback, () => setFeedback(null))

  useEffect(() => {
    let mounted = true

    Promise.all([getClientes(1, 100), getServicos(1, 100), getAgendamentos(1, 200)])
      .then(([clientesResponse, servicosResponse, agendamentosResponse]) => {
        if (!mounted) return

        setClientes(clientesResponse.data ?? [])
        setServicos((servicosResponse.data ?? []).filter((servico) => servico.status === Status.Ativo))
        setAgendamentos(normalizeSchedulings(agendamentosResponse.data ?? []))
      })
      .catch((error: Error) => {
        if (mounted) {
          setFeedbackVariant('error')
          setFeedback(toFeedbackContent(error))
        }
      })

    return () => {
      mounted = false
    }
  }, [])

  const handleSubmit = async (payload: SchedulingRequest | SchedulingUpdateRequest, agendamentoId?: number) => {
    setFeedback(null)
    setFeedbackVariant('error')

    try {
      if (agendamentoId) {
        const response = await adminUpdateAgendamento(agendamentoId, payload as SchedulingUpdateRequest)
        const updatedAgendamento = response.data ? normalizeScheduling(response.data) : null
        if (updatedAgendamento) {
          setAgendamentos((current) =>
            current.map((agendamento) => (agendamento.id === updatedAgendamento.id ? updatedAgendamento : agendamento)),
          )
          setEditingAgendamentoId(undefined)
          setFeedbackVariant('success')
          setFeedback('Agendamento atualizado com sucesso.')
        }
        return
      }

      const response = await createAgendamento(payload as SchedulingRequest)
      const createdAgendamento = response.data ? normalizeScheduling(response.data) : null
      if (createdAgendamento) {
        setAgendamentos((current) => [createdAgendamento, ...current])
        setSelectedDate(new Date(createdAgendamento.dataHora))
        setMonth(new Date(createdAgendamento.dataHora))
        setFeedbackVariant('success')
        setFeedback('Agendamento criado com sucesso.')
      }
    } catch (error) {
      setFeedbackVariant('error')
      setFeedback(toFeedbackContent(error))
    }
  }

  const handleDelete = async (agendamento: Scheduling) => {
    setAgendamentoToDelete(agendamento)
  }

  const confirmDelete = async () => {
    if (!agendamentoToDelete) return
    try {
      await deleteAgendamento(agendamentoToDelete.id)
      setAgendamentos((current) => current.filter((item) => item.id !== agendamentoToDelete.id))
      if (editingAgendamentoId === agendamentoToDelete.id) setEditingAgendamentoId(undefined)
      setFeedbackVariant('success')
      setFeedback('Agendamento excluido com sucesso.')
      setAgendamentoToDelete(null)
    } catch (error) {
      setFeedbackVariant('error')
      setFeedback(toFeedbackContent(error))
    }
  }

  return (
    <div className="admin-page">
      <PageHeader subtitle="Gerencie os horarios do salao" title="Agenda" />

      <div className="schedule-grid">
        <CalendarPanel
          datesWithAppointments={datesWithAppointments}
          month={month}
          onMonthChange={setMonth}
          onSelectDate={setSelectedDate}
          selectedDate={selectedDate}
        />

        <section className="panel schedule-day-panel">
          <header className="panel__header">
            <h2>
              <Icon name="calendar" />
              {weekdayDateFormatter.format(selectedDate)}
            </h2>
          </header>
          <AppointmentList
            agendamentos={selectedDateAgendamentos}
            onDelete={handleDelete}
            onEdit={(agendamento) => setEditingAgendamentoId(agendamento.id)}
          />
        </section>
      </div>

      <section className="panel">
        <h2>{editingAgendamentoId ? 'Editar agendamento' : 'Novo agendamento'}</h2>
        <AgendamentoForm
          agendamentoId={editingAgendamentoId}
          allowStatusEdit
          clientes={clientes}
          feedback={feedback}
          feedbackVariant={feedbackVariant}
          initialData={editingAgendamento}
          key={editingAgendamentoId ?? 'novo-agendamento'}
          onCancel={() => {
            setEditingAgendamentoId(undefined)
            setFeedback(null)
          }}
          onSubmit={handleSubmit}
          servicos={servicos}
        />
      </section>

      {agendamentoToDelete ? (
        <ConfirmModal
          confirmLabel="Excluir atendimento"
          description="Deseja confirmar a exclusao deste atendimento?"
          onCancel={() => setAgendamentoToDelete(null)}
          onConfirm={confirmDelete}
          title="Excluir atendimento"
        />
      ) : null}
    </div>
  )
}

export default AgendamentosPage
