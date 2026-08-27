import { Injectable, signal } from '@angular/core';

export type ActivePlayer = 1 | 2 | null;

@Injectable({
  providedIn: 'root'
})
export class DroneAnimationService {
  readonly activePlayer = signal<ActivePlayer>(null);

  setTurn(nextPlayerId: number, playerOneId: number, playerTwoId: number): void {
    const activePlayer = nextPlayerId === playerOneId
      ? 1
      : nextPlayerId === playerTwoId
        ? 2
        : null;

    this.activePlayer.set(activePlayer);
  }

  clear(): void {
    this.activePlayer.set(null);
  }
}
