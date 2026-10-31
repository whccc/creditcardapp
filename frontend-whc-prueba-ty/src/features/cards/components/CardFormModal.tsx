import { useState, type ChangeEvent, type FormEvent } from 'react'
import { Alert } from '../../../shared/components/Alert'
import { Button } from '../../../shared/components/Button'
import { SelectField, TextField } from '../../../shared/components/FormField'
import { Modal } from '../../../shared/components/Modal'
import type { Card, CreateCardRequest, UpdateCardRequest } from '../../../shared/types/contracts'
import { getErrorMessage } from '../../../shared/utils/errors'
import { formatCurrency } from '../../../shared/utils/formatters'
import {
  HOLDER_NAME_MAX_LENGTH,
  NICKNAME_MAX_LENGTH,
  toCreatePayload,
  toFormValues,
  toUpdatePayload,
  validateCardForm,
  type CardFormErrors,
  type CardFormField,
  type CardFormValues,
} from '../utils/cardForm'
import { detectBrand, formatCardNumber, previewCardNumber } from '../utils/cardNumber'
import { CreditCard } from './CreditCard'

const CURRENT_YEAR = new Date().getFullYear()
const MONTHS = Array.from({ length: 12 }, (_, index) => String(index + 1))
const YEARS = Array.from({ length: 11 }, (_, index) => String(CURRENT_YEAR + index))

function previewExpiry(month: string, year: string): string {
  if (!month || !year) {
    return ''
  }

  return `${month.padStart(2, '0')}/${year.slice(-2)}`
}

type CardFormModalProps =
  | { card: null; onClose: () => void; onSubmit: (payload: CreateCardRequest) => Promise<unknown> }
  | { card: Card; onClose: () => void; onSubmit: (payload: UpdateCardRequest) => Promise<unknown> }

export function CardFormModal(props: CardFormModalProps) {
  const { card, onClose } = props
  const [values, setValues] = useState<CardFormValues>(() => toFormValues(card))
  const [errors, setErrors] = useState<CardFormErrors>({})
  const [submitError, setSubmitError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  const updateField =
    (field: CardFormField, transform: (value: string) => string = (value) => value) =>
    (event: ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
      const nextValue = transform(event.target.value)
      setValues((current) => ({ ...current, [field]: nextValue }))
      setErrors((current) => ({
        ...current,
        [field]: undefined,
        ...(field.startsWith('expiry') && { expiry: undefined }),
      }))
    }

  const submitPayload = (): Promise<unknown> =>
    props.card === null ? props.onSubmit(toCreatePayload(values)) : props.onSubmit(toUpdatePayload(values))

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    const validationErrors = validateCardForm(values, { isEdit: card !== null })
    setErrors(validationErrors)

    if (Object.keys(validationErrors).length > 0) {
      return
    }

    setIsSubmitting(true)
    setSubmitError(null)

    try {
      await submitPayload()
      onClose()
    } catch (error) {
      setSubmitError(getErrorMessage(error, 'No fue posible guardar la tarjeta.'))
      setIsSubmitting(false)
    }
  }

  const preview = card
    ? { brand: card.brand, maskedNumber: card.maskedNumber, expiry: card.expiry }
    : {
        brand: detectBrand(values.cardNumber),
        maskedNumber: previewCardNumber(values.cardNumber),
        expiry: previewExpiry(values.expiryMonth, values.expiryYear),
      }

  return (
    <Modal
      title={card ? 'Editar tarjeta' : 'Nueva tarjeta'}
      description={
        card
          ? 'Puedes actualizar el titular, el alias y el cupo de la tarjeta.'
          : 'Solo almacenamos los últimos 4 dígitos. Nunca solicitamos el CVV.'
      }
      onClose={onClose}
      size="lg"
    >
      <div className="grid gap-8 md:grid-cols-[minmax(0,1fr)_minmax(0,1.1fr)]">
        <div className="order-first md:order-last">
          <CreditCard
            brand={preview.brand}
            maskedNumber={preview.maskedNumber}
            holderName={values.holderName.toUpperCase()}
            expiry={preview.expiry}
            nickname={values.nickname}
            status={card?.status}
            isExpired={card?.isExpired}
          />
          <p className="mt-3 text-center text-xs text-slate-500">Vista previa</p>
        </div>

        <form onSubmit={handleSubmit} noValidate className="space-y-4">
          {!card && (
            <TextField
              label="Número de tarjeta"
              inputMode="numeric"
              autoComplete="cc-number"
              placeholder="4111 1111 1111 1111"
              value={values.cardNumber}
              onChange={updateField('cardNumber', formatCardNumber)}
              error={errors.cardNumber}
              inputClassName="font-mono tracking-wider"
              autoFocus
            />
          )}

          <TextField
            label="Titular"
            autoComplete="cc-name"
            placeholder="Como aparece en la tarjeta"
            maxLength={HOLDER_NAME_MAX_LENGTH}
            value={values.holderName}
            onChange={updateField('holderName')}
            error={errors.holderName}
            autoFocus={card !== null}
          />

          <TextField
            label="Alias (opcional)"
            placeholder="Ej: Compras online"
            maxLength={NICKNAME_MAX_LENGTH}
            value={values.nickname}
            onChange={updateField('nickname')}
            error={errors.nickname}
          />

          {!card && (
            <div>
              <div className="grid grid-cols-2 gap-3">
                <SelectField
                  label="Mes"
                  autoComplete="cc-exp-month"
                  value={values.expiryMonth}
                  onChange={updateField('expiryMonth')}
                  isInvalid={Boolean(errors.expiry)}
                >
                  <option value="">MM</option>
                  {MONTHS.map((month) => (
                    <option key={month} value={month}>
                      {month.padStart(2, '0')}
                    </option>
                  ))}
                </SelectField>
                <SelectField
                  label="Año"
                  autoComplete="cc-exp-year"
                  value={values.expiryYear}
                  onChange={updateField('expiryYear')}
                  isInvalid={Boolean(errors.expiry)}
                >
                  <option value="">AAAA</option>
                  {YEARS.map((year) => (
                    <option key={year} value={year}>
                      {year}
                    </option>
                  ))}
                </SelectField>
              </div>
              {errors.expiry && <p className="mt-1.5 text-xs text-rose-400">{errors.expiry}</p>}
            </div>
          )}

          <TextField
            label="Cupo total"
            type="number"
            inputMode="decimal"
            min="0.01"
            step="0.01"
            placeholder="5000"
            value={values.creditLimit}
            onChange={updateField('creditLimit')}
            error={errors.creditLimit}
            hint={card ? `Utilizado actualmente: ${formatCurrency(card.usedCredit, card.currency)}` : undefined}
          />

          {submitError && <Alert tone="error">{submitError}</Alert>}

          <div className="flex flex-col-reverse gap-3 pt-2 sm:flex-row sm:justify-end">
            <Button variant="secondary" onClick={onClose} disabled={isSubmitting}>
              Cancelar
            </Button>
            <Button type="submit" isLoading={isSubmitting}>
              {card ? 'Guardar cambios' : 'Agregar tarjeta'}
            </Button>
          </div>
        </form>
      </div>
    </Modal>
  )
}
