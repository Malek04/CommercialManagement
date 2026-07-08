import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Order } from '../Models/order';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root',
})
export class OrderService {

  constructor(private http: HttpClient) {}

  get(): Observable<Order[]> {
    return this.http.get<Order[]>(`${environment.apiUrl}/Order`);
  }

  getById(id: string): Observable<Order> {
    return this.http.get<Order>(`${environment.apiUrl}/Order/${id}`);
  }

  post(data: any): Observable<any> {
    return this.http.post(`${environment.apiUrl}/Order`, data);
  }

  update(id: string, data: any): Observable<any> {
    return this.http.put(`${environment.apiUrl}/Order/${id}`, data);
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/Order/${id}`);
  }
}