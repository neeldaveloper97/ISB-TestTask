import { CurrencyFormatPipe } from './currency-format.pipe';

describe('CurrencyFormatPipe', () => {
  let pipe: CurrencyFormatPipe;

  beforeEach(() => (pipe = new CurrencyFormatPipe()));

  it('formats EUR amounts', () => {
    expect(pipe.transform(130000, 'EUR')).toContain('130,000');
  });

  it('returns "N/A" for null/undefined amounts', () => {
    expect(pipe.transform(null, 'EUR')).toBe('N/A');
    expect(pipe.transform(undefined, 'USD')).toBe('N/A');
  });

  it('falls back gracefully for an unknown currency code', () => {
    const result = pipe.transform(100, 'ZZZ');
    expect(result).toContain('100');
  });

  it('defaults to USD when currency is missing', () => {
    expect(pipe.transform(10, null)).toContain('10');
  });
});
