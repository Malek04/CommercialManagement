import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Client } from '../Models/client';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root',
})
export class ClientService {

  constructor(private http: HttpClient) {}

  get(): Observable<Client[]> {
    return this.http.get<Client[]>(`${environment.apiUrl}/Client`);
  }

  getById(id: string): Observable<Client> {
    return this.http.get<Client>(`${environment.apiUrl}/Client/${id}`);
  }

  post(data: any): Observable<any> {
    return this.http.post(`${environment.apiUrl}/Client`, data);
  }

  update(id: string, data: any): Observable<any> {
    return this.http.put(`${environment.apiUrl}/Client/${id}`, data);
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/Client/${id}`);
  }
}