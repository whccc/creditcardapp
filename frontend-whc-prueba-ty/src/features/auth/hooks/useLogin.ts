import { useCallback, useState } from 'react'
import { useAuth } from '../../../context/AuthContext'
import type { LoginRequest } from '../../../shared/types/contracts'
import { getErrorMessage } from '../../../shared/utils/errors'
import { authService } from '../services/authService'

export function useLogin() {
  const { login } = useAuth()
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const submit = useCallback(
    async (credentials: LoginRequest): Promise<boolean> => {
      setIsSubmitting(true)
      setError(null)

      try {
        const response = await authService.login(credentials)
        login(response)
        return true
      } catch (requestError) {
        setError(getErrorMessage(requestError, 'No fue posible iniciar sesión.'))
        return false
      } finally {
        setIsSubmitting(false)
      }
    },
    [login],
  )

  return { submit, isSubmitting, error }
}
