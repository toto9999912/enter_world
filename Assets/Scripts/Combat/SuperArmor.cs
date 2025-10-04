using System;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// 霸體系統 - 管理角色的霸體狀態與控場免疫
    /// Boss 和高級敵人使用此系統避免被玩家控制技能鎖死
    /// </summary>
    public class SuperArmor
    {
        // ===== 霸體參數 =====
        /// <summary>霸體值上限</summary>
        private float maxArmorValue;

        /// <summary>當前霸體值</summary>
        private float currentArmorValue;

        /// <summary>霸體恢復速度 (每秒)</summary>
        private float recoveryRate;

        /// <summary>是否啟用霸體系統</summary>
        private bool isEnabled;

        /// <summary>霸體破壞後冷卻時間 (秒)</summary>
        private float breakCooldown = 5f;

        /// <summary>當前冷卻剩餘時間</summary>
        private float currentCooldown = 0f;

        // ===== 控場抵抗 =====
        /// <summary>當前生效的控場效果列表</summary>
        private List<CrowdControlEffect> activeEffects = new List<CrowdControlEffect>();

        /// <summary>控場抵抗率 (0-100%)</summary>
        private float ccResistance = 0f;

        // ===== 狀態標記 =====
        /// <summary>霸體是否已破壞</summary>
        private bool isBroken = false;

        // ===== 事件 =====
        /// <summary>霸體值變化事件 (當前值, 最大值)</summary>
        public event Action<float, float> OnArmorChanged;

        /// <summary>霸體破壞事件</summary>
        public event Action OnArmorBroken;

        /// <summary>霸體恢復事件</summary>
        public event Action OnArmorRestored;

        /// <summary>受到控場效果事件</summary>
        public event Action<CrowdControlEffect> OnCrowdControlApplied;

        /// <summary>控場效果結束事件</summary>
        public event Action<CrowdControlEffect> OnCrowdControlRemoved;

        // ===== 屬性 =====
        /// <summary>當前霸體值</summary>
        public float CurrentArmor => currentArmorValue;

        /// <summary>最大霸體值</summary>
        public float MaxArmor => maxArmorValue;

        /// <summary>霸體百分比 (0-1)</summary>
        public float ArmorPercent => maxArmorValue > 0 ? currentArmorValue / maxArmorValue : 0;

        /// <summary>是否處於霸體狀態 (霸體值 > 0)</summary>
        public bool HasSuperArmor => isEnabled && currentArmorValue > 0 && !isBroken;

        /// <summary>是否免疫控場</summary>
        public bool IsImmuneToCC => HasSuperArmor;

        /// <summary>控場抵抗率</summary>
        public float CCResistance => ccResistance;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="maxArmor">最大霸體值</param>
        /// <param name="recoveryRate">恢復速度 (每秒)</param>
        /// <param name="ccResistance">控場抵抗率 (0-100)</param>
        /// <param name="enabled">是否啟用</param>
        public SuperArmor(float maxArmor = 100f, float recoveryRate = 10f, float ccResistance = 0f, bool enabled = true)
        {
            this.maxArmorValue = maxArmor;
            this.currentArmorValue = maxArmor;
            this.recoveryRate = recoveryRate;
            this.ccResistance = Mathf.Clamp(ccResistance, 0f, 100f);
            this.isEnabled = enabled;
        }

        /// <summary>
        /// 更新霸體系統 (每幀調用)
        /// </summary>
        /// <param name="deltaTime">時間增量</param>
        public void Update(float deltaTime)
        {
            // 更新冷卻
            if (currentCooldown > 0f)
            {
                currentCooldown -= deltaTime;
                if (currentCooldown <= 0f && isBroken)
                {
                    // 冷卻結束，恢復霸體
                    RestoreArmor();
                }
            }

            // 更新霸體恢復
            if (isEnabled && !isBroken && currentArmorValue < maxArmorValue)
            {
                float recovered = recoveryRate * deltaTime;
                currentArmorValue = Mathf.Min(currentArmorValue + recovered, maxArmorValue);
                OnArmorChanged?.Invoke(currentArmorValue, maxArmorValue);
            }

            // 更新控場效果
            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                if (!activeEffects[i].Update(deltaTime))
                {
                    // 效果結束
                    var effect = activeEffects[i];
                    activeEffects.RemoveAt(i);
                    OnCrowdControlRemoved?.Invoke(effect);
                }
            }
        }

        /// <summary>
        /// 受到控場攻擊 (消耗霸體值)
        /// </summary>
        /// <param name="damage">對霸體的傷害</param>
        /// <returns>是否成功抵抗</returns>
        public bool TakeCrowdControlDamage(float damage)
        {
            if (!isEnabled) return false;
            if (isBroken) return false;

            // 消耗霸體值
            currentArmorValue -= damage;
            OnArmorChanged?.Invoke(currentArmorValue, maxArmorValue);

            // 檢查是否破壞
            if (currentArmorValue <= 0f)
            {
                BreakArmor();
                return false;
            }

            return true; // 成功抵抗
        }

        /// <summary>
        /// 嘗試套用控場效果 (整合韌性抵抗)
        /// </summary>
        /// <param name="effect">控場效果</param>
        /// <param name="toughness">目標韌性值 (降低控場命中率)</param>
        /// <param name="skillHitRate">技能基礎命中率 (0-100)</param>
        /// <returns>是否成功套用</returns>
        public bool TryApplyCrowdControl(CrowdControlEffect effect, float toughness = 0f, float skillHitRate = 100f)
        {
            // 1. 檢查霸體免疫
            if (HasSuperArmor)
            {
                Debug.Log($"[SuperArmor] 霸體抵抗控場: {effect.type}");
                return false;
            }

            // 2. 韌性降低控場技能命中率
            // 公式: 實際命中率 = 技能命中率 × (1 - 韌性 / 200)
            float actualHitRate = skillHitRate * (1f - toughness / 200f);
            actualHitRate = Mathf.Clamp(actualHitRate, 5f, 95f); // 5-95% 範圍

            // 判定命中
            float hitRoll = UnityEngine.Random.Range(0f, 100f);
            if (hitRoll >= actualHitRate)
            {
                Debug.Log($"[SuperArmor] 韌性抵抗控場: {effect.type} (韌性: {toughness}, 命中率: {actualHitRate:F1}%)");
                return false;
            }

            // 3. 檢查控場抵抗率 (額外機率抵抗)
            if (ccResistance > 0f)
            {
                float resistRoll = UnityEngine.Random.Range(0f, 100f);
                if (resistRoll < ccResistance)
                {
                    Debug.Log($"[SuperArmor] 控場抵抗成功: {effect.type} (抵抗率: {ccResistance}%)");
                    return false;
                }
            }

            // 4. 套用控場效果
            ApplyCrowdControl(effect);
            return true;
        }

        /// <summary>
        /// 套用控場效果
        /// </summary>
        private void ApplyCrowdControl(CrowdControlEffect effect)
        {
            // 檢查是否已有相同類型的控場
            var existing = activeEffects.Find(e => e.type == effect.type);
            if (existing != null)
            {
                // 刷新持續時間
                existing.Refresh();
            }
            else
            {
                // 新增控場效果
                activeEffects.Add(effect);
                OnCrowdControlApplied?.Invoke(effect);
            }
        }

        /// <summary>
        /// 移除特定類型的控場效果
        /// </summary>
        public void RemoveCrowdControl(CCType type)
        {
            var effect = activeEffects.Find(e => e.type == type);
            if (effect != null)
            {
                activeEffects.Remove(effect);
                OnCrowdControlRemoved?.Invoke(effect);
            }
        }

        /// <summary>
        /// 清除所有控場效果
        /// </summary>
        public void ClearAllCrowdControl()
        {
            foreach (var effect in activeEffects)
            {
                OnCrowdControlRemoved?.Invoke(effect);
            }
            activeEffects.Clear();
        }

        /// <summary>
        /// 檢查是否受到特定控場影響
        /// </summary>
        public bool HasCrowdControl(CCType type)
        {
            return activeEffects.Exists(e => e.type == type);
        }

        /// <summary>
        /// 檢查是否受到任何控場影響
        /// </summary>
        public bool HasAnyCrowdControl()
        {
            return activeEffects.Count > 0;
        }

        /// <summary>
        /// 取得所有生效的控場效果
        /// </summary>
        public List<CrowdControlEffect> GetActiveCrowdControls()
        {
            return new List<CrowdControlEffect>(activeEffects);
        }

        /// <summary>
        /// 破壞霸體
        /// </summary>
        private void BreakArmor()
        {
            currentArmorValue = 0f;
            isBroken = true;
            currentCooldown = breakCooldown;

            OnArmorBroken?.Invoke();
            Debug.Log($"[SuperArmor] 霸體已破壞! 冷卻時間: {breakCooldown}秒");
        }

        /// <summary>
        /// 恢復霸體
        /// </summary>
        private void RestoreArmor()
        {
            isBroken = false;
            currentArmorValue = maxArmorValue;
            currentCooldown = 0f;

            OnArmorRestored?.Invoke();
            OnArmorChanged?.Invoke(currentArmorValue, maxArmorValue);
            Debug.Log("[SuperArmor] 霸體已恢復!");
        }

        /// <summary>
        /// 設定霸體參數
        /// </summary>
        public void SetArmorParameters(float maxArmor, float recoveryRate, float breakCooldown = 5f)
        {
            this.maxArmorValue = maxArmor;
            this.recoveryRate = recoveryRate;
            this.breakCooldown = breakCooldown;

            if (currentArmorValue > maxArmor)
            {
                currentArmorValue = maxArmor;
            }

            OnArmorChanged?.Invoke(currentArmorValue, maxArmorValue);
        }

        /// <summary>
        /// 設定控場抵抗率
        /// </summary>
        public void SetCCResistance(float resistance)
        {
            this.ccResistance = Mathf.Clamp(resistance, 0f, 100f);
        }

        /// <summary>
        /// 啟用/停用霸體系統
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            this.isEnabled = enabled;
            if (!enabled)
            {
                ClearAllCrowdControl();
            }
        }

        /// <summary>
        /// 立即恢復霸體值
        /// </summary>
        public void RestoreArmorValue(float amount)
        {
            if (!isEnabled || isBroken) return;

            currentArmorValue = Mathf.Min(currentArmorValue + amount, maxArmorValue);
            OnArmorChanged?.Invoke(currentArmorValue, maxArmorValue);
        }

        /// <summary>
        /// 取得霸體狀態文字 (用於 UI 顯示)
        /// </summary>
        public string GetStatusText()
        {
            if (!isEnabled)
                return "霸體未啟用";

            if (isBroken)
                return $"霸體破壞 (冷卻: {currentCooldown:F1}秒)";

            if (HasSuperArmor)
                return $"霸體: {currentArmorValue:F0}/{maxArmorValue:F0}";

            return "無霸體";
        }
    }
}
