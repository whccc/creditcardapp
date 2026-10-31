import { useLocation, useNavigate } from 'react-router-dom'
import { LoginForm } from '../features/auth'
import { CreditCard } from '../features/cards'
import type { RedirectState } from '../routes/ProtectedRoute'
import { Logo } from '../shared/components/Logo'
import type { LoginRequest } from '../shared/types/contracts'
import { toSafeRedirectPath } from '../shared/utils/navigation'

const DEMO_CREDENTIALS: LoginRequest | null = import.meta.env.DEV
  ? { email: 'demo@creditcard.app', password: 'Demo1234!' }
  : null

export function LoginPage() {
  const navigate = useNavigate()
  const location = useLocation()
  const redirectTo = toSafeRedirectPath((location.state as RedirectState | null)?.from)

  return (
    <div className="grid min-h-screen lg:grid-cols-2">
      <aside className="relative hidden overflow-hidden bg-linear-to-br from-indigo-950 via-slate-950 to-slate-950 p-12 lg:flex lg:flex-col lg:justify-between">
        <div aria-hidden="true" className="absolute -top-40 -left-40 size-[520px] rounded-full bg-indigo-600/25 blur-3xl" />
        <div aria-hidden="true" className="absolute -right-32 bottom-0 size-[420px] rounded-full bg-violet-600/20 blur-3xl" />

        <Logo className="relative text-lg" />

        <div className="relative mx-auto w-full max-w-md py-16" aria-hidden="true">
          <CreditCard
            brand="amex"
            maskedNumber="•••• •••••• •1005"
            holderName="USUARIO DEMO"
            expiry="03/30"
            nickname="Viajes"
            className="absolute inset-x-10 -top-2 -rotate-6 opacity-70"
          />
          <CreditCard
            brand="visa"
            maskedNumber="•••• •••• •••• 1111"
            holderName="USUARIO DEMO"
            expiry="08/29"
            nickname="Tarjeta principal"
            className="relative translate-y-14 rotate-3"
          />
        </div>

        <div className="relative max-w-md">
          <h2 className="text-3xl font-bold tracking-tight text-white">Tus tarjetas, bajo control.</h2>
          <p className="mt-3 text-slate-400">
            Administra tus tarjetas de crédito, realiza pagos y revisa tu historial desde un solo lugar.
          </p>
        </div>
      </aside>

      <main className="flex items-center justify-center px-4 py-12 sm:px-8">
        <div className="w-full max-w-sm">
          <Logo className="mb-10 lg:hidden" />
          <h1 className="text-2xl font-bold tracking-tight text-white">Inicia sesión</h1>
          <p className="mt-2 mb-8 text-sm text-slate-400">Ingresa con tu correo y contraseña para continuar.</p>
          <LoginForm demoCredentials={DEMO_CREDENTIALS} onSuccess={() => navigate(redirectTo, { replace: true })} />
        </div>
      </main>
    </div>
  )
}
