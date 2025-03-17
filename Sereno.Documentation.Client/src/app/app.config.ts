import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { DocumentationListComponent } from './components/documentation-list/documentation-list.component';
import { DocumentationSearchComponent } from './components/documentation-search/documentation-search.component';
import { DocumentationDetailComponent } from './components/documentation-detail/documentation-detail.component';
import { HeaderComponent } from './components/header/header.component';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes)
  ]
}; 