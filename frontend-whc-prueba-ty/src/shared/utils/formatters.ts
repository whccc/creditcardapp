const LOCALE = 'es-CL'

const currencyFormatters = new Map<string, Intl.NumberFormat>()

const dateTimeFormatter = new Intl.DateTimeFormat(LOCALE, {
  dateStyle: 'medium',
  timeStyle: 'short',
})

function currencyFormatter(currency: string): Intl.NumberFormat {
  let formatter = currencyFormatters.get(currency)

  if (!formatter) {
    formatter = new Intl.NumberFormat(LOCALE, {
      style: 'currency',
      currency,
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    })
    currencyFormatters.set(currency, formatter)
  }

  return formatter
}

export function formatCurrency(amount: number, currency = 'USD'): string {
  return currencyFormatter(currency).format(amount)
}

export function formatDateTime(value: string | Date): string {
  return dateTimeFormatter.format(new Date(value))
}

export function formatPercentage(value: number): string {
  return `${value.toLocaleString(LOCALE, { maximumFractionDigits: 1 })}%`
}
