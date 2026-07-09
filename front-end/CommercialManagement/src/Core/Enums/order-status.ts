export enum OrderStatus {
  Draft = 0,
  Validated = 1,
  Cancelled = 2,
}

export const OrderStatusLabels: Record<number, string> = {
  [OrderStatus.Draft]: 'Brouillon',
  [OrderStatus.Validated]: 'Validée',
  [OrderStatus.Cancelled]: 'Annulée',
};