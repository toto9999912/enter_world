using System;
using System.Collections.Generic;
using UnityEngine;
using Stats;

namespace Item
{
    /// <summary>
    /// 裝備欄位類型
    /// </summary>
    public enum EquipmentSlot
    {
        /// <summary>武器</summary>
        Weapon,

        /// <summary>頭部</summary>
        Head,

        /// <summary>胸部</summary>
        Chest,

        /// <summary>鞋子</summary>
        Shoes,

        /// <summary>飾品</summary>
        Accessory
    }

    /// <summary>
    /// 裝備資料 (ScriptableObject)
    /// </summary>
    [CreateAssetMenu(fileName = "EquipmentData", menuName = "Game/Item/Equipment")]
    public class EquipmentData : ItemData
    {
        [Header("裝備屬性")]
        [Tooltip("裝備欄位")]
        public EquipmentSlot slot = EquipmentSlot.Weapon;

        [Tooltip("需求等級")]
        public int requiredLevel = 1;

        [Tooltip("基礎屬性加成")]
        public StatModifierData[] baseModifiers;

        [Header("強化系統")]
        [Tooltip("最大強化等級")]
        public int maxEnhanceLevel = 9;

        [Tooltip("每級屬性提升 (百分比)")]
        public float enhanceBonusPerLevel = 10f;

        [Header("隱藏屬性 (強化解鎖)")]
        [Tooltip("+3 解鎖的隱藏屬性")]
        public StatModifierData[] hiddenModifiers_Plus3;

        [Tooltip("+6 解鎖的隱藏屬性")]
        public StatModifierData[] hiddenModifiers_Plus6;

        [Tooltip("+9 解鎖的隱藏屬性")]
        public StatModifierData[] hiddenModifiers_Plus9;

        [Header("套裝系統")]
        [Tooltip("套裝 ID (相同 ID 為同一套)")]
        public string setId = "";

        [Tooltip("套裝名稱")]
        public string setName = "";

        [Tooltip("套裝需求件數")]
        public int setRequiredPieces = 3;

        [Tooltip("套裝效果")]
        public StatModifierData[] setBonus;

        [Header("耐久系統")]
        [Tooltip("初始耐久度")]
        public int baseMaxDurability = 100;

        [Tooltip("修復費用 (每點耐久)")]
        public int repairCostPerPoint = 10;

        /// <summary>
        /// 計算強化後的屬性加成
        /// </summary>
        public List<StatModifier> GetModifiersAtLevel(int enhanceLevel)
        {
            List<StatModifier> modifiers = new List<StatModifier>();

            // 1. 基礎屬性 (受強化影響)
            float enhanceMultiplier = 1f + (enhanceLevel * enhanceBonusPerLevel / 100f);
            foreach (var modData in baseModifiers)
            {
                float enhancedValue = modData.value * enhanceMultiplier;
                modifiers.Add(new StatModifier(
                    modData.statType,
                    modData.modifierType,
                    enhancedValue,
                    itemId
                ));
            }

            // 2. 隱藏屬性 (+3)
            if (enhanceLevel >= 3 && hiddenModifiers_Plus3 != null)
            {
                foreach (var modData in hiddenModifiers_Plus3)
                {
                    modifiers.Add(new StatModifier(
                        modData.statType,
                        modData.modifierType,
                        modData.value,
                        itemId + "_hidden3"
                    ));
                }
            }

            // 3. 隱藏屬性 (+6)
            if (enhanceLevel >= 6 && hiddenModifiers_Plus6 != null)
            {
                foreach (var modData in hiddenModifiers_Plus6)
                {
                    modifiers.Add(new StatModifier(
                        modData.statType,
                        modData.modifierType,
                        modData.value,
                        itemId + "_hidden6"
                    ));
                }
            }

            // 4. 隱藏屬性 (+9)
            if (enhanceLevel >= 9 && hiddenModifiers_Plus9 != null)
            {
                foreach (var modData in hiddenModifiers_Plus9)
                {
                    modifiers.Add(new StatModifier(
                        modData.statType,
                        modData.modifierType,
                        modData.value,
                        itemId + "_hidden9"
                    ));
                }
            }

            return modifiers;
        }

        public override string GetFullDescription()
        {
            string desc = base.GetFullDescription();
            desc += $"\n裝備欄位: {slot}";
            desc += $"\n需求等級: {requiredLevel}";

            if (!string.IsNullOrEmpty(setId))
            {
                desc += $"\n\n套裝: {setName} ({setRequiredPieces}件)";
            }

            return desc;
        }
    }
}
