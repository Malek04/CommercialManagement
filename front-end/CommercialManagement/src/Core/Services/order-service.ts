import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Order } from '../Models/order';
import { OrderRequest } from '../Models/order-request';
import { environment } from '../../environment/environment';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private baseUrl = `${environment.apiUrl}/orders`;

  constructor(private http: HttpClient) {}

  get(): Observable<Order[]> {
    return this.http.get<Order[]>(this.baseUrl);
  }
  
  getById(id: string): Observable<Order> {
    return this.http.get<Order>(`${this.baseUrl}/${id}`);
  }

  post(order: OrderRequest): Observable<Order> {
    return this.http.post<Order>(this.baseUrl, order);
  }

  update(id: string, order: OrderRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, order);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  validate(id: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/${id}/validate`, {});
  }

  cancel(id: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/${id}/cancel`, {});
  }
  
}