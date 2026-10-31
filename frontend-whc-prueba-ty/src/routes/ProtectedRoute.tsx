import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { FullPageSpinner } from '../shared/components/FullPageSpinner'

export interface RedirectState {
  from?: string
}

export function ProtectedRoute() {
  const { status } = useAuth()
  const location = useLocation()

  if (status === 'checking') {
    return <FullPageSpinner label="Verificando sesión…" />
  }

  if (status === 'anonymous') {
    const state: RedirectState = { from: `${location.pathname}${location.search}` }
    return <Navigate to="/login" replace state={state} />
  }

  return <Outlet />
}
