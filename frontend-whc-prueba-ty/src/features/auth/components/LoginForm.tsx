import { useState, type ChangeEvent, type FormEvent } from 'react'
import { Alert } from '../../../shared/components/Alert'
import { Button } from '../../../shared/components/Button'
import { TextField } from '../../../shared/components/FormField'
import { EyeIcon, EyeOffIcon } from '../../../shared/components/icons'
import type { LoginRequest } from '../../../shared/types/contracts'
import { useLogin } from '../hooks/useLogin'

type LoginField = keyof LoginRequest
type LoginErrors = Partial<Record<LoginField, string>>

const EMAIL_PATTERN = /^[^@\s]+@[^@\s]+\.[^@\s]+$/

function validate({ email, password }: LoginRequest): LoginErrors {
  const errors: LoginErrors = {}

  if (!EMAIL_PATTERN.test(email.trim())) {
    errors.email = 'Ingresa un correo válido.'
  }

  if (!password) {
    errors.password = 'Ingresa tu contraseña.'
  }

  return errors
}

interface LoginFormProps {
  onSuccess?: () => void
  demoCredentials?: LoginRequest | null
}

export function LoginForm({ onSuccess, demoCredentials }: LoginFormProps) {
  const { submit, isSubmitting, error } = useLogin()
  const [values, setValues] = useState<LoginRequest>({ email: '', password: '' })
  const [errors, setErrors] = useState<LoginErrors>({})
  const [showPassword, setShowPassword] = useState(false)

  const updateField = (field: LoginField) => (event: ChangeEvent<HTMLInputElement>) => {
    setValues((current) => ({ ...current, [field]: event.target.value }))
    setErrors((current) => ({ ...current, [field]: undefined }))
  }

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    const validationErrors = validate(values)
    setErrors(validationErrors)

    if (Object.keys(validationErrors).length > 0) {
      return
    }

    if (await submit({ email: values.email.trim(), password: values.password })) {
      onSuccess?.()
    }
  }

  return (
    <form onSubmit={handleSubmit} noValidate className="space-y-5">
      <TextField
        label="Correo electrónico"
        type="email"
        autoComplete="email"
        placeholder="tu@correo.com"
        value={values.email}
        onChange={updateField('email')}
        error={errors.email}
        autoFocus
      />

      <TextField
        label="Contraseña"
        type={showPassword ? 'text' : 'password'}
        autoComplete="current-password"
        placeholder="••••••••"
        value={values.password}
        onChange={updateField('password')}
        error={errors.password}
        trailing={
          <button
            type="button"
            onClick={() => setShowPassword((visible) => !visible)}
            className="rounded-lg p-1.5 text-slate-400 transition hover:bg-white/5 hover:text-white"
            aria-label={showPassword ? 'Ocultar contraseña' : 'Mostrar contraseña'}
          >
            {showPassword ? <EyeOffIcon className="size-4" /> : <EyeIcon className="size-4" />}
          </button>
        }
      />

      {error && <Alert tone="error">{error}</Alert>}

      <Button type="submit" size="lg" className="w-full" isLoading={isSubmitting}>
        Iniciar sesión
      </Button>

      {demoCredentials && (
        <div className="rounded-2xl border border-dashed border-white/10 p-4 text-sm text-slate-400">
          <p className="font-medium text-slate-300">Cuenta de demostración</p>
          <p className="mt-1 font-mono text-xs">
            {demoCredentials.email} · {demoCredentials.password}
          </p>
          <button
            type="button"
            onClick={() => {
              setValues(demoCredentials)
              setErrors({})
            }}
            className="mt-3 text-xs font-semibold text-indigo-400 transition hover:text-indigo-300"
          >
            Usar credenciales demo
          </button>
        </div>
      )}
    </form>
  )
}
