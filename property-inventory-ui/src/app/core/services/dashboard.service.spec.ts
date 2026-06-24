import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../environments/environment';
import { DashboardRow } from '../models/models';
import { DashboardService } from './dashboard.service';

describe('DashboardService', () => {
  let service: DashboardService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [DashboardService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(DashboardService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('fetches dashboard rows from the api', () => {
    const rows: DashboardRow[] = [{
      id: '1', propertyId: 'p1', propertyName: 'Maisonette', askingPrice: 130000,
      askingPriceCurrency: 'EUR', ownerName: 'Carmen Attard', dateOfPurchase: '2024-01-15',
      soldAtPriceOriginal: 120000, soldAtPriceCurrency: 'EUR', soldAtPriceUsd: 130480
    }];

    let received: DashboardRow[] | undefined;
    service.getDashboard().subscribe((r) => (received = r));

    const req = httpMock.expectOne(`${environment.apiUrl}/dashboard`);
    expect(req.request.method).toBe('GET');
    req.flush(rows);

    expect(received).toEqual(rows);
  });
});
