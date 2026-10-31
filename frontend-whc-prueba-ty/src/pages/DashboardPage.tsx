import { useCallback, useState } from 'react'
import { CardFormModal, CardList, useCardActions } from '../features/cards'
import { CreditSummary, useDashboard } from '../features/dashboard'
import { PaymentForm, PaymentHistory } from '../features/payments'
import { Alert } from '../shared/components/Alert'
import { Button } from '../shared/components/Button'
import { ConfirmDialog } from '../shared/components/ConfirmDialog'
import { PlusIcon, RefreshIcon } from '../shared/components/icons'
import type { Card } from '../shared/types/contracts'
import { getErrorMessage } from '../shared/utils/errors'

type CardFormState = { mode: 'create' } | { mode: 'edit'; card: Card }

function cardLabel(card: Card): string {
  return `${card.nickname || card.brandName} •••• ${card.last4Digits}`
}

function DashboardSkeleton() {
  return (
    <div className="animate-pulse space-y-10" aria-busy="true" aria-label="Cargando resumen">
      <div className="h-10 w-64 rounded-xl bg-white/5" />
      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        {Array.from({ length: 4 }, (_, index) => (
          <div key={index} className="h-28 rounded-2xl bg-white/5" />
        ))}
      </div>
      <div className="grid gap-6 sm:grid-cols-2 xl:grid-cols-3">
        {Array.from({ length: 3 }, (_, index) => (
          <div key={index} className="aspect-[1.586] rounded-2xl bg-white/5" />
        ))}
      </div>
    </div>
  )
}

export function DashboardPage() {
  const { data, error, isLoading, reload } = useDashboard()
  const cardActions = useCardActions({ onChanged: reload })
  const [selectedCardId, setSelectedCardId] = useState<string | null>(null)
  const [historyRefreshKey, setHistoryRefreshKey] = useState(0)
  const [cardForm, setCardForm] = useState<CardFormState | null>(null)
  const [cardToDelete, setCardToDelete] = useState<Card | null>(null)
  const [deleteError, setDeleteError] = useState<string | null>(null)
  const [actionError, setActionError] = useState<string | null>(null)

  const cards = data?.cards ?? []
  const selectedCard = cards.find((card) => card.id === selectedCardId) ?? null

  const refreshHistory = useCallback(() => setHistoryRefreshKey((current) => current + 1), [])
  const closeCardForm = useCallback(() => setCardForm(null), [])
  const cancelDelete = useCallback(() => {
    setCardToDelete(null)
    setDeleteError(null)
  }, [])

  const handleSelectCard = (cardId: string) =>
    setSelectedCardId((current) => (current === cardId ? null : cardId))

  const handleToggleBlock = async (card: Card) => {
    setActionError(null)

    try {
      await cardActions.toggleBlock(card)
    } catch (requestError) {
      setActionError(getErrorMessage(requestError, 'No fue posible cambiar el estado de la tarjeta.'))
    }
  }

  const handleConfirmDelete = async () => {
    if (!cardToDelete) {
      return
    }

    setDeleteError(null)

    try {
      await cardActions.deleteCard(cardToDelete.id)
      setCardToDelete(null)
      refreshHistory()
    } catch (requestError) {
      setDeleteError(getErrorMessage(requestError, 'No fue posible eliminar la tarjeta.'))
    }
  }

  const handlePaymentProcessed = () => {
    reload()
    refreshHistory()
  }

  if (!data && isLoading) {
    return <DashboardSkeleton />
  }

  if (!data) {
    return (
      <Alert tone="error" title="No pudimos cargar tu información">
        <p>{error}</p>
        <Button variant="secondary" size="sm" className="mt-3" onClick={reload}>
          <RefreshIcon className="size-3.5" />
          Reintentar
        </Button>
      </Alert>
    )
  }

  return (
    <div className="space-y-10">
      <header className="flex flex-wrap items-end justify-between gap-4">
        <div>
          <p className="text-sm text-slate-400">Bienvenido de vuelta</p>
          <h1 className="mt-1 text-3xl font-bold tracking-tight text-white">Hola, {data.user.firstName}</h1>
        </div>
        <Button onClick={() => setCardForm({ mode: 'create' })}>
          <PlusIcon className="size-4" />
          Nueva tarjeta
        </Button>
      </header>

      {error && (
        <Alert tone="warning" title="Mostrando información desactualizada">
          {error}
        </Alert>
      )}

      <CreditSummary summary={data.summary} />

      <section className="space-y-5" aria-labelledby="cards-heading">
        <div>
          <h2 id="cards-heading" className="text-lg font-semibold text-white">
            Mis tarjetas
          </h2>
          <p className="mt-1 text-sm text-slate-400">
            Selecciona una tarjeta para pagar con ella y filtrar su historial.
          </p>
        </div>

        {actionError && (
          <Alert tone="error" onDismiss={() => setActionError(null)}>
            {actionError}
          </Alert>
        )}

        <CardList
          cards={cards}
          selectedCardId={selectedCard?.id ?? null}
          pendingCardId={cardActions.pendingCardId}
          onSelect={handleSelectCard}
          onAdd={() => setCardForm({ mode: 'create' })}
          onEdit={(card) => setCardForm({ mode: 'edit', card })}
          onToggleBlock={handleToggleBlock}
          onDelete={setCardToDelete}
        />
      </section>

      <div className="grid gap-6 lg:grid-cols-5">
        <div className="lg:col-span-2">
          <PaymentForm
            key={selectedCard?.id ?? 'none'}
            cards={cards}
            defaultCardId={selectedCard?.id ?? null}
            onProcessed={handlePaymentProcessed}
          />
        </div>
        <div className="lg:col-span-3">
          <PaymentHistory
            key={selectedCard?.id ?? 'all'}
            cardId={selectedCard?.id ?? null}
            filterLabel={selectedCard ? cardLabel(selectedCard) : null}
            refreshKey={historyRefreshKey}
            onClearFilter={() => setSelectedCardId(null)}
          />
        </div>
      </div>

      {cardForm?.mode === 'create' && (
        <CardFormModal key="new" card={null} onClose={closeCardForm} onSubmit={cardActions.createCard} />
      )}

      {cardForm?.mode === 'edit' && (
        <CardFormModal
          key={cardForm.card.id}
          card={cardForm.card}
          onClose={closeCardForm}
          onSubmit={(payload) => cardActions.updateCard(cardForm.card.id, payload)}
        />
      )}

      {cardToDelete && (
        <ConfirmDialog
          title="Eliminar tarjeta"
          message={`¿Seguro que deseas eliminar ${cardLabel(cardToDelete)}? Su historial de pagos se conservará.`}
          confirmLabel="Eliminar"
          isLoading={cardActions.pendingCardId === cardToDelete.id}
          error={deleteError}
          onConfirm={handleConfirmDelete}
          onCancel={cancelDelete}
        />
      )}
    </div>
  )
}
