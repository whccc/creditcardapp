export function CardChip() {
  return (
    <div
      aria-hidden="true"
      className="relative h-8 w-11 overflow-hidden rounded-md bg-linear-to-br from-amber-100 via-yellow-400 to-amber-600 shadow-inner ring-1 ring-black/20"
    >
      <span className="absolute inset-y-0 left-1/3 w-px bg-black/25" />
      <span className="absolute inset-y-0 right-1/3 w-px bg-black/25" />
      <span className="absolute inset-x-0 top-1/2 h-px bg-black/25" />
      <span className="absolute top-1/2 left-1/2 h-3.5 w-4 -translate-x-1/2 -translate-y-1/2 rounded-sm border border-black/25 bg-yellow-300/60" />
    </div>
  )
}
