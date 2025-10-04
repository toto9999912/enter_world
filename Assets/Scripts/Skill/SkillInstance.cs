using System;
using UnityEngine;

namespace Skill
{
    /// <summary>
    /// 技能實例
    /// 代表玩家已學習的技能，包含等級與冷卻狀態
    /// </summary>
    [Serializable]
    public class SkillInstance
    {
        // ===== 基礎資料 =====
        [SerializeField] private SkillData skillData;
        [SerializeField] private int currentLevel;
        [SerializeField] private float cooldownRemaining;

        // ===== 事件 =====
        public event Action<int> OnLevelUp;
        public event Action OnCooldownStart;
        public event Action OnCooldownComplete;

        // ===== 屬性 =====
        public SkillData Data => skillData;
        public int CurrentLevel => currentLevel;
        public int MaxLevel => skillData.maxSkillLevel;
        public bool IsMaxLevel => currentLevel >= MaxLevel;
        public bool IsOnCooldown => cooldownRemaining > 0f;
        public float CooldownRemaining => cooldownRemaining;
        public float CooldownPercent => skillData.cooldown > 0 ? cooldownRemaining / skillData.cooldown : 0f;

        /// <summary>
        /// 建構子
        /// </summary>
        public SkillInstance(SkillData skillData, int initialLevel = 1)
        {
            this.skillData = skillData;
            this.currentLevel = Mathf.Clamp(initialLevel, 1, skillData.maxSkillLevel);
            this.cooldownRemaining = 0f;
        }

        /// <summary>
        /// 更新冷卻 (每幀調用)
        /// </summary>
        public void Update(float deltaTime)
        {
            if (cooldownRemaining > 0f)
            {
                cooldownRemaining -= deltaTime;
                if (cooldownRemaining <= 0f)
                {
                    cooldownRemaining = 0f;
                    OnCooldownComplete?.Invoke();
                }
            }
        }

        /// <summary>
        /// 升級技能
        /// </summary>
        public bool LevelUp()
        {
            if (IsMaxLevel)
            {
                Debug.LogWarning($"[Skill] {skillData.skillName} 已達最高等級！");
                return false;
            }

            currentLevel++;
            OnLevelUp?.Invoke(currentLevel);
            Debug.Log($"[Skill] {skillData.skillName} 升級至 Lv.{currentLevel}");
            return true;
        }

        /// <summary>
        /// 開始冷卻
        /// </summary>
        public void StartCooldown(float skillHaste = 0f)
        {
            float actualCooldown = skillData.GetActualCooldown(skillHaste);
            cooldownRemaining = actualCooldown;
            OnCooldownStart?.Invoke();
        }

        /// <summary>
        /// 重置冷卻 (立即可用)
        /// </summary>
        public void ResetCooldown()
        {
            cooldownRemaining = 0f;
            OnCooldownComplete?.Invoke();
        }

        /// <summary>
        /// 減少冷卻時間 (特殊道具/技能)
        /// </summary>
        public void ReduceCooldown(float amount)
        {
            if (cooldownRemaining > 0f)
            {
                cooldownRemaining = Mathf.Max(0f, cooldownRemaining - amount);
                if (cooldownRemaining <= 0f)
                {
                    OnCooldownComplete?.Invoke();
                }
            }
        }

        /// <summary>
        /// 取得當前等級的傷害倍率
        /// </summary>
        public float GetDamageMultiplier()
        {
            return skillData.GetDamageMultiplier(currentLevel);
        }

        /// <summary>
        /// 取得當前等級的魔力消耗
        /// </summary>
        public float GetManaCost()
        {
            return skillData.GetManaCost(currentLevel);
        }

        /// <summary>
        /// 取得顯示資訊
        /// </summary>
        public string GetDisplayInfo()
        {
            string info = $"{skillData.skillName} Lv.{currentLevel}/{MaxLevel}\n";
            info += $"消耗: {GetManaCost():F0} MP\n";
            info += $"冷卻: {skillData.cooldown:F1} 秒\n";

            if (IsOnCooldown)
                info += $"冷卻中: {cooldownRemaining:F1} 秒";

            return info;
        }
    }
}
