import { CreditCardIcon } from '../../../shared/components/icons'
import { cn } from '../../../shared/utils/cn'
import type { DisplayBrand } from '../utils/cardNumber'

interface BrandLogoProps {
  brand: DisplayBrand
  className?: string
}

export function BrandLogo({ brand, className }: BrandLogoProps) {
  switch (brand) {
    case 'visa':
      return (
        <span className={cn('text-2xl leading-none font-black tracking-tight text-white italic', className)} aria-label="Visa">
          VISA
        </span>
      )
    case 'mastercard':
      return (
        <span className={cn('flex items-center', className)} aria-label="Mastercard">
          <span className="size-7 rounded-full bg-[#eb001b]" />
          <span className="-ml-3 size-7 rounded-full bg-[#f79e1b]/90" />
        </span>
      )
    case 'amex':
      return (
        <span
          className={cn(
            'rounded-md bg-white/90 px-1.5 py-1 text-[10px] leading-none font-black tracking-widest text-sky-700',
            className,
          )}
          aria-label="American Express"
        >
          AMEX
        </span>
      )
    default:
      return <CreditCardIcon className={cn('size-7 text-white/70', className)} />
  }
}
