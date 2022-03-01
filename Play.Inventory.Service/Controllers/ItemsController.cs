using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Play.Inventory.Service.Utils;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> _inventoryItemRepository;
        private readonly IRepository<CatalogItem> _catalogItemRepository;

        public ItemsController(IRepository<InventoryItem> itemRepository, IRepository<CatalogItem> catalogItemRepository)
        {
            _inventoryItemRepository = itemRepository;
            _catalogItemRepository = catalogItemRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty) return BadRequest();

            var inventoryItems = await _inventoryItemRepository.GetAllAsync(item => item.UserId == userId);
            var itemIds = inventoryItems.Select(item => item.CatalogItemId);
            var catalogItems = await _catalogItemRepository.GetAllAsync(item => itemIds.Contains(item.Id));


            var items = inventoryItems.Select(inventoryItem =>
            {
                var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto itemDto)
        {
            var inventoryItem = await _inventoryItemRepository.GetAsync(item =>
                item.UserId.Equals(itemDto.UserId) && item.CatalogItemId.Equals(itemDto.CatalogItemId));

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem()
                {
                    CatalogItemId = itemDto.CatalogItemId,
                    UserId = itemDto.UserId,
                    Quantity = itemDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow,
                };

                await _inventoryItemRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += itemDto.Quantity;
                await _inventoryItemRepository.UpdateAsync(inventoryItem);
            }

            return Ok();
        }
    }
}