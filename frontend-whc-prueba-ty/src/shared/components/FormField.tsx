import { useId, type InputHTMLAttributes, type ReactNode, type SelectHTMLAttributes } from 'react'
import { cn } from '../utils/cn'

const CONTROL_CLASSES =
  'block h-11 w-full rounded-xl border-0 bg-slate-900/80 px-3.5 text-sm text-white ring-1 ring-inset placeholder:text-slate-500 transition focus:ring-2 focus:ring-inset focus:outline-none disabled:cursor-not-allowed disabled:opacity-60'

function controlClasses(hasError: boolean, className?: string | false): string {
  return cn(
    CONTROL_CLASSES,
    hasError ? 'ring-rose-500/70 focus:ring-rose-400' : 'ring-white/10 focus:ring-indigo-400',
    className,
  )
}

function describedBy(id: string, error?: string, hint?: string): string | undefined {
  if (error) {
    return `${id}-error`
  }

  return hint ? `${id}-hint` : undefined
}

interface FieldShellProps {
  id: string
  label: string
  error?: string | undefined
  hint?: string | undefined
  className?: string | undefined
  trailing?: ReactNode
  children: ReactNode
}

function FieldShell({ id, label, error, hint, className, trailing, children }: FieldShellProps) {
  return (
    <div className={className}>
      <label htmlFor={id} className="mb-1.5 block text-xs font-medium tracking-wide text-slate-400 uppercase">
        {label}
      </label>
      <div className="relative">
        {children}
        {trailing && <div className="absolute inset-y-0 right-0 flex items-center pr-2">{trailing}</div>}
      </div>
      {error ? (
        <p id={`${id}-error`} className="mt-1.5 text-xs text-rose-400">
          {error}
        </p>
      ) : (
        hint && (
          <p id={`${id}-hint`} className="mt-1.5 text-xs text-slate-500">
            {hint}
          </p>
        )
      )}
    </div>
  )
}

interface BaseFieldProps {
  label: string
  error?: string | undefined
  hint?: string | undefined
  className?: string
}

interface TextFieldProps extends BaseFieldProps, Omit<InputHTMLAttributes<HTMLInputElement>, 'className'> {
  inputClassName?: string
  trailing?: ReactNode
}

export function TextField({ id, label, error, hint, className, inputClassName, trailing, ...props }: TextFieldProps) {
  const generatedId = useId()
  const fieldId = id ?? generatedId

  return (
    <FieldShell id={fieldId} label={label} error={error} hint={hint} className={className} trailing={trailing}>
      <input
        id={fieldId}
        aria-invalid={Boolean(error)}
        aria-describedby={describedBy(fieldId, error, hint)}
        className={controlClasses(Boolean(error), cn(Boolean(trailing) && 'pr-11', inputClassName))}
        {...props}
      />
    </FieldShell>
  )
}

interface SelectFieldProps extends BaseFieldProps, Omit<SelectHTMLAttributes<HTMLSelectElement>, 'className'> {
  isInvalid?: boolean
}

export function SelectField({
  id,
  label,
  error,
  hint,
  isInvalid = false,
  className,
  children,
  ...props
}: SelectFieldProps) {
  const generatedId = useId()
  const fieldId = id ?? generatedId
  const hasError = Boolean(error) || isInvalid

  return (
    <FieldShell id={fieldId} label={label} error={error} hint={hint} className={className}>
      <select
        id={fieldId}
        aria-invalid={hasError}
        aria-describedby={describedBy(fieldId, error, hint)}
        className={controlClasses(hasError, 'appearance-none pr-9')}
        {...props}
      >
        {children}
      </select>
      <svg
        className="pointer-events-none absolute top-1/2 right-3 size-4 -translate-y-1/2 text-slate-500"
        viewBox="0 0 20 20"
        fill="currentColor"
        aria-hidden="true"
      >
        <path d="M5.2 7.2a.75.75 0 0 1 1.06 0L10 10.94l3.74-3.74a.75.75 0 1 1 1.06 1.06l-4.27 4.27a.75.75 0 0 1-1.06 0L5.2 8.26a.75.75 0 0 1 0-1.06Z" />
      </svg>
    </FieldShell>
  )
}
