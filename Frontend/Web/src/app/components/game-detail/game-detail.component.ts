import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { GameService } from '../../services/game.service';
import { ToastrService } from 'ngx-toastr';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { GlobalService } from '../../services/global.service';
import { Observer } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-game-detail',
  templateUrl: './game-detail.component.html',
  styleUrls: ['./game-detail.component.css'],
  standalone: true,
  imports: [FormsModule, CommonModule]
})
export class GameDetail implements OnInit {
  gameData: any = null;
  selectedMove: any = null;

  constructor(private globalService: GlobalService, private gameService: GameService,
    private toastr: ToastrService, private router: Router, private cdr: ChangeDetectorRef) { }

  ngOnInit() {
    this.loadGameDetails(false);
  }

  loadGameDetails(showTurn = true) {
    this.gameService.getGameDetails().subscribe({
      next: (response) => {
        this.gameData = response.data;
        this.cdr.detectChanges();
        const nextTurnPlayer = this.gameData.rounds[this.gameData.rounds.length - 1]?.nextTurnPlayer;

        if (nextTurnPlayer && showTurn) {
          this.toastr.success('Turno de ' + nextTurnPlayer.name, '¡Movimiento registrado!');
          return;
        }
      },
      error: (error) => {
        this.toastr.error('Error al obtener los detalles del juego');
        console.error('Error en la solicitud:', error);
      }
    });
  }

  submitMove() {
    // Obtengo el jugador del siguiente turno de la última ronda
    const nextTurnPlayer = this.gameData.rounds[this.gameData.rounds.length - 1]?.nextTurnPlayer;

    if (!nextTurnPlayer) {
      this.toastr.warning('No se ha determinado el siguiente turno');
      return;
    }

    if (!this.selectedMove) {
      this.toastr.warning('Por favor selecciona un movimiento', nextTurnPlayer.name);
      return;
    }

    this.globalService.loading(true);

    this.gameService.playMove(nextTurnPlayer.id, this.selectedMove.id).subscribe({
      next: (response) => {
        this.globalService.loading(false);
        if (response.success) {
          if (response.data && response.data.winner) {
            this.toastr.success(response.data.winner, '¡Tenemos un ganador!');
            this.globalService.launchConfetti();
          }
          this.loadGameDetails();
          this.selectedMove = null;
        }
      },
      error: (error) => {
        this.globalService.loading(false);
        this.toastr.error('Error al registrar el movimiento');
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
          this.toastr.success('Turno de ' + this.gameData.playerOne.name.trim(), 'Juego reiniciado');
        } else {
          this.toastr.error(`Error al reiniciado el juego: ${response.message}`);
        }
      },
      error: (response) => {
        console.log(response.error)
        if (response.error && response.error.message) {
          this.toastr.error(response.error.message);
        } else {
          this.toastr.error('Error en la solicitud. Inténtalo de nuevo.');
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
