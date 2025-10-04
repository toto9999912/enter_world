using Core;
using Item;
using Inventory;
using System.Collections.Generic;

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
        
        // 新增: 支援存檔系統
        /// <summary>
        /// 取得所有非空的物品槽
        /// </summary>
        IEnumerable<ItemSlot> GetAllItems();
        
        /// <summary>
        /// 清空背包 (載入存檔時使用)
        /// </summary>
        void ClearInventory();
        
        /// <summary>
        /// 設定背包容量 (載入存檔時使用)
        /// </summary>
        void SetCapacity(int capacity);
    }
}
