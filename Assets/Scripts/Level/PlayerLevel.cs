using System;
using UnityEngine;
using Stats;

namespace Level
{
    /// <summary>
    /// 玩家等級與經驗系統
    /// </summary>
    public class PlayerLevel
    {
        // ===== 配置參數 =====
        public static int baseExpRequirement = 100;    // 基礎經驗需求
        public static float expGrowthRate = 1.5f;      // 成長率
        public static int skillPointsPerLevel = 2;     // 每級獲得技能點數
        public static int talentPointsPerLevel = 1;    // 每級獲得天賦點數
        public static int attributePointsPerLevel = 5; // 每級獲得屬性點數

        // ===== 當前數據 =====
        private int currentLevel;
        private int currentExp;
        private int expToNextLevel;
        private int availableAttributePoints;

        // ===== 事件 =====
        public event Action<int> OnLevelUp;
        public event Action<int, int> OnExpGained; // 當前經驗, 升級所需
        public event Action<int> OnAttributePointsChanged;

        // ===== 屬性 =====
        public int CurrentLevel => currentLevel;
        public int CurrentExp => currentExp;
        public int ExpToNextLevel => expToNextLevel;
        public float ExpPercent => (float)currentExp / expToNextLevel;
        public int AvailableAttributePoints => availableAttributePoints;

        public PlayerLevel(int startLevel = 1)
        {
            currentLevel = startLevel;
            currentExp = 0;
            expToNextLevel = CalculateExpForLevel(startLevel);
            availableAttributePoints = 0;
        }

        /// <summary>
        /// 獲得經驗值
        /// </summary>
        public int GainExp(int exp)
        {
            currentExp += exp;
            int levelsGained = 0;

            while (currentExp >= expToNextLevel)
            {
                LevelUp();
                levelsGained++;
            }

            OnExpGained?.Invoke(currentExp, expToNextLevel);
            return levelsGained;
        }

        /// <summary>
        /// 升級
        /// </summary>
        private void LevelUp()
        {
            currentExp -= expToNextLevel;
            currentLevel++;
            expToNextLevel = CalculateExpForLevel(currentLevel);
            availableAttributePoints += attributePointsPerLevel;

            OnLevelUp?.Invoke(currentLevel);
            OnAttributePointsChanged?.Invoke(availableAttributePoints);
        }

        /// <summary>
        /// 計算等級所需經驗 (可調公式)
        /// </summary>
        private int CalculateExpForLevel(int level)
        {
            return Mathf.CeilToInt(baseExpRequirement * Mathf.Pow(level, expGrowthRate));
        }

        /// <summary>
        /// 消耗屬性點
        /// </summary>
        public bool SpendAttributePoint(CharacterStats stats, StatType statType, int points = 1)
        {
            if (availableAttributePoints < points)
                return false;

            float currentValue = stats.GetBaseValue(statType);
            stats.SetBaseValue(statType, currentValue + points);
            availableAttributePoints -= points;

            OnAttributePointsChanged?.Invoke(availableAttributePoints);
            return true;
        }
    }
}
