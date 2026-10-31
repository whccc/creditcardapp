import { useCallback, useState } from 'react'
import type { CreatePaymentRequest, Payment } from '../../../shared/types/contracts'
import { paymentsService } from '../services/paymentsService'

export function useProcessPayment() {
  const [isSubmitting, setIsSubmitting] = useState(false)

  const processPayment = useCallback(async (payload: CreatePaymentRequest): Promise<Payment> => {
    setIsSubmitting(true)

    try {
      return await paymentsService.create(payload)
    } finally {
      setIsSubmitting(false)
    }
  }, [])

  return { processPayment, isSubmitting }
}
