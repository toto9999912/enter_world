using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Stats;
using Core;

namespace Companion
{
    /// <summary>
    /// 眷屬管理器
    /// 管理玩家的所有眷屬、SP 消耗、重生機制
    /// </summary>
    public class CompanionManager
    {
        // ===== 配置參數 (預設值，可調整) =====

        /// <summary>重生時間公式: 基礎時間 × (A - B × SP百分比)</summary>
        public static float reviveTimeMultiplierA = 2.0f; // 預設: 2.0 (SP=0% 時 2 倍時間)
        public static float reviveTimeMultiplierB = 1.0f; // 預設: 1.0 (SP=100% 時 1 倍時間)

        /// <summary>重生時間隨機浮動範圍 (±百分比)</summary>
        public static float reviveTimeVariance = 0.1f; // ±10%

        // ===== 引用 =====
        private CharacterStats playerStats; // 玩家屬性 (用於取得 SP)
        private CompanionRarityConfig rarityConfig; // 稀有度配置

        // ===== 眷屬資料 =====
        private List<CompanionInstance> ownedCompanions = new List<CompanionInstance>();
        private List<CompanionInstance> activeCompanions = new List<CompanionInstance>();

        // ===== 事件 =====
        public event Action<CompanionInstance> OnCompanionAdded;
        public event Action<CompanionInstance> OnCompanionRemoved;
        public event Action<CompanionInstance> OnCompanionDeployed;
        public event Action<CompanionInstance> OnCompanionRecalled;
        public event Action<CompanionInstance> OnCompanionDied;
        public event Action<CompanionInstance> OnCompanionRevived;
        public event Action<float, float> OnSPChanged; // 當前 SP, 最大 SP

        // ===== 屬性 =====
        public IReadOnlyList<CompanionInstance> OwnedCompanions => ownedCompanions;
        public IReadOnlyList<CompanionInstance> ActiveCompanions => activeCompanions;

        /// <summary>當前使用的 SP</summary>
        public float CurrentSPUsage
        {
            get
            {
                float total = 0f;
                foreach (var companion in activeCompanions)
                {
                    total += companion.Data.GetSPCost();
                }
                return total;
            }
        }

        /// <summary>最大 SP (從玩家屬性取得)</summary>
        public float MaxSP => playerStats?.GetFinalValue(StatType.SP) ?? 100f;

        /// <summary>可用 SP</summary>
        public float AvailableSP => MaxSP - CurrentSPUsage;

        /// <summary>SP 百分比 (0-1)</summary>
        public float SPPercent => MaxSP > 0 ? (MaxSP - CurrentSPUsage) / MaxSP : 0f;

        /// <summary>
        /// 建構子
        /// </summary>
        public CompanionManager(CharacterStats playerStats, CompanionRarityConfig rarityConfig = null)
        {
            this.playerStats = playerStats;
            this.rarityConfig = rarityConfig;
        }

        /// <summary>
        /// 更新管理器 (每幀調用)
        /// </summary>
        public void Update(float deltaTime)
        {
            // 更新所有眷屬的重生計時
            foreach (var companion in ownedCompanions)
            {
                companion.Update(deltaTime);
            }
        }

        /// <summary>
        /// 新增眷屬
        /// </summary>
        public bool AddCompanion(CompanionInstance companion)
        {
            if (ownedCompanions.Contains(companion))
            {
                Debug.LogWarning($"[CompanionManager] 眷屬 {companion.Data.companionName} 已存在！");
                return false;
            }

            ownedCompanions.Add(companion);

            // 監聽眷屬死亡事件
            companion.OnDeath += () => HandleCompanionDeath(companion);
            companion.OnReviveComplete += () => HandleCompanionRevived(companion);

            OnCompanionAdded?.Invoke(companion);
            Debug.Log($"[CompanionManager] 新增眷屬: {companion.Data.companionName} Lv.{companion.Level}");
            return true;
        }

        /// <summary>
        /// 移除眷屬
        /// </summary>
        public bool RemoveCompanion(CompanionInstance companion)
        {
            if (!ownedCompanions.Contains(companion))
                return false;

            // 如果正在出戰，先召回
            if (activeCompanions.Contains(companion))
            {
                RecallCompanion(companion);
            }

            ownedCompanions.Remove(companion);
            OnCompanionRemoved?.Invoke(companion);
            return true;
        }

        /// <summary>
        /// 部署眷屬
        /// </summary>
        public bool DeployCompanion(CompanionInstance companion)
        {
            if (!ownedCompanions.Contains(companion))
            {
                Debug.LogWarning("[CompanionManager] 該眷屬不屬於玩家！");
                return false;
            }

            if (activeCompanions.Contains(companion))
            {
                Debug.LogWarning("[CompanionManager] 眷屬已在場上！");
                return false;
            }

            // 檢查 SP 是否足夠
            float requiredSP = companion.Data.GetSPCost();
            if (AvailableSP < requiredSP)
            {
                Debug.LogWarning($"[CompanionManager] SP 不足！需要 {requiredSP}，可用 {AvailableSP}");
                return false;
            }

            // 部署
            if (companion.Deploy())
            {
                activeCompanions.Add(companion);
                OnCompanionDeployed?.Invoke(companion);
                OnSPChanged?.Invoke(AvailableSP, MaxSP);

                // 發布全域事件
                EventBus.Publish(new CompanionDeployedEvent
                {
                    CompanionId = companion.InstanceId
                });

                Debug.Log($"[CompanionManager] 部署眷屬: {companion.Data.companionName}");
                return true;
            }

            return false;
        }

        /// <summary>
        /// 召回眷屬
        /// </summary>
        public bool RecallCompanion(CompanionInstance companion)
        {
            if (!activeCompanions.Contains(companion))
            {
                Debug.LogWarning("[CompanionManager] 眷屬不在場上！");
                return false;
            }

            if (companion.Recall())
            {
                activeCompanions.Remove(companion);
                OnCompanionRecalled?.Invoke(companion);
                OnSPChanged?.Invoke(AvailableSP, MaxSP);
                Debug.Log($"[CompanionManager] 召回眷屬: {companion.Data.companionName}");
                return true;
            }

            return false;
        }

        /// <summary>
        /// 召回所有眷屬
        /// </summary>
        public void RecallAllCompanions()
        {
            // 複製列表避免迭代時修改
            var companions = activeCompanions.ToList();
            foreach (var companion in companions)
            {
                RecallCompanion(companion);
            }
        }

        /// <summary>
        /// 處理眷屬死亡
        /// </summary>
        private void HandleCompanionDeath(CompanionInstance companion)
        {
            // 從場上移除
            if (activeCompanions.Contains(companion))
            {
                activeCompanions.Remove(companion);
                OnSPChanged?.Invoke(AvailableSP, MaxSP);
            }

            OnCompanionDied?.Invoke(companion);

            // 開始重生流程 (如果 SP > 0)
            if (SPPercent > 0f)
            {
                StartReviveProcess(companion);
            }
            else
            {
                Debug.LogWarning($"[CompanionManager] SP = 0，{companion.Data.companionName} 無法重生！");
            }
        }

        /// <summary>
        /// 開始重生流程
        /// </summary>
        private void StartReviveProcess(CompanionInstance companion)
        {
            float reviveTime = CalculateReviveTime(companion);
            companion.StartRevive(reviveTime);
        }

        /// <summary>
        /// 計算重生時間
        /// 公式: 基礎時間(稀有度) × (A - B × SP%) × (1 ± 隨機浮動)
        /// </summary>
        private float CalculateReviveTime(CompanionInstance companion)
        {
            // 取得基礎重生時間
            float baseTime = 30f; // 預設值
            if (rarityConfig != null)
            {
                baseTime = rarityConfig.GetBaseReviveTime(companion.Data.rarity);
            }

            // SP 百分比影響
            float spMultiplier = reviveTimeMultiplierA - (reviveTimeMultiplierB * SPPercent);
            spMultiplier = Mathf.Max(0.5f, spMultiplier); // 最低 0.5 倍

            // 基礎時間 × SP 倍率
            float calculatedTime = baseTime * spMultiplier;

            // 隨機浮動
            float variance = calculatedTime * reviveTimeVariance;
            float randomOffset = UnityEngine.Random.Range(-variance, variance);
            calculatedTime += randomOffset;

            // 最低 1 秒
            return Mathf.Max(1f, calculatedTime);
        }

        /// <summary>
        /// 處理眷屬重生完成
        /// </summary>
        private void HandleCompanionRevived(CompanionInstance companion)
        {
            OnCompanionRevived?.Invoke(companion);
            Debug.Log($"[CompanionManager] {companion.Data.companionName} 重生完成！");
        }

        /// <summary>
        /// 取得特定狀態的眷屬
        /// </summary>
        public List<CompanionInstance> GetCompanionsByState(CompanionState state)
        {
            return ownedCompanions.Where(c => c.State == state).ToList();
        }

        /// <summary>
        /// 取得正在重生的眷屬數量
        /// </summary>
        public int GetRevivingCount()
        {
            return ownedCompanions.Count(c => c.State == CompanionState.Reviving);
        }

        /// <summary>
        /// 檢查是否能部署更多眷屬
        /// </summary>
        public bool CanDeployMore(float requiredSP)
        {
            return AvailableSP >= requiredSP;
        }

        /// <summary>
        /// 立即復活所有死亡眷屬 (特殊道具/技能)
        /// </summary>
        public void ReviveAllDead(float hpPercent = 0.5f, float mpPercent = 0.5f)
        {
            foreach (var companion in ownedCompanions)
            {
                if (companion.State == CompanionState.Dead || companion.State == CompanionState.Reviving)
                {
                    companion.InstantRevive(hpPercent, mpPercent);
                }
            }
        }

        /// <summary>
        /// 取得管理器狀態資訊
        /// </summary>
        public string GetStatusInfo()
        {
            return $"眷屬管理器\n" +
                   $"擁有: {ownedCompanions.Count} 隻\n" +
                   $"出戰: {activeCompanions.Count} 隻\n" +
                   $"SP: {AvailableSP:F0}/{MaxSP:F0} ({SPPercent * 100f:F1}%)\n" +
                   $"重生中: {GetRevivingCount()} 隻";
        }
    }
}
