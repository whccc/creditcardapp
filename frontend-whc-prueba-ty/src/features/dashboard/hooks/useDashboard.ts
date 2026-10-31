import { useCallback, useEffect, useState } from 'react'
import type { DashboardResponse } from '../../../shared/types/contracts'
import { getErrorMessage } from '../../../shared/utils/errors'
import { dashboardService } from '../services/dashboardService'

interface DashboardState {
  version: number | null
  data: DashboardResponse | null
  error: string | null
}

export function useDashboard() {
  const [version, setVersion] = useState(0)
  const [state, setState] = useState<DashboardState>({ version: null, data: null, error: null })

  useEffect(() => {
    let isActive = true

    dashboardService
      .get()
      .then((data) => {
        if (isActive) {
          setState({ version, data, error: null })
        }
      })
      .catch((error: unknown) => {
        if (isActive) {
          setState((current) => ({
            version,
            data: current.data,
            error: getErrorMessage(error, 'No fue posible cargar tu resumen.'),
          }))
        }
      })

    return () => {
      isActive = false
    }
  }, [version])

  const reload = useCallback(() => setVersion((current) => current + 1), [])

  return {
    data: state.data,
    error: state.error,
    isLoading: state.version !== version,
    reload,
  }
}
