import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../environments/environment';
import { PagedResult, Property } from '../models/models';
import { PropertyService } from './property.service';

describe('PropertyService', () => {
  let service: PropertyService;
  let httpMock: HttpTestingController;
  const base = `${environment.apiUrl}/properties`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PropertyService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(PropertyService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('builds query params for getAll and only includes set filters', () => {
    service.getAll({ page: 2, pageSize: 5, name: 'Villa' }).subscribe();

    const req = httpMock.expectOne((r) => r.url === base);
    expect(req.request.params.get('page')).toBe('2');
    expect(req.request.params.get('pageSize')).toBe('5');
    expect(req.request.params.get('name')).toBe('Villa');
    expect(req.request.params.has('address')).toBeFalse();
    req.flush({ data: [], totalCount: 0, page: 2, pageSize: 5 } as PagedResult<Property>);
  });

  it('posts to the ownership endpoint when assigning an owner', () => {
    const id = 'abc';
    service.assignOwner(id, { contactId: 'c1', acquisitionPrice: 100, acquisitionCurrency: 'EUR' }).subscribe();

    const req = httpMock.expectOne(`${base}/${id}/ownership`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body.contactId).toBe('c1');
    req.flush({} as Property);
  });

  it('issues DELETE to the correct url', () => {
    service.delete('xyz').subscribe();
    const req = httpMock.expectOne(`${base}/xyz`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
