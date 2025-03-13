import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Documentation } from '../../models/documentation.model';
import { DocumentationService } from '../../services/documentation.service';

@Component({
  selector: 'app-documentation-list',
  standalone: true,
  imports: [CommonModule, RouterModule, DatePipe],
  templateUrl: './documentation-list.component.html',
  styleUrls: ['./documentation-list.component.css']
})
export class DocumentationListComponent implements OnInit {
  documentations: Documentation[] = [];
  libraries: string[] = [];
  selectedLibrary: string = '';
  loading: boolean = false;

  constructor(private documentationService: DocumentationService) { }

  ngOnInit(): void {
    this.loadDocumentations();
  }

  loadDocumentations(): void {
    this.loading = true;
    this.documentationService.getAllDocumentations().subscribe({
      next: (docs: Documentation[]) => {
        this.documentations = docs;
        this.libraries = [...new Set(docs.map(doc => doc.libraryPath))];
        this.loading = false;
      },
      error: (error: Error) => {
        console.error('Fehler beim Laden der Dokumentationen:', error);
        this.loading = false;
      }
    });
  }

  filterByLibrary(library: string): void {
    this.selectedLibrary = library;
    this.loading = true;
    
    if (library) {
      this.documentationService.getAllDocumentations().subscribe({
        next: (docs: Documentation[]) => {
          this.documentations = docs.filter(doc => doc.libraryPath === library);
          this.loading = false;
        },
        error: (error: Error) => {
          console.error('Fehler beim Filtern nach Bibliothek:', error);
          this.loading = false;
        }
      });
    } else {
      this.loadDocumentations();
    }
  }

  truncateContent(content: string | null): string {
    if (!content) return '';
    const words = content.split(' ');
    if (words.length > 30) {
      return words.slice(0, 30).join(' ') + '...';
    }
    return content;
  }
} 