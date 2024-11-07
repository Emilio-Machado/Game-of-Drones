import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { GameService } from '../../services/game.service';
import { ToastrService } from 'ngx-toastr';
import { Observable, Observer } from 'rxjs';
import { Router } from '@angular/router';
import { GlobalService } from '../../services/global.service';

@Component({
  selector: 'app-start-game',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './start-game.component.html',
  styleUrls: ['./start-game.component.css']
})
export class StartGame {
  playerOneName: string = '';
  playerTwoName: string = '';

  constructor(private globalService: GlobalService, private gameService: GameService, private toastr: ToastrService, private router: Router) { }

  startGame() {
    if (!this.playerOneName.trim() || !this.playerTwoName.trim()) {
      this.toastr.warning('Debes ingresar ambos nombres de los jugadores');
      return;
    }

    this.globalService.loading(true);
    const observer: Observer<any> = {
      next: (response) => {
        if (response.success) {
          // Guarda el token en localStorage
          this.gameService.saveToken(response.data);
          this.toastr.success('Juego iniciado correctamente', 'Turno de ' + this.playerOneName.trim());
          this.router.navigate(['/game']);
        } else {
          this.toastr.error(`Error al iniciar el juego: ${response.message}`);
        }
      },
      error: (response) => {
        console.log(response.error)
        if (response.error && response.error.message) {
          this.toastr.error(response.error.message);
        } else {
          this.toastr.error('Error en la solicitud. Inténtalo de nuevo.');
        }
        console.error('Error en la solicitud:', response.error);
        this.globalService.loading(false);
      },
      complete: () => {
        this.globalService.loading(false);
      }
    };

    this.gameService.startGame(this.playerOneName, this.playerTwoName).subscribe(observer);
  }
}
