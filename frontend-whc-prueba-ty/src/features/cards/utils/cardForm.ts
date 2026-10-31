import type { Card, CreateCardRequest, UpdateCardRequest } from '../../../shared/types/contracts'
import { digitsOnly, validateCardNumber } from './cardNumber'

export const HOLDER_NAME_MAX_LENGTH = 60
export const NICKNAME_MAX_LENGTH = 40
export const MAX_CREDIT_LIMIT = 1_000_000

const TWO_DECIMALS = /^\d+(\.\d{1,2})?$/

export interface CardFormValues {
  cardNumber: string
  holderName: string
  nickname: string
  expiryMonth: string
  expiryYear: string
  creditLimit: string
}

export type CardFormField = keyof CardFormValues

export type CardFormErrors = Partial<Record<CardFormField | 'expiry', string>>

export function toFormValues(card: Card | null): CardFormValues {
  return {
    cardNumber: '',
    holderName: card?.holderName ?? '',
    nickname: card?.nickname ?? '',
    expiryMonth: card ? String(card.expiryMonth) : '',
    expiryYear: card ? String(card.expiryYear) : '',
    creditLimit: card ? String(card.creditLimit) : '',
  }
}

export function validateCardForm(values: CardFormValues, { isEdit }: { isEdit: boolean }): CardFormErrors {
  const errors: CardFormErrors = {}
  const holderName = values.holderName.trim()
  const creditLimit = Number(values.creditLimit)

  if (holderName.length < 2 || holderName.length > HOLDER_NAME_MAX_LENGTH) {
    errors.holderName = `El titular debe tener entre 2 y ${HOLDER_NAME_MAX_LENGTH} caracteres.`
  }

  if (values.nickname.trim().length > NICKNAME_MAX_LENGTH) {
    errors.nickname = `El alias no puede superar ${NICKNAME_MAX_LENGTH} caracteres.`
  }

  if (!TWO_DECIMALS.test(values.creditLimit) || creditLimit <= 0 || creditLimit > MAX_CREDIT_LIMIT) {
    errors.creditLimit = 'Ingresa un cupo válido (máximo 2 decimales).'
  }

  if (isEdit) {
    return errors
  }

  const cardNumberError = validateCardNumber(values.cardNumber)
  if (cardNumberError) {
    errors.cardNumber = cardNumberError
  }

  const month = Number(values.expiryMonth)
  const year = Number(values.expiryYear)
  const today = new Date()

  if (!month || !year) {
    errors.expiry = 'Selecciona mes y año de vencimiento.'
  } else if (year < today.getFullYear() || (year === today.getFullYear() && month < today.getMonth() + 1)) {
    errors.expiry = 'La tarjeta no puede estar vencida.'
  }

  return errors
}

export function toCreatePayload(values: CardFormValues): CreateCardRequest {
  return {
    cardNumber: digitsOnly(values.cardNumber),
    holderName: values.holderName.trim(),
    nickname: values.nickname.trim() || null,
    expiryMonth: Number(values.expiryMonth),
    expiryYear: Number(values.expiryYear),
    creditLimit: Number(values.creditLimit),
  }
}

export function toUpdatePayload(values: CardFormValues): UpdateCardRequest {
  return {
    holderName: values.holderName.trim(),
    nickname: values.nickname.trim() || null,
    creditLimit: Number(values.creditLimit),
  }
}
