import { OrderStatus } from '../Enums/order-status';
import { OrderLine } from '../Models/order-line';

export class Order {
  id!: string;
  orderNumber?: string;
  orderDate: Date = new Date();
  status: OrderStatus = OrderStatus.Draft;
  totalHT!: number;
  totalTTC!: number;

  // Client
  clientId!: string;
  lastName?: string;
  firstName?: string;
  email?: string;
  phone?: string;
  created!: Date;

  // Address
  rue?: string;
  ville?: string;
  codePostal?: string;
  pays?: string;

  lines: OrderLine[] = [];
}