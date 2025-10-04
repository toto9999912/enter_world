using System;
using UnityEngine;
using Stats;
using Element;

namespace Skill
{
    /// <summary>
    /// 天賦類型
    /// </summary>
    public enum TalentType
    {
        /// <summary>被動屬性強化</summary>
        PassiveStat,

        /// <summary>技能強化</summary>
        SkillEnhancement,

        /// <summary>特殊機制</summary>
        SpecialMechanic
    }

    /// <summary>
    /// 天賦資料 (ScriptableObject)
    /// 主元素專屬天賦樹
    /// </summary>
    [CreateAssetMenu(fileName = "TalentData", menuName = "Game/Skill/Talent")]
    public class TalentData : ScriptableObject
    {
        [Header("基本資訊")]
        [Tooltip("天賦 ID")]
        public string talentId;

        [Tooltip("天賦名稱")]
        public string talentName = "天賦";

        [Tooltip("天賦描述")]
        [TextArea(2, 4)]
        public string description = "";

        [Tooltip("天賦圖示")]
        public Sprite icon;

        [Header("分類")]
        [Tooltip("所屬元素 (只能學習主元素天賦)")]
        public ElementType requiredElement;

        [Tooltip("天賦類型")]
        public TalentType talentType;

        [Header("學習條件")]
        [Tooltip("需要的天賦點數")]
        public int requiredTalentPoints = 1;

        [Tooltip("前置天賦 (可選)")]
        public TalentData prerequisite;

        [Tooltip("最小等級需求")]
        public int minLevelRequirement = 1;

        [Tooltip("最大等級")]
        public int maxLevel = 5;

        [Header("效果配置")]
        [Tooltip("被動屬性修飾器 (每級)")]
        public StatModifierData[] passiveModifiersPerLevel;

        [Tooltip("技能強化 - 目標技能 ID")]
        public string enhancedSkillId;

        [Tooltip("技能強化 - 冷卻減少 (每級, 秒)")]
        public float cooldownReductionPerLevel = 0f;

        [Tooltip("技能強化 - 傷害/治療增幅 (每級, %)")]
        public float powerIncreasePerLevel = 0f;

        [Tooltip("技能強化 - 範圍增加 (每級, %)")]
        public float rangeIncreasePerLevel = 0f;

        [Tooltip("特殊機制 - 自訂效果描述")]
        [TextArea(2, 3)]
        public string specialMechanicDescription = "";

        /// <summary>
        /// 取得當前等級的屬性修飾器
        /// </summary>
        public StatModifier[] GetModifiersAtLevel(int level)
        {
            if (passiveModifiersPerLevel == null || passiveModifiersPerLevel.Length == 0)
                return new StatModifier[0];

            StatModifier[] modifiers = new StatModifier[passiveModifiersPerLevel.Length];
            for (int i = 0; i < passiveModifiersPerLevel.Length; i++)
            {
                var data = passiveModifiersPerLevel[i];
                modifiers[i] = new StatModifier(
                    data.statType,
                    data.modifierType,
                    data.value * level,
                    talentId
                );
            }

            return modifiers;
        }

        /// <summary>
        /// 取得完整描述 (含等級效果)
        /// </summary>
        public string GetFullDescription(int currentLevel)
        {
            string desc = $"{talentName} (Lv.{currentLevel}/{maxLevel})\n{description}\n";

            switch (talentType)
            {
                case TalentType.PassiveStat:
                    desc += "\n【被動效果】\n";
                    foreach (var mod in passiveModifiersPerLevel)
                    {
                        float totalValue = mod.value * currentLevel;
                        string statName = GetStatTypeName(mod.statType);
                        string valueText = mod.modifierType == ModifierType.Percentage
                            ? $"+{totalValue}%"
                            : $"+{totalValue}";
                        desc += $"{statName} {valueText}\n";
                    }
                    break;

                case TalentType.SkillEnhancement:
                    desc += "\n【技能強化】\n";
                    if (cooldownReductionPerLevel > 0)
                        desc += $"冷卻時間 -{cooldownReductionPerLevel * currentLevel} 秒\n";
                    if (powerIncreasePerLevel > 0)
                        desc += $"效果強度 +{powerIncreasePerLevel * currentLevel}%\n";
                    if (rangeIncreasePerLevel > 0)
                        desc += $"作用範圍 +{rangeIncreasePerLevel * currentLevel}%\n";
                    break;

                case TalentType.SpecialMechanic:
                    desc += $"\n【特殊效果】\n{specialMechanicDescription}";
                    break;
            }

            return desc;
        }

        /// <summary>
        /// 取得屬性類型名稱 (中文)
        /// </summary>
        private string GetStatTypeName(StatType statType)
        {
            switch (statType)
            {
                case StatType.HP: return "生命值";
                case StatType.MP: return "魔力值";
                case StatType.ATK: return "攻擊力";
                case StatType.Magic: return "魔法力";
                case StatType.DEF: return "防禦力";
                case StatType.SPD: return "移動速度";
                case StatType.SP: return "精神力";
                case StatType.CritRate: return "暴擊率";
                case StatType.CritDamage: return "暴擊傷害";
                case StatType.DodgeRate: return "迴避率";
                case StatType.Toughness: return "韌性";
                case StatType.Penetration: return "穿透";
                case StatType.SkillHaste: return "技能急速";
                case StatType.HitRate: return "命中率";
                default: return statType.ToString();
            }
        }
    }

    /// <summary>
    /// 天賦修飾器資料 (用於序列化)
    /// </summary>
    [Serializable]
    public class TalentModifierData
    {
        public StatType statType;
        public ModifierType modifierType;
        public float value;
    }
}
