using System;

namespace Game_of_Drones.Models
{
    public class Game
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PlayerOneId { get; set; }
        public int PlayerTwoId { get; set; }
        public int? WinnerId { get; set; }
    }
}
