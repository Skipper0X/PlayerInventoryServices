using Catalog.Service.Entities;
using Catalog.Service.Utilities;
using MassTransit;
using Play.Common;

namespace Catalog.Service.Controllers;

using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<Item> _itemRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ItemsController(IRepository<Item> itemRepository, IPublishEndpoint publishEndpoint)
    {
        _itemRepository = itemRepository;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<IEnumerable<ItemDto>> GetAsync()
    {
        var items = (await _itemRepository.GetAllAsync()).Select(item => item.AsDto());
        return items;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ItemDto>> GetByIdAsync_(Guid id)
    {
        var item = await _itemRepository.GetAsync(id);
        return item.AsDto();
    }

    [HttpPost]
    public async Task<ActionResult<ItemDto>> Post(CreateItemDto createItemDto)
    {
        var (name, description, price) = createItemDto;

        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await _itemRepository.CreateAsync(item);
        await _publishEndpoint.Publish<CatalogItemCreated>(new(item.Id, item.Name, item.Description));

        return CreatedAtAction(nameof(GetByIdAsync_), new { id = item.Id }, item);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
    {
        var existingItem = await _itemRepository.GetAsync(id);
        if (existingItem == null) return NotFound();

        existingItem.Name = updateItemDto.Name;
        existingItem.Description = updateItemDto.Description;
        existingItem.Price = updateItemDto.Price;

        await _itemRepository.UpdateAsync(existingItem);
        await _publishEndpoint.Publish<CatalogItemUpdated>(new(existingItem.Id, existingItem.Name, existingItem.Description));

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingItem = await _itemRepository.GetAsync(id);
        if (existingItem == null) return NotFound();

        await _itemRepository.RemoveAsync(id);
        await _publishEndpoint.Publish<CatalogItemDeleted>(new(id));

        return NoContent();
    }
}