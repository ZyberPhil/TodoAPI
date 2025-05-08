using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Models; // hinzufuegen
using Microsoft.EntityFrameworkCore; // hinzufuegen
using System.Linq; // hinzufuegen
using System.Collections.Generic; // hinzufuegen

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly TodoDbContext _context;
        public TestController(TodoDbContext context)
        {
            _context = context;
        }

        [HttpGet] // Anotation (sind befehle die in den Eckigen klammern stehen [HttpGet])
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _context.Items.ToListAsync();
            return Ok(items);
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = await _context.Items.SingleOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] Item item)
        {
            if (ModelState.IsValid) // es wird auch überprüft ob die Datentypen richtig sind
            {
                await _context.Items.AddAsync(item);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(CreateItem), new { id = item.Id }, item);
            }
            else
            {
                return BadRequest("Item couldnt be created");
            }

        }


    }
}
