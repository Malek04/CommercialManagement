export enum OrderStatus {
  Draft = 0,
  Confirmed = 1,
  Cancelled = 2,
}

export const OrderStatusLabels: Record<number, string> = {
  [OrderStatus.Draft]: 'Brouillon',
  [OrderStatus.Confirmed]: 'Validée',
  [OrderStatus.Cancelled]: 'Annulée',
};
