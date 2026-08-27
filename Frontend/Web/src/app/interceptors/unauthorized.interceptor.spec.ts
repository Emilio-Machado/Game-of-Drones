import { HttpErrorResponse, HttpRequest } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { throwError } from 'rxjs';
import { unauthorizedInterceptor } from './unauthorized.interceptor';

describe('unauthorizedInterceptor', () => {
  const navigate = vi.fn();

  beforeEach(() => {
    navigate.mockReset();
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [{ provide: Router, useValue: { navigate } }]
    });
  });

  afterEach(() => localStorage.clear());

  it('clears the token, redirects to start, and preserves a 401 error', () => {
    const originalError = new HttpErrorResponse({ status: 401, statusText: 'Unauthorized' });
    localStorage.setItem('gameToken', 'expired-token');

    TestBed.runInInjectionContext(() =>
      unauthorizedInterceptor(
        new HttpRequest('GET', '/api/protected'),
        () => throwError(() => originalError)
      )
    ).subscribe({
      error: error => expect(error).toBe(originalError)
    });

    expect(localStorage.getItem('gameToken')).toBeNull();
    expect(navigate).toHaveBeenCalledOnce();
    expect(navigate).toHaveBeenCalledWith(['/start']);
  });

  it('leaves the session untouched for non-401 errors', () => {
    const originalError = new HttpErrorResponse({ status: 500, statusText: 'Server Error' });
    localStorage.setItem('gameToken', 'valid-token');

    TestBed.runInInjectionContext(() =>
      unauthorizedInterceptor(
        new HttpRequest('GET', '/api/protected'),
        () => throwError(() => originalError)
      )
    ).subscribe({
      error: error => expect(error).toBe(originalError)
    });

    expect(localStorage.getItem('gameToken')).toBe('valid-token');
    expect(navigate).not.toHaveBeenCalled();
  });
});
