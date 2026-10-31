import { useState, type ChangeEvent, type FormEvent } from 'react'
import { Alert } from '../../../shared/components/Alert'
import { Button } from '../../../shared/components/Button'
import { SelectField, TextField } from '../../../shared/components/FormField'
import { Panel } from '../../../shared/components/Panel'
import type { Card, Payment } from '../../../shared/types/contracts'
import { getErrorMessage } from '../../../shared/utils/errors'
import { formatCurrency } from '../../../shared/utils/formatters'
import { useProcessPayment } from '../hooks/useProcessPayment'

const TWO_DECIMALS = /^\d+(\.\d{1,2})?$/

interface PaymentFormValues {
  cardId: string
  amount: string
  merchant: string
}

type PaymentFormField = keyof PaymentFormValues
type PaymentFormErrors = Partial<Record<PaymentFormField, string>>

type PaymentResult =
  | { type: 'approved' | 'declined'; payment: Payment }
  | { type: 'error'; message: string }

function isUsable(card: Card): boolean {
  return card.status === 'active' && !card.isExpired
}

function cardOptionLabel(card: Card): string {
  const name = card.nickname || card.brandName
  const suffix = card.status === 'blocked' ? ' · bloqueada' : card.isExpired ? ' · vencida' : ''

  return `${name} •••• ${card.last4Digits}${suffix}`
}

function validate({ cardId, amount, merchant }: PaymentFormValues): PaymentFormErrors {
  const errors: PaymentFormErrors = {}

  if (!cardId) {
    errors.cardId = 'Selecciona una tarjeta.'
  }

  if (!TWO_DECIMALS.test(amount) || Number(amount) <= 0) {
    errors.amount = 'Ingresa un monto mayor a 0 con hasta 2 decimales.'
  }

  if (merchant.trim().length < 2) {
    errors.merchant = 'Ingresa el comercio (mínimo 2 caracteres).'
  }

  return errors
}

interface PaymentFormProps {
  cards: Card[]
  defaultCardId?: string | null
  onProcessed?: (payment: Payment) => void
}

export function PaymentForm({ cards, defaultCardId, onProcessed }: PaymentFormProps) {
  const { processPayment, isSubmitting } = useProcessPayment()
  const initialCard = cards.find((card) => card.id === defaultCardId && isUsable(card)) ?? cards.find(isUsable)
  const [values, setValues] = useState<PaymentFormValues>({ cardId: initialCard?.id ?? '', amount: '', merchant: '' })
  const [errors, setErrors] = useState<PaymentFormErrors>({})
  const [result, setResult] = useState<PaymentResult | null>(null)

  const selectedCard = cards.find((card) => card.id === values.cardId)
  const exceedsAvailable = selectedCard !== undefined && Number(values.amount) > selectedCard.availableCredit

  const updateField = (field: PaymentFormField) => (event: ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
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

    setResult(null)

    try {
      const payment = await processPayment({
        cardId: values.cardId,
        amount: Number(values.amount),
        merchant: values.merchant.trim(),
      })

      setResult({ type: payment.status, payment })

      if (payment.status === 'approved') {
        setValues((current) => ({ ...current, amount: '', merchant: '' }))
      }

      onProcessed?.(payment)
    } catch (error) {
      setResult({ type: 'error', message: getErrorMessage(error, 'No fue posible procesar el pago.') })
    }
  }

  return (
    <Panel title="Realizar pago" description="El monto se descuenta del cupo disponible de la tarjeta.">
      {cards.length === 0 ? (
        <Alert tone="info">Agrega una tarjeta para poder realizar pagos.</Alert>
      ) : (
        <form onSubmit={handleSubmit} noValidate className="space-y-4">
          <SelectField label="Tarjeta" value={values.cardId} onChange={updateField('cardId')} error={errors.cardId}>
            <option value="">Selecciona una tarjeta</option>
            {cards.map((card) => (
              <option key={card.id} value={card.id} disabled={!isUsable(card)}>
                {cardOptionLabel(card)}
              </option>
            ))}
          </SelectField>

          {selectedCard && (
            <p className="-mt-2 text-xs text-slate-400">
              Disponible:{' '}
              <span className="font-semibold text-slate-200 tabular-nums">
                {formatCurrency(selectedCard.availableCredit, selectedCard.currency)}
              </span>
            </p>
          )}

          <TextField
            label="Monto"
            type="number"
            inputMode="decimal"
            min="0.01"
            step="0.01"
            placeholder="0,00"
            value={values.amount}
            onChange={updateField('amount')}
            error={errors.amount}
            hint={exceedsAvailable ? 'El monto supera el cupo disponible: el emisor rechazará el pago.' : undefined}
          />

          <TextField
            label="Comercio"
            placeholder="Ej: Supermercado"
            maxLength={80}
            value={values.merchant}
            onChange={updateField('merchant')}
            error={errors.merchant}
          />

          {result?.type === 'approved' && (
            <Alert tone="success" title="Pago aprobado" onDismiss={() => setResult(null)}>
              {formatCurrency(result.payment.amount, result.payment.currency)} en {result.payment.merchant} con{' '}
              {result.payment.cardLabel}.
            </Alert>
          )}

          {result?.type === 'declined' && (
            <Alert tone="error" title="Pago rechazado" onDismiss={() => setResult(null)}>
              {result.payment.declineReason}
            </Alert>
          )}

          {result?.type === 'error' && (
            <Alert tone="error" onDismiss={() => setResult(null)}>
              {result.message}
            </Alert>
          )}

          <Button type="submit" size="lg" className="w-full" isLoading={isSubmitting}>
            Pagar
          </Button>
        </form>
      )}
    </Panel>
  )
}
