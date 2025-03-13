import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Documentation } from '../models/documentation.model';

@Injectable({
  providedIn: 'root'
})
export class DocumentationService {
  private apiUrl = 'https://localhost:7000/api/documentations'; // Vollständige URL für den DocumentationController

  constructor(private http: HttpClient) { }

  getAllDocumentations(): Observable<Documentation[]> {
    return this.http.get<Documentation[]>(`${this.apiUrl}`);
  }

  getDocumentationsByTopic(topic: string): Observable<Documentation[]> {
    return this.http.get<Documentation[]>(`${this.apiUrl}/topic/${topic}`);
  }

  getDocumentationById(id: string): Observable<Documentation> {
    return this.http.get<Documentation>(`${this.apiUrl}/${id}`);
  }

  getDocumentationsByLibrary(library: string): Observable<Documentation[]> {
    return this.http.get<Documentation[]>(`${this.apiUrl}`, {
      params: { library }
    });
  }

  searchDocumentations(query: string): Observable<Documentation[]> {
    if (!query.trim()) {
      return new Observable(subscriber => {
        subscriber.next([]);
        subscriber.complete();
      });
    }
    
    return this.http.get<Documentation[]>(`${this.apiUrl}/search`, {
      params: { query }
    });
  }
} 
