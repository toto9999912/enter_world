using System;
using UnityEngine;
using Stats;
using Combat;
using Element;
using Core;

namespace Companion
{
    /// <summary>
    /// 眷屬實例狀態
    /// </summary>
    public enum CompanionState
    {
        /// <summary>待命 (未出戰)</summary>
        Standby,

        /// <summary>戰鬥中</summary>
        Active,

        /// <summary>死亡 (重生中)</summary>
        Dead,

        /// <summary>重生冷卻中</summary>
        Reviving
    }

    /// <summary>
    /// 眷屬實例
    /// 代表玩家擁有的一隻具體眷屬，包含等級、經驗值、狀態等
    /// </summary>
    [Serializable]
    public class CompanionInstance
    {
        // ===== 基礎資料 =====
        [SerializeField] private string instanceId; // 唯一 ID
        [SerializeField] private CompanionData data; // 眷屬資料模板

        // ===== 等級與經驗 =====
        [SerializeField] private int level;
        [SerializeField] private int currentExp;
        [SerializeField] private int expToNextLevel;

        // ===== 狀態 =====
        [SerializeField] private CompanionState state = CompanionState.Standby;
        [SerializeField] private float reviveTimeRemaining = 0f;

        // ===== 系統引用 (運行時創建) =====
        private CharacterStats stats;
        private HealthSystem health;
        private CombatSystem combat;

        // ===== 事件 =====
        public event Action<int> OnLevelUp;
        public event Action OnDeath;
        public event Action OnReviveComplete;
        public event Action<CompanionState, CompanionState> OnStateChanged;

        // ===== 屬性 =====
        public string InstanceId => instanceId;
        public CompanionData Data => data;
        public int Level => level;
        public int CurrentExp => currentExp;
        public int ExpToNextLevel => expToNextLevel;
        public CompanionState State => state;
        public float ReviveTimeRemaining => reviveTimeRemaining;
        public CharacterStats Stats => stats;
        public HealthSystem Health => health;
        public CombatSystem Combat => combat;

        /// <summary>
        /// 建構子
        /// </summary>
        public CompanionInstance(CompanionData data, int level = 1)
        {
            this.instanceId = Guid.NewGuid().ToString();
            this.data = data;
            this.level = Mathf.Max(1, level);
            this.currentExp = 0;
            this.expToNextLevel = CalculateExpForNextLevel(level);

            InitializeSystems();
        }

        /// <summary>
        /// 初始化屬性/生命/戰鬥系統
        /// </summary>
        private void InitializeSystems()
        {
            // 創建屬性系統
            stats = new CharacterStats();
            ApplyLevelStats();

            // 創建生命系統
            health = new HealthSystem(stats, initializeAtMax: true);
            health.OnDeath += HandleDeath;

            // 創建戰鬥系統
            combat = new CombatSystem(stats, health);
            combat.Element = data.element;
        }

        /// <summary>
        /// 套用等級對應的屬性值
        /// </summary>
        private void ApplyLevelStats()
        {
            stats.SetBaseValue(StatType.HP, data.CalculateStat(StatType.HP, level));
            stats.SetBaseValue(StatType.MP, data.CalculateStat(StatType.MP, level));
            stats.SetBaseValue(StatType.ATK, data.CalculateStat(StatType.ATK, level));
            stats.SetBaseValue(StatType.Magic, data.CalculateStat(StatType.Magic, level));
            stats.SetBaseValue(StatType.DEF, data.CalculateStat(StatType.DEF, level));
            stats.SetBaseValue(StatType.SPD, data.CalculateStat(StatType.SPD, level));
        }

        /// <summary>
        /// 更新眷屬 (每幀調用)
        /// </summary>
        public void Update(float deltaTime)
        {
            // 處理重生計時
            if (state == CompanionState.Reviving)
            {
                reviveTimeRemaining -= deltaTime;
                if (reviveTimeRemaining <= 0f)
                {
                    CompleteRevive();
                }
            }
        }

        /// <summary>
        /// 獲得經驗值
        /// </summary>
        public void GainExp(int exp)
        {
            if (state == CompanionState.Dead) return;

            currentExp += exp;

            // 檢查升級
            while (currentExp >= expToNextLevel)
            {
                LevelUp();
            }
        }

        /// <summary>
        /// 升級
        /// </summary>
        private void LevelUp()
        {
            currentExp -= expToNextLevel;
            level++;
            expToNextLevel = CalculateExpForNextLevel(level);

            // 更新屬性
            ApplyLevelStats();

            // 回滿血魔
            health.InitializeToMax();

            OnLevelUp?.Invoke(level);
            Debug.Log($"[Companion] {data.companionName} 升級至 Lv.{level}！");
        }

        /// <summary>
        /// 計算升級所需經驗 (可調整公式)
        /// </summary>
        private int CalculateExpForNextLevel(int currentLevel)
        {
            // 預設公式: 100 + (level * 50)
            // Lv1→2: 150, Lv2→3: 200, Lv10→11: 600
            return 100 + (currentLevel * 50);
        }

        /// <summary>
        /// 出戰
        /// </summary>
        public bool Deploy()
        {
            if (state != CompanionState.Standby)
            {
                Debug.LogWarning($"[Companion] {data.companionName} 無法出戰，當前狀態: {state}");
                return false;
            }

            ChangeState(CompanionState.Active);
            return true;
        }

        /// <summary>
        /// 召回
        /// </summary>
        public bool Recall()
        {
            if (state != CompanionState.Active)
            {
                Debug.LogWarning($"[Companion] {data.companionName} 無法召回，當前狀態: {state}");
                return false;
            }

            ChangeState(CompanionState.Standby);
            return true;
        }

        /// <summary>
        /// 處理死亡
        /// </summary>
        private void HandleDeath()
        {
            ChangeState(CompanionState.Dead);
            OnDeath?.Invoke();
            Debug.Log($"[Companion] {data.companionName} 已死亡！");
        }

        /// <summary>
        /// 開始重生 (由 CompanionManager 調用)
        /// </summary>
        public void StartRevive(float reviveTime)
        {
            if (state != CompanionState.Dead)
            {
                Debug.LogWarning($"[Companion] {data.companionName} 不是死亡狀態，無法重生");
                return;
            }

            reviveTimeRemaining = reviveTime;
            ChangeState(CompanionState.Reviving);
            Debug.Log($"[Companion] {data.companionName} 開始重生，需要 {reviveTime:F1} 秒");
        }

        /// <summary>
        /// 完成重生
        /// </summary>
        private void CompleteRevive()
        {
            // 復活並回復一定 HP/MP
            health.Revive(hpPercent: 0.5f, mpPercent: 0.5f);

            ChangeState(CompanionState.Standby);
            OnReviveComplete?.Invoke();
            Debug.Log($"[Companion] {data.companionName} 重生完成！");
        }

        /// <summary>
        /// 立即復活 (特殊道具/技能)
        /// </summary>
        public void InstantRevive(float hpPercent = 1.0f, float mpPercent = 1.0f)
        {
            if (state != CompanionState.Dead && state != CompanionState.Reviving)
                return;

            health.Revive(hpPercent, mpPercent);
            reviveTimeRemaining = 0f;
            ChangeState(CompanionState.Standby);
            OnReviveComplete?.Invoke();
        }

        /// <summary>
        /// 改變狀態
        /// </summary>
        private void ChangeState(CompanionState newState)
        {
            if (state == newState) return;

            CompanionState oldState = state;
            state = newState;
            OnStateChanged?.Invoke(oldState, newState);
        }

        /// <summary>
        /// 取得顯示資訊
        /// </summary>
        public string GetDisplayInfo()
        {
            return $"{data.companionName} Lv.{level}\n" +
                   $"HP: {health.CurrentHP:F0}/{health.MaxHP:F0}\n" +
                   $"Exp: {currentExp}/{expToNextLevel}\n" +
                   $"狀態: {GetStateText()}";
        }

        /// <summary>
        /// 取得狀態文字
        /// </summary>
        private string GetStateText()
        {
            return state switch
            {
                CompanionState.Standby => "待命",
                CompanionState.Active => "戰鬥中",
                CompanionState.Dead => "死亡",
                CompanionState.Reviving => $"重生中 ({reviveTimeRemaining:F1}秒)",
                _ => "未知"
            };
        }
    }
}
