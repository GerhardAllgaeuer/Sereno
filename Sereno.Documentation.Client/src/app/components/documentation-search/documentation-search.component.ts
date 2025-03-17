import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { debounceTime, distinctUntilChanged, switchMap, catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { Documentation } from '../../models/documentation.model';
import { DocumentationService } from '../../services/documentation.service';

@Component({
  selector: 'app-documentation-search',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './documentation-search.component.html',
  styleUrls: ['./documentation-search.component.scss']
})
export class DocumentationSearchComponent implements OnInit {
  searchControl = new FormControl('');
  searchResults: Documentation[] = [];
  loading: boolean = false;
  hasSearched: boolean = false;

  constructor(private documentationService: DocumentationService) { }

  ngOnInit(): void {
    this.setupSearch();
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