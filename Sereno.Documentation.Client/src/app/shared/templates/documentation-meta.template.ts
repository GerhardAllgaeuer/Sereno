import { Directive, TemplateRef, Input } from '@angular/core';

@Directive({
  selector: '[documentationMeta]',
  standalone: true
})
export class DocumentationMetaTemplate {
  @Input() documentationMeta: TemplateRef<any>;
} 