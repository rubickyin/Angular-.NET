import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Documento } from '../models/documento.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class DocumentoService {
  private api = `${environment.apiUrl}/documentos`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Documento[]> {
    return this.http.get<Documento[]>(this.api);
  }

  getById(id: number): Observable<Documento> {
    return this.http.get<Documento>(`${this.api}/${id}`);
  }

  crear(formData: FormData): Observable<Documento> {
    return this.http.post<Documento>(this.api, formData);
  }

  actualizar(id: number, formData: FormData): Observable<void> {
    return this.http.put<void>(`${this.api}/${id}`, formData);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/${id}`);
  }

  descargarUrl(id: number): string {
    return `${this.api}/${id}/descargar`;
  }
}