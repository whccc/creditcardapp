import { apiClient } from '../../../config/axios'
import type {
  CreatePaymentRequest,
  PagedResponse,
  Payment,
  PaymentHistoryQuery,
} from '../../../shared/types/contracts'

export const paymentsService = {
  async create(payload: CreatePaymentRequest): Promise<Payment> {
    const { data } = await apiClient.post<Payment>('/payments', payload)
    return data
  },

  async getHistory({ cardId = null, page = 1, pageSize = 10 }: PaymentHistoryQuery = {}): Promise<PagedResponse<Payment>> {
    const { data } = await apiClient.get<PagedResponse<Payment>>('/payments', {
      params: { page, pageSize, ...(cardId && { cardId }) },
    })
    return data
  },
}
