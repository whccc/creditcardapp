import { useEffect, useId, type ReactNode } from 'react'
import { createPortal } from 'react-dom'
import { cn } from '../utils/cn'
import { CloseIcon } from './icons'

export type ModalSize = 'sm' | 'md' | 'lg'

const SIZES: Record<ModalSize, string> = {
  sm: 'max-w-md',
  md: 'max-w-lg',
  lg: 'max-w-3xl',
}

interface ModalProps {
  title: string
  description?: string
  onClose: () => void
  size?: ModalSize
  children: ReactNode
}

export function Modal({ title, description, onClose, size = 'md', children }: ModalProps) {
  const titleId = useId()

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === 'Escape') {
        onClose()
      }
    }

    const previousOverflow = document.body.style.overflow
    document.body.style.overflow = 'hidden'
    document.addEventListener('keydown', handleKeyDown)

    return () => {
      document.body.style.overflow = previousOverflow
      document.removeEventListener('keydown', handleKeyDown)
    }
  }, [onClose])

  return createPortal(
    <div className="fixed inset-0 z-50 flex items-end justify-center p-0 sm:items-center sm:p-4">
      <div className="absolute inset-0 bg-slate-950/80 backdrop-blur-sm" onClick={onClose} aria-hidden="true" />
      <div
        role="dialog"
        aria-modal="true"
        aria-labelledby={titleId}
        className={cn(
          'relative max-h-[92vh] w-full overflow-y-auto rounded-t-3xl bg-slate-900 shadow-2xl ring-1 ring-white/10 sm:rounded-3xl',
          SIZES[size],
        )}
      >
        <header className="flex items-start justify-between gap-4 border-b border-white/5 px-6 py-5">
          <div>
            <h2 id={titleId} className="text-lg font-semibold text-white">
              {title}
            </h2>
            {description && <p className="mt-1 text-sm text-slate-400">{description}</p>}
          </div>
          <button
            type="button"
            onClick={onClose}
            className="-m-1.5 rounded-lg p-1.5 text-slate-400 transition hover:bg-white/5 hover:text-white"
            aria-label="Cerrar"
          >
            <CloseIcon />
          </button>
        </header>
        <div className="px-6 py-5">{children}</div>
      </div>
    </div>,
    document.body,
  )
}
