import { Link } from 'react-router-dom'
import { Logo } from '../shared/components/Logo'

export function NotFoundPage() {
  return (
    <main className="flex min-h-screen flex-col items-center justify-center gap-6 px-4 text-center">
      <Logo />
      <p className="text-7xl font-black text-indigo-400/80">404</p>
      <div>
        <h1 className="text-xl font-semibold text-white">Página no encontrada</h1>
        <p className="mt-2 text-sm text-slate-400">La dirección que buscas no existe o fue movida.</p>
      </div>
      <Link
        to="/"
        className="rounded-xl bg-indigo-500 px-4 py-2.5 text-sm font-semibold text-white transition hover:bg-indigo-400"
      >
        Volver al inicio
      </Link>
    </main>
  )
}
