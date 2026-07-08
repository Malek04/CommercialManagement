import { Routes } from '@angular/router';
import { Product } from '../Components/product/product';
import { Dashboard } from '../Components/dashboard/dashboard';
import { Order } from '../Components/order/order';
import { OrderLine } from '../Components/order-line/order-line';
import { Client } from '../Components/client/client';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },

  { path: 'dashboard', component: Dashboard },
  { path: 'clients', component: Client },    
  { path: 'products', component: Product },
  { path: 'orders', component: Order },
  { path: 'order-lines', component: OrderLine },

  { path: '**', redirectTo: 'dashboard' }
];