import { Component, OnInit, AfterViewInit, ViewChild, ElementRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { debounceTime, distinctUntilChanged, switchMap, catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { Documentation } from '../../models/documentation.model';
import { DocumentationService } from '../../services/documentation.service';
import { DocumentationItemComponent } from '../documentation-item/documentation-item.component';

@Component({
  selector: 'app-documentation-search',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    DocumentationItemComponent
  ],
  templateUrl: './documentation-search.component.html',
  styleUrls: ['./documentation-search.component.scss']
})
export class DocumentationSearchComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('searchInput') searchInput!: ElementRef;
  searchControl = new FormControl('');
  searchResults: Documentation[] = [];
  loading: boolean = false;
  hasSearched: boolean = false;
  private readonly STORAGE_KEY = 'documentation-search-state';
  private scrollPosition = 0;

  constructor(private documentationService: DocumentationService) { }

  ngOnInit(): void {
    const savedState = sessionStorage.getItem(this.STORAGE_KEY);
    if (savedState) {
      const state = JSON.parse(savedState);
      this.searchControl.setValue(state.searchTerm, { emitEvent: false });
      
      // Initial search with restored term
      if (state.searchTerm) {
        this.loading = true;
        this.hasSearched = true;
        this.documentationService.searchDocumentations(state.searchTerm)
          .pipe(
            catchError(error => {
              console.error('Fehler bei der Suche:', error);
              return of([]);
            })
          )
          .subscribe(results => {
            this.searchResults = results;
            this.loading = false;
            // Scroll position after results are loaded
            if (state.scrollPosition) {
              setTimeout(() => window.scrollTo(0, state.scrollPosition), 100);
            }
          });
      }
    }

    // Scroll-Position speichern
    window.addEventListener('scroll', this.saveScrollPosition.bind(this));

    this.setupSearch();
  }

  ngAfterViewInit() {
    setTimeout(() => {
      this.searchInput.nativeElement.focus();
    });
  }

  ngOnDestroy() {
    // Zustand speichern
    const state = {
      searchTerm: this.searchControl.value,
      scrollPosition: this.scrollPosition
    };
    sessionStorage.setItem(this.STORAGE_KEY, JSON.stringify(state));
    window.removeEventListener('scroll', this.saveScrollPosition.bind(this));
  }

  private saveScrollPosition() {
    this.scrollPosition = window.scrollY;
  }

  setupSearch(): void {
    this.searchControl.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(query => {
        if (!query || query.trim() === '') {
          this.searchResults = [];
          this.hasSearched = false;
          this.loading = false;
          return of([]);
        }
        
        this.loading = true;
        this.hasSearched = true;
        return this.documentationService.searchDocumentations(query).pipe(
          catchError(error => {
            console.error('Fehler bei der Suche:', error);
            return of([]);
          })
        );
      })
    ).subscribe(results => {
      this.searchResults = results;
      this.loading = false;
    });
  }

  truncateContent(content: string | null): string {
    if (!content) return '';
    const words = content.split(' ');
    if (words.length > 30) {
      return words.slice(0, 30).join(' ') + '...';
    }
    return content;
  }

  clearSearch(): void {
    this.searchControl.setValue('');
  }
} 