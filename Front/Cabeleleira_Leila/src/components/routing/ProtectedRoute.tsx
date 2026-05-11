import type { ReactNode } from 'react'
import { Navigate, useLocation } from 'react-router-dom'
import { UserRole, type UserRoleType } from '../../Types'
import { useAuth } from '../../context/useAuth'

interface ProtectedRouteProps {
  children: ReactNode
  requiredRole: UserRoleType
}

const ProtectedRoute = ({ children, requiredRole }: ProtectedRouteProps) => {
  const auth = useAuth()
  const location = useLocation()

  if (auth.loading) {
    return <div className="screen-loader">Carregando...</div>
  }

  if (!auth.user) {
    return (
      <Navigate
        to={requiredRole === UserRole.Admin ? '/admin/login' : '/login'}
        state={{ from: location }}
        replace
      />
    )
  }

  if (auth.role !== requiredRole) {
    return (
      <Navigate
        to={requiredRole === UserRole.Admin ? '/admin/login' : '/login'}
        state={{ from: location }}
        replace
      />
    )
  }

  return children
}

export default ProtectedRoute
