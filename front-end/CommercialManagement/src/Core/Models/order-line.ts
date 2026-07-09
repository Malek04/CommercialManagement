export class OrderLine {
  id!: string;
  productId!: string;
  productReference?: string;
  productName?: string;
  quantity!: number;
  unitPrice!: number;
  totalLine!: number;
  productStockQuantity!: number;
}
