import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private router: Router) { }

  canActivate(): boolean {
    const token = localStorage.getItem('gameToken');
    if (token) {
      return true;
    } else {
      this.router.navigate(['/start']);
      return false;
    }
  }
}

@Injectable({
  providedIn: 'root'
})
export class NoAuthGuard implements CanActivate {
  constructor(private router: Router) { }

  canActivate(): boolean {
    const token = localStorage.getItem('gameToken');
    if (!token) {
      return true;
    } else {
      this.router.navigate(['/game']);
      return false;
    }
  }
}
