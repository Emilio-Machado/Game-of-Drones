import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { GameService } from '../../services/game.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-change-rules',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './change-rules.component.html',
  styleUrls: ['./change-rules.component.css']
})
export class ChangeRules implements OnInit {
  moves: any[] = [];
  newMoveName = '';
  newMoveKills = '';

  constructor(private gameService: GameService, private toastr: ToastrService) { }

  ngOnInit() {
    this.loadMoves();
  }

  loadMoves() {
    this.gameService.getAllMoves().subscribe({
      next: (response) => {
        this.moves = response.data;
      },
      error: (error) => {
        this.toastr.error('Error al cargar movimientos', 'Error');
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
          this.toastr.success(`Movimiento "${moveToDelete}" eliminado.`);
        },
        error: (error) => {
          this.toastr.error(`Error al eliminar el movimiento "${moveToDelete}"`, 'Error');
          console.error(`Error al eliminar el movimiento "${moveToDelete}":`, error);
        }
      });
    } else {
      this.toastr.warning('Deben existir almenos 3 movimientos');
    }
  }

  addMove() {
    if (this.newMoveName && this.newMoveKills) {
      const newMove = { name: this.newMoveName, killMoveId: this.newMoveKills };

      this.gameService.addMove(newMove).subscribe({
        next: () => {
          this.toastr.success('Movimiento agregado');
          this.newMoveName = '';
          this.newMoveKills = '';
          this.loadMoves();
        },
        error: (error) => {
          this.toastr.error('Error al agregar el movimiento', 'Error');
          console.error('Error al agregar el movimiento:', error);
        }
      });
    } else {
      this.toastr.warning('Debes completar los datos');
    }
  }

  updateMove(move: any) {
    const moveName = move.name;
    const killMoveName = this.moves.find(option => option.id == move.killMoveId)?.name;
    this.gameService.updateMove(move).subscribe({
      next: () => {
        this.toastr.success(`Ahora ${moveName} mata a ${killMoveName}.`, `Movimiento actualizado`);
      },
      error: (error) => {
        this.toastr.error(`Error al actualizar el movimiento "${moveName}"`, 'Error');
        console.error(`Error al actualizar el movimiento "${moveName}":`, error);
      }
    });
  }
}
