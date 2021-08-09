using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Dtos;
using Catalog.Entities;
using Catalog.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers
{
    // GET /items

    [ApiController] // Features extras de api -> só retorna dados em JSON direto e não views (ou precisar formatar para JSON)
    [Route("items")] // Usa o nome da classe controller -> Mesma coisa que [Route("[controller]")]
    public class ItemsController : ControllerBase // classe base com métodos para controller -> a "controller" tem método implementado para view, o base não
    {
        private readonly IItemsRepository repository;

        public ItemsController(IItemsRepository repository) => this.repository = repository;

        // GET /items
        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItems() => (await repository.GetItemsAsync()).Select(item => item.AsDto());

        // GET /items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItem(Guid id)
        {
            var item = await repository.GetItemAsync(id);

            return item is null ? NotFound() : item.AsDto();
        }

        // POST /items
        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItem(CreateItemDto itemDto)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await repository.CreateItemAsync(item);

            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item.AsDto());
        }

        // PUT /items/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItem(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = await repository.GetItemAsync(id);

            if (existingItem is null) return NotFound();

            Item updatedItem = existingItem with
            {
                Name = itemDto.Name,
                Price = itemDto.Price
            };

            await repository.UpdateItemAsync(updatedItem);

            return NoContent();
        }

        // DELETE /items/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItem(Guid id)
        {
            var existingItem = await repository.GetItemAsync(id);

            if (existingItem is null) return NotFound();

            await repository.DeleteItemAsync(id);

            return NoContent();
        }
    }
}