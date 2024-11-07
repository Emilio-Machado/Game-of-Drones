import { Routes } from '@angular/router';
import { StartGame } from './components/start-game/start-game.component';
import { GameDetail } from './components/game-detail/game-detail.component';
import { ChangeRules } from './components/change-rules/change-rules.component';
import { AuthGuard, NoAuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', component: StartGame },
  { path: 'start', component: StartGame },
  { path: 'game', component: GameDetail, canActivate: [AuthGuard] },
  { path: 'rules', component: ChangeRules, canActivate: [NoAuthGuard] },
  { path: '**', component: StartGame }
];
