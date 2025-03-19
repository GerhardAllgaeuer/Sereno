import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-documentation-meta',
  templateUrl: './documentation-meta.component.html',
  styleUrls: ['./documentation-meta.component.scss'],
  standalone: true,
  imports: [CommonModule]
})
export class DocumentationMetaComponent {
  @Input() author?: string;
  @Input() updatedAt?: Date;
  @Input() libraryPath?: string;

  copyToClipboard(text: string) {
    navigator.clipboard.writeText(text);
  }
} 