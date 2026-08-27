import { DroneAnimationService } from './drone-animation.service';

describe('DroneAnimationService', () => {
  it('maps each next-turn player to the corresponding drone', () => {
    const service = new DroneAnimationService();

    service.setTurn(10, 10, 20);
    expect(service.activePlayer()).toBe(1);

    service.setTurn(20, 10, 20);
    expect(service.activePlayer()).toBe(2);

    service.clear();
    expect(service.activePlayer()).toBeNull();
  });
});
