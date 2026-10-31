import { isAxiosError } from 'axios'
import type { ProblemDetails } from '../types/contracts'

const DEFAULT_MESSAGE = 'Ocurrió un error inesperado. Intenta nuevamente.'

const STATUS_MESSAGES: Partial<Record<number, string>> = {
  401: 'Tu sesión expiró. Vuelve a iniciar sesión.',
  403: 'No tienes permisos para realizar esta acción.',
  404: 'El recurso solicitado no existe.',
  429: 'Demasiadas solicitudes. Espera un momento e inténtalo nuevamente.',
  500: 'El servidor no pudo procesar la solicitud.',
}

export function getErrorMessage(error: unknown, fallback = DEFAULT_MESSAGE): string {
  if (!isAxiosError<ProblemDetails>(error)) {
    return fallback
  }

  const { response } = error

  if (!response) {
    return error.code === 'ECONNABORTED'
      ? 'El servidor tardó demasiado en responder.'
      : 'No fue posible conectar con el servidor. Verifica que el BFF esté en ejecución.'
  }

  return response.data?.detail ?? STATUS_MESSAGES[response.status] ?? fallback
}
