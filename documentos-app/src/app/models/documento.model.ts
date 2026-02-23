export interface Documento {
  id: number;
  nombre: string;
  descripcion?: string;
  nombreArchivo: string;
  tipoArchivo?: string;
  tamanio?: number;
  fechaSubida: string;
}