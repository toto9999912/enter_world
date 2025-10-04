using System;
using UnityEngine;

namespace Item
{
    /// <summary>
    /// 物品類型 (6大分類)
    /// </summary>
    public enum ItemType
    {
        /// <summary>消耗品 (藥水、Buff道具)</summary>
        Consumable,

        /// <summary>裝備 (武器、防具)</summary>
        Equipment,

        /// <summary>道具 (一般道具)</summary>
        Item,

        /// <summary>材料 (製作材料)</summary>
        Material,

        /// <summary>任務物品</summary>
        QuestItem,

        /// <summary>卡片 (長CD強效)</summary>
        Card
    }

    /// <summary>
    /// 物品稀有度 (6階)
    /// </summary>
    public enum ItemRarity
    {
        /// <summary>灰色 - 普通</summary>
        Common,

        /// <summary>綠色 - 優良</summary>
        Uncommon,

        /// <summary>藍色 - 稀有</summary>
        Rare,

        /// <summary>紫色 - 史詩</summary>
        Epic,

        /// <summary>紅色 - 傳說</summary>
        Legendary,

        /// <summary>金色 - 神話</summary>
        Mythic
    }

    /// <summary>
    /// 稀有度輔助工具
    /// </summary>
    public static class RarityHelper
    {
        /// <summary>取得稀有度顏色</summary>
        public static Color GetRarityColor(ItemRarity rarity)
        {
            return rarity switch
            {
                ItemRarity.Common => new Color(0.6f, 0.6f, 0.6f),      // 灰色
                ItemRarity.Uncommon => new Color(0.1f, 0.8f, 0.1f),    // 綠色
                ItemRarity.Rare => new Color(0.2f, 0.5f, 1f),          // 藍色
                ItemRarity.Epic => new Color(0.7f, 0.2f, 0.9f),        // 紫色
                ItemRarity.Legendary => new Color(1f, 0.2f, 0.2f),     // 紅色
                ItemRarity.Mythic => new Color(1f, 0.8f, 0f),          // 金色
                _ => Color.white
            };
        }

        /// <summary>取得稀有度名稱</summary>
        public static string GetRarityName(ItemRarity rarity)
        {
            return rarity switch
            {
                ItemRarity.Common => "普通",
                ItemRarity.Uncommon => "優良",
                ItemRarity.Rare => "稀有",
                ItemRarity.Epic => "史詩",
                ItemRarity.Legendary => "傳說",
                ItemRarity.Mythic => "神話",
                _ => "未知"
            };
        }
    }
}
