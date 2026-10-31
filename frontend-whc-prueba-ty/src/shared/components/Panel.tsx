import type { ReactNode } from 'react'
import { cn } from '../utils/cn'

interface PanelProps {
  title?: string
  description?: string
  actions?: ReactNode
  className?: string
  children: ReactNode
}

export function Panel({ title, description, actions, className, children }: PanelProps) {
  return (
    <section className={cn('rounded-3xl bg-slate-900/60 p-5 ring-1 ring-white/5 backdrop-blur sm:p-6', className)}>
      {(title || actions) && (
        <header className="mb-5 flex flex-wrap items-start justify-between gap-3">
          <div>
            {title && <h2 className="text-base font-semibold text-white">{title}</h2>}
            {description && <p className="mt-1 text-sm text-slate-400">{description}</p>}
          </div>
          {actions}
        </header>
      )}
      {children}
    </section>
  )
}
