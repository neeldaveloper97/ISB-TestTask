export interface PagedResult<T> {
  data: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface PropertyOwnership {
  id: string;
  propertyId: string;
  contactId: string;
  contactName: string;
  effectiveFrom: string;
  effectiveTill: string | null;
  acquisitionPrice: number;
  acquisitionCurrency: string;
  isCurrentOwner: boolean;
}

export interface PropertyPriceHistory {
  id: string;
  propertyId: string;
  amount: number;
  currency: string;
  effectiveDate: string;
}

export interface Property {
  id: string;
  name: string;
  address: string;
  price: number;
  currency: string;
  dateOfRegistration: string;
  ownerships: PropertyOwnership[];
  priceHistories: PropertyPriceHistory[];
}

export interface CreateProperty {
  name: string;
  address: string;
  price: number;
  currency: string;
  dateOfRegistration?: string | null;
}

export interface UpdateProperty {
  id: string;
  name?: string;
  address?: string;
  price?: number;
  currency?: string;
}

export interface AssignOwner {
  contactId: string;
  effectiveFrom?: string | null;
  acquisitionPrice: number;
  acquisitionCurrency: string;
}

export interface OwnedProperty {
  ownershipId: string;
  propertyId: string;
  propertyName: string;
  propertyAddress: string;
  effectiveFrom: string;
  effectiveTill: string | null;
  acquisitionPrice: number;
  acquisitionCurrency: string;
  isCurrentOwner: boolean;
}

export interface Contact {
  id: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  emailAddress: string;
  ownedProperties: OwnedProperty[];
}

export interface CreateContact {
  firstName: string;
  lastName: string;
  phoneNumber: string;
  emailAddress: string;
}

export interface UpdateContact {
  id: string;
  firstName?: string;
  lastName?: string;
  phoneNumber?: string;
  emailAddress?: string;
}

export interface DashboardRow {
  id: string;
  propertyId: string;
  propertyName: string;
  askingPrice: number;
  askingPriceCurrency: string;
  ownerName: string;
  dateOfPurchase: string;
  soldAtPriceOriginal: number;
  soldAtPriceCurrency: string;
  soldAtPriceUsd: number | null;
}

export interface PropertyQuery {
  page?: number;
  pageSize?: number;
  name?: string;
  address?: string;
  minPrice?: number;
  maxPrice?: number;
}

export interface ContactQuery {
  page?: number;
  pageSize?: number;
  firstName?: string;
  lastName?: string;
  email?: string;
}

export const CURRENCIES = ['EUR', 'USD', 'GBP'] as const;
