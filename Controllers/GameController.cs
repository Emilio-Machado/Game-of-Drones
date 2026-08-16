using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Game_of_Drones.DataAccess;
using Game_of_Drones.Models;
using Microsoft.EntityFrameworkCore;
using Game_of_Drones.DataAccess.Models;
using Serilog;
using Game_of_Drones.DTOs;
using Game_of_Drones.Security;

namespace Game_of_Drones.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly GameDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;

    public GameController(GameDbContext context, IJwtTokenService jwtTokenService)
        => (_context, _jwtTokenService) = (context, jwtTokenService);

    [HttpPost("start")]
    public async Task<IActionResult> StartGame([FromBody] GameStartRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PlayerOneName) || string.IsNullOrWhiteSpace(request.PlayerTwoName))
                return BadRequest(new ApiResponse<string>(false, "Debes ingresar los nombres de los jugadores"));

            // Verifica o crea los jugadores
            var playerOne = await GetOrCreatePlayerAsync(request.PlayerOneName);
            var playerTwo = await GetOrCreatePlayerAsync(request.PlayerTwoName);

            if (playerOne.Id == playerTwo.Id)
                return BadRequest(new ApiResponse<string>(false, "Los jugadores deben ser diferentes"));

            // Inserta un nuevo juego
            var newGame = new Game
            {
                StartDate = DateTime.Now,
                PlayerOneId = playerOne.Id,
                PlayerTwoId = playerTwo.Id
            };

            _context.Games.Add(newGame);
            await _context.SaveChangesAsync();

            // Inicia una nueva ronda para el juego
            var newRound = new Round
            {
                GameId = newGame.Id,
                StartDate = DateTime.Now,
            };

            _context.Rounds.Add(newRound);
            await _context.SaveChangesAsync();

            // Genera el token JWT
            var token = _jwtTokenService.CreateGameToken(newGame.Id);

            return Ok(new ApiResponse<string>(true, "Juego iniciado correctamente", token));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al iniciar el juego.");
            return StatusCode(500, new ApiResponse<string>(false, "Ocurrió un error inesperado al iniciar el juego."));
        }
    }

    [HttpGet("details")]
    [Authorize]
    public async Task<IActionResult> GetGameDetails()
    {
        try
        {
            if (!int.TryParse(User.FindFirst(GameClaims.GameId)?.Value, out var gameId))
                return Unauthorized(new ApiResponse<string>(false, "Token inválido."));

            // Obtiene el juego básico
            var game = await _context.Games
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
                return NotFound(new ApiResponse<string>(false, "Juego no encontrado."));

            // Obtiene los detalles de PlayerOne y PlayerTwo
            var playerOne = await _context.Players.FindAsync(game.PlayerOneId) ?? throw new InvalidOperationException("PlayerOne no encontrado.");
            var playerTwo = await _context.Players.FindAsync(game.PlayerTwoId) ?? throw new InvalidOperationException("PlayerTwo no encontrado.");

            // Calcula los juegos ganados por cada jugador
            var playerOneGamesWon = await _context.Games.CountAsync(g => g.WinnerId == playerOne.Id);
            var playerTwoGamesWon = await _context.Games.CountAsync(g => g.WinnerId == playerTwo.Id);

            // Obtiene los rounds asociados al juego
            var rounds = await _context.Rounds
                .Where(r => r.GameId == gameId)
                .ToListAsync();

            // Calcula las rondas ganadas por cada jugador en el juego actual
            var playerOneRoundsWon = rounds.Count(r => r.WinnerId == playerOne.Id);
            var playerTwoRoundsWon = rounds.Count(r => r.WinnerId == playerTwo.Id);

            // Mapea PlayerOne y PlayerTwo a PlayerDto con juegos y rondas ganadas
            var playerOneDto = new PlayerDto
            {
                Id = playerOne.Id,
                Name = playerOne.Name,
                DateJoined = playerOne.DateJoined,
                GamesWon = playerOneGamesWon,
                RoundsWon = playerOneRoundsWon
            };

            var playerTwoDto = new PlayerDto
            {
                Id = playerTwo.Id,
                Name = playerTwo.Name,
                DateJoined = playerTwo.DateJoined,
                GamesWon = playerTwoGamesWon,
                RoundsWon = playerTwoRoundsWon
            };

            // Prepara la estructura de rounds con detalles de ganador y próximo turno en RoundDto
            var roundsDto = rounds.OrderBy(r => r.Id).Select(r => new RoundDto
            {
                Id = r.Id,
                Winner = r.WinnerId.HasValue ? _context.Players.Find(r.WinnerId.Value) : null,
                NextTurnPlayer = r.PlayerOneMoveId == null
                    ? playerOne // Siguiente turno para PlayerOne
                    : r.PlayerTwoMoveId == null
                        ? playerTwo // Siguiente turno para PlayerTwo
                        : null // Ambos han hecho su movimiento, por lo tanto, no hay turno pendiente
            }).ToList();

            // Obtiene la lista completa de movimientos activos y los convierte en MoveDto
            var activeMovesDto = await _context.Moves
                .Where(m => m.IsActive)
                .Select(m => new MoveDto
                {
                    Id = m.Id,
                    Name = m.Name
                })
                .ToListAsync();

            // Crea el objeto GameDto con todos los detalles
            var gameDto = new GameDto
            {
                Id = game.Id,
                StartDate = game.StartDate,
                EndDate = game.EndDate,
                PlayerOne = playerOneDto,
                PlayerTwo = playerTwoDto,
                Winner = game.WinnerId.HasValue ? await _context.Players.FindAsync(game.WinnerId.Value) : null,
                Moves = activeMovesDto,
                Rounds = roundsDto
            };

            return Ok(new ApiResponse<GameDto>(true, "Detalles del juego obtenidos exitosamente", gameDto));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al obtener los detalles del juego.");
            return StatusCode(500, new ApiResponse<string>(false, "Ocurrió un error inesperado al obtener los detalles del juego."));
        }
    }

    private async Task<Player> GetOrCreatePlayerAsync(string playerName)
    {
        var player = await _context.Players.SingleOrDefaultAsync(p => p.Name == playerName);
        if (player == null)
        {
            player = new Player
            {
                Name = playerName,
                DateJoined = DateTime.Now
            };
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
        }
        return player;
    }

}

public class GameStartRequest
{
    public required string PlayerOneName { get; set; }
    public required string PlayerTwoName { get; set; }
}
