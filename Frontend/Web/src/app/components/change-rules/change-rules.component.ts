import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { GameService } from '../../services/game.service';
import { NotificationService } from '../../services/notification.service';

@Component({
    selector: 'app-change-rules',
    imports: [FormsModule],
    templateUrl: './change-rules.component.html',
    changeDetection: ChangeDetectionStrategy.Eager,
    styleUrls: ['./change-rules.component.css']
})
export class ChangeRules implements OnInit {
  moves: any[] = [];
  newMoveName = '';
  newMoveKills = '';

  constructor(private gameService: GameService, private notifications: NotificationService) { }

  ngOnInit() {
    this.loadMoves();
  }

  loadMoves() {
    this.gameService.getAllMoves().subscribe({
      next: (response) => {
        this.moves = response.data;
      },
      error: (error) => {
        this.notifications.error('Error al cargar movimientos', 'Error');
        console.error('Error al cargar movimientos:', error);
      }
    });
  }

  getMoveOptions(currentMove: any) {
    return this.moves.filter(move => move.id !== currentMove.id);
  }

  removeMove(moveId: number) {
    if (this.moves.length > 3) {
      const moveToDelete = this.moves.find(move => move.id === moveId)?.name;
      this.moves = this.moves.filter(move => move.id !== moveId);

      this.gameService.deleteMove(moveId).subscribe({
        next: () => {
          this.notifications.success(`Movimiento "${moveToDelete}" eliminado.`);
        },
        error: (error) => {
          this.notifications.error(`Error al eliminar el movimiento "${moveToDelete}"`, 'Error');
          console.error(`Error al eliminar el movimiento "${moveToDelete}":`, error);
        }
      });
    } else {
      this.notifications.warning('Deben existir almenos 3 movimientos');
    }
  }

  addMove() {
    if (this.newMoveName && this.newMoveKills) {
      const newMove = { name: this.newMoveName, killMoveId: this.newMoveKills };

      this.gameService.addMove(newMove).subscribe({
        next: () => {
          this.notifications.success('Movimiento agregado');
          this.newMoveName = '';
          this.newMoveKills = '';
          this.loadMoves();
        },
        error: (error) => {
          this.notifications.error('Error al agregar el movimiento', 'Error');
          console.error('Error al agregar el movimiento:', error);
        }
      });
    } else {
      this.notifications.warning('Debes completar los datos');
    }
  }

  updateMove(move: any) {
    const moveName = move.name;
    const killMoveName = this.moves.find(option => option.id == move.killMoveId)?.name;
    this.gameService.updateMove(move).subscribe({
      next: () => {
        this.notifications.success(`Ahora ${moveName} mata a ${killMoveName}.`, `Movimiento actualizado`);
      },
      error: (error) => {
        this.notifications.error(`Error al actualizar el movimiento "${moveName}"`, 'Error');
        console.error(`Error al actualizar el movimiento "${moveName}":`, error);
      }
    });
  }
}
