import { OrderStatus } from '../Enums/order-status';
import { OrderLine } from '../Models/order-line';

export class Order {
  id!: string;
  orderNumber?: string;
  clientId!: string;
  orderDate: Date = new Date();
  status: OrderStatus = OrderStatus.Draft;
  totalHT!: number;
  totalTTC!: number;
  orderLines: OrderLine[] = [];
  clientName?: string;
}
