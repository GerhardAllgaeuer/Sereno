import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-documentation-item',
  templateUrl: './documentation-item.component.html',
  styleUrls: ['./documentation-item.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    RouterModule
  ]
})
export class DocumentationItemComponent {
  @Input() documentation: any;
  @Input() showLibrary: boolean = false;

  truncateContent(content: string): string {
    if (!content) return '';
    return content.length > 200 ? content.substring(0, 200) + '...' : content;
  }

  copyToClipboard(path: string): void {
    navigator.clipboard.writeText(path);
  }
} 