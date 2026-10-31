import { cn } from '../utils/cn'

export function Logo({ className }: { className?: string }) {
  return (
    <span className={cn('inline-flex items-center gap-2.5 font-semibold tracking-tight text-white', className)}>
      <span className="relative flex size-9 items-center justify-center overflow-hidden rounded-xl bg-linear-to-br from-indigo-400 to-violet-700 shadow-lg shadow-indigo-500/30">
        <span className="absolute top-2.5 left-2 h-2 w-3 rounded-sm bg-amber-300" />
        <span className="absolute right-2 bottom-2.5 left-2 h-1 rounded-full bg-white/70" />
      </span>
      <span>
        CreditCard<span className="text-indigo-400">App</span>
      </span>
    </span>
  )
}
