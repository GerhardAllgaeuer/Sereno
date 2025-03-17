import { Routes } from '@angular/router';
import { DocumentationListComponent } from './components/documentation-list/documentation-list.component';
import { DocumentationDetailComponent } from './components/documentation-detail/documentation-detail.component';

export const routes: Routes = [
  { 
    path: '', 
    redirectTo: 'documentation', 
    pathMatch: 'full' 
  },
  { 
    path: 'documentation', 
    component: DocumentationListComponent 
  },
  { 
    path: 'documentation/:id', // Diese Route ermöglicht den Zugriff auf einzelne Dokumente
    component: DocumentationDetailComponent 
  },
  { 
    path: '**', 
    redirectTo: 'documentation' 
  }
]; 