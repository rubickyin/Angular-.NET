import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { DocumentoService } from '../../services/documento.service';

@Component({
  selector: 'app-editar-documento',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './editar-documento.component.html'
})
export class EditarDocumentoComponent implements OnInit {
  id = 0;
  nombre = '';
  descripcion = '';
  archivoSeleccionado: File | null = null;
  guardando = false;
  cargando = true;
  error = '';

  constructor(
    private svc: DocumentoService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.svc.getById(this.id).subscribe({
      next: (doc) => {
        this.nombre = doc.nombre;
        this.descripcion = doc.descripcion ?? '';
        this.cargando = false;
      },
      error: () => {
        this.error = 'No se pudo cargar el documento.';
        this.cargando = false;
      }
    });
  }

  onArchivoChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.archivoSeleccionado = input.files?.[0] ?? null;
  }

  guardar(): void {
    if (!this.nombre.trim()) { this.error = 'El nombre es obligatorio.'; return; }
    this.guardando = true;
    const formData = new FormData();
    formData.append('nombre', this.nombre);
    formData.append('descripcion', this.descripcion);
    if (this.archivoSeleccionado) formData.append('archivo', this.archivoSeleccionado);

    this.svc.actualizar(this.id, formData).subscribe({
      next: () => this.router.navigate(['/']),
      error: () => { this.error = 'Error al actualizar.'; this.guardando = false; }
    });
  }
}