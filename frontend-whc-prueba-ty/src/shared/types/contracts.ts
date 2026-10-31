export type CardBrand = 'visa' | 'mastercard' | 'amex'

export type CardStatus = 'active' | 'blocked'

export type PaymentStatus = 'approved' | 'declined'

export interface SessionUser {
  id: string
  fullName: string
  firstName: string
  initials: string
  email: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface SessionResponse {
  expiresAt: string
  user: SessionUser
}

export interface Card {
  id: string
  brand: CardBrand
  brandName: string
  maskedNumber: string
  last4Digits: string
  holderName: string
  nickname: string | null
  expiry: string
  expiryMonth: number
  expiryYear: number
  isExpired: boolean
  status: CardStatus
  creditLimit: number
  availableCredit: number
  usedCredit: number
  utilizationPercentage: number
  currency: string
}

export interface CreateCardRequest {
  cardNumber: string
  holderName: string
  nickname: string | null
  expiryMonth: number
  expiryYear: number
  creditLimit: number
}

export interface UpdateCardRequest {
  holderName: string
  nickname: string | null
  creditLimit: number
}

export interface CreatePaymentRequest {
  cardId: string
  amount: number
  merchant: string
}

export interface Payment {
  id: string
  cardId: string
  cardBrand: CardBrand
  cardLast4Digits: string
  cardLabel: string
  merchant: string
  amount: number
  currency: string
  status: PaymentStatus
  declineReason: string | null
  createdAt: string
}

export interface PaymentHistoryQuery {
  cardId?: string | null
  page?: number
  pageSize?: number
}

export interface PagedResponse<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

export interface CreditSummary {
  totalCreditLimit: number
  totalAvailableCredit: number
  totalUsedCredit: number
  utilizationPercentage: number
  totalCards: number
  activeCards: number
  blockedCards: number
  currency: string
}

export interface DashboardResponse {
  user: SessionUser
  summary: CreditSummary
  cards: Card[]
}

export interface ProblemDetails {
  type?: string
  title?: string
  status?: number
  detail?: string
  code?: string
  traceId?: string
}
