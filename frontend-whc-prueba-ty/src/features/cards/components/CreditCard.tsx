import { ContactlessIcon, LockIcon } from '../../../shared/components/icons'
import type { CardStatus } from '../../../shared/types/contracts'
import { cn } from '../../../shared/utils/cn'
import type { DisplayBrand } from '../utils/cardNumber'
import { BrandLogo } from './BrandLogo'
import { CardChip } from './CardChip'

const BRAND_THEMES: Record<DisplayBrand, string> = {
  visa: 'from-indigo-500 via-blue-700 to-slate-900',
  mastercard: 'from-orange-500 via-rose-700 to-slate-950',
  amex: 'from-teal-400 via-cyan-700 to-slate-900',
  unknown: 'from-slate-600 via-slate-800 to-slate-950',
}

export interface CreditCardProps {
  brand?: DisplayBrand
  maskedNumber: string
  holderName?: string
  expiry?: string
  nickname?: string | null
  status?: CardStatus
  isExpired?: boolean
  isSelected?: boolean
  onSelect?: () => void
  className?: string
}

function CardFace({
  brand,
  maskedNumber,
  holderName,
  expiry,
  nickname,
  statusLabel,
}: Required<Pick<CreditCardProps, 'brand' | 'maskedNumber'>> &
  Pick<CreditCardProps, 'holderName' | 'expiry' | 'nickname'> & { statusLabel: string | null }) {
  return (
    <>
      <span aria-hidden="true" className="pointer-events-none absolute -top-24 -right-16 size-64 rounded-full bg-white/15 blur-2xl" />
      <span aria-hidden="true" className="pointer-events-none absolute -bottom-28 -left-12 size-64 rounded-full bg-black/35 blur-2xl" />
      <span
        aria-hidden="true"
        className="pointer-events-none absolute inset-0 bg-[radial-gradient(circle_at_20%_0%,rgba(255,255,255,0.22),transparent_55%)]"
      />

      <span className="relative flex h-full flex-col justify-between">
        <span className="flex items-start justify-between gap-3">
          <span className="truncate text-sm font-medium text-white/85">{nickname || 'Tarjeta de crédito'}</span>
          <BrandLogo brand={brand} />
        </span>

        <span className="flex items-center gap-3">
          <CardChip />
          <ContactlessIcon className="size-6 text-white/70" />
        </span>

        <span className="block font-mono text-lg tracking-[0.16em] text-white [text-shadow:0_1px_2px_rgb(0_0_0/0.45)] sm:text-xl">
          {maskedNumber}
        </span>

        <span className="flex items-end justify-between gap-4 text-[10px] tracking-widest text-white/60 uppercase">
          <span className="min-w-0">
            <span className="block">Titular</span>
            <span className="block truncate text-sm font-semibold tracking-wider text-white">
              {holderName || 'NOMBRE APELLIDO'}
            </span>
          </span>
          <span className="shrink-0 text-right">
            <span className="block">Vence</span>
            <span className="block font-mono text-sm font-semibold text-white">{expiry || 'MM/AA'}</span>
          </span>
        </span>
      </span>

      {statusLabel && (
        <span className="absolute top-1/2 right-5 inline-flex -translate-y-1/2 items-center gap-1.5 rounded-full bg-black/60 px-3 py-1 text-xs font-semibold text-white ring-1 ring-white/20 backdrop-blur">
          <LockIcon className="size-3.5" />
          {statusLabel}
        </span>
      )}
    </>
  )
}

export function CreditCard({
  brand = 'unknown',
  maskedNumber,
  holderName,
  expiry,
  nickname,
  status = 'active',
  isExpired = false,
  isSelected = false,
  onSelect,
  className,
}: CreditCardProps) {
  const statusLabel = status === 'blocked' ? 'Bloqueada' : isExpired ? 'Vencida' : null
  const classes = cn(
    'group relative isolate block aspect-[1.586] w-full overflow-hidden rounded-2xl bg-linear-to-br p-5 text-left text-white shadow-2xl ring-1 shadow-black/40 transition duration-300 select-none',
    BRAND_THEMES[brand],
    isSelected ? 'ring-2 ring-white/80' : 'ring-white/15',
    Boolean(statusLabel) && 'saturate-[.35]',
    className,
  )
  const face = (
    <CardFace
      brand={brand}
      maskedNumber={maskedNumber}
      holderName={holderName}
      expiry={expiry}
      nickname={nickname}
      statusLabel={statusLabel}
    />
  )

  if (!onSelect) {
    return <div className={classes}>{face}</div>
  }

  return (
    <button
      type="button"
      onClick={onSelect}
      aria-pressed={isSelected}
      aria-label={`${nickname || 'Tarjeta'} terminada en ${maskedNumber.slice(-4)}`}
      className={cn(
        classes,
        'cursor-pointer hover:-translate-y-1 hover:shadow-indigo-950/60 focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-indigo-400',
      )}
    >
      {face}
    </button>
  )
}
