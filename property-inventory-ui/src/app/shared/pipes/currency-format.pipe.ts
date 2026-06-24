import { Pipe, PipeTransform } from '@angular/core';

/**
 * Formats a numeric amount with its ISO currency code, e.g. (130000, 'EUR') -> "€130,000.00".
 * Falls back to "<code> <amount>" when Intl can't resolve the currency.
 */
@Pipe({ name: 'currencyFormat', standalone: true })
export class CurrencyFormatPipe implements PipeTransform {
  transform(amount: number | null | undefined, currency: string | null | undefined): string {
    if (amount === null || amount === undefined) {
      return 'N/A';
    }
    const code = (currency || 'USD').toUpperCase();
    try {
      return new Intl.NumberFormat('en-US', { style: 'currency', currency: code }).format(amount);
    } catch {
      return `${code} ${amount.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
    }
  }
}
