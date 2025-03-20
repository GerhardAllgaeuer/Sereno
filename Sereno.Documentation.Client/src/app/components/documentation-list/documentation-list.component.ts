import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Documentation } from '../../models/documentation.model';
import { DocumentationService } from '../../services/documentation.service';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-documentation-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    DatePipe,
    HttpClientModule,
  ],
  providers: [DocumentationService],
  templateUrl: './documentation-list.component.html',
  styleUrls: ['./documentation-list.component.scss']
})
export class DocumentationListComponent implements OnInit, OnDestroy {
  documentations: Documentation[] = [];
  libraries: string[] = [];
  selectedLibrary: string = '';
  loading: boolean = false;
  private readonly STORAGE_KEY = 'documentation-list-state';
  private scrollPosition = 0;

  constructor(
    private documentationService: DocumentationService
  ) { }

  ngOnInit(): void {
    const savedState = sessionStorage.getItem(this.STORAGE_KEY);
    let stateToRestore = null;

    if (savedState) {
      stateToRestore = JSON.parse(savedState);
    }

    this.loading = true;
    this.documentationService.getAllDocumentations().subscribe({
      next: (docs: Documentation[]) => {
        this.documentations = docs;
        this.libraries = [...new Set(docs.map(doc => doc.topic))];

        // Zustand nach dem Laden wiederherstellen
        if (stateToRestore) {
          this.selectedLibrary = stateToRestore.selectedLibrary;
          if (this.selectedLibrary) {
            this.documentations = docs.filter(doc => doc.topic === this.selectedLibrary);
          }
          if (stateToRestore.scrollPosition) {
            setTimeout(() => window.scrollTo(0, stateToRestore.scrollPosition), 100);
          }
        }

        this.loading = false;
      },
      error: (error: Error) => {
        console.error('Fehler beim Laden der Dokumentationen:', error);
        this.loading = false;
      }
    });

    window.addEventListener('scroll', this.saveScrollPosition.bind(this));
  }

  ngOnDestroy() {
    // Zustand speichern
    const state = {
      selectedLibrary: this.selectedLibrary,
      scrollPosition: this.scrollPosition
    };
    sessionStorage.setItem(this.STORAGE_KEY, JSON.stringify(state));
    window.removeEventListener('scroll', this.saveScrollPosition.bind(this));
  }

  private saveScrollPosition() {
    this.scrollPosition = window.scrollY;
  }

  loadDocumentations(): void {
    this.loading = true;
    this.documentationService.getAllDocumentations().subscribe({
      next: (docs: Documentation[]) => {
        this.documentations = docs;
        this.libraries = [...new Set(docs.map(doc => doc.topic))];
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
          this.documentations = docs.filter(doc => doc.topic === library);
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
