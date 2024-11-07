using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Game_of_Drones.DataAccess;
using Game_of_Drones.DTOs;
using Game_of_Drones.Models;

namespace Game_of_Drones.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoveController : ControllerBase
    {
        private readonly GameDbContext _context;

        public MoveController(GameDbContext context)
        {
            _context = context;
        }

        // Endpoint para obtener todos los movimientos activos
        [HttpGet("all")]
        public async Task<IActionResult> GetAllMoves()
        {
            try
            {
                var moves = await _context.Moves
                    .Where(m => m.IsActive)
                    .ToListAsync();

                return Ok(new ApiResponse<List<Move>>(true, "Movimientos obtenidos exitosamente.", moves));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener la lista de movimientos.");
                return StatusCode(500, new ApiResponse<string>(false, "Ocurrió un error inesperado al obtener la lista de movimientos."));
            }
        }

        // Endpoint para actualizar un movimiento existente
        [HttpPut]
        public async Task<IActionResult> UpdateMove([FromBody] UpdateMoveDto moveDto)
        {
            try
            {
                var existingMove = await _context.Moves.FindAsync(moveDto.Id);
                if (existingMove == null)
                {
                    return NotFound(new ApiResponse<string>(false, "Movimiento no encontrado."));
                }

                // Actualiza solo el KillMoveId
                existingMove.KillMoveId = moveDto.KillMoveId;
                existingMove.DateModified = DateTime.UtcNow;

                _context.Moves.Update(existingMove);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<Move>(true, "Movimiento actualizado exitosamente.", existingMove));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al actualizar el movimiento con ID {MoveId}", moveDto.Id);
                return StatusCode(500, new ApiResponse<string>(false, "Ocurrió un error inesperado al actualizar el movimiento."));
            }
        }


        // Endpoint para borrado lógico de un movimiento
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMove(int id)
        {
            try
            {
                var move = await _context.Moves.FindAsync(id);
                if (move == null)
                {
                    return NotFound(new ApiResponse<string>(false, "Movimiento no encontrado."));
                }

                move.IsActive = false;
                move.DateModified = DateTime.UtcNow;

                _context.Moves.Update(move);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<string>(true, "Movimiento eliminado exitosamente."));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al realizar el borrado lógico del movimiento.");
                return StatusCode(500, new ApiResponse<string>(false, "Ocurrió un error inesperado al eliminar el movimiento."));
            }
        }

        // Endpoint para agregar o reactivar un movimiento
        [HttpPost("add")]
        public async Task<IActionResult> AddOrReactivateMove([FromBody] UpdateMoveDto moveDto)
        {
            try
            {
                // Busca el movimiento por nombre (ignorando mayúsculas/minúsculas)
                var existingMove = await _context.Moves
                    .FirstOrDefaultAsync(m => m.Name.ToLower() == moveDto.Name!.ToLower());

                if (existingMove != null)
                {
                    // Si el movimiento existe pero está inactivo, lo reactiva
                    if (!existingMove.IsActive)
                    {
                        existingMove.IsActive = true;
                        existingMove.DateModified = DateTime.UtcNow;
                        existingMove.KillMoveId = moveDto.KillMoveId;

                        _context.Moves.Update(existingMove);
                        await _context.SaveChangesAsync();

                        return Ok(new ApiResponse<string>(true, "Movimiento reactivado exitosamente."));
                    }

                    return BadRequest(new ApiResponse<string>(false, "El movimiento ya existe y está activo."));
                }

                // Si no existe, crea un nuevo movimiento
                var newMove = new Move
                {
                    Name = moveDto.Name!,
                    KillMoveId = moveDto.KillMoveId,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true
                };

                await _context.Moves.AddAsync(newMove);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<string>(true, "Movimiento agregado exitosamente."));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al agregar o reactivar el movimiento.");
                return StatusCode(500, new ApiResponse<string>(false, "Ocurrió un error inesperado al agregar o reactivar el movimiento."));
            }
        }
    }
}
