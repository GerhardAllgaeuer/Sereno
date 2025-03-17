import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { HeaderComponent } from './components/header/header.component';
import { DocumentationListComponent } from './components/documentation-list/documentation-list.component';
import { DocumentationDetailComponent } from './components/documentation-detail/documentation-detail.component';
import { DocumentationSearchComponent } from './components/documentation-search/documentation-search.component';
import { SafeHtmlPipe } from './pipes/safe-html.pipe';

@NgModule({
  imports: [
    BrowserModule,
    AppComponent,
    HeaderComponent,
    DocumentationListComponent,
    DocumentationDetailComponent,
    DocumentationSearchComponent,
    SafeHtmlPipe
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
