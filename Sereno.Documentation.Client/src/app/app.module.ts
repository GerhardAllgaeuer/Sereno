import { HttpClientModule } from "@angular/common/http";
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HeaderComponent } from './components/header/header.component';
import { DocumentationListComponent } from './components/documentation-list/documentation-list.component';
import { DocumentationDetailComponent } from './components/documentation-detail/documentation-detail.component';
import { DocumentationSearchComponent } from './components/documentation-search/documentation-search.component';

@NgModule({
  declarations: [],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    ReactiveFormsModule,
    AppComponent,
    HeaderComponent,
    DocumentationListComponent,
    DocumentationDetailComponent,
    DocumentationSearchComponent
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
