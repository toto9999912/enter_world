using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Item;

namespace Inventory
{
    /// <summary>
    /// 物品槽
    /// </summary>
    [Serializable]
    public class ItemSlot
    {
        public ItemData itemData;
        public int quantity;
        public object itemInstance; // 可以是 EquipmentInstance, CardInstance 等

        public bool IsEmpty => itemData == null || quantity <= 0;

        public ItemSlot()
        {
            itemData = null;
            quantity = 0;
            itemInstance = null;
        }
    }

    /// <summary>
    /// 背包系統
    /// </summary>
    public class InventorySystem
    {
        // ===== 配置參數 =====
        public static int initialCapacity = 50;      // 初始容量
        public static int maxCapacity = 200;          // 最大容量
        public static int expansionCost = 1000;      // 擴充費用 (每次)
        public static float expansionCostMultiplier = 1.5f; // 費用成長倍率

        // ===== 背包資料 =====
        private List<ItemSlot> slots;
        private int currentCapacity;
        private int expansionCount = 0;

        // ===== 事件 =====
        public event Action<ItemData, int> OnItemAdded;
        public event Action<ItemData, int> OnItemRemoved;
        public event Action<int> OnCapacityChanged;

        // ===== 屬性 =====
        public int CurrentCapacity => currentCapacity;
        public int UsedSlots => slots.Count(s => !s.IsEmpty);
        public int FreeSlots => currentCapacity - UsedSlots;
        public bool IsFull => UsedSlots >= currentCapacity;

        /// <summary>
        /// 建構子
        /// </summary>
        public InventorySystem(int capacity = 0)
        {
            currentCapacity = capacity > 0 ? capacity : initialCapacity;
            slots = new List<ItemSlot>(currentCapacity);

            for (int i = 0; i < currentCapacity; i++)
            {
                slots.Add(new ItemSlot());
            }
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        public bool AddItem(ItemData itemData, int quantity = 1, object instance = null)
        {
            if (itemData == null || quantity <= 0)
                return false;

            // 可堆疊物品
            if (itemData.maxStackSize > 1)
            {
                return AddStackableItem(itemData, quantity);
            }
            // 不可堆疊物品
            else
            {
                return AddNonStackableItem(itemData, instance);
            }
        }

        /// <summary>
        /// 添加可堆疊物品
        /// </summary>
        private bool AddStackableItem(ItemData itemData, int quantity)
        {
            int remainingQuantity = quantity;

            // 1. 先嘗試堆疊到現有格子
            foreach (var slot in slots)
            {
                if (slot.itemData == itemData && slot.quantity < itemData.maxStackSize)
                {
                    int canAdd = Mathf.Min(remainingQuantity, itemData.maxStackSize - slot.quantity);
                    slot.quantity += canAdd;
                    remainingQuantity -= canAdd;

                    if (remainingQuantity <= 0)
                    {
                        OnItemAdded?.Invoke(itemData, quantity);
                        return true;
                    }
                }
            }

            // 2. 需要新格子
            while (remainingQuantity > 0)
            {
                ItemSlot emptySlot = FindEmptySlot();
                if (emptySlot == null)
                {
                    Debug.LogWarning("[Inventory] 背包已滿！");
                    return false;
                }

                int addAmount = Mathf.Min(remainingQuantity, itemData.maxStackSize);
                emptySlot.itemData = itemData;
                emptySlot.quantity = addAmount;
                remainingQuantity -= addAmount;
            }

            OnItemAdded?.Invoke(itemData, quantity);
            return true;
        }

        /// <summary>
        /// 添加不可堆疊物品 (裝備/卡片)
        /// </summary>
        private bool AddNonStackableItem(ItemData itemData, object instance)
        {
            ItemSlot emptySlot = FindEmptySlot();
            if (emptySlot == null)
            {
                Debug.LogWarning("[Inventory] 背包已滿！");
                return false;
            }

            emptySlot.itemData = itemData;
            emptySlot.quantity = 1;
            emptySlot.itemInstance = instance;

            OnItemAdded?.Invoke(itemData, 1);
            return true;
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        public bool RemoveItem(ItemData itemData, int quantity = 1)
        {
            if (!itemData.canDrop)
            {
                Debug.LogWarning($"[Inventory] {itemData.itemName} 不可丟棄！");
                return false;
            }

            int remainingToRemove = quantity;

            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].itemData == itemData)
                {
                    int removeAmount = Mathf.Min(remainingToRemove, slots[i].quantity);
                    slots[i].quantity -= removeAmount;
                    remainingToRemove -= removeAmount;

                    if (slots[i].quantity <= 0)
                    {
                        slots[i] = new ItemSlot(); // 清空格子
                    }

                    if (remainingToRemove <= 0)
                        break;
                }
            }

            if (remainingToRemove < quantity)
            {
                OnItemRemoved?.Invoke(itemData, quantity - remainingToRemove);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 檢查是否有足夠數量的物品
        /// </summary>
        public bool HasItem(ItemData itemData, int quantity = 1)
        {
            int totalCount = slots
                .Where(s => s.itemData == itemData)
                .Sum(s => s.quantity);

            return totalCount >= quantity;
        }

        /// <summary>
        /// 取得物品數量
        /// </summary>
        public int GetItemCount(ItemData itemData)
        {
            return slots
                .Where(s => s.itemData == itemData)
                .Sum(s => s.quantity);
        }

        /// <summary>
        /// 擴充背包
        /// </summary>
        public int ExpandInventory()
        {
            if (currentCapacity >= maxCapacity)
            {
                Debug.LogWarning("[Inventory] 背包已達最大容量！");
                return -1;
            }

            // 計算費用
            int cost = Mathf.CeilToInt(expansionCost * Mathf.Pow(expansionCostMultiplier, expansionCount));

            // 擴充 (預設每次 +10 格)
            int addSlots = 10;
            currentCapacity = Mathf.Min(currentCapacity + addSlots, maxCapacity);

            for (int i = 0; i < addSlots && slots.Count < currentCapacity; i++)
            {
                slots.Add(new ItemSlot());
            }

            expansionCount++;
            OnCapacityChanged?.Invoke(currentCapacity);

            Debug.Log($"[Inventory] 背包已擴充！容量: {currentCapacity} (費用: {cost})");
            return cost;
        }

        /// <summary>
        /// 取得特定類型的物品
        /// </summary>
        public List<ItemSlot> GetItemsByType(ItemType type)
        {
            return slots
                .Where(s => !s.IsEmpty && s.itemData.itemType == type)
                .ToList();
        }

        /// <summary>
        /// 尋找空格子
        /// </summary>
        private ItemSlot FindEmptySlot()
        {
            return slots.FirstOrDefault(s => s.IsEmpty);
        }

        /// <summary>
        /// 取得所有非空格子
        /// </summary>
        public List<ItemSlot> GetAllItems()
        {
            return slots.Where(s => !s.IsEmpty).ToList();
        }

        /// <summary>
        /// 整理背包 (堆疊相同物品)
        /// </summary>
        public void SortInventory()
        {
            // TODO: 實作整理邏輯
            Debug.Log("[Inventory] 背包已整理");
        }
    }
}
