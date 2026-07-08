import { Component, OnInit, AfterViewInit, ViewChild, ElementRef, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormArray, FormBuilder, Validators } from '@angular/forms';
import { Order as OrderModel } from '../../Core/Models/order';
import { OrderService } from '../../Core/Services/order-service';
import { ClientService } from '../../Core/Services/client-serivce';
import { ProductService } from '../../Core/Services/product-service';
import { Client as ClientModel } from '../../Core/Models/client';
import { Product as ProductModel } from '../../Core/Models/product';
import { OrderStatus, OrderStatusLabels } from '../../Core/Enums/order-status';
import Swal from 'sweetalert2';
import { Modal } from 'bootstrap';

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

  statusOptions = Object.values(OrderStatus).filter(
    (v) => typeof v === 'number'
  ) as number[];
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
    this.addOrEditForm = this.fb.group({
      id: ['00000000-0000-0000-0000-000000000000'],
      orderNumber: ['', Validators.required],
      clientId: ['', Validators.required],
      orderDate: [new Date().toISOString(), Validators.required],
      status: [OrderStatus.Draft, Validators.required],
      orderLines: this.fb.array([]),
    });
  }

  get orderLines(): FormArray {
    return this.addOrEditForm.get('orderLines') as FormArray;
  }

  private createOrderLine(line?: any): FormGroup {
    return this.fb.group({
      id: [line?.id || '00000000-0000-0000-0000-000000000000'],
      productId: [line?.productId || '', Validators.required],
      quantity: [line?.quantity || 1, [Validators.required, Validators.min(1)]],
      unitPrice: [line?.unitPrice || 0, [Validators.required, Validators.min(0)]],
    });
  }

  addLine(): void {
    this.orderLines.push(this.createOrderLine());
  }

  removeLine(index: number): void {
    this.orderLines.removeAt(index);
  }

  // Auto-fill unit price when a product is selected on a line
  onProductChange(index: number): void {
    const lineGroup = this.orderLines.at(index) as FormGroup;
    const productId = lineGroup.get('productId')?.value;
    const product = this.products().find((p) => p.id === productId);
    if (product) {
      lineGroup.patchValue({ unitPrice: product.unitPriceHT });
    }
  }

  lineTotal(index: number): number {
    const line = this.orderLines.at(index).value;
    return (line.quantity || 0) * (line.unitPrice || 0);
  }

  get totalHT(): number {
    return this.orderLines.controls.reduce((sum, ctrl) => {
      const v = ctrl.value;
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
        Swal.fire({
          icon: 'error',
          title: 'Erreur',
          text: 'Impossible de charger la liste des commandes.',
        });
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

  productName(productId: string): string {
    const p = this.products().find((p) => p.id === productId);
    return p ? p.name || '-' : '-';
  }

  statusLabel(status: number): string {
    return this.statusLabels[status] || String(status);
  }

  openAddModal(): void {
    this.isEdit = false;
    this.selectedId = '';
    this.resetForm();
    this.addLine(); // start with one empty line
    this.modalInstance.show();
  }

  openEditModal(order: OrderModel): void {
    this.isEdit = true;
    this.selectedId = order.id;

    this.orderLines.clear();
    (order.orderLines || []).forEach((line) => {
      this.orderLines.push(this.createOrderLine(line));
    });

    this.addOrEditForm.patchValue({
      id: order.id,
      orderNumber: order.orderNumber,
      clientId: order.clientId,
      orderDate: order.orderDate,
      status: order.status,
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

    // Include live-calculated totals since the backend model expects them
    const formValue = {
      ...this.addOrEditForm.value,
      totalHT: this.totalHT,
      totalTTC: this.totalTTC,
    };

    if (this.isEdit) {
      this.orderService.update(this.selectedId, formValue).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadOrders();
          this.resetForm();
          Swal.fire({
            icon: 'success',
            title: 'Commande mise à jour',
            text: 'Les informations ont été mises à jour avec succès.',
            timer: 2000,
            showConfirmButton: false,
          });
        },
        error: (err: any) => {
          this.submitting.set(false);
          console.error('❌ Update failed:', err);
          console.error('❌ Validation details:', err?.error);
          Swal.fire({
            icon: 'error',
            title: 'Erreur',
            text: err?.error?.message || 'Une erreur est survenue lors de la mise à jour de la commande.',
          });
        },
      });
    } else {
      this.orderService.post(formValue).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadOrders();
          this.resetForm();
          Swal.fire({
            icon: 'success',
            title: 'Commande ajoutée',
            text: 'La commande a été ajoutée avec succès.',
            timer: 2000,
            showConfirmButton: false,
          });
        },
        error: (err: any) => {
          this.submitting.set(false);
          console.error('❌ Add failed:', err);
          console.error('❌ Validation details:', err?.error);
          Swal.fire({
            icon: 'error',
            title: 'Erreur',
            text: err?.error?.message || "Une erreur est survenue lors de l'ajout de la commande.",
          });
        },
      });
    }
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
            Swal.fire({
              icon: 'success',
              title: 'Supprimée',
              text: 'La commande a été supprimée.',
              timer: 1500,
              showConfirmButton: false,
            });
          },
          error: (err: any) => {
            console.error('Delete error:', err);
            Swal.fire({
              icon: 'error',
              title: 'Erreur',
              text: 'Impossible de supprimer cette commande.',
            });
          },
        });
      }
    });
  }

  resetForm(): void {
    this.orderLines.clear();
    this.addOrEditForm.reset({
      id: '00000000-0000-0000-0000-000000000000',
      orderDate: new Date().toISOString(),
      status: OrderStatus.Draft,
    });
  }

  trackById(index: number, order: OrderModel): string {
    return order.id;
  }
}