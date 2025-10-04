using System;
using System.Collections.Generic;
using UnityEngine;
using Item;

namespace Shop
{
    /// <summary>
    /// 商店類型
    /// </summary>
    public enum ShopType
    {
        /// <summary>固定商店 (不刷新)</summary>
        Fixed,

        /// <summary>流浪商人 (定時刷新)</summary>
        Wandering,

        /// <summary>黑市商人 (隨機刷新)</summary>
        BlackMarket
    }

    /// <summary>
    /// 商店物品
    /// </summary>
    [Serializable]
    public class ShopItem
    {
        public ItemData itemData;
        public int price;
        public int stock; // -1 = 無限
        public bool isOnSale; // 折扣中
        public float discountPercent; // 折扣百分比
        public bool isLimited; // 限量商品
        public int limitedQuantity; // 限量數量
        public int soldCount; // 已售出數量

        public int ActualPrice => isOnSale ? Mathf.CeilToInt(price * (1f - discountPercent / 100f)) : price;
        public bool IsOutOfStock => stock == 0 || (isLimited && soldCount >= limitedQuantity);
    }

    /// <summary>
    /// 商店系統
    /// </summary>
    public class ShopSystem
    {
        // ===== 配置參數 =====
        public static float wanderingRefreshHours = 24f;    // 流浪商人刷新時間 (小時)
        public static float blackMarketRefreshHours = 6f;   // 黑市刷新時間 (小時)

        // ===== 商店資料 =====
        private string shopId;
        private string shopName;
        private ShopType shopType;
        private List<ShopItem> items = new List<ShopItem>();
        private long lastRefreshTimestamp;

        // ===== 事件 =====
        public event Action OnShopRefreshed;
        public event Action<ShopItem> OnItemPurchased;
        public event Action<ItemData> OnItemSold;

        // ===== 屬性 =====
        public string ShopId => shopId;
        public string ShopName => shopName;
        public ShopType Type => shopType;
        public IReadOnlyList<ShopItem> Items => items;

        public ShopSystem(string id, string name, ShopType type)
        {
            this.shopId = id;
            this.shopName = name;
            this.shopType = type;
            this.lastRefreshTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 添加商品
        /// </summary>
        public void AddItem(ShopItem item)
        {
            items.Add(item);
        }

        /// <summary>
        /// 購買物品
        /// </summary>
        public bool PurchaseItem(ShopItem item, int quantity, int playerMoney)
        {
            if (item.IsOutOfStock)
            {
                Debug.LogWarning("[Shop] 商品已售罄！");
                return false;
            }

            int totalCost = item.ActualPrice * quantity;
            if (playerMoney < totalCost)
            {
                Debug.LogWarning("[Shop] 金幣不足！");
                return false;
            }

            // 扣除庫存
            if (item.stock > 0)
                item.stock -= quantity;

            if (item.isLimited)
                item.soldCount += quantity;

            OnItemPurchased?.Invoke(item);
            Debug.Log($"[Shop] 購買: {item.itemData.itemName} ×{quantity} (花費 {totalCost})");
            return true;
        }

        /// <summary>
        /// 出售物品
        /// </summary>
        public int SellItem(ItemData itemData, int quantity)
        {
            if (!itemData.canSell)
            {
                Debug.LogWarning($"[Shop] {itemData.itemName} 不可出售！");
                return 0;
            }

            int totalValue = itemData.sellPrice * quantity;
            OnItemSold?.Invoke(itemData);
            Debug.Log($"[Shop] 出售: {itemData.itemName} ×{quantity} (獲得 {totalValue})");
            return totalValue;
        }

        /// <summary>
        /// 檢查是否需要刷新
        /// </summary>
        public bool ShouldRefresh()
        {
            if (shopType == ShopType.Fixed)
                return false;

            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long elapsedSeconds = currentTime - lastRefreshTimestamp;

            float refreshInterval = shopType == ShopType.Wandering
                ? wanderingRefreshHours * 3600f
                : blackMarketRefreshHours * 3600f;

            return elapsedSeconds >= refreshInterval;
        }

        /// <summary>
        /// 刷新商店
        /// </summary>
        public void Refresh()
        {
            if (shopType == ShopType.Fixed)
            {
                Debug.LogWarning("[Shop] 固定商店無法刷新！");
                return;
            }

            // 重置所有商品庫存與折扣
            foreach (var item in items)
            {
                // TODO: 隨機生成新商品或重置庫存
                item.isOnSale = UnityEngine.Random.value < 0.3f; // 30% 機率折扣
                if (item.isOnSale)
                    item.discountPercent = UnityEngine.Random.Range(10f, 50f);
            }

            lastRefreshTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            OnShopRefreshed?.Invoke();
            Debug.Log($"[Shop] {shopName} 已刷新！");
        }
    }
}
