import { apiClient } from '../../../config/axios'
import type { Card, CreateCardRequest, UpdateCardRequest } from '../../../shared/types/contracts'

export const cardsService = {
  async list(): Promise<Card[]> {
    const { data } = await apiClient.get<Card[]>('/cards')
    return data
  },

  async getById(cardId: string): Promise<Card> {
    const { data } = await apiClient.get<Card>(`/cards/${cardId}`)
    return data
  },

  async create(payload: CreateCardRequest): Promise<Card> {
    const { data } = await apiClient.post<Card>('/cards', payload)
    return data
  },

  async update(cardId: string, payload: UpdateCardRequest): Promise<Card> {
    const { data } = await apiClient.put<Card>(`/cards/${cardId}`, payload)
    return data
  },

  async block(cardId: string): Promise<Card> {
    const { data } = await apiClient.post<Card>(`/cards/${cardId}/block`)
    return data
  },

  async unblock(cardId: string): Promise<Card> {
    const { data } = await apiClient.post<Card>(`/cards/${cardId}/unblock`)
    return data
  },

  async remove(cardId: string): Promise<void> {
    await apiClient.delete(`/cards/${cardId}`)
  },
}
