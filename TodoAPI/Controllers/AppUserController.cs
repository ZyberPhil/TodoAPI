using System;
using System.Threading.Tasks;
using TodoAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private readonly TodoDbContext _context;

        public AppUserController(TodoDbContext context)
        {
            _context = context;
        }

        // DTO
        public class CreateItemRequest
        {
            public int ItemId { get; set; }
            public string Name { get; set; }
            public string Beschreibung { get; set; }
            public int Status { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime DueDate { get; set; }
            public int UserId { get; set; }
        }

        // POST: api/AppUser/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateItemForUser(CreateItemRequest request)
        {
            try
            {
                var user = await _context.Appusers.FindAsync(request.UserId);
                if (user == null)
                {
                    return NotFound($"User with ID {request.UserId} not found");
                }

                var item = new Item
                {
                    Id = request.ItemId,
                    Name = request.Name,
                    Beschreibung = request.Beschreibung,
                    Status = request.Status,
                    StartDate = request.StartDate,
                    DueDate = request.DueDate
                };

                _context.Items.Add(item);
                user.Items.Add(item);

                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
    }
}