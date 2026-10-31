import { apiClient } from '../../../config/axios'
import type { DashboardResponse } from '../../../shared/types/contracts'

export const dashboardService = {
  async get(): Promise<DashboardResponse> {
    const { data } = await apiClient.get<DashboardResponse>('/dashboard')
    return data
  },
}
