import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import type { ReactNode } from 'react'
import AdminLayout from './components/layout/AdminLayout'
import CustomerLayout from './components/layout/CustomerLayout'
import PublicLayout from './components/layout/PublicLayout'
import ProtectedRoute from './components/routing/ProtectedRoute'
import { AuthProvider } from './context/AuthContext'
import AgendamentosPage from './pages/AgendamentosPage'
import ClienteAgendamentosPage from './pages/ClienteAgendamentosPage'
import ClientesPage from './pages/ClientesPage'
import DashboardPage from './pages/DashboardPage'
import HomePage from './pages/HomePage'
import LoginPage from './pages/LoginPage'
import NotFoundPage from './pages/NotFoundPage'
import PublicServicosPage from './pages/PublicServicosPage'
import RelatoriosPage from './pages/RelatoriosPage'
import ServicosPage from './pages/ServicosPage'
import SobrePage from './pages/SobrePage'
import { UserRole } from './Types'

const publicPage = (page: ReactNode) => <PublicLayout>{page}</PublicLayout>

const adminPage = (page: ReactNode) => (
  <ProtectedRoute requiredRole={UserRole.Admin}>
    <AdminLayout>{page}</AdminLayout>
  </ProtectedRoute>
)

const customerPage = (page: ReactNode) => (
  <ProtectedRoute requiredRole={UserRole.Cliente}>
    <CustomerLayout>{page}</CustomerLayout>
  </ProtectedRoute>
)

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/" element={publicPage(<HomePage />)} />
          <Route path="/servicos" element={publicPage(<PublicServicosPage />)} />
          <Route path="/sobre" element={publicPage(<SobrePage />)} />
          <Route path="/login" element={publicPage(<LoginPage scope="cliente" />)} />
          <Route path="/admin/login" element={publicPage(<LoginPage scope="admin" />)} />
          <Route path="/minha-agenda" element={customerPage(<ClienteAgendamentosPage />)} />
          <Route path="/minha-agenda/servicos" element={customerPage(<PublicServicosPage />)} />

          <Route path="/admin" element={adminPage(<DashboardPage />)} />
          <Route path="/admin/agendamentos" element={adminPage(<AgendamentosPage />)} />
          <Route path="/admin/clientes" element={adminPage(<ClientesPage />)} />
          <Route path="/admin/relatorios" element={adminPage(<RelatoriosPage />)} />
          <Route path="/admin/servicos" element={adminPage(<ServicosPage />)} />

          <Route path="/dashboard" element={<Navigate to="/admin" replace />} />
          <Route path="/agendamentos" element={<Navigate to="/admin/agendamentos" replace />} />
          <Route path="/clientes" element={<Navigate to="/admin/clientes" replace />} />
          <Route path="*" element={publicPage(<NotFoundPage />)} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  )
}

export default App
