import { cn } from '../utils/cn'

function toneFor(value: number): string {
  if (value >= 85) {
    return 'from-rose-500 to-orange-400'
  }

  if (value >= 60) {
    return 'from-amber-400 to-yellow-300'
  }

  return 'from-emerald-400 to-teal-300'
}

interface ProgressBarProps {
  value: number
  label: string
  className?: string
}

export function ProgressBar({ value, label, className }: ProgressBarProps) {
  const clamped = Math.min(Math.max(Number.isFinite(value) ? value : 0, 0), 100)

  return (
    <div
      role="progressbar"
      aria-label={label}
      aria-valuemin={0}
      aria-valuemax={100}
      aria-valuenow={clamped}
      className={cn('h-2 w-full overflow-hidden rounded-full bg-white/5', className)}
    >
      <div
        className={cn('h-full rounded-full bg-linear-to-r transition-[width] duration-700 ease-out', toneFor(clamped))}
        style={{ width: `${clamped}%` }}
      />
    </div>
  )
}
