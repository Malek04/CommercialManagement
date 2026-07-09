import { Component, OnInit, AfterViewInit, ViewChild, ElementRef, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormArray, FormBuilder, Validators } from '@angular/forms';
import { Order as OrderModel } from '../../Core/Models/order';
import { OrderRequest, OrderLineRequest } from '../../Core/Models/order-request';
import { OrderService } from '../../Core/Services/order-service';
import { ClientService } from '../../Core/Services/client-serivce';
import { ProductService } from '../../Core/Services/product-service';
import { Client as ClientModel } from '../../Core/Models/client';
import { Product as ProductModel } from '../../Core/Models/product';
import { OrderStatus, OrderStatusLabels } from '../../Core/Enums/order-status';
import Swal from 'sweetalert2';
import { Modal } from 'bootstrap';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-order',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './order.html',
  styleUrl: './order.css',
})
export class Order implements OnInit, AfterViewInit {
  @ViewChild('orderModal') orderModalRef!: ElementRef;
  private modalInstance!: Modal;

  orders = signal<OrderModel[]>([]);
  clients = signal<ClientModel[]>([]);
  products = signal<ProductModel[]>([]);

  addOrEditForm!: FormGroup;
  isEdit = false;
  selectedId = '';
  loading = signal(false);
  submitting = signal(false);
  dataLoaded = signal(false);

  OrderStatus = OrderStatus; // exposed for template comparisons
  statusLabels = OrderStatusLabels;

  constructor(
    private fb: FormBuilder,
    private orderService: OrderService,
    private clientService: ClientService,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadOrders();
    this.loadClients();
    this.loadProducts();
  }

  ngAfterViewInit(): void {
    this.modalInstance = new Modal(this.orderModalRef.nativeElement);
  }

  private initForm(): void {
    // orderNumber and status are server-controlled: number is generated,
    // status starts at Draft and only changes via validate/cancel endpoints.
    this.addOrEditForm = this.fb.group({
      clientId: ['', Validators.required],
      orderDate: [new Date().toISOString(), Validators.required],
      orderLines: this.fb.array([]),
    });
  }

  get orderLines(): FormArray {
    return this.addOrEditForm.get('orderLines') as FormArray;
  }

  private createOrderLine(line?: any): FormGroup {
    return this.fb.group({
      id: [line?.id || null],
      productId: [line?.productId || '', Validators.required],
      quantity: [line?.quantity || 1, [Validators.required, Validators.min(1)]],
      // unitPrice is display-only: always resynced from the selected
      // product, never sent to the server.
      unitPrice: [{ value: line?.unitPrice || 0, disabled: true }],
    });
  }

  addLine(): void {
    this.orderLines.push(this.createOrderLine());
  }

  removeLine(index: number): void {
    this.orderLines.removeAt(index);
  }

  onProductChange(index: number): void {
    const lineGroup = this.orderLines.at(index) as FormGroup;
    const productId = lineGroup.get('productId')?.value;
    const product = this.products().find((p) => p.id === productId);
    if (product) {
      lineGroup.patchValue({ unitPrice: product.unitPriceHT });
    }
  }

  lineTotal(index: number): number {
    const line = this.orderLines.at(index).getRawValue();
    return (line.quantity || 0) * (line.unitPrice || 0);
  }

  get totalHT(): number {
    return this.orderLines.controls.reduce((sum, ctrl) => {
      const v = (ctrl as FormGroup).getRawValue();
      return sum + (v.quantity || 0) * (v.unitPrice || 0);
    }, 0);
  }

  get totalTTC(): number {
    return this.totalHT * 1.19;
  }

  loadOrders(): void {
    this.loading.set(true);
    this.dataLoaded.set(false);

    this.orderService.get().subscribe({
      next: (data: OrderModel[]) => {
        this.orders.set([...data]);
        this.loading.set(false);
        this.dataLoaded.set(true);
      },
      error: (err: any) => {
        this.loading.set(false);
        this.dataLoaded.set(true);
        console.error('❌ Error loading orders:', err);
        Swal.fire({ icon: 'error', title: 'Erreur', text: 'Impossible de charger la liste des commandes.' });
      },
    });
  }

  loadClients(): void {
    this.clientService.get().subscribe({
      next: (data: ClientModel[]) => this.clients.set([...data]),
      error: (err: any) => console.error('❌ Error loading clients:', err),
    });
  }

  loadProducts(): void {
    this.productService.get().subscribe({
      next: (data: ProductModel[]) => this.products.set([...data]),
      error: (err: any) => console.error('❌ Error loading products:', err),
    });
  }

  clientName(clientId: string): string {
    const c = this.clients().find((c) => c.id === clientId);
    return c ? `${c.firstName} ${c.lastName}` : '-';
  }

  statusLabel(status: number): string {
    return this.statusLabels[status] || String(status);
  }

  isValidated(order: OrderModel): boolean {
    return order.status === OrderStatus.Validated;
  }

  isCancelled(order: OrderModel): boolean {
    return order.status === OrderStatus.Cancelled;
  }

  // Draft is the only status where edit/delete/validate/cancel are all allowed
  isDraft(order: OrderModel): boolean {
    return order.status === OrderStatus.Draft;
  }

  openAddModal(): void {
    this.isEdit = false;
    this.selectedId = '';
    this.resetForm();
    this.addLine();
    this.modalInstance.show();
  }

  openEditModal(order: OrderModel): void {
    if (!this.isDraft(order)) {
      Swal.fire({
        icon: 'info',
        title: 'Modification impossible',
        text: 'Seules les commandes en brouillon peuvent être modifiées.',
      });
      return;
    }

    this.isEdit = true;
    this.selectedId = order.id;

    this.orderLines.clear();
    (order.lines || []).forEach((line) => {
      this.orderLines.push(this.createOrderLine(line));
    });

    this.addOrEditForm.patchValue({
      clientId: order.clientId,
      orderDate: order.orderDate 
    ? new Date(order.orderDate).toISOString().substring(0, 10)
    : ''
    });

    this.modalInstance.show();
  }

  closeModal(): void {
    this.modalInstance.hide();
  }

  submit(): void {
  if (this.addOrEditForm.invalid || this.orderLines.length === 0) {
    this.addOrEditForm.markAllAsTouched();
    Swal.fire({
      icon: 'warning',
      title: 'Formulaire incomplet',
      text:
        this.orderLines.length === 0
          ? 'Ajoutez au moins une ligne de commande.'
          : 'Merci de remplir correctement tous les champs obligatoires.',
    });
    return;
  }

  this.submitting.set(true);

  const raw = this.addOrEditForm.getRawValue();
  const payload: OrderRequest = {
    clientId: raw.clientId,
    orderDate: raw.orderDate,
    lines: raw.orderLines.map((l: any): OrderLineRequest => ({
      id: l.id || undefined,
      productId: l.productId,
      quantity: l.quantity,
    })),
  };

  // Explicit Observable<any> avoids TS trying to unify two different
  // subscribe() overload sets from Observable<Order> vs Observable<void>.
  const request$: Observable<any> = this.isEdit
    ? this.orderService.update(this.selectedId, payload)
    : this.orderService.post(payload);

  request$.subscribe({
    next: () => {
      this.submitting.set(false);
      this.closeModal();
      this.loadOrders();
      this.resetForm();
      Swal.fire({
        icon: 'success',
        title: this.isEdit ? 'Commande mise à jour' : 'Commande ajoutée',
        text: this.isEdit
          ? 'Les informations ont été mises à jour avec succès.'
          : 'La commande a été ajoutée avec succès.',
        timer: 2000,
        showConfirmButton: false,
      });
    },
    error: (err: any) => {
      this.submitting.set(false);
      console.error('❌ Save failed:', err);
      Swal.fire({
        icon: 'error',
        title: 'Erreur',
        text: err?.error?.Message || err?.error?.message || 'Une erreur est survenue.',
      });
    },
  });
}

  validateOrder(id: string): void {
    Swal.fire({
      icon: 'question',
      title: 'Valider cette commande ?',
      text: 'Le stock sera déduit et la commande ne pourra plus être modifiée.',
      showCancelButton: true,
      confirmButtonText: 'Oui, valider',
      cancelButtonText: 'Annuler',
    }).then((result) => {
      if (!result.isConfirmed) return;

      this.orderService.validate(id).subscribe({
        next: () => {
          this.loadOrders();
          Swal.fire({ icon: 'success', title: 'Commande validée', timer: 1500, showConfirmButton: false });
        },
        error: (err: any) => {
          console.error('❌ Validate failed:', err);
          Swal.fire({
            icon: 'error',
            title: 'Erreur',
            text: err?.error?.Message || err?.error?.message || 'Impossible de valider cette commande.',
          });
        },
      });
    });
  }

  cancelOrder(id: string): void {
    Swal.fire({
      icon: 'warning',
      title: 'Annuler cette commande ?',
      text: 'Cette action ne peut pas être annulée une fois faite.',
      showCancelButton: true,
      confirmButtonText: "Oui, annuler la commande",
      cancelButtonText: 'Retour',
      confirmButtonColor: '#dc3545',
    }).then((result) => {
      if (!result.isConfirmed) return;

      this.orderService.cancel(id).subscribe({
        next: () => {
          this.loadOrders();
          Swal.fire({ icon: 'success', title: 'Commande annulée', timer: 1500, showConfirmButton: false });
        },
        error: (err: any) => {
          console.error('❌ Cancel failed:', err);
          Swal.fire({
            icon: 'error',
            title: 'Erreur',
            text: err?.error?.Message || err?.error?.message || "Impossible d'annuler cette commande.",
          });
        },
      });
    });
  }

  delete(id: string): void {
    Swal.fire({
      icon: 'warning',
      title: 'Êtes-vous sûr ?',
      text: 'Cette action est irréversible.',
      showCancelButton: true,
      confirmButtonText: 'Oui, supprimer',
      cancelButtonText: 'Annuler',
      confirmButtonColor: '#dc3545',
    }).then((result) => {
      if (result.isConfirmed) {
        this.orderService.delete(id).subscribe({
          next: () => {
            this.loadOrders();
            Swal.fire({ icon: 'success', title: 'Supprimée', timer: 1500, showConfirmButton: false });
          },
          error: (err: any) => {
            console.error('Delete error:', err);
            Swal.fire({ icon: 'error', title: 'Erreur', text: 'Impossible de supprimer cette commande.' });
          },
        });
      }
    });
  }

  resetForm(): void {
    this.orderLines.clear();
    this.addOrEditForm.reset({
      clientId: '',
      orderDate: new Date().toISOString(),
    });
  }

  trackById(index: number, order: OrderModel): string {
    return order.id;
  }
}