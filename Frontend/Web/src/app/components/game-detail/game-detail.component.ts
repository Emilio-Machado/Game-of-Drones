import { Component, OnInit, OnDestroy, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { GameService } from '../../services/game.service';
import { NotificationService } from '../../services/notification.service';
import { FormsModule } from '@angular/forms';

import { GlobalService } from '../../services/global.service';
import { Observer } from 'rxjs';
import { Router } from '@angular/router';
import { DroneAnimationService } from '../../services/drone-animation.service';

@Component({
    selector: 'app-game-detail',
    templateUrl: './game-detail.component.html',
    styleUrls: ['./game-detail.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [FormsModule]
})
export class GameDetail implements OnInit, OnDestroy {
  gameData: any = null;
  selectedMove: any = null;

  constructor(private globalService: GlobalService, private gameService: GameService,
    private notifications: NotificationService, private router: Router, private cdr: ChangeDetectorRef,
    private droneAnimation: DroneAnimationService) { }

  ngOnInit() {
    this.loadGameDetails(false);
  }

  ngOnDestroy(): void {
    this.droneAnimation.clear();
  }

  loadGameDetails(showTurn = true) {
    this.gameService.getGameDetails().subscribe({
      next: (response) => {
        this.gameData = response.data;
        this.cdr.detectChanges();
        const nextTurnPlayer = this.gameData.rounds[this.gameData.rounds.length - 1]?.nextTurnPlayer;

        if (nextTurnPlayer && !this.gameData.winner) {
          this.droneAnimation.setTurn(
            nextTurnPlayer.id,
            this.gameData.playerOne.id,
            this.gameData.playerTwo.id
          );
        } else {
          this.droneAnimation.clear();
        }

        if (nextTurnPlayer && showTurn) {
          this.notifications.success('Turno de ' + nextTurnPlayer.name, '¡Movimiento registrado!');
          return;
        }
      },
      error: (error) => {
        this.notifications.error('Error al obtener los detalles del juego');
        console.error('Error en la solicitud:', error);
      }
    });
  }

  submitMove() {
    // Obtengo el jugador del siguiente turno de la última ronda
    const nextTurnPlayer = this.gameData.rounds[this.gameData.rounds.length - 1]?.nextTurnPlayer;

    if (!nextTurnPlayer) {
      this.notifications.warning('No se ha determinado el siguiente turno');
      return;
    }

    if (!this.selectedMove) {
      this.notifications.warning('Por favor selecciona un movimiento', nextTurnPlayer.name);
      return;
    }

    this.globalService.loading(true);

    this.gameService.playMove(nextTurnPlayer.id, this.selectedMove.id).subscribe({
      next: (response) => {
        this.globalService.loading(false);
        if (response.success) {
          if (response.data && response.data.winner) {
            this.notifications.success(response.data.winner, '¡Tenemos un ganador!');
            this.globalService.launchConfetti();
          }
          this.loadGameDetails();
          this.selectedMove = null;
        }
      },
      error: (error) => {
        this.globalService.loading(false);
        this.notifications.error('Error al registrar el movimiento');
        console.error('Error en la solicitud:', error);
      }
    });
  }

  reinitiate() {
    this.globalService.loading(true);
    const observer: Observer<any> = {
      next: (response) => {
        if (response.success) {
          // Guarda el nuevo token en localStorage
          this.gameService.saveToken(response.data);
          this.loadGameDetails(false);
          this.notifications.success('Turno de ' + this.gameData.playerOne.name.trim(), 'Juego reiniciado');
        } else {
          this.notifications.error(`Error al reiniciado el juego: ${response.message}`);
        }
      },
      error: (response) => {
        console.log(response.error)
        if (response.error && response.error.message) {
          this.notifications.error(response.error.message);
        } else {
          this.notifications.error('Error en la solicitud. Inténtalo de nuevo.');
        }
        this.globalService.loading(false);
      },
      complete: () => {
        this.globalService.loading(false);
      }
    };

    this.gameService.startGame(this.gameData.playerOne.name, this.gameData.playerTwo.name).subscribe(observer);
  }


  changeMoves() {
    this.gameService.removeToken();
    this.router.navigate(['/rules']);
  }

  home() {
    this.gameService.removeToken();
    this.router.navigate(['/']);
  }
}
