using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Game_of_Drones.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Serilog;
using Game_of_Drones.DataAccess;
using Game_of_Drones.DTOs;
using System;

namespace Game_of_Drones.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoundController : ControllerBase
    {
        private readonly GameDbContext _context;

        public RoundController(GameDbContext context)
        {
            _context = context;
        }

        [HttpPost("play-move")]
        public async Task<IActionResult> PlayMove([FromBody] PlayMoveRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Request.Headers["Authorization"]))
                    return Unauthorized(new ApiResponse<string>(false, "No autorizado."));

                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var gameIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "GameId");

                if (gameIdClaim == null)
                    return Unauthorized(new ApiResponse<string>(false, "Token inválido."));

                var gameId = int.Parse(gameIdClaim.Value);
                var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == gameId);

                if (game == null)
                    return NotFound(new ApiResponse<string>(false, "Juego no encontrado."));

                if (game.EndDate != null)
                    return BadRequest(new ApiResponse<string>(false, "El juego ya ha terminado."));

                bool isPlayerOne = game.PlayerOneId == request.PlayerId;
                bool isPlayerTwo = game.PlayerTwoId == request.PlayerId;

                if (!isPlayerOne && !isPlayerTwo)
                    return BadRequest(new ApiResponse<string>(false, "El jugador no pertenece al juego."));

                var move = await _context.Moves
                    .Where(m => m.Id == request.MoveId && m.IsActive)
                    .FirstOrDefaultAsync();

                if (move == null)
                    return BadRequest(new ApiResponse<string>(false, "Movimiento inválido."));

                // Obtener la ronda actual del juego
                var currentRound = await _context.Rounds
                    .Where(r => r.GameId == gameId && (r.PlayerOneMoveId == null || r.PlayerTwoMoveId == null))
                    .OrderBy(r => r.Id)
                    .FirstOrDefaultAsync();

                if (currentRound == null)
                    return NotFound(new ApiResponse<string>(false, "No se encontró una ronda activa."));

                // Marcar el movimiento en la ronda actual
                if (isPlayerOne)
                    currentRound.PlayerOneMoveId = request.MoveId;
                else if (isPlayerTwo)
                    currentRound.PlayerTwoMoveId = request.MoveId;

                // Verificar si ambos jugadores han respondido
                if (currentRound.PlayerOneMoveId != null && currentRound.PlayerTwoMoveId != null)
                {
                    currentRound.EndDate = DateTime.Now;

                    // Evaluar el ganador de la ronda
                    var playerOneMove = await _context.Moves.FindAsync(currentRound.PlayerOneMoveId);
                    var playerTwoMove = await _context.Moves.FindAsync(currentRound.PlayerTwoMoveId);

                    if(playerOneMove != null && playerTwoMove != null)
                    {
                        if (playerOneMove.KillMoveId == playerTwoMove.Id)
                            currentRound.WinnerId = game.PlayerOneId;
                        else if (playerTwoMove.KillMoveId == playerOneMove.Id)
                            currentRound.WinnerId = game.PlayerTwoId;
                        else
                            currentRound.WinnerId = null; // Empate
                    }

                    await _context.SaveChangesAsync();

                    // Verificar si alguien ha ganado tres rondas
                    var playerOneWins = await _context.Rounds.CountAsync(r => r.GameId == gameId && r.WinnerId == game.PlayerOneId);
                    var playerTwoWins = await _context.Rounds.CountAsync(r => r.GameId == gameId && r.WinnerId == game.PlayerTwoId);

                    if (playerOneWins == 3)
                    {
                        game.EndDate = DateTime.Now;
                        game.WinnerId = game.PlayerOneId;

                        // Obtener el nombre del jugador 1
                        var playerOne = await _context.Players.FindAsync(game.PlayerOneId);

                        await _context.SaveChangesAsync();
                        return Ok(new ApiResponse<object>(true, "¡Tenemos un ganador!", new { Winner = $"¡{playerOne!.Name} es el nuevo emperador!" }));
                    }
                    else if (playerTwoWins == 3)
                    {
                        game.EndDate = DateTime.Now;
                        game.WinnerId = game.PlayerTwoId;

                        // Obtener el nombre del jugador 1
                        var playerTwo = await _context.Players.FindAsync(game.PlayerTwoId);

                        await _context.SaveChangesAsync();
                        return Ok(new ApiResponse<object>(true, "¡Tenemos un ganador!", new { Winner = $"¡{playerTwo!.Name} es el nuevo emperador!"}));
                    }

                    // Iniciar una nueva ronda
                    var newRound = new Round
                    {
                        GameId = gameId,
                        StartDate = DateTime.Now
                    };
                    await _context.Rounds.AddAsync(newRound);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _context.SaveChangesAsync();
                }

                return Ok(new ApiResponse<string>(true, "Movimiento registrado exitosamente."));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al registrar el movimiento en el juego.");
                return StatusCode(500, new ApiResponse<string>(false, "Ocurrió un error inesperado al procesar la solicitud."));
            }
        }
    }

    // Clase para manejar el request del movimiento
    public class PlayMoveRequest
    {
        public int PlayerId { get; set; }
        public int MoveId { get; set; }
    }
}
