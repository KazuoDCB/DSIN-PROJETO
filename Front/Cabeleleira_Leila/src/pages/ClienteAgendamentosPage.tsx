import { useEffect, useMemo, useState } from 'react'
import AppointmentList from '../components/admin/AppointmentList'
import AgendamentoForm from '../components/forms/AgendamentoForm'
import ConfirmModal from '../components/ui/ConfirmModal'
import EmptyState from '../components/ui/EmptyState'
import PageHeader from '../components/ui/PageHeader'
import { useAuth } from '../context/useAuth'
import { useAutoDismiss } from '../hooks/useAutoDismiss'
import {
  createAgendamento,
  deleteAgendamento,
  getAgendamentosByCliente,
  updateAgendamento,
} from '../services/agendamentoService'
import { getServicos } from '../services/servicoService'
import { Status, type Scheduling, type SchedulingRequest, type SchedulingUpdateRequest, type Servico } from '../Types'
import { toFeedbackContent } from '../utils/apiErrors'
import type { FeedbackContent } from '../utils/apiErrors'
import { normalizeScheduling, normalizeSchedulings } from '../utils/scheduling'

const ClienteAgendamentosPage = () => {
  const { user } = useAuth()
  const [agendamentos, setAgendamentos] = useState<Scheduling[]>([])
  const [editingAgendamentoId, setEditingAgendamentoId] = useState<number | undefined>()
  const [feedback, setFeedback] = useState<FeedbackContent | null>(null)
  const [feedbackVariant, setFeedbackVariant] = useState<'error' | 'success'>('error')
  const [isLoading, setIsLoading] = useState(true)
  const [servicos, setServicos] = useState<Servico[]>([])
  const [agendamentoToDelete, setAgendamentoToDelete] = useState<Scheduling | null>(null)

  const clienteId = user?.id ?? 0
  const editingAgendamento = useMemo(
    () => agendamentos.find((agendamento) => agendamento.id === editingAgendamentoId) ?? null,
    [agendamentos, editingAgendamentoId],
  )

  useAutoDismiss(feedback, () => setFeedback(null))

  useEffect(() => {
    if (!clienteId) return

    let mounted = true

    Promise.all([getServicos(1, 100), getAgendamentosByCliente(clienteId)])
      .then(([servicosResponse, agendamentosResponse]) => {
        if (!mounted) return

        setServicos((servicosResponse.data ?? []).filter((servico) => servico.status === Status.Ativo))
        setAgendamentos(normalizeSchedulings(agendamentosResponse.data ?? []))
      })
      .catch((error: Error) => {
        if (mounted) {
          setFeedbackVariant('error')
          setFeedback(toFeedbackContent(error))
        }
      })
      .finally(() => {
        if (mounted) setIsLoading(false)
      })

    return () => {
      mounted = false
    }
  }, [clienteId])

  const handleSubmit = async (payload: SchedulingRequest | SchedulingUpdateRequest, agendamentoId?: number) => {
    setFeedback(null)
    setFeedbackVariant('error')

    try {
      if (agendamentoId) {
        const response = await updateAgendamento(agendamentoId, payload as SchedulingUpdateRequest)
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

      const response = await createAgendamento({ ...(payload as SchedulingRequest), clienteId })
      const createdAgendamento = response.data ? normalizeScheduling(response.data) : null
      if (createdAgendamento) {
        setAgendamentos((current) => [createdAgendamento, ...current])
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

  if (!user) return null

  return (
    <div className="customer-page">
      <PageHeader subtitle="Agende e acompanhe seus horarios no salao" title="Minha agenda" />

      <div className="customer-grid">
        <section className="panel">
          <h2>{editingAgendamentoId ? 'Editar horario' : 'Agendar ida ao salao'}</h2>
          <AgendamentoForm
            agendamentoId={editingAgendamentoId}
            clientes={[user]}
            defaultClienteId={user.id}
            feedback={feedback}
            feedbackVariant={feedbackVariant}
            initialData={editingAgendamento}
            key={editingAgendamentoId ?? 'novo-agendamento-cliente'}
            onCancel={() => {
              setEditingAgendamentoId(undefined)
              setFeedback(null)
            }}
            onSubmit={handleSubmit}
            servicos={servicos}
          />
        </section>

        <section className="panel">
          <header className="panel__header">
            <h2>Meus agendamentos</h2>
            <span>{agendamentos.length} registros</span>
          </header>

          {isLoading ? (
            <EmptyState title="Carregando agendamentos..." />
          ) : (
            <AppointmentList
              agendamentos={agendamentos}
              onDelete={handleDelete}
              onEdit={(agendamento) => setEditingAgendamentoId(agendamento.id)}
            />
          )}
        </section>
      </div>

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

export default ClienteAgendamentosPage
