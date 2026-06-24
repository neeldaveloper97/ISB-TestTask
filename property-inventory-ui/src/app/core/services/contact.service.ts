import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Contact, ContactQuery, CreateContact, PagedResult, UpdateContact
} from '../models/models';

@Injectable({ providedIn: 'root' })
export class ContactService {
  private readonly baseUrl = `${environment.apiUrl}/contacts`;

  constructor(private http: HttpClient) {}

  getAll(query: ContactQuery = {}): Observable<PagedResult<Contact>> {
    let params = new HttpParams();
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, String(value));
      }
    });
    return this.http.get<PagedResult<Contact>>(this.baseUrl, { params });
  }

  getById(id: string): Observable<Contact> {
    return this.http.get<Contact>(`${this.baseUrl}/${id}`);
  }

  create(dto: CreateContact): Observable<Contact> {
    return this.http.post<Contact>(this.baseUrl, dto);
  }

  createBulk(dtos: CreateContact[]): Observable<Contact[]> {
    return this.http.post<Contact[]>(`${this.baseUrl}/bulk`, dtos);
  }

  update(id: string, dto: UpdateContact): Observable<Contact> {
    return this.http.put<Contact>(`${this.baseUrl}/${id}`, dto);
  }

  updateBulk(dtos: UpdateContact[]): Observable<Contact[]> {
    return this.http.put<Contact[]>(`${this.baseUrl}/bulk`, dtos);
  }
}
