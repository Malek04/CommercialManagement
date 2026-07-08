export class Product {
  id!: string;
  reference?: string;
  name?: string;
  description?: string;
  unitPriceHT!: number;
  stockQuantity!: number;
  created!: Date;
}