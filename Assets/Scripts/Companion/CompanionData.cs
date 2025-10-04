using System;
using UnityEngine;
using Stats;
using Element;

namespace Companion
{
    /// <summary>
    /// 眷屬資料 (ScriptableObject)
    /// 定義眷屬的基礎屬性、稀有度、SP 佔用等
    /// </summary>
    [CreateAssetMenu(fileName = "CompanionData", menuName = "Game/Companion/Companion Data")]
    public class CompanionData : ScriptableObject
    {
        [Header("基本資訊")]
        [Tooltip("眷屬名稱")]
        public string companionName = "眷屬";

        [Tooltip("眷屬描述")]
        [TextArea(3, 5)]
        public string description = "";

        [Tooltip("眷屬圖示")]
        public Sprite icon;

        [Tooltip("眷屬 Prefab")]
        public GameObject prefab;

        [Header("稀有度與類型")]
        [Tooltip("稀有度 (影響重生時間)")]
        public CompanionRarity rarity = CompanionRarity.Common;

        [Tooltip("眷屬類型")]
        public CompanionType type = CompanionType.Light;

        [Tooltip("SP 佔用量 (如果為 0 則根據類型自動計算)")]
        public float spCost = 0f;

        [Header("屬性配置")]
        [Tooltip("元素屬性")]
        public ElementType element = ElementType.None;

        [Tooltip("基礎等級")]
        public int baseLevel = 1;

        [Header("基礎屬性值")]
        public float baseHP = 100f;
        public float baseMP = 50f;
        public float baseATK = 10f;
        public float baseMagic = 10f;
        public float baseDEF = 5f;
        public float baseSPD = 5f;

        [Header("成長屬性 (每級提升)")]
        public float hpGrowth = 10f;
        public float mpGrowth = 5f;
        public float atkGrowth = 2f;
        public float intGrowth = 2f;
        public float defGrowth = 1f;
        public float spdGrowth = 0.5f;

        [Header("捕捉配置")]
        [Tooltip("基礎捕捉率 (0-100)")]
        [Range(0f, 100f)]
        public float baseCaptureRate = 50f;

        [Tooltip("是否可被捕捉")]
        public bool canBeCaptured = true;

        /// <summary>
        /// 取得預設 SP 佔用 (根據類型)
        /// </summary>
        public float GetSPCost()
        {
            if (spCost > 0f)
                return spCost;

            // 根據類型返回預設值
            return type switch
            {
                CompanionType.Light => 10f,
                CompanionType.Medium => 20f,
                CompanionType.Heavy => 40f,
                CompanionType.Special => 50f,
                _ => 10f
            };
        }

        /// <summary>
        /// 計算特定等級的屬性值
        /// </summary>
        public float CalculateStat(StatType statType, int level)
        {
            int levelDiff = Mathf.Max(0, level - baseLevel);

            return statType switch
            {
                StatType.HP => baseHP + (hpGrowth * levelDiff),
                StatType.MP => baseMP + (mpGrowth * levelDiff),
                StatType.ATK => baseATK + (atkGrowth * levelDiff),
                StatType.Magic => baseMagic + (intGrowth * levelDiff),
                StatType.DEF => baseDEF + (defGrowth * levelDiff),
                StatType.SPD => baseSPD + (spdGrowth * levelDiff),
                _ => 0f
            };
        }
    }
}
