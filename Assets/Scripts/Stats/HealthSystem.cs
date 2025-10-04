using System;
using UnityEngine;
using Core;

namespace Stats
{
    /// <summary>
    /// 生命值系統 - 管理當前 HP/MP
    /// 負責處理傷害、治療、死亡等邏輯
    /// </summary>
    [Serializable]
    public class HealthSystem
    {
        // ===== 引用 =====
        private CharacterStats stats;

        // ===== 當前數值 =====
        private float currentHP;
        private float currentMP;

        // ===== 狀態標記 =====
        private bool isDead = false;

        // ===== 事件 =====
        /// <summary>HP 變化事件 (當前HP, 最大HP)</summary>
        public event Action<float, float> OnHPChanged;

        /// <summary>MP 變化事件 (當前MP, 最大MP)</summary>
        public event Action<float, float> OnMPChanged;

        /// <summary>受到傷害事件 (傷害值)</summary>
        public event Action<float> OnTakeDamage;

        /// <summary>治療事件 (治療量)</summary>
        public event Action<float> OnHeal;

        /// <summary>死亡事件</summary>
        public event Action OnDeath;

        /// <summary>復活事件</summary>
        public event Action OnRevive;

        // ===== 屬性 =====
        /// <summary>當前生命值</summary>
        public float CurrentHP => currentHP;

        /// <summary>當前魔力值</summary>
        public float CurrentMP => currentMP;

        /// <summary>最大生命值 (從 CharacterStats 取得)</summary>
        public float MaxHP => stats.GetFinalValue(StatType.HP);

        /// <summary>最大魔力值 (從 CharacterStats 取得)</summary>
        public float MaxMP => stats.GetFinalValue(StatType.MP);

        /// <summary>是否死亡</summary>
        public bool IsDead => isDead;

        /// <summary>HP 百分比 (0-1)</summary>
        public float HPPercent => MaxHP > 0 ? currentHP / MaxHP : 0;

        /// <summary>MP 百分比 (0-1)</summary>
        public float MPPercent => MaxMP > 0 ? currentMP / MaxMP : 0;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="stats">角色屬性數據</param>
        /// <param name="initializeAtMax">是否初始化為滿血滿魔 (預設 true)</param>
        public HealthSystem(CharacterStats stats, bool initializeAtMax = true)
        {
            this.stats = stats;

            if (initializeAtMax)
            {
                InitializeToMax();
            }
        }

        /// <summary>
        /// 初始化為滿血滿魔
        /// </summary>
        public void InitializeToMax()
        {
            currentHP = MaxHP;
            currentMP = MaxMP;
            isDead = false;
        }

        /// <summary>
        /// 受到傷害
        /// </summary>
        /// <param name="damage">傷害值 (正數)</param>
        /// <returns>實際受到的傷害 (扣除溢出部分)</returns>
        public float TakeDamage(float damage)
        {
            if (isDead) return 0f;

            // 確保傷害為正數
            damage = Mathf.Max(0f, damage);

            // 計算實際受到的傷害 (不能低於 0)
            float actualDamage = Mathf.Min(damage, currentHP);

            currentHP -= actualDamage;
            currentHP = Mathf.Max(0f, currentHP);

            // 觸發事件
            OnTakeDamage?.Invoke(actualDamage);
            OnHPChanged?.Invoke(currentHP, MaxHP);

            // 檢查死亡
            if (currentHP <= 0f && !isDead)
            {
                Die();
            }

            return actualDamage;
        }

        /// <summary>
        /// 治療
        /// </summary>
        /// <param name="healAmount">治療量 (正數)</param>
        /// <returns>實際治療量 (扣除溢出部分)</returns>
        public float Heal(float healAmount)
        {
            if (isDead) return 0f;

            // 確保治療量為正數
            healAmount = Mathf.Max(0f, healAmount);

            // 計算實際治療量 (不能超過最大值)
            float actualHeal = Mathf.Min(healAmount, MaxHP - currentHP);

            currentHP += actualHeal;
            currentHP = Mathf.Min(currentHP, MaxHP);

            // 觸發本地事件
            OnHeal?.Invoke(actualHeal);
            OnHPChanged?.Invoke(currentHP, MaxHP);

            // 發布全域事件
            if (actualHeal > 0)
            {
                EventBus.Publish(new HealingEvent
                {
                    Target = this,
                    Amount = actualHeal
                });
            }

            return actualHeal;
        }

        /// <summary>
        /// 消耗魔力
        /// </summary>
        /// <param name="amount">消耗量</param>
        /// <returns>是否成功消耗 (魔力不足時返回 false)</returns>
        public bool ConsumeMana(float amount)
        {
            if (isDead) return false;
            if (currentMP < amount) return false;

            currentMP -= amount;
            currentMP = Mathf.Max(0f, currentMP);

            OnMPChanged?.Invoke(currentMP, MaxMP);
            return true;
        }

        /// <summary>
        /// 恢復魔力
        /// </summary>
        /// <param name="amount">恢復量</param>
        /// <returns>實際恢復量</returns>
        public float RestoreMana(float amount)
        {
            if (isDead) return 0f;

            amount = Mathf.Max(0f, amount);
            float actualRestore = Mathf.Min(amount, MaxMP - currentMP);

            currentMP += actualRestore;
            currentMP = Mathf.Min(currentMP, MaxMP);

            OnMPChanged?.Invoke(currentMP, MaxMP);
            return actualRestore;
        }

        /// <summary>
        /// 死亡處理
        /// </summary>
        private void Die()
        {
            isDead = true;
            currentHP = 0f;

            // 觸發本地事件
            OnDeath?.Invoke();

            // 發布全域事件
            EventBus.Publish(new EntityDeathEvent
            {
                Entity = this
            });
        }

        /// <summary>
        /// 復活
        /// </summary>
        /// <param name="hpPercent">復活時的 HP 百分比 (0-1)，預設 0.5 (50%)</param>
        /// <param name="mpPercent">復活時的 MP 百分比 (0-1)，預設 0.5 (50%)</param>
        public void Revive(float hpPercent = 0.5f, float mpPercent = 0.5f)
        {
            if (!isDead) return;

            isDead = false;

            // 設定復活後的 HP/MP
            hpPercent = Mathf.Clamp01(hpPercent);
            mpPercent = Mathf.Clamp01(mpPercent);

            currentHP = MaxHP * hpPercent;
            currentMP = MaxMP * mpPercent;

            OnRevive?.Invoke();
            OnHPChanged?.Invoke(currentHP, MaxHP);
            OnMPChanged?.Invoke(currentMP, MaxMP);
        }

        /// <summary>
        /// 直接設定 HP (用於特殊情況，例如編輯器測試)
        /// </summary>
        /// <param name="value">目標 HP 值</param>
        public void SetHP(float value)
        {
            currentHP = Mathf.Clamp(value, 0f, MaxHP);

            // 檢查死亡狀態
            if (currentHP <= 0f && !isDead)
            {
                Die();
            }
            else if (currentHP > 0f && isDead)
            {
                isDead = false;
            }

            OnHPChanged?.Invoke(currentHP, MaxHP);
        }

        /// <summary>
        /// 直接設定 MP (用於特殊情況，例如編輯器測試)
        /// </summary>
        /// <param name="value">目標 MP 值</param>
        public void SetMP(float value)
        {
            currentMP = Mathf.Clamp(value, 0f, MaxMP);
            OnMPChanged?.Invoke(currentMP, MaxMP);
        }

        /// <summary>
        /// 檢查是否有足夠的魔力
        /// </summary>
        /// <param name="amount">所需魔力</param>
        /// <returns>是否足夠</returns>
        public bool HasEnoughMana(float amount)
        {
            return currentMP >= amount;
        }

        /// <summary>
        /// 當屬性變化時更新 HP/MP 上限
        /// 如果當前值超過新上限，會自動調整
        /// </summary>
        public void UpdateMaxValues()
        {
            // 調整 HP (保持百分比)
            float hpPercent = HPPercent;
            currentHP = Mathf.Min(currentHP, MaxHP);

            // 如果 HP 從 0 變成有值，且不是死亡狀態，可能是裝備改變
            // 這裡不自動復活，需要手動呼叫 Revive()

            // 調整 MP (保持百分比)
            currentMP = Mathf.Min(currentMP, MaxMP);

            OnHPChanged?.Invoke(currentHP, MaxHP);
            OnMPChanged?.Invoke(currentMP, MaxMP);
        }
    }
}
