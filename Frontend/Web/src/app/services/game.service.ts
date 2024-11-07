import { Injectable } from '@angular/core';
import { GlobalService } from './global.service';
import { Observable } from 'rxjs';
import { HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  constructor(private globalService: GlobalService) { }

  startGame(playerOneName: string, playerTwoName: string): Observable<any> {
    const body = { PlayerOneName: playerOneName, PlayerTwoName: playerTwoName };
    return this.globalService.post<any>('game/start', body);
  }

  saveToken(token: string): void {
    localStorage.setItem('gameToken', token);
  }

  removeToken(): void {
    localStorage.removeItem('gameToken');
  }

  haveToken(): boolean {
    return !!localStorage.getItem('gameToken');
  }

  getGameDetails(): Observable<any> {
    const token = localStorage.getItem('gameToken');
    const headers = { Authorization: `Bearer ${token}` };
    return this.globalService.get('game/details', { headers });
  }

  playMove(playerId: number, moveId: number): Observable<any> {
    const token = localStorage.getItem('gameToken');
    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    const body = { PlayerId: playerId, MoveId: moveId };
    return this.globalService.post<any>('round/play-move', body, { headers });
  }

  getAllMoves(): Observable<any> {
    return this.globalService.get('move/all');
  }

  deleteMove(moveId: number): Observable<any> {
    return this.globalService.delete(`move/${moveId}`);
  }

  addMove(move: { name: string, killMoveId: string }): Observable<any> {
    return this.globalService.post('move/add', move);
  }

  updateMove(move: any): Observable<any> {
    return this.globalService.put('move', move);
  }
}
