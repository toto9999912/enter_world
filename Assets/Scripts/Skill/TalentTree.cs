using System;
using System.Collections.Generic;
using UnityEngine;
using Stats;
using Element;

namespace Skill
{
    /// <summary>
    /// 天賦樹系統
    /// 主元素專屬，只能學習與主元素相符的天賦
    /// </summary>
    [Serializable]
    public class TalentTree
    {
        // ===== 引用 =====
        private CharacterStats stats;
        private ElementType mainElement;

        // ===== 天賦數據 =====
        private Dictionary<string, TalentNode> talents = new Dictionary<string, TalentNode>();
        private int availableTalentPoints = 0;

        // ===== 事件 =====
        /// <summary>學習/升級天賦時觸發 (天賦ID, 新等級)</summary>
        public event Action<string, int> OnTalentChanged;

        /// <summary>天賦點數變化時觸發 (剩餘點數)</summary>
        public event Action<int> OnTalentPointsChanged;

        /// <summary>主元素變更時觸發 (新元素)</summary>
        public event Action<ElementType> OnMainElementChanged;

        // ===== 屬性 =====
        /// <summary>可用天賦點數</summary>
        public int AvailableTalentPoints => availableTalentPoints;

        /// <summary>主元素</summary>
        public ElementType MainElement => mainElement;

        /// <summary>已學習的天賦</summary>
        public IReadOnlyDictionary<string, TalentNode> Talents => talents;

        /// <summary>
        /// 建構子
        /// </summary>
        public TalentTree(CharacterStats stats, ElementType mainElement)
        {
            this.stats = stats;
            this.mainElement = mainElement;
        }

        /// <summary>
        /// 初始化天賦樹 (註冊所有可用天賦)
        /// </summary>
        /// <param name="availableTalents">該元素的所有天賦資料</param>
        public void InitializeTalents(TalentData[] availableTalents)
        {
            talents.Clear();

            foreach (var talentData in availableTalents)
            {
                if (talentData.requiredElement != mainElement)
                {
                    Debug.LogWarning($"[TalentTree] {talentData.talentName} 不屬於主元素 {mainElement}，跳過！");
                    continue;
                }

                talents[talentData.talentId] = new TalentNode(talentData);
            }

            Debug.Log($"[TalentTree] 初始化 {mainElement} 天賦樹，共 {talents.Count} 個天賦");
        }

        /// <summary>
        /// 增加天賦點數 (通常來自升級)
        /// </summary>
        public void AddTalentPoints(int points)
        {
            availableTalentPoints += points;
            OnTalentPointsChanged?.Invoke(availableTalentPoints);
            Debug.Log($"[TalentTree] 獲得 {points} 天賦點，當前: {availableTalentPoints}");
        }

        /// <summary>
        /// 學習或升級天賦
        /// </summary>
        /// <param name="talentId">天賦 ID</param>
        /// <param name="playerLevel">玩家當前等級</param>
        /// <returns>是否成功</returns>
        public bool LearnOrUpgradeTalent(string talentId, int playerLevel)
        {
            if (!talents.ContainsKey(talentId))
            {
                Debug.LogWarning($"[TalentTree] 找不到天賦 ID: {talentId}");
                return false;
            }

            TalentNode node = talents[talentId];
            TalentData data = node.Data;

            // 檢查天賦點數
            if (availableTalentPoints < data.requiredTalentPoints)
            {
                Debug.LogWarning($"[TalentTree] 天賦點數不足！需要 {data.requiredTalentPoints}，當前 {availableTalentPoints}");
                return false;
            }

            // 檢查前置條件
            bool prerequisiteMet = true;
            if (data.prerequisite != null)
            {
                string prereqId = data.prerequisite.talentId;
                prerequisiteMet = talents.ContainsKey(prereqId) && talents[prereqId].IsUnlocked;

                if (!prerequisiteMet)
                {
                    Debug.LogWarning($"[TalentTree] 需要先學習前置天賦: {data.prerequisite.talentName}");
                    return false;
                }
            }

            // 檢查其他條件
            string errorMsg = node.CanLearnOrUpgrade(playerLevel, prerequisiteMet);
            if (errorMsg != null)
            {
                Debug.LogWarning($"[TalentTree] {errorMsg}");
                return false;
            }

            // 執行學習/升級
            bool success = false;
            if (!node.IsUnlocked)
            {
                success = node.Unlock();
            }
            else
            {
                success = node.LevelUp();
            }

            if (success)
            {
                // 扣除天賦點數
                availableTalentPoints -= data.requiredTalentPoints;
                OnTalentPointsChanged?.Invoke(availableTalentPoints);

                // 應用天賦效果到 CharacterStats
                ApplyTalentModifiers(node);

                OnTalentChanged?.Invoke(talentId, node.CurrentLevel);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 應用天賦修飾器到角色屬性
        /// </summary>
        private void ApplyTalentModifiers(TalentNode node)
        {
            if (stats == null) return;

            // 先移除舊的修飾器 (由於等級變化)
            stats.RemoveModifiersBySource(node.Data.talentId);

            // 添加新的修飾器
            StatModifier[] modifiers = node.GetCurrentModifiers();
            foreach (var modifier in modifiers)
            {
                stats.AddModifier(modifier);
            }

            Debug.Log($"[TalentTree] 應用天賦效果: {node.Data.talentName} Lv.{node.CurrentLevel}");
        }

        /// <summary>
        /// 取得技能強化效果 (冷卻減少)
        /// </summary>
        /// <param name="skillId">技能 ID</param>
        public float GetSkillCooldownReduction(string skillId)
        {
            float totalReduction = 0f;

            foreach (var node in talents.Values)
            {
                if (node.Data.talentType == TalentType.SkillEnhancement &&
                    node.Data.enhancedSkillId == skillId)
                {
                    totalReduction += node.GetCooldownReduction();
                }
            }

            return totalReduction;
        }

        /// <summary>
        /// 取得技能強化效果 (威力增幅)
        /// </summary>
        /// <param name="skillId">技能 ID</param>
        public float GetSkillPowerIncrease(string skillId)
        {
            float totalIncrease = 0f;

            foreach (var node in talents.Values)
            {
                if (node.Data.talentType == TalentType.SkillEnhancement &&
                    node.Data.enhancedSkillId == skillId)
                {
                    totalIncrease += node.GetPowerIncrease();
                }
            }

            return totalIncrease;
        }

        /// <summary>
        /// 取得技能強化效果 (範圍增加)
        /// </summary>
        /// <param name="skillId">技能 ID</param>
        public float GetSkillRangeIncrease(string skillId)
        {
            float totalIncrease = 0f;

            foreach (var node in talents.Values)
            {
                if (node.Data.talentType == TalentType.SkillEnhancement &&
                    node.Data.enhancedSkillId == skillId)
                {
                    totalIncrease += node.GetRangeIncrease();
                }
            }

            return totalIncrease;
        }

        /// <summary>
        /// 重置所有天賦 (轉換主元素時使用)
        /// </summary>
        /// <param name="newMainElement">新的主元素</param>
        /// <param name="newAvailableTalents">新元素的天賦列表</param>
        /// <returns>退還的天賦點數</returns>
        public int ResetAllTalents(ElementType newMainElement, TalentData[] newAvailableTalents)
        {
            // 計算退還的點數
            int refundedPoints = 0;
            foreach (var node in talents.Values)
            {
                if (node.IsUnlocked)
                {
                    refundedPoints += node.Data.requiredTalentPoints * node.CurrentLevel;
                }
            }

            // 移除所有修飾器
            foreach (var node in talents.Values)
            {
                if (stats != null && node.IsUnlocked)
                {
                    stats.RemoveModifiersBySource(node.Data.talentId);
                }
                node.Reset();
            }

            // 更新主元素並重新初始化
            mainElement = newMainElement;
            InitializeTalents(newAvailableTalents);

            // 退還點數
            availableTalentPoints += refundedPoints;
            OnMainElementChanged?.Invoke(newMainElement);
            OnTalentPointsChanged?.Invoke(availableTalentPoints);

            Debug.Log($"[TalentTree] 重置天賦樹，切換至 {newMainElement}，退還 {refundedPoints} 點");
            return refundedPoints;
        }

        /// <summary>
        /// 取得天賦節點
        /// </summary>
        public TalentNode GetTalent(string talentId)
        {
            return talents.ContainsKey(talentId) ? talents[talentId] : null;
        }

        /// <summary>
        /// 取得已投入的總天賦點數
        /// </summary>
        public int GetTotalInvestedPoints()
        {
            int total = 0;
            foreach (var node in talents.Values)
            {
                if (node.IsUnlocked)
                {
                    total += node.Data.requiredTalentPoints * node.CurrentLevel;
                }
            }
            return total;
        }

        /// <summary>
        /// 更新所有天賦效果 (屬性變化時呼叫)
        /// </summary>
        public void RefreshAllTalents()
        {
            if (stats == null) return;

            foreach (var node in talents.Values)
            {
                if (node.IsUnlocked)
                {
                    ApplyTalentModifiers(node);
                }
            }

            Debug.Log("[TalentTree] 刷新所有天賦效果");
        }
    }
}
