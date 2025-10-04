using System;
using UnityEngine;
using Stats;

namespace Item
{
    /// <summary>
    /// 物品資料基類 (ScriptableObject)
    /// 所有物品的基礎資料
    /// </summary>
    public abstract class ItemData : ScriptableObject
    {
        [Header("基本資訊")]
        [Tooltip("物品 ID (唯一識別)")]
        public string itemId;

        [Tooltip("物品名稱")]
        public string itemName = "物品";

        [Tooltip("物品描述")]
        [TextArea(3, 5)]
        public string description = "";

        [Tooltip("物品圖示")]
        public Sprite icon;

        [Header("分類")]
        [Tooltip("物品類型")]
        public ItemType itemType;

        [Tooltip("物品稀有度")]
        public ItemRarity rarity = ItemRarity.Common;

        [Header("堆疊與丟棄")]
        [Tooltip("最大堆疊數量 (0 = 不可堆疊)")]
        public int maxStackSize = 1;

        [Tooltip("是否可丟棄")]
        public bool canDrop = true;

        [Tooltip("是否可出售")]
        public bool canSell = true;

        [Tooltip("出售價格")]
        public int sellPrice = 10;

        [Tooltip("購買價格 (商店)")]
        public int buyPrice = 50;

        /// <summary>
        /// 取得顯示名稱 (含稀有度顏色)
        /// </summary>
        public string GetColoredName()
        {
            Color color = RarityHelper.GetRarityColor(rarity);
            string hexColor = ColorUtility.ToHtmlStringRGB(color);
            return $"<color=#{hexColor}>{itemName}</color>";
        }

        /// <summary>
        /// 取得完整描述 (含稀有度)
        /// </summary>
        public virtual string GetFullDescription()
        {
            string rarityText = RarityHelper.GetRarityName(rarity);
            return $"[{rarityText}] {itemName}\n{description}";
        }
    }

    /// <summary>
    /// 消耗品資料
    /// </summary>
    [CreateAssetMenu(fileName = "ConsumableData", menuName = "Game/Item/Consumable")]
    public class ConsumableData : ItemData
    {
        [Header("消耗品效果")]
        [Tooltip("恢復 HP 量")]
        public float healHP = 0f;

        [Tooltip("恢復 MP 量")]
        public float healMP = 0f;

        [Tooltip("恢復 HP 百分比")]
        public float healHPPercent = 0f;

        [Tooltip("恢復 MP 百分比")]
        public float healMPPercent = 0f;

        [Tooltip("施加的 Buff")]
        public StatModifierData[] buffModifiers;

        [Tooltip("Buff 持續時間")]
        public float buffDuration = 60f;

        [Tooltip("使用冷卻時間 (秒)")]
        public float cooldown = 0f;

        public override string GetFullDescription()
        {
            string desc = base.GetFullDescription();
            if (healHP > 0) desc += $"\n回復 {healHP} HP";
            if (healMP > 0) desc += $"\n回復 {healMP} MP";
            if (healHPPercent > 0) desc += $"\n回復 {healHPPercent}% HP";
            if (healMPPercent > 0) desc += $"\n回復 {healMPPercent}% MP";
            return desc;
        }
    }

    /// <summary>
    /// 材料資料
    /// </summary>
    [CreateAssetMenu(fileName = "MaterialData", menuName = "Game/Item/Material")]
    public class MaterialData : ItemData
    {
        [Header("材料屬性")]
        [Tooltip("材料等級")]
        public int materialTier = 1;
    }

    /// <summary>
    /// 任務物品資料
    /// </summary>
    [CreateAssetMenu(fileName = "QuestItemData", menuName = "Game/Item/Quest Item")]
    public class QuestItemData : ItemData
    {
        [Header("任務屬性")]
        [Tooltip("所屬任務 ID")]
        public string questId;

        [Tooltip("是否為關鍵物品 (不可丟棄)")]
        public bool isKeyItem = true;
    }

    /// <summary>
    /// 屬性修飾器資料 (用於序列化)
    /// </summary>
    [Serializable]
    public class StatModifierData
    {
        public StatType statType;
        public ModifierType modifierType;
        public float value;
    }
}
