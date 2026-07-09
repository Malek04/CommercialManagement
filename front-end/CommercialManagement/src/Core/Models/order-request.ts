export interface OrderLineRequest {
  id?: string;
  productId: string;
  quantity: number;
}

export interface OrderRequest {
  clientId: string;
  orderDate: Date;
  lines: OrderLineRequest[];
}