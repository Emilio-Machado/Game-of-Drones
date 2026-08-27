import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { GameService } from './services/game.service';
import Swal from 'sweetalert2';
import { GlobalService } from './services/global.service';
import { StartGame } from './components/start-game/start-game.component';
import { DroneAnimationService } from './services/drone-animation.service';


@Component({
    selector: 'app-root',
    imports: [RouterOutlet],
    templateUrl: './app.component.html',
    changeDetection: ChangeDetectionStrategy.Eager,
    styleUrl: './app.component.css'
})
export class AppComponent {
  private readonly droneAnimation = inject(DroneAnimationService);

  title = 'Juego de Drones';
  isStartScreen = false;
  readonly activePlayer = this.droneAnimation.activePlayer;

  constructor(private router: Router, private gameService: GameService, private globalService: GlobalService) { }

  onRouteActivated(component: unknown): void {
    this.isStartScreen = component instanceof StartGame;
  }

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
