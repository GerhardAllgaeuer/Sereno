import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DocumentationListComponent } from './components/documentation-list/documentation-list.component';
import { DocumentationDetailComponent } from './components/documentation-detail/documentation-detail.component';
import { DocumentationSearchComponent } from './components/documentation-search/documentation-search.component';

const routes: Routes = [
  { path: '', component: DocumentationListComponent },
  { path: 'documentation/:id', component: DocumentationDetailComponent },
  { path: 'search', component: DocumentationSearchComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
