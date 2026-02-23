import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DocumentoService } from '../../services/documento.service';
import { Documento } from '../../models/documento.model';

@Component({
  selector: 'app-lista-documentos',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './lista-documentos.component.html',
  styleUrl: './lista-documentos.component.css'
})
export class ListaDocumentosComponent implements OnInit {
  documentos: Documento[] = [];
  cargando = false;

  constructor(private svc: DocumentoService) {}

  ngOnInit(): void { this.cargar(); }

  cargar(): void {
    this.cargando = true;
    this.svc.getAll().subscribe({
      next: (data) => { this.documentos = data; this.cargando = false; },
      error: () => { this.cargando = false; }
    });
  }

  eliminar(id: number): void {
    if (!confirm('¿Seguro que deseas eliminar este documento?')) return;
    this.svc.eliminar(id).subscribe(() => this.cargar());
  }

  descargar(id: number): void {
    window.open(this.svc.descargarUrl(id), '_blank');
  }

  formatBytes(bytes?: number): string {
    if (!bytes) return '—';
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1048576) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / 1048576).toFixed(1)} MB`;
  }
}