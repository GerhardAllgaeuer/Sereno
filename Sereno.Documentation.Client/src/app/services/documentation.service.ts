import { Injectable } from '@angular/core';
import { Observable, of, delay } from 'rxjs';
import { Documentation } from '../models/documentation.model';

@Injectable({
  providedIn: 'root'
})
export class DocumentationService {
  private dummyDocumentations: Documentation[] = [
    {
      id: 1,
      title: 'Angular Komponenten',
      content: 'Angular Komponenten sind die grundlegenden Bausteine einer Angular-Anwendung. Sie bestehen aus einem Template, einer Klasse und Metadaten. Das Template definiert die Benutzeroberfläche, die Klasse enthält die Logik und die Metadaten stellen zusätzliche Informationen bereit.',
      topic: 'Angular',
      createdAt: new Date('2023-01-15'),
      updatedAt: new Date('2023-02-20')
    },
    {
      id: 2,
      title: 'TypeScript Grundlagen',
      content: 'TypeScript ist eine von Microsoft entwickelte Programmiersprache, die auf JavaScript aufbaut und es um statische Typisierung erweitert. TypeScript-Code wird zu JavaScript kompiliert und kann in jedem Browser oder JavaScript-Laufzeitumgebung ausgeführt werden.',
      topic: 'TypeScript',
      createdAt: new Date('2023-03-10'),
      updatedAt: new Date('2023-03-15')
    },
    {
      id: 3,
      title: 'CSS Flexbox Layout',
      content: 'Flexbox ist ein CSS-Layout-Modell, das es ermöglicht, Elemente in einer flexiblen und responsiven Weise anzuordnen. Es bietet eine einfache Möglichkeit, Elemente horizontal oder vertikal auszurichten und den verfügbaren Platz zwischen ihnen zu verteilen.',
      topic: 'CSS',
      createdAt: new Date('2023-04-05'),
      updatedAt: new Date('2023-04-10')
    },
    {
      id: 4,
      title: 'Angular Routing',
      content: 'Angular Routing ermöglicht die Navigation zwischen verschiedenen Ansichten in einer Single-Page-Application. Es unterstützt die Verwendung von URL-Segmenten, Query-Parametern und Route-Guards für die Zugriffskontrolle.',
      topic: 'Angular',
      createdAt: new Date('2023-05-20'),
      updatedAt: new Date('2023-05-25')
    },
    {
      id: 5,
      title: 'JavaScript Promises',
      content: 'Promises in JavaScript sind Objekte, die den eventuellen Abschluss oder Fehler einer asynchronen Operation repräsentieren. Sie ermöglichen es, asynchronen Code auf eine sauberere Weise zu schreiben als mit Callbacks.',
      topic: 'JavaScript',
      createdAt: new Date('2023-06-15'),
      updatedAt: new Date('2023-06-20')
    }
  ];

  constructor() { }

  getAllDocumentations(): Observable<Documentation[]> {
    // Simuliere einen API-Aufruf mit einer Verzögerung von 500ms
    return of(this.dummyDocumentations).pipe(delay(500));
  }

  getDocumentationsByTopic(topic: string): Observable<Documentation[]> {
    const filteredDocs = this.dummyDocumentations.filter(doc => 
      doc.topic.toLowerCase() === topic.toLowerCase()
    );
    return of(filteredDocs).pipe(delay(500));
  }

  getDocumentationById(id: number): Observable<Documentation | undefined> {
    const doc = this.dummyDocumentations.find(doc => doc.id === id);
    return of(doc).pipe(delay(500));
  }

  searchDocumentations(query: string): Observable<Documentation[]> {
    if (!query.trim()) {
      return of([]);
    }
    
    const lowercaseQuery = query.toLowerCase();
    const filteredDocs = this.dummyDocumentations.filter(doc => 
      doc.title.toLowerCase().includes(lowercaseQuery) || 
      doc.content.toLowerCase().includes(lowercaseQuery)
    );
    
    return of(filteredDocs).pipe(delay(500));
  }
} 