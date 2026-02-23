import { Routes } from '@angular/router';
import { ListaDocumentosComponent } from './components/lista-documentos/lista-documentos.component';
import { SubirDocumentoComponent } from './components/subir-documento/subir-documento.component';
import { EditarDocumentoComponent } from './components/editar-documento/editar-documento.component';

export const routes: Routes = [
  { path: '', component: ListaDocumentosComponent },
  { path: 'subir', component: SubirDocumentoComponent },
  { path: 'editar/:id', component: EditarDocumentoComponent },
  { path: '**', redirectTo: '' }
];