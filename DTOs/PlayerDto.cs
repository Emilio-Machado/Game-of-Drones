namespace Game_of_Drones.DTOs
{
    public class PlayerDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime DateJoined { get; set; }
        public int GamesWon { get; set; }
        public int RoundsWon { get; set; }
    }
}
