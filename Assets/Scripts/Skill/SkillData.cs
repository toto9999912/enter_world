using System;
using UnityEngine;
using Element;
using Combat;
using Stats;

namespace Skill
{
    /// <summary>
    /// 技能資料 (ScriptableObject)
    /// 定義技能的所有屬性、效果、消耗等
    /// </summary>
    [CreateAssetMenu(fileName = "SkillData", menuName = "Game/Skill/Skill Data")]
    public class SkillData : ScriptableObject
    {
        [Header("基本資訊")]
        [Tooltip("技能 ID (唯一識別)")]
        public string skillId;

        [Tooltip("技能名稱")]
        public string skillName = "技能";

        [Tooltip("技能描述")]
        [TextArea(3, 5)]
        public string description = "";

        [Tooltip("技能圖示")]
        public Sprite icon;

        [Header("分類")]
        [Tooltip("技能分類")]
        public SkillCategory category = SkillCategory.General;

        [Tooltip("技能元素屬性")]
        public ElementType element = ElementType.None;

        [Tooltip("是否為被動技能")]
        public bool isPassive = false;

        [Header("學習需求")]
        [Tooltip("需要的技能點數")]
        public int requiredSkillPoints = 1;

        [Tooltip("非主屬性學習時技能點數倍率 (預設 2.0)")]
        public float crossElementMultiplier = 2.0f;

        [Tooltip("前置技能 (需要先學習的技能 ID)")]
        public string[] prerequisiteSkills;

        [Tooltip("最低等級需求")]
        public int requiredLevel = 1;

        [Header("技能等級")]
        [Tooltip("最大技能等級")]
        public int maxSkillLevel = 5;

        [Tooltip("每級提升效果 (傷害倍率增加)")]
        public float damagePerLevel = 0.2f;

        [Header("施放配置")]
        [Tooltip("施放類型")]
        public SkillCastType castType = SkillCastType.Instant;

        [Tooltip("施法時間 (秒，僅 Channeled 類型)")]
        public float castTime = 0f;

        [Tooltip("目標類型")]
        public SkillTargetType targetType = SkillTargetType.SingleEnemy;

        [Tooltip("施放距離")]
        public float castRange = 5f;

        [Tooltip("AOE 範圍 (僅範圍技能)")]
        public float aoeRadius = 0f;

        [Header("消耗")]
        [Tooltip("魔力消耗")]
        public float manaCost = 10f;

        [Tooltip("每級魔力消耗增加")]
        public float manaCostPerLevel = 5f;

        [Header("冷卻時間")]
        [Tooltip("冷卻時間 (秒)")]
        public float cooldown = 5f;

        [Tooltip("是否受技能急速影響")]
        public bool affectedBySkillHaste = true;

        [Header("傷害配置")]
        [Tooltip("傷害類型")]
        public DamageType damageType = DamageType.Magical;

        [Tooltip("基礎傷害倍率 (基於 ATK 或 INT)")]
        public float baseDamageMultiplier = 1.5f;

        [Tooltip("固定傷害 (不受屬性影響)")]
        public float flatDamage = 0f;

        [Header("控場效果 (如果有)")]
        [Tooltip("是否施加控場效果")]
        public bool appliesCrowdControl = false;

        [Tooltip("控場類型")]
        public CCType crowdControlType = CCType.Stun;

        [Tooltip("控場持續時間 (秒)")]
        public float crowdControlDuration = 2f;

        [Tooltip("控場基礎命中率 (0-100)")]
        public float crowdControlHitRate = 80f;

        [Tooltip("對霸體傷害值")]
        public float superArmorDamage = 50f;

        [Header("治療效果 (如果有)")]
        [Tooltip("是否有治療效果")]
        public bool healsTarget = false;

        [Tooltip("治療倍率 (基於 INT)")]
        public float healMultiplier = 1.0f;

        [Tooltip("固定治療量")]
        public float flatHeal = 0f;

        [Header("Buff 效果 (如果有)")]
        [Tooltip("是否施加 Buff")]
        public bool appliesBuff = false;

        [Tooltip("Buff 持續時間")]
        public float buffDuration = 10f;

        [Tooltip("Buff 提供的屬性修飾器")]
        public StatModifierData[] buffModifiers;

        [Header("視覺/音效")]
        [Tooltip("技能特效 Prefab")]
        public GameObject effectPrefab;

        [Tooltip("施放音效")]
        public AudioClip castSound;

        [Tooltip("命中音效")]
        public AudioClip hitSound;

        /// <summary>
        /// 計算實際技能點數消耗
        /// </summary>
        /// <param name="playerMainElement">玩家主屬性</param>
        /// <returns>需要消耗的技能點數</returns>
        public int CalculateSkillPointCost(ElementType playerMainElement)
        {
            // 如果是主屬性技能，正常消耗
            if (element == playerMainElement || element == ElementType.None)
            {
                return requiredSkillPoints;
            }

            // 跨系技能，雙倍消耗
            return Mathf.CeilToInt(requiredSkillPoints * crossElementMultiplier);
        }

        /// <summary>
        /// 計算特定等級的傷害倍率
        /// </summary>
        public float GetDamageMultiplier(int skillLevel)
        {
            skillLevel = Mathf.Clamp(skillLevel, 1, maxSkillLevel);
            return baseDamageMultiplier + (damagePerLevel * (skillLevel - 1));
        }

        /// <summary>
        /// 計算特定等級的魔力消耗
        /// </summary>
        public float GetManaCost(int skillLevel)
        {
            skillLevel = Mathf.Clamp(skillLevel, 1, maxSkillLevel);
            return manaCost + (manaCostPerLevel * (skillLevel - 1));
        }

        /// <summary>
        /// 計算實際冷卻時間 (考慮技能急速)
        /// </summary>
        public float GetActualCooldown(float skillHaste)
        {
            if (!affectedBySkillHaste)
                return cooldown;

            // 技能急速減少冷卻時間
            float reduction = skillHaste / 100f;
            return cooldown * (1f - reduction);
        }
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
        public string source;
    }
}
