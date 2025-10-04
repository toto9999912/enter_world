using System;
using UnityEngine;
using Stats;

namespace Skill
{
    /// <summary>
    /// 天賦節點
    /// 代表玩家已學習的天賦及其等級
    /// </summary>
    [Serializable]
    public class TalentNode
    {
        // ===== 資料引用 =====
        [SerializeField] private TalentData data;
        [SerializeField] private int currentLevel;

        // ===== 狀態 =====
        [SerializeField] private bool isUnlocked = false;

        // ===== 事件 =====
        public event Action<int> OnLevelUp;
        public event Action OnUnlocked;

        // ===== 屬性 =====
        public TalentData Data => data;
        public int CurrentLevel => currentLevel;
        public int MaxLevel => data.maxLevel;
        public bool IsUnlocked => isUnlocked;
        public bool IsMaxLevel => currentLevel >= MaxLevel;

        /// <summary>
        /// 建構子
        /// </summary>
        public TalentNode(TalentData data)
        {
            this.data = data;
            this.currentLevel = 0;
            this.isUnlocked = false;
        }

        /// <summary>
        /// 解鎖天賦 (學習第一級)
        /// </summary>
        public bool Unlock()
        {
            if (isUnlocked)
            {
                Debug.LogWarning($"[TalentNode] {data.talentName} 已解鎖！");
                return false;
            }

            isUnlocked = true;
            currentLevel = 1;

            OnUnlocked?.Invoke();
            OnLevelUp?.Invoke(currentLevel);

            Debug.Log($"[TalentNode] 解鎖天賦: {data.talentName} Lv.{currentLevel}");
            return true;
        }

        /// <summary>
        /// 升級天賦
        /// </summary>
        public bool LevelUp()
        {
            if (!isUnlocked)
            {
                Debug.LogWarning($"[TalentNode] {data.talentName} 尚未解鎖！");
                return false;
            }

            if (IsMaxLevel)
            {
                Debug.LogWarning($"[TalentNode] {data.talentName} 已達最高等級！");
                return false;
            }

            currentLevel++;
            OnLevelUp?.Invoke(currentLevel);

            Debug.Log($"[TalentNode] {data.talentName} 升級至 Lv.{currentLevel}");
            return true;
        }

        /// <summary>
        /// 取得當前等級的屬性修飾器
        /// </summary>
        public StatModifier[] GetCurrentModifiers()
        {
            if (!isUnlocked || currentLevel == 0)
                return new StatModifier[0];

            return data.GetModifiersAtLevel(currentLevel);
        }

        /// <summary>
        /// 取得技能強化效果 (冷卻減少)
        /// </summary>
        public float GetCooldownReduction()
        {
            if (!isUnlocked || data.talentType != TalentType.SkillEnhancement)
                return 0f;

            return data.cooldownReductionPerLevel * currentLevel;
        }

        /// <summary>
        /// 取得技能強化效果 (威力增幅)
        /// </summary>
        public float GetPowerIncrease()
        {
            if (!isUnlocked || data.talentType != TalentType.SkillEnhancement)
                return 0f;

            return data.powerIncreasePerLevel * currentLevel;
        }

        /// <summary>
        /// 取得技能強化效果 (範圍增加)
        /// </summary>
        public float GetRangeIncrease()
        {
            if (!isUnlocked || data.talentType != TalentType.SkillEnhancement)
                return 0f;

            return data.rangeIncreasePerLevel * currentLevel;
        }

        /// <summary>
        /// 重置天賦 (轉換主元素時使用)
        /// </summary>
        public void Reset()
        {
            currentLevel = 0;
            isUnlocked = false;
            Debug.Log($"[TalentNode] 重置天賦: {data.talentName}");
        }

        /// <summary>
        /// 取得顯示資訊
        /// </summary>
        public string GetDisplayInfo()
        {
            if (!isUnlocked)
                return $"{data.talentName} (未解鎖)\n需要 {data.requiredTalentPoints} 天賦點";

            return data.GetFullDescription(currentLevel);
        }

        /// <summary>
        /// 檢查是否可以學習/升級
        /// </summary>
        /// <param name="playerLevel">玩家等級</param>
        /// <param name="prerequisiteMet">前置條件是否滿足</param>
        /// <returns>錯誤訊息 (null 表示可以學習)</returns>
        public string CanLearnOrUpgrade(int playerLevel, bool prerequisiteMet)
        {
            if (!isUnlocked)
            {
                // 檢查解鎖條件
                if (playerLevel < data.minLevelRequirement)
                    return $"需要等級 {data.minLevelRequirement}";

                if (!prerequisiteMet)
                    return "需要先學習前置天賦";

                return null; // 可以解鎖
            }
            else
            {
                // 檢查升級條件
                if (IsMaxLevel)
                    return "已達最高等級";

                return null; // 可以升級
            }
        }
    }
}
