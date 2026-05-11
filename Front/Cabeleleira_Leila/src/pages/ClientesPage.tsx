import { useEffect, useMemo, useState } from 'react'
import ClienteForm from '../components/forms/ClienteForm'
import EmptyState from '../components/ui/EmptyState'
import Icon from '../components/ui/Icon'
import PageHeader from '../components/ui/PageHeader'
import { useAutoDismiss } from '../hooks/useAutoDismiss'
import { createCliente, deleteCliente, getClientes, updateCliente } from '../services/clienteService'
import { statusText } from '../Types'
import type { Cliente, ClienteRequest, ClienteUpdateRequest } from '../Types'
import { toFeedbackContent } from '../utils/apiErrors'
import type { FeedbackContent } from '../utils/apiErrors'

const ClientesPage = () => {
  const [clientes, setClientes] = useState<Cliente[]>([])
  const [editingClienteId, setEditingClienteId] = useState<number | undefined>()
  const [feedback, setFeedback] = useState<FeedbackContent | null>(null)
  const [feedbackVariant, setFeedbackVariant] = useState<'error' | 'success'>('error')
  const [isLoading, setIsLoading] = useState(true)

  const editingCliente = useMemo(
    () => clientes.find((cliente) => cliente.id === editingClienteId) ?? null,
    [clientes, editingClienteId],
  )

  useAutoDismiss(feedback, () => setFeedback(null))

  useEffect(() => {
    let mounted = true

    getClientes(1, 100)
      .then((response) => {
        if (mounted) setClientes(response.data ?? [])
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

  const handleSubmit = async (payload: ClienteRequest | ClienteUpdateRequest, clienteId?: number) => {
    setFeedback(null)
    setFeedbackVariant('error')

    try {
      if (clienteId) {
        const response = await updateCliente(clienteId, payload as ClienteUpdateRequest)
        const updatedCliente = response.data
        if (updatedCliente) {
          setClientes((current) => current.map((cliente) => (cliente.id === updatedCliente.id ? updatedCliente : cliente)))
          setEditingClienteId(undefined)
          setFeedbackVariant('success')
          setFeedback('Cliente atualizado com sucesso.')
        }
        return
      }

      const response = await createCliente(payload as ClienteRequest)
      const createdCliente = response.data
      if (createdCliente) {
        setClientes((current) => [createdCliente, ...current])
        setFeedbackVariant('success')
        setFeedback('Cliente criado com sucesso.')
      }
    } catch (error) {
      setFeedbackVariant('error')
      setFeedback(toFeedbackContent(error))
    }
  }

  const handleDelete = async (id: number) => {
    if (!window.confirm('Deseja realmente excluir este cliente?')) return

    try {
      await deleteCliente(id)
      setClientes((current) => current.filter((cliente) => cliente.id !== id))
      if (editingClienteId === id) setEditingClienteId(undefined)
      setFeedbackVariant('success')
      setFeedback('Cliente removido.')
    } catch (error) {
      setFeedbackVariant('error')
      setFeedback(toFeedbackContent(error))
    }
  }

  return (
    <div className="admin-page">
      <PageHeader subtitle="Cadastro e manutencao de clientes" title="Clientes" />

      <div className="admin-grid admin-grid--form">
        <section className="panel">
          <h2>{editingClienteId ? 'Editar cliente' : 'Novo cliente'}</h2>
          <ClienteForm
            clienteId={editingClienteId}
            feedback={feedback}
            feedbackVariant={feedbackVariant}
            initialData={editingCliente}
            key={editingClienteId ?? 'novo-cliente'}
            onCancel={() => {
              setEditingClienteId(undefined)
              setFeedback(null)
            }}
            onSubmit={handleSubmit}
          />
        </section>

        <section className="panel">
          <header className="panel__header">
            <h2>Lista de clientes</h2>
            <span>{clientes.length} registros</span>
          </header>

          {isLoading ? (
            <EmptyState icon="users" title="Carregando clientes..." />
          ) : clientes.length === 0 ? (
            <EmptyState icon="users" title="Nenhum cliente encontrado" />
          ) : (
            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th>Nome</th>
                    <th>Email</th>
                    <th>Telefone</th>
                    <th>Status</th>
                    <th>Acoes</th>
                  </tr>
                </thead>
                <tbody>
                  {clientes.map((cliente) => (
                    <tr key={cliente.id}>
                      <td>{cliente.name}</td>
                      <td>{cliente.email}</td>
                      <td>{cliente.number}</td>
                      <td>{statusText[cliente.status]}</td>
                      <td>
                        <div className="row-actions">
                          <button
                            aria-label="Editar cliente"
                            className="icon-button"
                            onClick={() => setEditingClienteId(cliente.id)}
                            type="button"
                          >
                            <Icon name="edit" />
                          </button>
                          <button
                            aria-label="Excluir cliente"
                            className="icon-button icon-button--danger"
                            onClick={() => handleDelete(cliente.id)}
                            type="button"
                          >
                            <Icon name="trash" />
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </section>
      </div>
    </div>
  )
}

export default ClientesPage
