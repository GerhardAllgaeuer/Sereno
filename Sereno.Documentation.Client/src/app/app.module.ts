import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { HeaderComponent } from './components/header/header.component';
import { DocumentationListComponent } from './components/documentation-list/documentation-list.component';
import { DocumentationDetailComponent } from './components/documentation-detail/documentation-detail.component';
import { DocumentationSearchComponent } from './components/documentation-search/documentation-search.component';
import { SafeHtmlPipe } from './pipes/safe-html.pipe';
import { DocumentationMetaComponent } from './components/documentation-meta/documentation-meta.component';
import { DocumentationItemComponent } from './components/documentation-item/documentation-item.component';

@NgModule({
  imports: [
    BrowserModule,
    RouterModule
  ],
  declarations: [
    AppComponent,
    HeaderComponent,
    DocumentationListComponent,
    DocumentationDetailComponent,
    DocumentationSearchComponent,
    DocumentationMetaComponent,
    DocumentationItemComponent
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
