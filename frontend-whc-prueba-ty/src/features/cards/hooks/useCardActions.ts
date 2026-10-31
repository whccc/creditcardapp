import { useCallback, useMemo, useState } from 'react'
import type { Card, CreateCardRequest, UpdateCardRequest } from '../../../shared/types/contracts'
import { cardsService } from '../services/cardsService'

const NEW_CARD_KEY = 'new'

interface UseCardActionsOptions {
  onChanged?: () => void
}

export interface CardActions {
  pendingCardId: string | null
  createCard: (payload: CreateCardRequest) => Promise<Card>
  updateCard: (cardId: string, payload: UpdateCardRequest) => Promise<Card>
  toggleBlock: (card: Card) => Promise<Card>
  deleteCard: (cardId: string) => Promise<void>
}

export function useCardActions({ onChanged }: UseCardActionsOptions = {}): CardActions {
  const [pendingCardId, setPendingCardId] = useState<string | null>(null)

  const run = useCallback(
    async <T>(cardId: string, action: () => Promise<T>): Promise<T> => {
      setPendingCardId(cardId)

      try {
        const result = await action()
        onChanged?.()
        return result
      } finally {
        setPendingCardId(null)
      }
    },
    [onChanged],
  )

  return useMemo<CardActions>(
    () => ({
      pendingCardId,
      createCard: (payload) => run(NEW_CARD_KEY, () => cardsService.create(payload)),
      updateCard: (cardId, payload) => run(cardId, () => cardsService.update(cardId, payload)),
      toggleBlock: (card) =>
        run(card.id, () => (card.status === 'blocked' ? cardsService.unblock(card.id) : cardsService.block(card.id))),
      deleteCard: (cardId) => run(cardId, () => cardsService.remove(cardId)),
    }),
    [pendingCardId, run],
  )
}
