import { Navigate, Outlet } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { FullPageSpinner } from '../shared/components/FullPageSpinner'

export function PublicOnlyRoute() {
  const { status } = useAuth()

  if (status === 'checking') {
    return <FullPageSpinner label="Verificando sesión…" />
  }

  return status === 'authenticated' ? <Navigate to="/" replace /> : <Outlet />
}
