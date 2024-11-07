using Game_of_Drones.DataAccess.Models;
using Game_of_Drones.Models;

namespace Game_of_Drones.DTOs
{
    public class RoundDto
    {
        public int Id { get; set; }

        // Información del ganador de la ronda
        public Player? Winner { get; set; }

        // Jugador que debe hacer el siguiente movimiento
        public Player? NextTurnPlayer { get; set; }
    }
}
