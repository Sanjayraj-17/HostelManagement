
using Hostel_Room_Management.Data;
using Hostel_Room_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Hostel_Room_Management.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/rooms
        // GET: api/rooms?availableOnly=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms(
            [FromQuery] bool availableOnly = false)
        {
            var query = _context.Rooms.AsNoTracking();

            if (availableOnly)
            {
                query = query.Where(
                    r => r.OccupiedBeds < r.Capacity);
            }

            var rooms = await query
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            return Ok(rooms);
        }

        // GET: api/rooms/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var room = await _context.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound(new
                {
                    message = "Room not found."
                });
            }

            return Ok(room);
        }

        // POST: api/rooms
        [HttpPost]
        public async Task<ActionResult<Room>> CreateRoom(Room room)
        {
            room.Id = 0;
            room.RoomNumber = room.RoomNumber.Trim();
            room.RoomType = room.RoomType.Trim();

            _context.Rooms.Add(room);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
                when (ex.GetBaseException() is SqlException sql &&
                      (sql.Number == 2601 || sql.Number == 2627))
            {
                return Conflict(new
                {
                    message = "This room number already exists."
                });
            }

            return CreatedAtAction(
                nameof(GetRoom),
                new { id = room.Id },
                room);
        }

        // PUT: api/rooms/1
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Room>> UpdateRoom(
            int id, Room input)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound(new
                {
                    message = "Room not found."
                });
            }

            input.RoomNumber = input.RoomNumber.Trim();
            input.RoomType = input.RoomType.Trim();

            room.RoomNumber = input.RoomNumber;
            room.RoomType = input.RoomType;
            room.Capacity = input.Capacity;
            room.OccupiedBeds = input.OccupiedBeds;
            room.MonthlyFee = input.MonthlyFee;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
                when (ex.GetBaseException() is SqlException sql &&
                      (sql.Number == 2601 || sql.Number == 2627))
            {
                return Conflict(new
                {
                    message = "This room number already exists."
                });
            }

            return Ok(room);
        }

        // DELETE: api/rooms/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound(new
                {
                    message = "Room not found."
                });
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
