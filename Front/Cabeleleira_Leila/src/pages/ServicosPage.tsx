import { useEffect, useMemo, useState } from 'react'
import ServicoForm from '../components/forms/ServicoForm'
import ConfirmModal from '../components/ui/ConfirmModal'
import EmptyState from '../components/ui/EmptyState'
import Icon from '../components/ui/Icon'
import PageHeader from '../components/ui/PageHeader'
import { useAutoDismiss } from '../hooks/useAutoDismiss'
import { createServico, deleteServico, getServicos, updateServico } from '../services/servicoService'
import { statusText } from '../Types'
import type { Servico, ServicoRequest, ServicoUpdateRequest } from '../Types'
import { toFeedbackContent } from '../utils/apiErrors'
import type { FeedbackContent } from '../utils/apiErrors'
import { formatCurrency } from '../utils/formatters'

const ServicosPage = () => {
  const [servicos, setServicos] = useState<Servico[]>([])
  const [editingServicoId, setEditingServicoId] = useState<number | undefined>()
  const [feedback, setFeedback] = useState<FeedbackContent | null>(null)
  const [feedbackVariant, setFeedbackVariant] = useState<'error' | 'success'>('error')
  const [isLoading, setIsLoading] = useState(true)
  const [servicoToDelete, setServicoToDelete] = useState<Servico | null>(null)

  const editingServico = useMemo(
    () => servicos.find((servico) => servico.id === editingServicoId) ?? null,
    [servicos, editingServicoId],
  )

  useAutoDismiss(feedback, () => setFeedback(null))

  useEffect(() => {
    let mounted = true

    getServicos(1, 100)
      .then((response) => {
        if (mounted) setServicos(response.data ?? [])
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
  }, [])

  const handleSubmit = async (payload: ServicoRequest | ServicoUpdateRequest, servicoId?: number) => {
    setFeedback(null)
    setFeedbackVariant('error')

    try {
      if (servicoId) {
        const response = await updateServico(servicoId, payload as ServicoUpdateRequest)
        const updatedServico = response.data
        if (updatedServico) {
          setServicos((current) => current.map((servico) => (servico.id === updatedServico.id ? updatedServico : servico)))
          setEditingServicoId(undefined)
          setFeedbackVariant('success')
          setFeedback('Servico atualizado com sucesso.')
        }
        return
      }

      const response = await createServico(payload as ServicoRequest)
      const createdServico = response.data
      if (createdServico) {
        setServicos((current) => [createdServico, ...current])
        setFeedbackVariant('success')
        setFeedback('Servico cadastrado com sucesso.')
      }
    } catch (error) {
      setFeedbackVariant('error')
      setFeedback(toFeedbackContent(error))
    }
  }

  const handleDelete = async (servico: Servico) => {
    setServicoToDelete(servico)
  }

  const confirmDelete = async () => {
    if (!servicoToDelete) return
    try {
      await deleteServico(servicoToDelete.id)
      setServicos((current) => current.filter((servico) => servico.id !== servicoToDelete.id))
      if (editingServicoId === servicoToDelete.id) setEditingServicoId(undefined)
      setFeedbackVariant('success')
      setFeedback('Servico removido.')
      setServicoToDelete(null)
    } catch (error) {
      setFeedbackVariant('error')
      setFeedback(toFeedbackContent(error))
    }
  }

  return (
    <div className="admin-page">
      <PageHeader subtitle="Controle de valores, duracao e disponibilidade" title="Servicos" />

      <div className="admin-grid admin-grid--form">
        <section className="panel">
          <h2>{editingServicoId ? 'Editar servico' : 'Novo servico'}</h2>
          <ServicoForm
            feedback={feedback}
            feedbackVariant={feedbackVariant}
            initialData={editingServico}
            key={editingServicoId ?? 'novo-servico'}
            onCancel={() => {
              setEditingServicoId(undefined)
              setFeedback(null)
            }}
            onSubmit={handleSubmit}
            servicoId={editingServicoId}
          />
        </section>

        <section className="panel">
          <header className="panel__header">
            <h2>Lista de servicos</h2>
            <span>{servicos.length} registros</span>
          </header>

          {isLoading ? (
            <EmptyState icon="bar-chart" title="Carregando servicos..." />
          ) : servicos.length === 0 ? (
            <EmptyState icon="bar-chart" title="Nenhum servico registrado" />
          ) : (
            <div className="admin-service-grid">
              {servicos.map((servico) => (
                <article className="admin-service-card" key={servico.id}>
                  <header className="admin-service-card__header">
                    <h3>{servico.name}</h3>
                    <div className="admin-service-card__header-actions">
                      <span className={`status-pill ${servico.status === 1 ? 'status-pill--success' : 'status-pill--danger'}`}>
                        {statusText[servico.status]}
                      </span>
                      <button
                        aria-label="Editar servico"
                        className="icon-button"
                        onClick={() => setEditingServicoId(servico.id)}
                        type="button"
                      >
                        <Icon name="edit" />
                      </button>
                      <button
                        aria-label="Excluir servico"
                        className="icon-button icon-button--danger"
                        onClick={() => handleDelete(servico)}
                        type="button"
                      >
                        <Icon name="trash" />
                      </button>
                    </div>
                  </header>

                  <div className="admin-service-card__body">
                    <p>{servico.description}</p>
                  </div>

                  <dl className="admin-service-card__meta">
                    <div>
                      <dt>Preco</dt>
                      <dd>{formatCurrency(servico.price)}</dd>
                    </div>
                    <div>
                      <dt>Duracao</dt>
                      <dd>{servico.duration} min</dd>
                    </div>
                  </dl>

                </article>
              ))}
            </div>
          )}
        </section>
      </div>

      {servicoToDelete ? (
        <ConfirmModal
          confirmLabel="Excluir servico"
          description={`Deseja confirmar a exclusao do servico "${servicoToDelete.name}"?`}
          onCancel={() => setServicoToDelete(null)}
          onConfirm={confirmDelete}
          title="Excluir servico"
        />
      ) : null}
    </div>
  )
}

export default ServicosPage
