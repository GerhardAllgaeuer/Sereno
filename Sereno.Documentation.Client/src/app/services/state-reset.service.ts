import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StateResetService {
  clearStoredStates() {
    sessionStorage.removeItem('documentation-search-state');
    sessionStorage.removeItem('documentation-list-state');
  }
} 