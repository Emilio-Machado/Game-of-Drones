import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { GameService } from './services/game.service';
import Swal from 'sweetalert2';
import { GlobalService } from './services/global.service';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'Juego de Drones';

  constructor(private router: Router, private gameService: GameService, private globalService: GlobalService) { }

  home() {
    if (this.gameService.haveToken()) {
      Swal.fire({
        title: '¿Estás seguro?',
        text: 'Vas a salir del juego actual. Esta acción no se puede deshacer.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#92c149',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Sí, salir',
        cancelButtonText: 'Cancelar'
      }).then((result) => {
        if (result.isConfirmed) {
          this.gameService.removeToken();
          this.router.navigate(['/']);
        }
      });
    } else {
      if (this.router.url === '/') {
        this.globalService.launchConfetti();
      } else {
        this.router.navigate(['/']);
      }
    }
  }

  

}
