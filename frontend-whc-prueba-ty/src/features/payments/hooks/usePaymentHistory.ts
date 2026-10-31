import { useEffect, useState } from 'react'
import type { PagedResponse, Payment } from '../../../shared/types/contracts'
import { getErrorMessage } from '../../../shared/utils/errors'
import { paymentsService } from '../services/paymentsService'

interface UsePaymentHistoryOptions {
  cardId?: string | null
  refreshKey?: number
  pageSize?: number
}

interface HistoryState {
  requestKey: string | null
  data: PagedResponse<Payment> | null
  error: string | null
}

export function usePaymentHistory({ cardId = null, refreshKey = 0, pageSize = 8 }: UsePaymentHistoryOptions = {}) {
  const [page, setPage] = useState(1)
  const [state, setState] = useState<HistoryState>({ requestKey: null, data: null, error: null })
  const requestKey = `${cardId ?? 'all'}|${page}|${pageSize}|${refreshKey}`

  useEffect(() => {
    let isActive = true

    paymentsService
      .getHistory({ cardId, page, pageSize })
      .then((data) => {
        if (isActive) {
          setState({ requestKey, data, error: null })
        }
      })
      .catch((error: unknown) => {
        if (isActive) {
          setState((current) => ({
            requestKey,
            data: current.data,
            error: getErrorMessage(error, 'No fue posible cargar el historial de pagos.'),
          }))
        }
      })

    return () => {
      isActive = false
    }
  }, [cardId, page, pageSize, requestKey])

  return {
    data: state.data,
    error: state.error,
    isLoading: state.requestKey !== requestKey,
    page,
    setPage,
  }
}
