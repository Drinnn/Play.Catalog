using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly ItemRepository itemRepository = new();

    [HttpPost]
    public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
    {
        var item = new Item(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price,
            DateTimeOffset.UtcNow);

        await itemRepository.CreateAsync(item);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
    }

    [HttpGet]
    public async Task<IEnumerable<ItemDto>> GetAsync()
    {
        var items = (await itemRepository.GetAllAsync()).Select(item => item.ToDto());

        return items;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
    {
        var item = await itemRepository.GetAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        return item.ToDto();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateItemDto updateItemDto)
    {
        var existingItem = await itemRepository.GetAsync(id);
        if (existingItem == null)
        {
            var item = new Item(Guid.NewGuid(), updateItemDto.Name, updateItemDto.Description, updateItemDto.Price,
                DateTimeOffset.UtcNow);

            await itemRepository.CreateAsync(item);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        existingItem.Name = updateItemDto.Name ?? existingItem.Name;
        existingItem.Description = updateItemDto.Description ?? existingItem.Description;
        existingItem.Price = updateItemDto.Price;

        await itemRepository.UpdateAsync(existingItem);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingItem = await itemRepository.GetAsync(id);

        if (existingItem == null)
        {
            return NotFound();
        }

        await itemRepository.RemoveAsync(id);

        return NoContent();
    }
}