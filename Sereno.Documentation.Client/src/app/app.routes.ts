import { Routes } from '@angular/router';
import { DocumentationListComponent } from './components/documentation-list/documentation-list.component';
import { DocumentationDetailComponent } from './components/documentation-detail/documentation-detail.component';

export const routes: Routes = [
  { path: '', component: DocumentationListComponent },
  { path: 'detail/:id', component: DocumentationDetailComponent },
  // weitere Routen...
]; 