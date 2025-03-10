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
  documentation: Documentation | undefined;
  loading: boolean = false;
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
    
    const id = Number(this.route.snapshot.paramMap.get('id'));
    
    if (isNaN(id)) {
      this.error = 'Ungültige Dokumentations-ID';
      this.loading = false;
      return;
    }
    
    this.documentationService.getDocumentationById(id).subscribe({
      next: (doc) => {
        if (doc) {
          this.documentation = doc;
        } else {
          this.error = 'Dokumentation nicht gefunden';
        }
        this.loading = false;
      },
      error: (err) => {
        console.error('Fehler beim Laden der Dokumentation:', err);
        this.error = 'Fehler beim Laden der Dokumentation';
        this.loading = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/']);
  }
} 