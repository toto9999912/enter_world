using System;
using UnityEngine;

namespace Companion
{
    /// <summary>
    /// 眷屬稀有度枚舉
    /// 注意: 可在 Inspector 中自定義名稱，這只是預設值
    /// </summary>
    public enum CompanionRarity
    {
        /// <summary>普通</summary>
        Common,

        /// <summary>精英</summary>
        Elite,

        /// <summary>稀有</summary>
        Rare,

        /// <summary>傳說</summary>
        Legendary
    }

    /// <summary>
    /// 眷屬類型
    /// </summary>
    public enum CompanionType
    {
        /// <summary>輕型眷屬 (低 SP 佔用)</summary>
        Light,

        /// <summary>中型眷屬 (中等 SP 佔用)</summary>
        Medium,

        /// <summary>重型眷屬 (高 SP 佔用)</summary>
        Heavy,

        /// <summary>特殊型眷屬 (自定義 SP 佔用)</summary>
        Special
    }

    /// <summary>
    /// 稀有度配置資料 (ScriptableObject)
    /// 允許自定義稀有度名稱與重生時間
    /// </summary>
    [CreateAssetMenu(fileName = "CompanionRarityConfig", menuName = "Game/Companion/Rarity Config")]
    public class CompanionRarityConfig : ScriptableObject
    {
        [System.Serializable]
        public class RarityData
        {
            [Tooltip("稀有度等級")]
            public CompanionRarity rarity;

            [Tooltip("自定義名稱 (例如: 普通 → 劣質)")]
            public string displayName;

            [Tooltip("基礎重生時間 (秒)")]
            public float baseReviveTime = 10f;

            [Tooltip("重生時間隨機範圍 (±秒)")]
            public float reviveTimeVariance = 2f;

            [Tooltip("稀有度顏色 (UI 顯示)")]
            public Color rarityColor = Color.white;
        }

        [Header("稀有度配置")]
        public RarityData[] rarityConfigs = new RarityData[]
        {
            new RarityData { rarity = CompanionRarity.Common, displayName = "普通", baseReviveTime = 10f, rarityColor = Color.gray },
            new RarityData { rarity = CompanionRarity.Elite, displayName = "精英", baseReviveTime = 20f, rarityColor = Color.green },
            new RarityData { rarity = CompanionRarity.Rare, displayName = "稀有", baseReviveTime = 40f, rarityColor = Color.blue },
            new RarityData { rarity = CompanionRarity.Legendary, displayName = "傳說", baseReviveTime = 60f, rarityColor = new Color(1f, 0.5f, 0f) }
        };

        /// <summary>
        /// 取得稀有度資料
        /// </summary>
        public RarityData GetRarityData(CompanionRarity rarity)
        {
            foreach (var data in rarityConfigs)
            {
                if (data.rarity == rarity)
                    return data;
            }

            // 預設返回普通
            return rarityConfigs[0];
        }

        /// <summary>
        /// 取得稀有度顯示名稱
        /// </summary>
        public string GetDisplayName(CompanionRarity rarity)
        {
            return GetRarityData(rarity).displayName;
        }

        /// <summary>
        /// 取得基礎重生時間
        /// </summary>
        public float GetBaseReviveTime(CompanionRarity rarity)
        {
            return GetRarityData(rarity).baseReviveTime;
        }

        /// <summary>
        /// 取得稀有度顏色
        /// </summary>
        public Color GetRarityColor(CompanionRarity rarity)
        {
            return GetRarityData(rarity).rarityColor;
        }
    }
}
