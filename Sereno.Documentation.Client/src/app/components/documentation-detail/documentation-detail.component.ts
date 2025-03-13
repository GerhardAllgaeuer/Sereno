import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Documentation } from '../../models/documentation.model';
import { DocumentationService } from '../../services/documentation.service';

@Component({
  selector: 'app-documentation-detail',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './documentation-detail.component.html',
  styleUrls: ['./documentation-detail.component.css']
})
export class DocumentationDetailComponent implements OnInit {
  documentation: Documentation | null = null;
  loading = false;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private documentationService: DocumentationService
  ) { }

  ngOnInit(): void {
    this.loadDocumentation();
  }

  loadDocumentation(): void {
    this.loading = true;
    this.error = null;
    
    const id = this.route.snapshot.paramMap.get('id');
    
    if (!id) {
      this.error = 'Keine ID gefunden';
      this.loading = false;
      return;
    }

    this.documentationService.getDocumentationById(id).subscribe({
      next: (doc) => {
        this.documentation = doc;
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Fehler beim Laden der Dokumentation';
        this.loading = false;
        console.error('Error loading documentation:', error);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/documentations']);
  }
} 