import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../Models/product';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root',
})
export class ProductService {

  constructor(private http: HttpClient) {}

  get(): Observable<Product[]> {
    return this.http.get<Product[]>(`${environment.apiUrl}/Product`);
  }

  getById(id: string): Observable<Product> {
    return this.http.get<Product>(`${environment.apiUrl}/Product/${id}`);
  }

  post(data: any): Observable<any> {
    return this.http.post(`${environment.apiUrl}/Product`, data);
  }

  update(id: string, data: any): Observable<any> {
    return this.http.put(`${environment.apiUrl}/Product/${id}`, data);
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/Product/${id}`);
  }
}