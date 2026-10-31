import { Spinner } from './Spinner'

export function FullPageSpinner({ label }: { label: string }) {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-3 text-slate-400">
      <Spinner className="size-8 text-indigo-400" label={label} />
      <p className="text-sm">{label}</p>
    </div>
  )
}
