using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private static readonly List<ItemDto> Items = new()
    {
        new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
        new ItemDto(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
        new ItemDto(Guid.NewGuid(), "Bronze sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow)
    };
    
    [HttpPost]
    public ActionResult<ItemDto> Post(CreateItemDto createItemDto)
    {
        var item = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTimeOffset.UtcNow);
        Items.Add(item);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpGet]
    public IEnumerable<ItemDto> Get()
    {
        return Items;
    }
    
    [HttpGet("{id:guid}")]
    public ActionResult<ItemDto> GetById(Guid id)
    {
        var item = Items.FirstOrDefault(item => item.Id == id);
        if (item is null)
        {
            return NotFound();
        }
        
        return item;
    }
    
    [HttpPut("{id:guid}")]
    public ActionResult Update(Guid id, UpdateItemDto updateItemDto)
    {
        var itemIndex = Items.FindIndex(item => item.Id == id);
        if (itemIndex < 0)
        {
            var item = new ItemDto(Guid.NewGuid(), updateItemDto.Name, updateItemDto.Description, updateItemDto.Price, DateTimeOffset.UtcNow);
            Items.Add(item);

            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        var updatedItem = Items[itemIndex] with
        {
            Name = updateItemDto.Name,
            Description = updateItemDto.Description,
            Price = updateItemDto.Price
        };

        Items[itemIndex] = updatedItem;

        return NoContent();
    }
    
    [HttpDelete("{id:guid}")]
    public ActionResult Delete(Guid id)
    {
        var itemIndex = Items.FindIndex(item => item.Id == id);
        if (itemIndex < 0)
        {
            return NotFound();
        }

        Items.RemoveAt(itemIndex);

        return NoContent();
    }
}