using System;
using UnityEngine;

namespace Item
{
    /// <summary>
    /// 卡片星級 (稀有度)
    /// </summary>
    public enum CardStarRating
    {
        /// <summary>1星</summary>
        OneStar = 1,

        /// <summary>2星</summary>
        TwoStar = 2,

        /// <summary>3星</summary>
        ThreeStar = 3,

        /// <summary>4星</summary>
        FourStar = 4,

        /// <summary>5星</summary>
        FiveStar = 5
    }

    /// <summary>
    /// 卡片資料 (ScriptableObject)
    /// </summary>
    [CreateAssetMenu(fileName = "CardData", menuName = "Game/Item/Card")]
    public class CardData : ItemData
    {
        [Header("卡片屬性")]
        [Tooltip("卡片星級")]
        public CardStarRating starRating = CardStarRating.OneStar;

        [Tooltip("最大等級")]
        public int maxLevel = 10;

        [Tooltip("冷卻時間 (秒，真實時間)")]
        public float cooldownTime = 300f; // 預設 5 分鐘

        [Header("卡片效果")]
        [Tooltip("效果描述")]
        [TextArea(3, 5)]
        public string effectDescription = "";

        [Tooltip("效果強度 (基礎值)")]
        public float baseEffectPower = 100f;

        [Tooltip("每級提升效果 (%)")]
        public float powerGrowthPerLevel = 10f;

        [Tooltip("效果持續時間 (如果有)")]
        public float effectDuration = 0f;

        [Header("融合配置")]
        [Tooltip("融合所需相同卡片數量")]
        public int cardsRequiredForFusion = 2;

        [Tooltip("融合成功率 (%)")]
        [Range(0f, 100f)]
        public float fusionSuccessRate = 100f;

        /// <summary>
        /// 計算特定等級的效果強度
        /// </summary>
        public float GetEffectPower(int level)
        {
            level = Mathf.Clamp(level, 1, maxLevel);
            return baseEffectPower * (1f + (powerGrowthPerLevel / 100f * (level - 1)));
        }

        /// <summary>
        /// 取得星級顏色
        /// </summary>
        public Color GetStarColor()
        {
            return starRating switch
            {
                CardStarRating.OneStar => Color.white,
                CardStarRating.TwoStar => Color.green,
                CardStarRating.ThreeStar => Color.cyan,
                CardStarRating.FourStar => new Color(1f, 0.5f, 0f), // 橘色
                CardStarRating.FiveStar => new Color(1f, 0.8f, 0f), // 金色
                _ => Color.white
            };
        }

        public override string GetFullDescription()
        {
            string desc = base.GetFullDescription();
            desc += $"\n星級: {(int)starRating}★";
            desc += $"\n冷卻時間: {cooldownTime / 60f:F1} 分鐘";
            desc += $"\n\n效果:\n{effectDescription}";
            return desc;
        }
    }
}
