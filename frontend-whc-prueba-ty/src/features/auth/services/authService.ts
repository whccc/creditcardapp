import { apiClient } from '../../../config/axios'
import type { LoginRequest, SessionResponse } from '../../../shared/types/contracts'

export const authService = {
  async login(credentials: LoginRequest): Promise<SessionResponse> {
    const { data } = await apiClient.post<SessionResponse>('/auth/login', credentials)
    return data
  },
}
