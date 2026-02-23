import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { DocumentoService } from '../../services/documento.service';

@Component({
  selector: 'app-subir-documento',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './subir-documento.component.html',
  styleUrl: './subir-documento.component.css'
})
export class SubirDocumentoComponent {
  nombre = '';
  descripcion = '';
  archivoSeleccionado: File | null = null;
  guardando = false;
  error = '';

  constructor(private svc: DocumentoService, private router: Router) {}

  onArchivoChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.archivoSeleccionado = input.files?.[0] ?? null;
  }

  guardar(): void {
    if (!this.nombre.trim() || !this.archivoSeleccionado) {
      this.error = 'El nombre y el archivo son obligatorios.';
      return;
    }
    this.error = '';
    this.guardando = true;

    const formData = new FormData();
    formData.append('nombre', this.nombre);
    formData.append('descripcion', this.descripcion);
    formData.append('archivo', this.archivoSeleccionado);

    this.svc.crear(formData).subscribe({
      next: () => this.router.navigate(['/']),
      error: () => {
        this.error = 'Error al subir el documento. Verifica que el backend esté corriendo.';
        this.guardando = false;
      }
    });
  }
}