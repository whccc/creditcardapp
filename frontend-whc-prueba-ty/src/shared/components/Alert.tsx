import type { ComponentType, ReactNode } from 'react'
import { cn } from '../utils/cn'
import { AlertTriangleIcon, CheckCircleIcon, CloseIcon, InfoIcon, XCircleIcon, type IconProps } from './icons'

export type AlertTone = 'success' | 'error' | 'warning' | 'info'

const TONES: Record<AlertTone, { classes: string; Icon: ComponentType<IconProps> }> = {
  success: { classes: 'bg-emerald-500/10 text-emerald-200 ring-emerald-500/30', Icon: CheckCircleIcon },
  error: { classes: 'bg-rose-500/10 text-rose-200 ring-rose-500/30', Icon: XCircleIcon },
  warning: { classes: 'bg-amber-500/10 text-amber-200 ring-amber-500/30', Icon: AlertTriangleIcon },
  info: { classes: 'bg-sky-500/10 text-sky-200 ring-sky-500/30', Icon: InfoIcon },
}

interface AlertProps {
  tone?: AlertTone
  title?: string
  children?: ReactNode
  onDismiss?: () => void
  className?: string
}

export function Alert({ tone = 'info', title, children, onDismiss, className }: AlertProps) {
  const { classes, Icon } = TONES[tone]

  return (
    <div
      role={tone === 'error' ? 'alert' : 'status'}
      className={cn('flex items-start gap-3 rounded-xl px-4 py-3 text-sm ring-1 ring-inset', classes, className)}
    >
      <Icon className="mt-0.5 size-5 shrink-0" />
      <div className="min-w-0 flex-1">
        {title && <p className="font-semibold">{title}</p>}
        {children && <div className={cn(Boolean(title) && 'mt-0.5', 'opacity-90')}>{children}</div>}
      </div>
      {onDismiss && (
        <button
          type="button"
          onClick={onDismiss}
          className="-m-1 rounded-lg p-1 opacity-70 transition hover:bg-white/10 hover:opacity-100"
          aria-label="Cerrar aviso"
        >
          <CloseIcon className="size-4" />
        </button>
      )}
    </div>
  )
}
