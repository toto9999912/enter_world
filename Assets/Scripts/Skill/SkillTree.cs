using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Element;

namespace Skill
{
    /// <summary>
    /// 技能樹
    /// 管理玩家的技能學習、升級、技能點數消耗
    /// </summary>
    public class SkillTree
    {
        // ===== 玩家資料 =====
        private ElementType playerMainElement;
        private int availableSkillPoints;

        // ===== 技能資料 =====
        private Dictionary<string, SkillInstance> learnedSkills = new Dictionary<string, SkillInstance>();
        private HashSet<string> unlockedSkills = new HashSet<string>(); // 已解鎖但未學習

        // ===== 事件 =====
        public event Action<SkillInstance> OnSkillLearned;
        public event Action<SkillInstance, int> OnSkillLevelUp;
        public event Action<int> OnSkillPointsChanged;

        // ===== 屬性 =====
        public int AvailableSkillPoints => availableSkillPoints;
        public ElementType PlayerMainElement => playerMainElement;
        public IReadOnlyDictionary<string, SkillInstance> LearnedSkills => learnedSkills;

        /// <summary>
        /// 建構子
        /// </summary>
        public SkillTree(ElementType mainElement, int initialSkillPoints = 0)
        {
            this.playerMainElement = mainElement;
            this.availableSkillPoints = initialSkillPoints;
        }

        /// <summary>
        /// 更新技能樹 (每幀調用，更新冷卻)
        /// </summary>
        public void Update(float deltaTime)
        {
            foreach (var skill in learnedSkills.Values)
            {
                skill.Update(deltaTime);
            }
        }

        /// <summary>
        /// 學習技能
        /// </summary>
        public bool LearnSkill(SkillData skillData)
        {
            // 檢查是否已學習
            if (learnedSkills.ContainsKey(skillData.skillId))
            {
                Debug.LogWarning($"[SkillTree] 技能 {skillData.skillName} 已學習！");
                return false;
            }

            // 檢查技能點數
            int requiredPoints = skillData.CalculateSkillPointCost(playerMainElement);
            if (availableSkillPoints < requiredPoints)
            {
                Debug.LogWarning($"[SkillTree] 技能點數不足！需要 {requiredPoints}，當前 {availableSkillPoints}");
                return false;
            }

            // 檢查前置技能
            if (!CheckPrerequisites(skillData))
            {
                Debug.LogWarning($"[SkillTree] 未滿足前置技能需求！");
                return false;
            }

            // 學習技能
            SkillInstance newSkill = new SkillInstance(skillData, initialLevel: 1);
            learnedSkills[skillData.skillId] = newSkill;

            // 消耗技能點數
            availableSkillPoints -= requiredPoints;

            OnSkillLearned?.Invoke(newSkill);
            OnSkillPointsChanged?.Invoke(availableSkillPoints);

            Debug.Log($"[SkillTree] 學習技能: {skillData.skillName} (消耗 {requiredPoints} 點)");
            return true;
        }

        /// <summary>
        /// 升級技能
        /// </summary>
        public bool UpgradeSkill(string skillId)
        {
            if (!learnedSkills.TryGetValue(skillId, out SkillInstance skill))
            {
                Debug.LogWarning($"[SkillTree] 技能未學習: {skillId}");
                return false;
            }

            if (skill.IsMaxLevel)
            {
                Debug.LogWarning($"[SkillTree] 技能已達最高等級: {skill.Data.skillName}");
                return false;
            }

            // 計算升級所需技能點 (升級只需 1 點)
            int requiredPoints = 1;
            if (availableSkillPoints < requiredPoints)
            {
                Debug.LogWarning($"[SkillTree] 技能點數不足！");
                return false;
            }

            // 升級
            skill.LevelUp();
            availableSkillPoints -= requiredPoints;

            OnSkillLevelUp?.Invoke(skill, skill.CurrentLevel);
            OnSkillPointsChanged?.Invoke(availableSkillPoints);

            return true;
        }

        /// <summary>
        /// 檢查前置技能
        /// </summary>
        private bool CheckPrerequisites(SkillData skillData)
        {
            if (skillData.prerequisiteSkills == null || skillData.prerequisiteSkills.Length == 0)
                return true;

            foreach (string prereqId in skillData.prerequisiteSkills)
            {
                if (!learnedSkills.ContainsKey(prereqId))
                {
                    Debug.LogWarning($"[SkillTree] 缺少前置技能: {prereqId}");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 增加技能點數
        /// </summary>
        public void AddSkillPoints(int points)
        {
            availableSkillPoints += points;
            OnSkillPointsChanged?.Invoke(availableSkillPoints);
            Debug.Log($"[SkillTree] 獲得技能點數: +{points} (總計: {availableSkillPoints})");
        }

        /// <summary>
        /// 取得已學習的技能
        /// </summary>
        public SkillInstance GetSkill(string skillId)
        {
            learnedSkills.TryGetValue(skillId, out SkillInstance skill);
            return skill;
        }

        /// <summary>
        /// 檢查是否學習了技能
        /// </summary>
        public bool HasLearnedSkill(string skillId)
        {
            return learnedSkills.ContainsKey(skillId);
        }

        /// <summary>
        /// 取得特定分類的技能
        /// </summary>
        public List<SkillInstance> GetSkillsByCategory(SkillCategory category)
        {
            return learnedSkills.Values
                .Where(s => s.Data.category == category)
                .ToList();
        }

        /// <summary>
        /// 取得特定元素的技能
        /// </summary>
        public List<SkillInstance> GetSkillsByElement(ElementType element)
        {
            return learnedSkills.Values
                .Where(s => s.Data.element == element)
                .ToList();
        }

        /// <summary>
        /// 重置所有技能冷卻 (特殊道具/技能)
        /// </summary>
        public void ResetAllCooldowns()
        {
            foreach (var skill in learnedSkills.Values)
            {
                skill.ResetCooldown();
            }
            Debug.Log("[SkillTree] 所有技能冷卻已重置！");
        }

        /// <summary>
        /// 更換主屬性 (使用特殊道具時)
        /// </summary>
        public void ChangeMainElement(ElementType newElement)
        {
            ElementType oldElement = playerMainElement;
            playerMainElement = newElement;
            Debug.Log($"[SkillTree] 主屬性變更: {oldElement} → {newElement}");
        }

        /// <summary>
        /// 取得技能樹統計資訊
        /// </summary>
        public string GetStatisticsInfo()
        {
            int totalSkills = learnedSkills.Count;
            int maxLevelSkills = learnedSkills.Values.Count(s => s.IsMaxLevel);

            var categoryCount = new Dictionary<SkillCategory, int>();
            foreach (var skill in learnedSkills.Values)
            {
                var cat = skill.Data.category;
                if (!categoryCount.ContainsKey(cat))
                    categoryCount[cat] = 0;
                categoryCount[cat]++;
            }

            string info = $"技能樹統計\n";
            info += $"主屬性: {ElementalAffinityTable.GetElementName(playerMainElement)}\n";
            info += $"技能點數: {availableSkillPoints}\n";
            info += $"已學習技能: {totalSkills}\n";
            info += $"滿級技能: {maxLevelSkills}\n\n";
            info += "分類統計:\n";

            foreach (var kvp in categoryCount)
            {
                info += $"  {kvp.Key}: {kvp.Value} 個\n";
            }

            return info;
        }
    }
}
