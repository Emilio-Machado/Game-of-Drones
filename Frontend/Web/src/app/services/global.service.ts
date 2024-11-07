import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import confetti from 'canvas-confetti';

@Injectable({
  providedIn: 'root'
})
export class GlobalService {
  private baseUrl = environment.apiBaseUrl; // URL base desde el archivo de entorno

  constructor(private http: HttpClient) { }

  get(url: string, options?: any): Observable<any> {
    return this.http.get(`${this.baseUrl}/${url}`, options);
  }

  post<T>(url: string, body: any, options?: { headers?: HttpHeaders }): Observable<T> {
    const headers = options?.headers || new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post<T>(`${this.baseUrl}/${url}`, body, { headers });
  }

  put<T>(url: string, body: any, options?: { headers?: HttpHeaders }): Observable<T> {
    const headers = options?.headers || new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.put<T>(`${this.baseUrl}/${url}`, body, { headers });
  }

  delete<T>(url: string): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl}/${url}`);
  }

  //// Auxiliares ////

  loading(flag: boolean) {
    const loader = document.getElementById("loader");
    if (loader) {
      if (flag) {
        loader.classList.remove("none");
      } else {
        loader.classList.add("none");
      }
    }
  }

  launchConfetti() {
    confetti({
      particleCount: 100,
      spread: 70,
      origin: { y: 0.6 }
    });
  }
}
