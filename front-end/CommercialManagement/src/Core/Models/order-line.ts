export class OrderLine {
    id!: string;
    orderId!: string;
    productId!: string;
    quantity!: number;
    unitPrice!: number;
    totalLine!: number;
    productName?: string;
}
