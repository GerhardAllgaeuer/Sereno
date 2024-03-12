import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService implements HttpInterceptor {

  constructor(private router: Router) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          let errorMessage = this.handleError(error);
          return throwError(() => new Error(errorMessage));
        })
      )
  }

  private handleError = (error: HttpErrorResponse): string => {
    if (error.status === 404) {
      return this.handleNotFound(error);
    } else if (error.status === 400) {
      return this.handleBadRequest(error);
    } else {
      return "An unexpected error occurred.";
    }
  }


  private handleNotFound = (error: HttpErrorResponse): string => {
    this.router.navigate(['/404']);
    return error.message;
  }

  private handleBadRequest = (error: HttpErrorResponse): string => {
    // Prüfen, ob das 'errors'-Feld im 'error.error'-Objekt existiert
    if (error.error && 'errors' in error.error) {
      let message = '';
      // Typzusicherung, dass 'errors' ein Objekt mit Strings als Werte ist
      const errors = error.error.errors as { [key: string]: string };
      const values = Object.values(errors);

      values.forEach((m: string) => {
        message += m + '<br>';
      });

      // Entfernen des letzten '<br>', falls vorhanden
      return message.slice(0, -4);
    } else {
      // Standard-Fehlerbehandlung, wenn kein 'errors'-Feld vorhanden ist
      return error.error ? JSON.stringify(error.error) : error.message;
    }
  }


}
