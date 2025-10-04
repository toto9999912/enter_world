using Core;
using Item;

namespace Services
{
    public interface IInventoryService : IService
    {
        bool AddItem(ItemData item, int quantity = 1);
        bool RemoveItem(string itemId, int quantity = 1);
        bool HasItem(string itemId, int quantity = 1);
        int GetItemCount(string itemId);
        bool ExpandInventory();
        int GetCapacity();
        int GetUsedSlots();
    }
}
