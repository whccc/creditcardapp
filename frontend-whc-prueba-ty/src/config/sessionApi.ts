import type { SessionResponse } from '../shared/types/contracts'
import { apiClient } from './axios'

export const sessionApi = {
  async get(): Promise<SessionResponse> {
    const { data } = await apiClient.get<SessionResponse>('/auth/session')
    return data
  },

  async end(): Promise<void> {
    await apiClient.post('/auth/logout')
  },
}
