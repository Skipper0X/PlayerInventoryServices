using Catalog.Service.Entities;

namespace Catalog.Service.Utilities;

public static class Extensions
{
    /// <summary>
    /// Convert <see cref="Item"/> Into An  <see cref="ItemDto"/> Object...
    /// </summary>
    /// <param name="item"><see cref="Item"/>'s Object</param>
    /// <returns></returns>
    public static ItemDto AsDto(this Item item)
        => new(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
}