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
  topics: string[] = [];
  selectedTopic: string = '';
  loading: boolean = false;

  constructor(private documentationService: DocumentationService) { }

  ngOnInit(): void {
    this.loadDocumentations();
  }

  loadDocumentations(): void {
    this.loading = true;
    this.documentationService.getAllDocumentations().subscribe({
      next: (docs) => {
        this.documentations = docs;
        // Extrahiere eindeutige Themen für den Filter
        this.topics = [...new Set(docs.map(doc => doc.topic))];
        this.loading = false;
      },
      error: (error) => {
        console.error('Fehler beim Laden der Dokumentationen:', error);
        this.loading = false;
      }
    });
  }

  filterByTopic(topic: string): void {
    this.selectedTopic = topic;
    this.loading = true;
    
    if (topic) {
      this.documentationService.getDocumentationsByTopic(topic).subscribe({
        next: (docs) => {
          this.documentations = docs;
          this.loading = false;
        },
        error: (error) => {
          console.error('Fehler beim Filtern nach Thema:', error);
          this.loading = false;
        }
      });
    } else {
      this.loadDocumentations();
    }
  }

  // Hilfsfunktion, um den Inhalt auf 2-3 Zeilen zu kürzen
  truncateContent(content: string): string {
    const words = content.split(' ');
    if (words.length > 30) {
      return words.slice(0, 30).join(' ') + '...';
    }
    return content;
  }
} 