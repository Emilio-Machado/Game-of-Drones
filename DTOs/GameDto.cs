using Game_of_Drones.DataAccess.Models;
using Game_of_Drones.Models;
using System;
using System.Collections.Generic;

namespace Game_of_Drones.DTOs
{
    public class GameDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public required PlayerDto PlayerOne { get; set; }
        public required PlayerDto PlayerTwo { get; set; }
        public Player? Winner { get; set; } // Objeto completo del Winner (puede ser null)

        public List<MoveDto> Moves { get; set; } = new List<MoveDto>(); // Posibles movimientos en el DTO
        public List<RoundDto> Rounds { get; set; } = new List<RoundDto>(); // Historial de rondas en el DTO
    }
}
