import { Component, OnInit, AfterViewInit, ViewChild, ElementRef, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Product as ProductModel } from '../../Core/Models/product';
import { ProductService } from '../../Core/Services/product-service';
import Swal from 'sweetalert2';
import { Modal } from 'bootstrap';

@Component({
  selector: 'app-product',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './product.html',
  styleUrl: './product.css',
})
export class Product implements OnInit, AfterViewInit {
  @ViewChild('productModal') productModalRef!: ElementRef;
  private modalInstance!: Modal;

  products = signal<ProductModel[]>([]);
  addOrEditForm!: FormGroup;
  
  isEdit = false;
  isView = false;       
  selectedId = '';
  selectedProduct: ProductModel | null = null;

  loading = signal(false);
  submitting = signal(false);
  dataLoaded = signal(false);

  constructor(
    private fb: FormBuilder,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadProducts();
  }

  ngAfterViewInit(): void {
    this.modalInstance = new Modal(this.productModalRef.nativeElement);
  }

  private initForm(): void {
    this.addOrEditForm = this.fb.group({
      id: ['00000000-0000-0000-0000-000000000000'],
      reference: [''],                    // ← Empty for backend generation
      name: ['', [Validators.required, Validators.minLength(2)]],
      description: [''],
      unitPriceHT: [0, [Validators.required, Validators.min(0)]],
      stockQuantity: [0, [Validators.required, Validators.min(0)]],
    });
  }

  loadProducts(): void {
    this.loading.set(true);
    this.dataLoaded.set(false);

    this.productService.get().subscribe({
      next: (data: ProductModel[]) => {
        this.products.set([...data]);
        this.loading.set(false);
        this.dataLoaded.set(true);
      },
      error: (err: any) => {
        this.loading.set(false);
        this.dataLoaded.set(true);
        console.error('❌ Error loading products:', err);
        Swal.fire({
          icon: 'error',
          title: 'Erreur',
          text: 'Impossible de charger la liste des produits.',
        });
      },
    });
  }

  openAddModal(): void {
    this.isEdit = false;
    this.isView = false;
    this.selectedProduct = null;
    this.selectedId = '';
    
    this.resetForm();        // Reference will be empty
    this.modalInstance.show();
  }

  openEditModal(product: ProductModel): void {
    this.isEdit = true;
    this.isView = false;
    this.selectedProduct = null;
    this.selectedId = product.id;
    this.patchProduct(product);
    this.modalInstance.show();
  }

  openViewModal(product: ProductModel): void {
    this.isEdit = false;
    this.isView = true;
    this.selectedProduct = product;
    this.selectedId = product.id;
    this.patchProduct(product);
    this.modalInstance.show();
  }

  private patchProduct(product: ProductModel): void {
    this.addOrEditForm.patchValue({
      id: product.id,
      reference: product.reference,
      name: product.name,
      description: product.description,
      unitPriceHT: product.unitPriceHT,
      stockQuantity: product.stockQuantity,
    });
  }

  closeModal(): void {
    this.modalInstance.hide();
    setTimeout(() => {
      this.isEdit = false;
      this.isView = false;
      this.selectedProduct = null;
    }, 300);
  }

  submit(): void {
    if (this.isView) return;

    if (this.addOrEditForm.invalid) {
      this.addOrEditForm.markAllAsTouched();
      Swal.fire({
        icon: 'warning',
        title: 'Formulaire incomplet',
        text: 'Merci de remplir correctement tous les champs obligatoires.',
      });
      return;
    }

    this.submitting.set(true);
    const formValue = this.addOrEditForm.value;

    if (this.isEdit) {
      this.productService.update(this.selectedId, formValue).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadProducts();
          this.resetForm();
          Swal.fire({
            icon: 'success',
            title: 'Produit mis à jour',
            text: 'Les informations ont été mises à jour avec succès.',
            timer: 2000,
            showConfirmButton: false,
          });
        },
        error: (err) => {
          this.submitting.set(false);
          Swal.fire({
            icon: 'error',
            title: 'Erreur',
            text: err?.error?.message || 'Une erreur est survenue lors de la mise à jour du produit.',
          });
        }
      });
    } else {
      this.productService.post(formValue).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadProducts();
          this.resetForm();
          Swal.fire({
            icon: 'success',
            title: 'Produit ajouté',
            text: 'Le produit a été ajouté avec succès.',
            timer: 2000,
            showConfirmButton: false,
          });
        },
        error: (err: any) => {
          this.submitting.set(false);
          Swal.fire({
            icon: 'error',
            title: 'Erreur',
            text: err?.error?.message || "Une erreur est survenue lors de l'ajout du produit.",
          });
        }
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
        this.productService.delete(id).subscribe({
          next: () => {
            this.loadProducts();
            Swal.fire({
              icon: 'success',
              title: 'Supprimé',
              text: 'Le produit a été supprimé.',
              timer: 1500,
              showConfirmButton: false,
            });
          },
          error: (err) => {
            console.error('Delete error:', err);
            Swal.fire({
              icon: 'error',
              title: 'Erreur',
              text: 'Impossible de supprimer ce produit.',
            });
          }
        });
      }
    });
  }

  resetForm(): void {
    this.addOrEditForm.reset({
      id: '00000000-0000-0000-0000-000000000000',
      reference: '',           // Empty when adding new
      unitPriceHT: 0,
      stockQuantity: 0,
    });
  }

  trackById(index: number, product: ProductModel): string {
    return product.id;
  }
}