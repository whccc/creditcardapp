import { Outlet } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { Button } from '../shared/components/Button'
import { LogoutIcon } from '../shared/components/icons'
import { Logo } from '../shared/components/Logo'

export function AppLayout() {
  const { user, logout } = useAuth()

  return (
    <div className="relative min-h-screen">
      <div
        aria-hidden="true"
        className="pointer-events-none fixed inset-x-0 top-0 -z-10 h-[520px] bg-[radial-gradient(ellipse_at_top,rgba(99,102,241,0.18),transparent_65%)]"
      />

      <header className="sticky top-0 z-40 border-b border-white/5 bg-slate-950/70 backdrop-blur-xl">
        <div className="mx-auto flex h-16 max-w-7xl items-center justify-between gap-4 px-4 sm:px-6 lg:px-8">
          <Logo />

          <div className="flex items-center gap-3">
            <div className="hidden text-right sm:block">
              <p className="text-sm font-medium text-white">{user?.fullName}</p>
              <p className="text-xs text-slate-400">{user?.email}</p>
            </div>
            <span
              aria-hidden="true"
              className="flex size-9 items-center justify-center rounded-full bg-linear-to-br from-indigo-400 to-violet-600 text-sm font-semibold text-white shadow-lg shadow-indigo-500/20"
            >
              {user?.initials}
            </span>
            <Button variant="ghost" size="sm" onClick={logout} aria-label="Cerrar sesión">
              <LogoutIcon className="size-4" />
              <span className="hidden sm:inline">Salir</span>
            </Button>
          </div>
        </div>
      </header>

      <main className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8 lg:py-10">
        <Outlet />
      </main>
    </div>
  )
}
