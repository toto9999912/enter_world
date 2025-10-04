using System;
using UnityEngine;
using Stats;
using System.Collections.Generic;

namespace Item
{
    /// <summary>
    /// 裝備實例
    /// 代表玩家擁有的一件具體裝備，包含強化等級、耐久等
    /// </summary>
    [Serializable]
    public class EquipmentInstance
    {
        // ===== 基礎資料 =====
        [SerializeField] private string instanceId;
        [SerializeField] private EquipmentData data;

        // ===== 強化與耐久 =====
        [SerializeField] private int enhanceLevel = 0;
        [SerializeField] private int currentDurability;
        [SerializeField] private int maxDurability;
        [SerializeField] private bool isDamaged = false;

        // ===== 事件 =====
        public event Action<int> OnEnhanced;
        public event Action OnDamaged;
        public event Action OnRepaired;

        // ===== 屬性 =====
        public string InstanceId => instanceId;
        public EquipmentData Data => data;
        public int EnhanceLevel => enhanceLevel;
        public int MaxEnhanceLevel => data.maxEnhanceLevel;
        public int CurrentDurability => currentDurability;
        public int MaxDurability => maxDurability;
        public float DurabilityPercent => maxDurability > 0 ? (float)currentDurability / maxDurability : 0f;
        public bool IsDamaged => isDamaged;
        public EquipmentSlot Slot => data.slot;

        /// <summary>
        /// 建構子
        /// </summary>
        public EquipmentInstance(EquipmentData data)
        {
            this.instanceId = Guid.NewGuid().ToString();
            this.data = data;
            this.enhanceLevel = 0;
            this.maxDurability = data.baseMaxDurability;
            this.currentDurability = maxDurability;
            this.isDamaged = false;
        }

        /// <summary>
        /// 取得當前裝備的所有屬性修飾器
        /// </summary>
        public List<StatModifier> GetModifiers()
        {
            if (isDamaged)
            {
                Debug.LogWarning($"[Equipment] {data.itemName} 已損毀，無法提供屬性加成！");
                return new List<StatModifier>();
            }

            return data.GetModifiersAtLevel(enhanceLevel);
        }

        /// <summary>
        /// 強化裝備
        /// </summary>
        /// <param name="success">是否成功</param>
        /// <param name="durabilityLoss">失敗時耐久損失</param>
        /// <returns>是否成功強化</returns>
        public bool Enhance(bool success, int durabilityLoss = 10)
        {
            if (enhanceLevel >= MaxEnhanceLevel)
            {
                Debug.LogWarning($"[Equipment] {data.itemName} 已達最高強化等級！");
                return false;
            }

            if (isDamaged)
            {
                Debug.LogWarning($"[Equipment] {data.itemName} 已損毀，無法強化！");
                return false;
            }

            if (success)
            {
                // 強化成功
                enhanceLevel++;
                OnEnhanced?.Invoke(enhanceLevel);
                Debug.Log($"[Equipment] {data.itemName} 強化成功！+{enhanceLevel}");
                return true;
            }
            else
            {
                // 強化失敗，降低耐久
                ReduceDurability(durabilityLoss);
                Debug.LogWarning($"[Equipment] {data.itemName} 強化失敗！耐久 -{durabilityLoss}");
                return false;
            }
        }

        /// <summary>
        /// 降低耐久度
        /// </summary>
        public void ReduceDurability(int amount)
        {
            currentDurability = Mathf.Max(0, currentDurability - amount);

            if (currentDurability == 0 && !isDamaged)
            {
                isDamaged = true;
                OnDamaged?.Invoke();
                Debug.LogWarning($"[Equipment] {data.itemName} 已損毀！");
            }
        }

        /// <summary>
        /// 修復裝備
        /// </summary>
        /// <param name="repairAmount">修復量 (0 = 全修)</param>
        /// <returns>修復費用</returns>
        public int Repair(int repairAmount = 0)
        {
            if (currentDurability >= maxDurability)
            {
                Debug.Log($"[Equipment] {data.itemName} 耐久已滿！");
                return 0;
            }

            // 計算修復量
            if (repairAmount == 0)
            {
                repairAmount = maxDurability - currentDurability;
            }
            else
            {
                repairAmount = Mathf.Min(repairAmount, maxDurability - currentDurability);
            }

            // 計算費用
            int cost = repairAmount * data.repairCostPerPoint;

            // 修復
            currentDurability += repairAmount;
            currentDurability = Mathf.Min(currentDurability, maxDurability);

            // 如果修復後耐久 > 0，解除損毀狀態
            if (currentDurability > 0 && isDamaged)
            {
                isDamaged = false;
                Debug.Log($"[Equipment] {data.itemName} 已修復！");
            }

            OnRepaired?.Invoke();
            return cost;
        }

        /// <summary>
        /// 取得顯示資訊
        /// </summary>
        public string GetDisplayInfo()
        {
            string info = $"{data.GetColoredName()}";
            if (enhanceLevel > 0)
                info += $" +{enhanceLevel}";

            info += $"\n耐久: {currentDurability}/{maxDurability}";

            if (isDamaged)
                info += "\n<color=red>[已損毀]</color>";

            return info;
        }

        /// <summary>
        /// 取得修復費用 (全修)
        /// </summary>
        public int GetFullRepairCost()
        {
            int missingDurability = maxDurability - currentDurability;
            return missingDurability * data.repairCostPerPoint;
        }
    }
}
