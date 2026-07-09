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
  @ViewChild('consultModal') consultModalRef!: ElementRef;

  private modalInstance!: Modal;
  private consultModalInstance!: Modal;

  orders = signal<OrderModel[]>([]);
  clients = signal<ClientModel[]>([]);
  products = signal<ProductModel[]>([]);

  addOrEditForm!: FormGroup;
  isEdit = false;
  selectedId = '';
  loading = signal(false);
  submitting = signal(false);
  dataLoaded = signal(false);

  // For consultation modal
  selectedOrder = signal<OrderModel | null>(null);

  OrderStatus = OrderStatus;
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
    this.consultModalInstance = new Modal(this.consultModalRef.nativeElement);
  }

  private todayIso(): string {
    return new Date().toISOString().substring(0, 10);
  }

  private initForm(): void {
    this.addOrEditForm = this.fb.group({
      clientId: ['', Validators.required],
      orderDate: [this.todayIso(), Validators.required],
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
    return this.statusLabels[status] ?? String(status);
  }

  // --- Comparaison robuste : gère le statut renvoyé en number (0,1,2) OU en string ("Draft","Validated",...) ---
  private statusEquals(order: OrderModel, target: OrderStatus): boolean {
    const raw: any = (order as any).status;

    if (typeof raw === 'number') {
      return raw === target;
    }

    if (typeof raw === 'string') {
      const asNumber = Number(raw);
      if (!Number.isNaN(asNumber)) {
        return asNumber === target;
      }
      // Comparaison par nom d'enum (ex: "Draft" === OrderStatus[0])
      const targetName = (OrderStatus as any)[target];
      return typeof targetName === 'string' && raw.toLowerCase() === targetName.toLowerCase();
    }

    return false;
  }

  isValidated(order: OrderModel): boolean {
    return this.statusEquals(order, OrderStatus.Validated);
  }

  isCancelled(order: OrderModel): boolean {
    return this.statusEquals(order, OrderStatus.Cancelled);
  }

  isDraft(order: OrderModel): boolean {
    return this.statusEquals(order, OrderStatus.Draft);
  }

  validateOrder(id: string): void {
    Swal.fire({
      title: 'Valider la commande ?',
      text: 'La commande sera définitivement validée.',
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: 'Oui, valider',
      cancelButtonText: 'Annuler',
    }).then((result) => {
      if (!result.isConfirmed) return;

      this.orderService.validate(id).subscribe({
        next: () => {
          Swal.fire({ icon: 'success', title: 'Commande validée', timer: 1500, showConfirmButton: false });
          this.loadOrders();

          const current = this.selectedOrder();
          if (current && current.id === id) {
            this.selectedOrder.set({ ...current, status: OrderStatus.Validated });
          }
        },
        error: (err: any) => {
          console.error('❌ Validation error:', err?.status, err?.error);
          Swal.fire({
            icon: 'error',
            title: 'Erreur',
            text: err?.error?.Message || err?.error?.message || err?.error || 'Impossible de valider la commande.',
          });
        },
      });
    });
  }

  cancelOrder(id: string): void {
    Swal.fire({
      title: 'Annuler la commande ?',
      text: 'Cette action changera le statut de la commande.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Oui, annuler',
      cancelButtonText: 'Retour',
    }).then((result) => {
      if (!result.isConfirmed) return;

      this.orderService.cancel(id).subscribe({
        next: () => {
          Swal.fire({ icon: 'success', title: 'Commande annulée', timer: 1500, showConfirmButton: false });
          this.loadOrders();

          const current = this.selectedOrder();
          if (current && current.id === id) {
            this.selectedOrder.set({ ...current, status: OrderStatus.Cancelled });
          }
        },
        error: (err: any) => {
          console.error('❌ Cancel error:', err?.status, err?.error);
          Swal.fire({
            icon: 'error',
            title: 'Erreur',
            text: err?.error?.Message || err?.error?.message || err?.error || "Impossible d'annuler la commande.",
          });
        },
      });
    });
  }

  // ==================== DELETE ORDER (méthode manquante — appelée par le template) ====================
  delete(id: string): void {
    Swal.fire({
      title: 'Supprimer la commande ?',
      text: 'Cette action est irréversible.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Oui, supprimer',
      cancelButtonText: 'Annuler',
    }).then((result) => {
      if (!result.isConfirmed) return;

      this.orderService.delete(id).subscribe({
        next: () => {
          Swal.fire({ icon: 'success', title: 'Commande supprimée', timer: 1500, showConfirmButton: false });
          this.loadOrders();
        },
        error: (err: any) => {
          console.error('❌ Delete error:', err?.status, err?.error);
          Swal.fire({
            icon: 'error',
            title: 'Erreur',
            text: err?.error?.Message || err?.error?.message || err?.error || 'Impossible de supprimer la commande.',
          });
        },
      });
    });
  }

  // ==================== CONSULT ORDER ====================
  consultOrder(order: OrderModel): void {
    this.selectedOrder.set({ ...order });
    this.consultModalInstance.show();
  }

  closeConsultModal(): void {
    this.consultModalInstance.hide();
    this.selectedOrder.set(null);
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
      orderDate: order.orderDate ? new Date(order.orderDate).toISOString().substring(0, 10) : this.todayIso(),
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
      lines: raw.orderLines.map(
        (l: any): OrderLineRequest => ({
          id: l.id || undefined,
          productId: l.productId,
          quantity: l.quantity,
        })
      ),
    };

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
        console.error('❌ Save failed:', err?.status, err?.error);
        Swal.fire({
          icon: 'error',
          title: 'Erreur',
          text: err?.error?.Message || err?.error?.message || err?.error || 'Une erreur est survenue.',
        });
      },
    });
  }

  resetForm(): void {
    this.orderLines.clear();
    this.isEdit = false;
    this.selectedId = '';
    this.addOrEditForm.reset({
      clientId: '',
      orderDate: this.todayIso(),
    });
  }

  trackById(index: number, order: OrderModel): string {
    return order.id;
  }
}