import { ProgressBar } from '../../../shared/components/ProgressBar'
import type { CreditSummary as CreditSummaryData } from '../../../shared/types/contracts'
import { cn } from '../../../shared/utils/cn'
import { formatCurrency, formatPercentage } from '../../../shared/utils/formatters'

interface StatTileProps {
  label: string
  value: string
  detail?: string
  accent: string
}

function StatTile({ label, value, detail, accent }: StatTileProps) {
  return (
    <div className="relative overflow-hidden rounded-2xl bg-slate-900/60 p-5 ring-1 ring-white/5">
      <span aria-hidden="true" className={cn('absolute -top-10 -right-10 size-28 rounded-full blur-2xl', accent)} />
      <p className="relative text-xs tracking-wide text-slate-400 uppercase">{label}</p>
      <p className="relative mt-2 text-2xl font-semibold text-white tabular-nums">{value}</p>
      {detail && <p className="relative mt-1 text-xs text-slate-500">{detail}</p>}
    </div>
  )
}

export function CreditSummary({ summary }: { summary: CreditSummaryData }) {
  const { currency } = summary

  return (
    <section aria-label="Resumen de crédito" className="space-y-4">
      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <StatTile
          label="Cupo total"
          value={formatCurrency(summary.totalCreditLimit, currency)}
          detail={`${summary.totalCards} ${summary.totalCards === 1 ? 'tarjeta' : 'tarjetas'}`}
          accent="bg-indigo-500/20"
        />
        <StatTile
          label="Disponible"
          value={formatCurrency(summary.totalAvailableCredit, currency)}
          detail="Listo para usar"
          accent="bg-emerald-500/20"
        />
        <StatTile
          label="Utilizado"
          value={formatCurrency(summary.totalUsedCredit, currency)}
          detail={`${formatPercentage(summary.utilizationPercentage)} del cupo total`}
          accent="bg-amber-500/20"
        />
        <StatTile
          label="Tarjetas activas"
          value={`${summary.activeCards} / ${summary.totalCards}`}
          detail={summary.blockedCards > 0 ? `${summary.blockedCards} bloqueada(s)` : 'Sin bloqueos'}
          accent="bg-sky-500/20"
        />
      </div>

      <div className="rounded-2xl bg-slate-900/60 p-5 ring-1 ring-white/5">
        <div className="flex items-baseline justify-between text-sm">
          <span className="text-slate-400">Uso global del crédito</span>
          <span className="font-semibold text-white tabular-nums">{formatPercentage(summary.utilizationPercentage)}</span>
        </div>
        <ProgressBar value={summary.utilizationPercentage} label="Uso global del crédito" className="mt-3 h-2.5" />
      </div>
    </section>
  )
}
