import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { DocumentationService } from './documentation.service';

@Component({
  selector: 'app-documentation-list',
  standalone: true,
  imports: [
    CommonModule,
    HttpClientModule,
  ],
  providers: [DocumentationService],
  // ... template and styles
})
export class DocumentationListComponent {
  // ... component code
} 