using Game_of_Drones.DataAccess.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Game_of_Drones.Models
{
    public class Round
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        public int GameId { get; set; }

        [ForeignKey("PlayerOneMove")]
        public int? PlayerOneMoveId { get; set; }

        [ForeignKey("PlayerTwoMove")]
        public int? PlayerTwoMoveId { get; set; }

        public int? WinnerId { get; set; } // Opcional, puede estar vacío si la ronda no tiene ganador
    }
}
