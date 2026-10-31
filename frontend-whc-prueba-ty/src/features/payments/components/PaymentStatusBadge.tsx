import type { PaymentStatus } from '../../../shared/types/contracts'
import { cn } from '../../../shared/utils/cn'

const STATUS: Record<PaymentStatus, { label: string; classes: string }> = {
  approved: { label: 'Aprobado', classes: 'bg-emerald-500/10 text-emerald-300 ring-emerald-500/30' },
  declined: { label: 'Rechazado', classes: 'bg-rose-500/10 text-rose-300 ring-rose-500/30' },
}

export function PaymentStatusBadge({ status }: { status: PaymentStatus }) {
  const { label, classes } = STATUS[status]

  return (
    <span className={cn('inline-flex items-center rounded-full px-2 py-0.5 text-[11px] font-semibold ring-1 ring-inset', classes)}>
      {label}
    </span>
  )
}
