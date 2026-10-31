import { Alert } from '../../../shared/components/Alert'
import { Button } from '../../../shared/components/Button'
import { ArrowUpRightIcon, ChevronLeftIcon, ChevronRightIcon, CloseIcon, XCircleIcon } from '../../../shared/components/icons'
import { Panel } from '../../../shared/components/Panel'
import { Spinner } from '../../../shared/components/Spinner'
import type { Payment } from '../../../shared/types/contracts'
import { cn } from '../../../shared/utils/cn'
import { formatCurrency, formatDateTime } from '../../../shared/utils/formatters'
import { usePaymentHistory } from '../hooks/usePaymentHistory'
import { PaymentStatusBadge } from './PaymentStatusBadge'

function PaymentRow({ payment }: { payment: Payment }) {
  const isApproved = payment.status === 'approved'
  const StatusIcon = isApproved ? ArrowUpRightIcon : XCircleIcon

  return (
    <li className="flex items-center gap-4 py-3.5">
      <span
        className={cn(
          'flex size-10 shrink-0 items-center justify-center rounded-xl',
          isApproved ? 'bg-indigo-500/10 text-indigo-300' : 'bg-rose-500/10 text-rose-300',
        )}
      >
        <StatusIcon className="size-5" />
      </span>
      <div className="min-w-0 flex-1">
        <p className="truncate text-sm font-medium text-white">{payment.merchant}</p>
        <p className="truncate text-xs text-slate-500">
          {payment.cardLabel} · {formatDateTime(payment.createdAt)}
        </p>
        {!isApproved && payment.declineReason && (
          <p className="mt-0.5 truncate text-xs text-rose-300/80">{payment.declineReason}</p>
        )}
      </div>
      <div className="flex shrink-0 flex-col items-end gap-1">
        <span className={cn('text-sm font-semibold tabular-nums', isApproved ? 'text-white' : 'text-slate-500 line-through')}>
          -{formatCurrency(payment.amount, payment.currency)}
        </span>
        <PaymentStatusBadge status={payment.status} />
      </div>
    </li>
  )
}

function HistorySkeleton() {
  return (
    <ul className="divide-y divide-white/5" aria-hidden="true">
      {Array.from({ length: 4 }, (_, index) => (
        <li key={index} className="flex animate-pulse items-center gap-4 py-3.5">
          <span className="size-10 rounded-xl bg-white/5" />
          <span className="flex-1 space-y-2">
            <span className="block h-3 w-1/3 rounded bg-white/10" />
            <span className="block h-2.5 w-1/2 rounded bg-white/5" />
          </span>
          <span className="h-3 w-16 rounded bg-white/10" />
        </li>
      ))}
    </ul>
  )
}

interface PaymentHistoryProps {
  cardId?: string | null
  filterLabel?: string | null
  refreshKey?: number
  onClearFilter?: () => void
}

export function PaymentHistory({ cardId = null, filterLabel, refreshKey, onClearFilter }: PaymentHistoryProps) {
  const { data, error, isLoading, page, setPage } = usePaymentHistory({ cardId, refreshKey })
  const payments = data?.items ?? []

  return (
    <Panel
      title="Historial de pagos"
      description={filterLabel ? `Mostrando pagos de ${filterLabel}` : 'Todos los pagos de tus tarjetas'}
      actions={
        <div className="flex items-center gap-2">
          {isLoading && data && <Spinner className="size-4 text-slate-400" label="Actualizando historial" />}
          {filterLabel && (
            <Button variant="secondary" size="sm" onClick={onClearFilter}>
              <CloseIcon className="size-3.5" />
              Ver todos
            </Button>
          )}
        </div>
      }
    >
      {error && (
        <Alert tone="error" className="mb-4">
          {error}
        </Alert>
      )}

      {!data && isLoading ? (
        <HistorySkeleton />
      ) : payments.length === 0 ? (
        <div className="rounded-2xl border border-dashed border-white/10 px-6 py-10 text-center text-sm text-slate-400">
          Aún no hay pagos registrados.
        </div>
      ) : (
        <ul className={cn('divide-y divide-white/5 transition-opacity', isLoading && 'opacity-60')}>
          {payments.map((payment) => (
            <PaymentRow key={payment.id} payment={payment} />
          ))}
        </ul>
      )}

      {data && data.totalPages > 1 && (
        <footer className="mt-4 flex items-center justify-between gap-3 border-t border-white/5 pt-4 text-xs text-slate-400">
          <span>
            Página {data.page} de {data.totalPages} · {data.totalCount} pagos
          </span>
          <div className="flex gap-2">
            <Button
              variant="secondary"
              size="sm"
              onClick={() => setPage(page - 1)}
              disabled={!data.hasPreviousPage || isLoading}
              aria-label="Página anterior"
            >
              <ChevronLeftIcon className="size-4" />
            </Button>
            <Button
              variant="secondary"
              size="sm"
              onClick={() => setPage(page + 1)}
              disabled={!data.hasNextPage || isLoading}
              aria-label="Página siguiente"
            >
              <ChevronRightIcon className="size-4" />
            </Button>
          </div>
        </footer>
      )}
    </Panel>
  )
}
