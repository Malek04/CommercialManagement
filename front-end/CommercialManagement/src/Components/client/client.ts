import { Component, OnInit, AfterViewInit, ViewChild, ElementRef, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Client as ClientModel } from '../../Core/Models/client';
import { ClientService } from '../../Core/Services/client-serivce';
import Swal from 'sweetalert2';
import { Modal } from 'bootstrap';

@Component({
  selector: 'app-client',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './client.html',
  styleUrl: './client.css',
})
export class Client implements OnInit, AfterViewInit {
  @ViewChild('clientModal') clientModalRef!: ElementRef;
  private modalInstance!: Modal;

  clients = signal<ClientModel[]>([]);
  addOrEditForm!: FormGroup;
  
  isEdit = false;
  isView = false;           // ← NEW: View mode
  selectedId = '';
  
  loading = signal(false);
  submitting = signal(false);
  dataLoaded = signal(false);

  constructor(
    private fb: FormBuilder,
    private clientService: ClientService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadClients();
  }

  ngAfterViewInit(): void {
    this.modalInstance = new Modal(this.clientModalRef.nativeElement);
  }

  private initForm(): void {
    this.addOrEditForm = this.fb.group({
      id: ['00000000-0000-0000-0000-000000000000'],
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', Validators.required],
      adresse: this.fb.group({
        rue: [''],
        ville: [''],
        codePostal: [''],
        pays: [''],
      }),
    });
  }

  loadClients(): void {
    this.loading.set(true);
    this.dataLoaded.set(false);

    this.clientService.get().subscribe({
      next: (response: any) => {
        let data: ClientModel[] = [];

        if (Array.isArray(response)) data = response;
        else if (response?.result) data = Array.isArray(response.result) ? response.result : [];
        else if (response?.data) data = Array.isArray(response.data) ? response.data : [];
        else if (response?.clients) data = Array.isArray(response.clients) ? response.clients : [];

        this.clients.set([...data]);
        this.loading.set(false);
        this.dataLoaded.set(true);
      },
      error: (err: any) => {
        this.loading.set(false);
        this.dataLoaded.set(true);
        console.error('❌ Error loading clients:', err);
        Swal.fire({ icon: 'error', title: 'Erreur', text: 'Impossible de charger la liste des clients.' });
      },
    });
  }

  // ✅ Open modal in Add mode
  openAddModal(): void {
    this.isEdit = false;
    this.isView = false;
    this.selectedId = '';
    this.resetForm();
    this.modalInstance.show();
  }

  // ✅ Open modal in Edit mode
  openEditModal(client: ClientModel): void {
    this.isEdit = true;
    this.isView = false;
    this.selectedId = client.id;
    this.patchClient(client);
    this.modalInstance.show();
  }

  // ✅ NEW: Open modal in View (Consulter) mode
  openViewModal(client: ClientModel): void {
    this.isEdit = false;
    this.isView = true;
    this.selectedId = client.id;
    this.patchClient(client);
    this.modalInstance.show();
  }

  private patchClient(client: ClientModel): void {
    this.addOrEditForm.patchValue({
      id: client.id,
      firstName: client.firstName,
      lastName: client.lastName,
      email: client.email,
      phone: client.phone,
      adresse: {
        rue: client.adresse?.rue || '',
        ville: client.adresse?.ville || '',
        codePostal: client.adresse?.codePostal || '',
        pays: client.adresse?.pays || '',
      },
    });
  }

  closeModal(): void {
    this.modalInstance.hide();
    // Reset modes after closing
    setTimeout(() => {
      this.isEdit = false;
      this.isView = false;
    }, 300);
  }

  submit(): void {
    if (this.isView) return; // Should not submit in view mode

    if (this.addOrEditForm.invalid) {
      this.addOrEditForm.markAllAsTouched();
      Swal.fire({ icon: 'warning', title: 'Formulaire incomplet', text: 'Merci de remplir correctement tous les champs obligatoires.' });
      return;
    }

    this.submitting.set(true);
    const formValue = this.addOrEditForm.value;

    if (this.isEdit) {
      this.clientService.update(this.selectedId, formValue).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadClients();
          this.resetForm();
          Swal.fire({ icon: 'success', title: 'Client mis à jour', text: 'Les informations ont été mises à jour avec succès.', timer: 2000, showConfirmButton: false });
        },
        error: (err) => {
          this.submitting.set(false);
          Swal.fire({ icon: 'error', title: 'Erreur', text: err?.error?.message || 'Une erreur est survenue lors de la mise à jour.' });
        }
      });
    } else {
      this.clientService.post(formValue).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadClients();
          this.resetForm();
          Swal.fire({ icon: 'success', title: 'Client ajouté', text: 'Le client a été ajouté avec succès.', timer: 2000, showConfirmButton: false });
        },
        error: (err: any) => {
          this.submitting.set(false);
          Swal.fire({ icon: 'error', title: 'Erreur', text: err?.error?.message || "Une erreur est survenue lors de l'ajout." });
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
        this.clientService.delete(id).subscribe({
          next: () => {
            this.loadClients();
            Swal.fire({ icon: 'success', title: 'Supprimé', text: 'Le client a été supprimé.', timer: 1500, showConfirmButton: false });
          },
          error: () => Swal.fire({ icon: 'error', title: 'Erreur', text: 'Impossible de supprimer ce client.' })
        });
      }
    });
  }

  resetForm(): void {
    this.addOrEditForm.reset({ id: '00000000-0000-0000-0000-000000000000' });
  }

  trackById(index: number, client: ClientModel): string {
    return client.id;
  }
}