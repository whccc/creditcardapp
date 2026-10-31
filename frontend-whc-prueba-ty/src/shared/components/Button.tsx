import type { ButtonHTMLAttributes } from 'react'
import { cn } from '../utils/cn'
import { Spinner } from './Spinner'

export type ButtonVariant = 'primary' | 'secondary' | 'danger' | 'ghost'
export type ButtonSize = 'sm' | 'md' | 'lg'

const VARIANTS: Record<ButtonVariant, string> = {
  primary:
    'bg-indigo-500 text-white shadow-lg shadow-indigo-500/25 hover:bg-indigo-400 focus-visible:outline-indigo-400',
  secondary: 'bg-white/5 text-slate-100 ring-1 ring-inset ring-white/10 hover:bg-white/10 focus-visible:outline-slate-400',
  danger: 'bg-rose-500 text-white shadow-lg shadow-rose-500/20 hover:bg-rose-400 focus-visible:outline-rose-400',
  ghost: 'text-slate-300 hover:bg-white/5 hover:text-white focus-visible:outline-slate-400',
}

const SIZES: Record<ButtonSize, string> = {
  sm: 'h-8 gap-1.5 rounded-lg px-3 text-xs',
  md: 'h-10 gap-2 rounded-xl px-4 text-sm',
  lg: 'h-12 gap-2 rounded-xl px-5 text-sm',
}

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant
  size?: ButtonSize
  isLoading?: boolean
}

export function Button({
  variant = 'primary',
  size = 'md',
  type = 'button',
  isLoading = false,
  disabled = false,
  className,
  children,
  ...props
}: ButtonProps) {
  return (
    <button
      type={type}
      disabled={disabled || isLoading}
      aria-busy={isLoading || undefined}
      className={cn(
        'inline-flex items-center justify-center font-semibold whitespace-nowrap transition focus-visible:outline-2 focus-visible:outline-offset-2 disabled:cursor-not-allowed disabled:opacity-60',
        VARIANTS[variant],
        SIZES[size],
        className,
      )}
      {...props}
    >
      {isLoading && <Spinner className="size-4" />}
      {children}
    </button>
  )
}
