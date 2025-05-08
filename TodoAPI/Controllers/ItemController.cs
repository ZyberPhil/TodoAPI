using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI.Models;
using TodoAPI.Services;

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly TodoDbContext _context;

        public ItemController(TodoDbContext context)
        {
            _context = context;
        }

        // GET: api/item
        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _context.Items.ToListAsync();
            await DiscordNotifier.SendNotification($"```Alle Items wurden geladen```");
            return Ok(items);
        }

        // GET: api/item/5
        [HttpGet("get/{id:int}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = await _context.Items.SingleOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                await DiscordNotifier.SendNotification($"```Item mit ID {id} wurde nicht gefunden```");
                return NotFound($"Item mit ID {id} wurde nicht gefunden.");
            }

            await DiscordNotifier.SendNotification($"```Item mit ID {id} wurde geladen```");
            return Ok(item);
        }

        // GET: api/item/status/1
        [HttpGet("status/{status:int}")]
        public async Task<IActionResult> GetAllItemsByStatus(int status)
        {
            var items = await _context.Items.Where(x => x.Status == status).ToListAsync();
            if (items == null || items.Count == 0)
            {
                await DiscordNotifier.SendNotification($"```Keine Items mit dem Status {status} gefunden```");
                return NotFound($"Keine Items mit dem Status {status} gefunden.");
            }

            await DiscordNotifier.SendNotification($"```Items mit dem Status {status} wurden geladen```");
            return Ok(items);
        }

        // GET: api/item/search?query=Test
        [HttpGet("search")]
        public async Task<IActionResult> SearchItems([FromQuery] string query)
        {
            var results = await _context.Items
                .Where(x => x.Name.Contains(query) || x.Beschreibung.Contains(query))
                .ToListAsync();
            if (results == null || results.Count == 0)
            {
                await DiscordNotifier.SendNotification($"```Keine Items mit dem Suchbegriff '{query}' gefunden```");
                return NotFound($"Keine Items mit dem Suchbegriff '{query}' gefunden.");
            }

            await DiscordNotifier.SendNotification($"```Items mit dem Suchbegriff '{query}' wurden geladen```");
            return Ok(results);
        }

        // GET: api/item/paged?page=1&pageSize=10
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedItems([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var items = await _context.Items
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            if (items == null || items.Count == 0)
            {
                await DiscordNotifier.SendNotification($"```Keine Items gefunden auf Seite {page}```");
                return NotFound($"Keine Items gefunden auf Seite {page}.");
            }

            await DiscordNotifier.SendNotification($"```Items auf Seite {page} wurden geladen```");
            return Ok(items);
        }

        // GET: api/item/statistics
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var total = await _context.Items.CountAsync();
            if (total == 0)
            {
                await DiscordNotifier.SendNotification($"```Keine Items gefunden.```");
                return NotFound($"Keine Items gefunden.");
            }
            var done = await _context.Items.CountAsync(x => x.Status == 1);
            var open = total - done;
            if (open == 0) 
            {
                await DiscordNotifier.SendNotification($"```Alle Items sind erledigt.```");
            }
            else
            {
                await DiscordNotifier.SendNotification($"```Es gibt {open} offene Items.```");
            }

            await DiscordNotifier.SendNotification($"```Statistiken wurden geladen```");
            return Ok(new { total, done, open });
        }


        // POST: api/item
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] Item item)
        {
            if (!ModelState.IsValid)
            {
                await DiscordNotifier.SendNotification($"```Ungültige Eingabedaten für Item mit ID {item.Id}```");
                return BadRequest("Ungültige Eingabedaten.");
            }

            await _context.Items.AddAsync(item);
            await DiscordNotifier.SendNotification($"```Item mit ID {item.Id} wurde erstellt```");
            await _context.SaveChangesAsync();
            await DiscordNotifier.SendNotification($"```Item mit ID {item.Id} wurde gespeichert```");

            return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
        }

        // POST: api/item/bulk
        // POST ermoeglicht mehrere Items gleichzeitig zu erstellen(oder loeschen)
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateMultipleItems([FromBody] List<Item> items)
        {
            await _context.Items.AddRangeAsync(items);
            await DiscordNotifier.SendNotification($"```Mehrere Items wurden erstellt```");
            await _context.SaveChangesAsync();
            await DiscordNotifier.SendNotification($"```Mehrere Items wurden gespeichert```");
            return Ok(items);
        }


        // DELETE: api/item/5
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                await DiscordNotifier.SendNotification($"```Item mit ID {id} wurde nicht gefunden```");
                return NotFound($"Item mit ID {id} wurde nicht gefunden.");
            }

            _context.Items.Remove(item);
            await DiscordNotifier.SendNotification($"```Item mit ID {id} wurde gelöscht```");
            await _context.SaveChangesAsync();
            await DiscordNotifier.SendNotification($"```Item mit ID {id} wurde gespeichert```");

            return Ok($"Item mit ID {id} wurde gelöscht.");
        }

        // PUT: api/item/5
        [HttpPut("update/{id:int}")]
        public async Task<IActionResult> UpdateItemById(int id, [FromBody] Item uItem)
        {
            if (!ModelState.IsValid)
            {
                await DiscordNotifier.SendNotification($"```Ungültige Eingabedaten für Item mit ID {uItem.Id}```");
                return BadRequest("Ungültige Eingabedaten.");
            }

            if (!(id == uItem.Id))
            {
                return BadRequest($"Ids stimmen nicht überein ID: {id}, BodyID: {uItem.Id}");
            }
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                await DiscordNotifier.SendNotification($"```Item mit ID {id} wurde nicht gefunden```");
                return NotFound($"Item mit ID {id} wurde nicht gefunden.");
            }

            item.Name = uItem.Name;
            item.Status = uItem.Status;
            item.Beschreibung = uItem.Beschreibung;
            item.DueDate = uItem.DueDate;

            _context.Update(item);
            await _context.SaveChangesAsync();
            await DiscordNotifier.SendNotification($"```Item mit ID {id} wurde aktualisiert```");

            return Ok(item);
        }

        // PATCH: api/item/5/status
        // PATCH bietet die moeglichkeit eine sache eines datensatzes zu aendern und nicht  den  Ganzen datensatz
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] int status)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                await DiscordNotifier.SendNotification($"```Item mit ID {id} wurde nicht gefunden```");
                return NotFound($"Item mit ID {id} wurde nicht gefunden.");
            }

            item.Status = status;
            await _context.SaveChangesAsync();
            await DiscordNotifier.SendNotification($"```Status von Item mit ID {id} wurde aktualisiert```");
            return Ok(item);
        }

    }
}
