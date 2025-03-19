import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Documentation } from '../../models/documentation.model';
import { DocumentationService } from '../../services/documentation.service';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { SafeHtmlPipe } from '../../pipes/safe-html.pipe';
import { DocumentationMetaTemplate } from '../../shared/templates/documentation-meta.template';

@Component({
  selector: 'app-documentation-detail',
  standalone: true,
  imports: [CommonModule, DatePipe, SafeHtmlPipe],
  templateUrl: './documentation-detail.component.html',
  styleUrls: ['./documentation-detail.component.scss']
})
export class DocumentationDetailComponent implements OnInit {
  @ViewChild('documentationMeta') documentationMeta!: DocumentationMetaTemplate;
  documentation!: Documentation;
  loading = false;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private documentationService: DocumentationService,
    private sanitizer: DomSanitizer
  ) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const id = params['id'];
      console.log('Loading documentation with ID:', id);
      this.loadDocumentation();
    });
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

  getSafeHtml(html: string | null): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(html || '');
  }

  copyToClipboard(text: string) {
    navigator.clipboard.writeText(text).then(() => {
      console.log('Pfad in Zwischenablage kopiert');
    }).catch(err => {
      console.error('Fehler beim Kopieren:', err);
    });
  }
} 
