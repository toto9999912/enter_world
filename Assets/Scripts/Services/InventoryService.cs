using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core;
using Item;
using Inventory;

namespace Services
{
    public class InventoryService : IInventoryService
    {
        private InventorySystem inventorySystem;

        public void Initialize()
        {
            inventorySystem = new InventorySystem(50);
            Debug.Log("[InventoryService] Initialized");
        }

        public void Shutdown()
        {
            Debug.Log("[InventoryService] Shutdown");
        }

        public bool AddItem(ItemData item, int quantity = 1)
        {
            bool success = inventorySystem.AddItem(item, quantity);
            if (success)
            {
                EventBus.Publish(new ItemAddedEvent
                {
                    ItemType = item.itemType,
                    ItemId = item.itemId,
                    Quantity = quantity
                });
            }
            return success;
        }

        public bool RemoveItem(string itemId, int quantity = 1)
        {
            // 先取得物品資料
            var itemData = FindItemDataById(itemId);
            if (itemData == null)
            {
                Debug.LogWarning($"[InventoryService] 找不到物品 ID: {itemId}");
                return false;
            }

            bool success = inventorySystem.RemoveItem(itemData, quantity);
            if (success)
            {
                EventBus.Publish(new ItemRemovedEvent
                {
                    ItemType = itemData.itemType,
                    ItemId = itemId,
                    Quantity = quantity
                });
            }
            return success;
        }

        public bool HasItem(string itemId, int quantity = 1)
        {
            var itemData = FindItemDataById(itemId);
            if (itemData == null) return false;
            return inventorySystem.GetItemCount(itemData) >= quantity;
        }

        public int GetItemCount(string itemId)
        {
            var itemData = FindItemDataById(itemId);
            if (itemData == null) return 0;
            return inventorySystem.GetItemCount(itemData);
        }

        private ItemData FindItemDataById(string itemId)
        {
            // 遍歷所有背包格子找到對應 itemId 的 ItemData
            foreach (ItemType itemType in System.Enum.GetValues(typeof(ItemType)))
            {
                var items = inventorySystem.GetItemsByType(itemType);
                var slot = items.FirstOrDefault(s => s.itemData?.itemId == itemId);
                if (slot.itemData != null)
                {
                    return slot.itemData;
                }
            }
            return null;
        }

        public bool ExpandInventory()
        {
            int cost = inventorySystem.ExpandInventory();
            return cost >= 0; // 返回 -1 表示失敗
        }

        public int GetCapacity()
        {
            return inventorySystem.CurrentCapacity;
        }

        public int GetUsedSlots()
        {
            return inventorySystem.UsedSlots;
        }
        
        /// <summary>
        /// 取得所有非空的物品槽 (支援存檔系統)
        /// </summary>
        public IEnumerable<ItemSlot> GetAllItems()
        {
            return inventorySystem.GetAllItems();
        }
        
        /// <summary>
        /// 清空背包 (載入存檔時使用)
        /// </summary>
        public void ClearInventory()
        {
            inventorySystem.Clear();
            Debug.Log("[InventoryService] 清空背包");
        }
        
        /// <summary>
        /// 設定背包容量 (載入存檔時使用)
        /// </summary>
        public void SetCapacity(int capacity)
        {
            inventorySystem.SetCapacity(capacity);
            Debug.Log($"[InventoryService] 設定容量為 {capacity}");
        }
    }
}
