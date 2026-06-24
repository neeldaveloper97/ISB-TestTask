import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AssignOwner, CreateProperty, PagedResult, Property, PropertyQuery, UpdateProperty
} from '../models/models';

@Injectable({ providedIn: 'root' })
export class PropertyService {
  private readonly baseUrl = `${environment.apiUrl}/properties`;

  constructor(private http: HttpClient) {}

  getAll(query: PropertyQuery = {}): Observable<PagedResult<Property>> {
    let params = new HttpParams();
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, String(value));
      }
    });
    return this.http.get<PagedResult<Property>>(this.baseUrl, { params });
  }

  getById(id: string): Observable<Property> {
    return this.http.get<Property>(`${this.baseUrl}/${id}`);
  }

  create(dto: CreateProperty): Observable<Property> {
    return this.http.post<Property>(this.baseUrl, dto);
  }

  createBulk(dtos: CreateProperty[]): Observable<Property[]> {
    return this.http.post<Property[]>(`${this.baseUrl}/bulk`, dtos);
  }

  update(id: string, dto: UpdateProperty): Observable<Property> {
    return this.http.put<Property>(`${this.baseUrl}/${id}`, dto);
  }

  updateBulk(dtos: UpdateProperty[]): Observable<Property[]> {
    return this.http.put<Property[]>(`${this.baseUrl}/bulk`, dtos);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  assignOwner(propertyId: string, dto: AssignOwner): Observable<Property> {
    return this.http.post<Property>(`${this.baseUrl}/${propertyId}/ownership`, dto);
  }
}
