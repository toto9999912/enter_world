using System;
using UnityEngine;
using Stats;
using Element;
using Core;

namespace Combat
{
    /// <summary>
    /// 戰鬥系統 - 處理傷害計算、爆擊判定、防禦計算等戰鬥邏輯
    /// </summary>
    public class CombatSystem
    {
        // ===== 引用 =====
        private CharacterStats attackerStats;
        private HealthSystem attackerHealth;

        // ===== 元素屬性 =====
        private ElementType element = ElementType.None;

        // ===== 事件 =====
        /// <summary>造成傷害事件 (傷害資訊)</summary>
        public event Action<DamageInfo> OnDealDamage;

        // ===== 屬性 =====
        /// <summary>角色元素屬性</summary>
        public ElementType Element
        {
            get => element;
            set => element = value;
        }

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="stats">攻擊者的屬性</param>
        /// <param name="health">攻擊者的生命系統 (選填)</param>
        public CombatSystem(CharacterStats stats, HealthSystem health = null)
        {
            this.attackerStats = stats;
            this.attackerHealth = health;
        }

        /// <summary>
        /// 對目標造成傷害 (完整計算流程)
        /// </summary>
        /// <param name="target">目標的生命系統</param>
        /// <param name="targetStats">目標的屬性</param>
        /// <param name="targetCombat">目標的戰鬥系統 (用於取得元素屬性)</param>
        /// <param name="damageMultiplier">傷害倍率 (例如技能倍率)</param>
        /// <param name="damageType">傷害類型</param>
        /// <param name="sourceDescription">傷害來源描述</param>
        /// <returns>傷害資訊</returns>
        public DamageInfo Attack(
            HealthSystem target,
            CharacterStats targetStats,
            CombatSystem targetCombat = null,
            float damageMultiplier = 1f,
            DamageType damageType = DamageType.Physical,
            string sourceDescription = "普通攻擊")
        {
            // 建立傷害資訊
            DamageInfo damageInfo = new DamageInfo(0f, damageType)
            {
                sourceDescription = sourceDescription,
                attackerElement = this.element,
                defenderElement = targetCombat?.Element ?? ElementType.None
            };

            // 1. 檢查命中
            if (!RollHit(targetStats))
            {
                damageInfo.isDodged = true;
                damageInfo.finalDamage = 0f;
                OnDealDamage?.Invoke(damageInfo);
                return damageInfo;
            }

            // 2. 檢查閃避
            if (RollDodge(targetStats))
            {
                damageInfo.isDodged = true;
                damageInfo.finalDamage = 0f;
                OnDealDamage?.Invoke(damageInfo);
                return damageInfo;
            }

            // 3. 計算基礎傷害
            float baseDamage = CalculateBaseDamage(damageType, damageMultiplier);
            damageInfo.baseDamage = baseDamage;

            // 4. 檢查爆擊
            bool isCritical = RollCritical(targetStats);
            damageInfo.isCritical = isCritical;

            if (isCritical)
            {
                float critDamage = attackerStats.GetFinalValue(StatType.CritDamage);
                baseDamage *= (critDamage / 100f);
            }

            // 5. 套用元素克制 (魔法傷害才計算元素)
            float finalDamage = baseDamage;
            if (damageType == DamageType.Magical && targetCombat != null)
            {
                float elementalMultiplier = ElementalAffinityTable.GetAffinityMultiplier(
                    this.element,
                    targetCombat.Element
                );
                damageInfo.elementalMultiplier = elementalMultiplier;
                finalDamage *= elementalMultiplier;
            }

            // 6. 計算防禦減免 (真實傷害無視防禦)
            if (damageType != DamageType.True)
            {
                finalDamage = ApplyDefense(finalDamage, targetStats, damageType);
            }

            // 7. 套用傷害
            damageInfo.finalDamage = finalDamage;
            target.TakeDamage(finalDamage);

            // 觸發本地事件
            OnDealDamage?.Invoke(damageInfo);

            // 發布全域事件 (EventBus)
            EventBus.Publish(new DamageDealtEvent
            {
                Attacker = this,
                Target = target,
                Damage = finalDamage,
                IsCritical = isCritical,
                Element = this.element
            });

            return damageInfo;
        }

        /// <summary>
        /// 計算基礎傷害 (未計算防禦)
        /// </summary>
        /// <param name="damageType">傷害類型</param>
        /// <param name="multiplier">傷害倍率</param>
        /// <returns>基礎傷害值</returns>
        private float CalculateBaseDamage(DamageType damageType, float multiplier)
        {
            float baseStat = damageType switch
            {
                DamageType.Physical => attackerStats.GetFinalValue(StatType.ATK),
                DamageType.Magical => attackerStats.GetFinalValue(StatType.Magic),
                DamageType.True => attackerStats.GetFinalValue(StatType.ATK), // 真實傷害也基於攻擊力
                _ => attackerStats.GetFinalValue(StatType.ATK)
            };

            return baseStat * multiplier;
        }

        /// <summary>
        /// 判定爆擊
        /// </summary>
        /// <param name="targetStats">目標屬性</param>
        /// <returns>是否爆擊</returns>
        private bool RollCritical(CharacterStats targetStats)
        {
            float critRate = attackerStats.GetFinalValue(StatType.CritRate);
            return UnityEngine.Random.Range(0f, 100f) < critRate;
        }

        /// <summary>
        /// 判定命中
        /// </summary>
        /// <param name="targetStats">目標屬性</param>
        /// <returns>是否命中</returns>
        private bool RollHit(CharacterStats targetStats)
        {
            float hitRate = attackerStats.GetFinalValue(StatType.HitRate);
            return UnityEngine.Random.Range(0f, 100f) < hitRate;
        }

        /// <summary>
        /// 判定閃避
        /// </summary>
        /// <param name="targetStats">目標屬性</param>
        /// <returns>是否閃避</returns>
        private bool RollDodge(CharacterStats targetStats)
        {
            float dodgeRate = targetStats.GetFinalValue(StatType.DodgeRate);
            return UnityEngine.Random.Range(0f, 100f) < dodgeRate;
        }

        /// <summary>
        /// 套用防禦減免
        /// 使用公式: 傷害 = 基礎傷害 × (100 / (100 + 有效防禦))
        /// 穿透會降低有效防禦
        /// </summary>
        /// <param name="baseDamage">基礎傷害</param>
        /// <param name="targetStats">目標屬性</param>
        /// <param name="damageType">傷害類型</param>
        /// <returns>套用防禦後的傷害</returns>
        private float ApplyDefense(float baseDamage, CharacterStats targetStats, DamageType damageType)
        {
            // 取得目標防禦力
            float defense = targetStats.GetFinalValue(StatType.DEF);

            // 取得攻擊者的穿透
            float penetration = attackerStats.GetFinalValue(StatType.Penetration);

            // 計算有效防禦 (穿透降低防禦)
            // 穿透公式: 有效防禦 = 防禦 × (1 - 穿透% / 100)
            float effectiveDefense = defense * (1f - penetration / 100f);
            effectiveDefense = Mathf.Max(0f, effectiveDefense);

            // 傷害減免公式: 傷害 = 基礎傷害 × (100 / (100 + 有效防禦))
            // 例如: 100 防禦 = 50% 減免
            //      200 防禦 = 66.7% 減免
            float damageMultiplier = 100f / (100f + effectiveDefense);
            float finalDamage = baseDamage * damageMultiplier;

            return finalDamage;
        }

        /// <summary>
        /// 造成固定傷害 (不經過任何計算，直接扣血)
        /// 用於特殊技能、DOT、環境傷害等
        /// </summary>
        /// <param name="target">目標生命系統</param>
        /// <param name="damage">傷害值</param>
        /// <param name="damageType">傷害類型 (用於顯示)</param>
        /// <param name="sourceDescription">來源描述</param>
        /// <returns>傷害資訊</returns>
        public DamageInfo DealRawDamage(
            HealthSystem target,
            float damage,
            DamageType damageType = DamageType.True,
            string sourceDescription = "固定傷害")
        {
            DamageInfo damageInfo = new DamageInfo(damage, damageType)
            {
                sourceDescription = sourceDescription,
                finalDamage = damage
            };

            target.TakeDamage(damage);
            OnDealDamage?.Invoke(damageInfo);

            return damageInfo;
        }

        /// <summary>
        /// 治療目標
        /// </summary>
        /// <param name="target">目標生命系統</param>
        /// <param name="healAmount">治療量</param>
        /// <returns>實際治療量</returns>
        public float Heal(HealthSystem target, float healAmount)
        {
            return target.Heal(healAmount);
        }

        /// <summary>
        /// 計算預期傷害 (不實際造成傷害，用於 AI 判斷或 UI 顯示)
        /// </summary>
        /// <param name="targetStats">目標屬性</param>
        /// <param name="damageMultiplier">傷害倍率</param>
        /// <param name="damageType">傷害類型</param>
        /// <returns>預期平均傷害</returns>
        public float CalculateExpectedDamage(
            CharacterStats targetStats,
            float damageMultiplier = 1f,
            DamageType damageType = DamageType.Physical)
        {
            // 計算基礎傷害
            float baseDamage = CalculateBaseDamage(damageType, damageMultiplier);

            // 計算爆擊期望值
            float critRate = attackerStats.GetFinalValue(StatType.CritRate);
            float toughness = targetStats.GetFinalValue(StatType.Toughness);
            float effectiveCritRate = Mathf.Max(0f, critRate - toughness) / 100f;

            float critDamage = attackerStats.GetFinalValue(StatType.CritDamage) / 100f;
            float critMultiplier = 1f + (effectiveCritRate * (critDamage - 1f));

            baseDamage *= critMultiplier;

            // 計算閃避期望值
            float dodgeRate = targetStats.GetFinalValue(StatType.DodgeRate) / 100f;
            baseDamage *= (1f - dodgeRate);

            // 套用防禦
            if (damageType != DamageType.True)
            {
                baseDamage = ApplyDefense(baseDamage, targetStats, damageType);
            }

            return baseDamage;
        }
    }
}
