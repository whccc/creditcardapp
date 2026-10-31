import type { CardBrand } from '../../../shared/types/contracts'

export type DisplayBrand = CardBrand | 'unknown'

interface BrandRules {
  lengths: number[]
  groups: number[]
  previewLength: number
}

const BRAND_RULES: Record<DisplayBrand, BrandRules> = {
  visa: { lengths: [13, 16, 19], groups: [4, 4, 4, 4, 3], previewLength: 16 },
  mastercard: { lengths: [16], groups: [4, 4, 4, 4], previewLength: 16 },
  amex: { lengths: [15], groups: [4, 6, 5], previewLength: 15 },
  unknown: { lengths: [16], groups: [4, 4, 4, 4], previewLength: 16 },
}

const MASK_CHARACTER = '•'

function groupCharacters(characters: string, groups: number[]): string {
  const parts: string[] = []
  let cursor = 0

  for (const size of groups) {
    if (cursor >= characters.length) {
      break
    }

    parts.push(characters.slice(cursor, cursor + size))
    cursor += size
  }

  return parts.join(' ')
}

export function digitsOnly(value: string): string {
  return value.replace(/\D/g, '')
}

export function detectBrand(value: string): DisplayBrand {
  const digits = digitsOnly(value)
  const firstTwo = Number(digits.slice(0, 2))
  const firstFour = Number(digits.slice(0, 4))

  if (digits.startsWith('4')) {
    return 'visa'
  }

  if ((firstTwo >= 51 && firstTwo <= 55) || (digits.length >= 4 && firstFour >= 2221 && firstFour <= 2720)) {
    return 'mastercard'
  }

  if (firstTwo === 34 || firstTwo === 37) {
    return 'amex'
  }

  return 'unknown'
}

export function formatCardNumber(value: string): string {
  const digits = digitsOnly(value)
  const rules = BRAND_RULES[detectBrand(digits)]

  return groupCharacters(digits.slice(0, Math.max(...rules.lengths)), rules.groups)
}

export function previewCardNumber(value: string): string {
  const digits = digitsOnly(value)
  const rules = BRAND_RULES[detectBrand(digits)]

  return groupCharacters(digits.padEnd(rules.previewLength, MASK_CHARACTER), rules.groups)
}

export function isLuhnValid(digits: string): boolean {
  let sum = 0
  let shouldDouble = false

  for (let index = digits.length - 1; index >= 0; index -= 1) {
    let digit = digits.charCodeAt(index) - 48

    if (shouldDouble) {
      digit *= 2
      if (digit > 9) {
        digit -= 9
      }
    }

    sum += digit
    shouldDouble = !shouldDouble
  }

  return digits.length > 0 && sum % 10 === 0
}

export function validateCardNumber(value: string): string | null {
  const digits = digitsOnly(value)

  if (!digits) {
    return 'Ingresa el número de la tarjeta.'
  }

  const brand = detectBrand(digits)

  if (brand === 'unknown') {
    return 'Solo se aceptan tarjetas Visa, Mastercard o American Express.'
  }

  if (!BRAND_RULES[brand].lengths.includes(digits.length) || !isLuhnValid(digits)) {
    return 'El número de tarjeta no es válido.'
  }

  return null
}
