import { Button } from '../../../shared/components/Button'
import { LockIcon, PencilIcon, PlusIcon, TrashIcon, UnlockIcon } from '../../../shared/components/icons'
import { ProgressBar } from '../../../shared/components/ProgressBar'
import type { Card } from '../../../shared/types/contracts'
import { formatCurrency } from '../../../shared/utils/formatters'
import { CreditCard } from './CreditCard'

interface CardHandlers {
  onSelect: (cardId: string) => void
  onEdit: (card: Card) => void
  onToggleBlock: (card: Card) => void
  onDelete: (card: Card) => void
}

interface CardTileProps extends CardHandlers {
  card: Card
  isSelected: boolean
  isPending: boolean
}

function CardTile({ card, isSelected, isPending, onSelect, onEdit, onToggleBlock, onDelete }: CardTileProps) {
  const isBlocked = card.status === 'blocked'

  return (
    <article className="space-y-3">
      <CreditCard
        brand={card.brand}
        maskedNumber={card.maskedNumber}
        holderName={card.holderName}
        expiry={card.expiry}
        nickname={card.nickname}
        status={card.status}
        isExpired={card.isExpired}
        isSelected={isSelected}
        onSelect={() => onSelect(card.id)}
      />

      <div className="rounded-2xl bg-slate-900/60 p-4 ring-1 ring-white/5">
        <div className="flex items-baseline justify-between gap-3">
          <span className="text-xs tracking-wide text-slate-400 uppercase">Disponible</span>
          <span className="text-base font-semibold text-white tabular-nums">
            {formatCurrency(card.availableCredit, card.currency)}
          </span>
        </div>
        <ProgressBar value={card.utilizationPercentage} label="Uso del cupo" className="mt-3" />
        <div className="mt-2 flex justify-between gap-3 text-xs text-slate-500 tabular-nums">
          <span>Usado {formatCurrency(card.usedCredit, card.currency)}</span>
          <span>Cupo {formatCurrency(card.creditLimit, card.currency)}</span>
        </div>

        <div className="mt-4 flex flex-wrap gap-2">
          <Button variant="secondary" size="sm" onClick={() => onEdit(card)} disabled={isPending}>
            <PencilIcon className="size-3.5" />
            Editar
          </Button>
          <Button variant="secondary" size="sm" onClick={() => onToggleBlock(card)} isLoading={isPending}>
            {!isPending && (isBlocked ? <UnlockIcon className="size-3.5" /> : <LockIcon className="size-3.5" />)}
            {isBlocked ? 'Desbloquear' : 'Bloquear'}
          </Button>
          <Button
            variant="ghost"
            size="sm"
            onClick={() => onDelete(card)}
            disabled={isPending}
            className="ml-auto text-rose-300 hover:bg-rose-500/10 hover:text-rose-200"
            aria-label={`Eliminar tarjeta terminada en ${card.last4Digits}`}
          >
            <TrashIcon className="size-3.5" />
          </Button>
        </div>
      </div>
    </article>
  )
}

function AddCardTile({ onAdd }: { onAdd: () => void }) {
  return (
    <button
      type="button"
      onClick={onAdd}
      className="flex aspect-[1.586] w-full flex-col items-center justify-center gap-3 rounded-2xl border-2 border-dashed border-white/10 text-slate-400 transition hover:border-indigo-400/60 hover:bg-indigo-500/5 hover:text-indigo-300 focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-indigo-400"
    >
      <span className="flex size-12 items-center justify-center rounded-full bg-white/5">
        <PlusIcon />
      </span>
      <span className="text-sm font-semibold">Agregar tarjeta</span>
    </button>
  )
}

interface CardListProps extends CardHandlers {
  cards: Card[]
  selectedCardId: string | null
  pendingCardId: string | null
  onAdd: () => void
}

export function CardList({
  cards,
  selectedCardId,
  pendingCardId,
  onSelect,
  onAdd,
  onEdit,
  onToggleBlock,
  onDelete,
}: CardListProps) {
  return (
    <ul className="grid gap-6 sm:grid-cols-2 xl:grid-cols-3">
      {cards.map((card) => (
        <li key={card.id}>
          <CardTile
            card={card}
            isSelected={card.id === selectedCardId}
            isPending={card.id === pendingCardId}
            onSelect={onSelect}
            onEdit={onEdit}
            onToggleBlock={onToggleBlock}
            onDelete={onDelete}
          />
        </li>
      ))}
      <li>
        <AddCardTile onAdd={onAdd} />
      </li>
    </ul>
  )
}
