using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stats
{
    /// <summary>
    /// 角色/物品屬性集合 - 儲存所有屬性數值
    /// </summary>
    [Serializable]
    public class CharacterStats
    {
        // ===== 主要屬性基礎值 =====
        [Header("主要屬性")]
        [SerializeField] private float baseHP = 100f;
        [SerializeField] private float baseMP = 50f;
        [SerializeField] private float baseATK = 10f;
        [SerializeField] private float baseMagic = 10f;
        [SerializeField] private float baseDEF = 8f;
        [SerializeField] private float baseSPD = 5f;
        [SerializeField] private float baseSP = 100f;

        // ===== 隱藏稀有屬性基礎值 =====
        [Header("隱藏稀有屬性")]
        [SerializeField, Range(0f, 100f)] private float baseCritRate = 5f;
        [SerializeField, Range(0f, 300f)] private float baseCritDamage = 150f;
        [SerializeField, Range(0f, 100f)] private float baseDodgeRate = 5f;
        [SerializeField, Range(0f, 100f)] private float baseToughness = 0f;
        [SerializeField, Range(0f, 100f)] private float basePenetration = 0f;
        [SerializeField, Range(0f, 40f)] private float baseSkillHaste = 0f;
        [SerializeField, Range(0f, 100f)] private float baseHitRate = 100f;

        // ===== 修飾器系統 =====
        // 屬性修飾器清單 (來自裝備、Buff等)
        // 注意: Unity 的 [Serializable] 類別在反序列化時不會呼叫建構子
        // 因此需要在使用前確保字典已初始化
        private Dictionary<StatType, List<StatModifier>> modifiers;

        // ===== 快取系統 =====
        // 快取計算結果，避免重複計算
        private Dictionary<StatType, float> cachedValues;

        // Dirty Flag - 標記是否需要重新計算
        // 當基礎值或修飾器改變時設為 true
        private bool isDirty = true;

        /// <summary>
        /// 建構子
        /// </summary>
        public CharacterStats()
        {
            EnsureInitialized();
        }

        /// <summary>
        /// 確保字典已初始化 (處理反序列化情況)
        /// Unity 的 [Serializable] 在反序列化時不會呼叫建構子
        /// 因此每次使用前都要確認初始化
        /// </summary>
        private void EnsureInitialized()
        {
            if (modifiers == null)
            {
                modifiers = new Dictionary<StatType, List<StatModifier>>();
                foreach (StatType statType in Enum.GetValues(typeof(StatType)))
                {
                    modifiers[statType] = new List<StatModifier>();
                }
            }

            if (cachedValues == null)
            {
                cachedValues = new Dictionary<StatType, float>();
            }
        }

        /// <summary>
        /// 取得基礎屬性值 (未加成)
        /// </summary>
        public float GetBaseValue(StatType statType)
        {
            switch (statType)
            {
                case StatType.HP: return baseHP;
                case StatType.MP: return baseMP;
                case StatType.ATK: return baseATK;
                case StatType.Magic: return baseMagic;
                case StatType.DEF: return baseDEF;
                case StatType.SPD: return baseSPD;
                case StatType.SP: return baseSP;
                case StatType.CritRate: return baseCritRate;
                case StatType.CritDamage: return baseCritDamage;
                case StatType.DodgeRate: return baseDodgeRate;
                case StatType.Toughness: return baseToughness;
                case StatType.Penetration: return basePenetration;
                case StatType.SkillHaste: return baseSkillHaste;
                case StatType.HitRate: return baseHitRate;
                default: return 0f;
            }
        }

        /// <summary>
        /// 設定基礎屬性值
        /// </summary>
        public void SetBaseValue(StatType statType, float value)
        {
            switch (statType)
            {
                case StatType.HP: baseHP = value; break;
                case StatType.MP: baseMP = value; break;
                case StatType.ATK: baseATK = value; break;
                case StatType.Magic: baseMagic = value; break;
                case StatType.DEF: baseDEF = value; break;
                case StatType.SPD: baseSPD = value; break;
                case StatType.SP: baseSP = value; break;
                case StatType.CritRate: baseCritRate = value; break;
                case StatType.CritDamage: baseCritDamage = value; break;
                case StatType.DodgeRate: baseDodgeRate = value; break;
                case StatType.Toughness: baseToughness = value; break;
                case StatType.Penetration: basePenetration = value; break;
                case StatType.SkillHaste: baseSkillHaste = value; break;
                case StatType.HitRate: baseHitRate = value; break;
            }
            isDirty = true;
        }

        /// <summary>
        /// 取得最終屬性值 (含所有加成)
        /// 使用快取機制，只在數值變化時重新計算
        /// </summary>
        /// <param name="statType">屬性類型</param>
        /// <returns>計算後的最終屬性值</returns>
        public float GetFinalValue(StatType statType)
        {
            EnsureInitialized();

            // 如果數值已變更 (isDirty = true)，重新計算所有屬性
            if (isDirty)
            {
                RecalculateAllStats();
            }

            return cachedValues.TryGetValue(statType, out float value) ? value : GetBaseValue(statType);
        }

        /// <summary>
        /// 新增屬性修飾器
        /// </summary>
        /// <param name="modifier">要新增的修飾器</param>
        public void AddModifier(StatModifier modifier)
        {
            EnsureInitialized();

            if (modifiers.TryGetValue(modifier.statType, out var list))
            {
                list.Add(modifier);
                isDirty = true; // 標記需要重新計算
            }
        }

        /// <summary>
        /// 移除屬性修飾器 (透過引用比較)
        /// 因為 StatModifier 改為 class，會比較物件引用
        /// </summary>
        /// <param name="modifier">要移除的修飾器</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveModifier(StatModifier modifier)
        {
            EnsureInitialized();

            if (modifiers.TryGetValue(modifier.statType, out var list))
            {
                bool removed = list.Remove(modifier);
                if (removed)
                {
                    isDirty = true; // 標記需要重新計算
                }
                return removed;
            }
            return false;
        }

        /// <summary>
        /// 透過 ID 移除屬性修飾器 (更精確的移除方式)
        /// </summary>
        /// <param name="modifierId">修飾器的唯一ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveModifierById(string modifierId)
        {
            EnsureInitialized();

            foreach (var list in modifiers.Values)
            {
                var modifier = list.Find(m => m.id == modifierId);
                if (modifier != null)
                {
                    list.Remove(modifier);
                    isDirty = true;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 移除來自特定來源的所有修飾器 (例如：移除某件裝備的所有加成)
        /// </summary>
        /// <param name="source">來源標識</param>
        /// <returns>移除的修飾器數量</returns>
        public int RemoveModifiersBySource(string source)
        {
            EnsureInitialized();

            int removedCount = 0;
            foreach (var list in modifiers.Values)
            {
                int removed = list.RemoveAll(m => m.source == source);
                removedCount += removed;
            }

            if (removedCount > 0)
            {
                isDirty = true;
            }

            return removedCount;
        }

        /// <summary>
        /// 清除所有修飾器
        /// </summary>
        public void ClearAllModifiers()
        {
            EnsureInitialized();

            foreach (var list in modifiers.Values)
            {
                list.Clear();
            }
            isDirty = true;
        }

        /// <summary>
        /// 重新計算所有屬性
        /// </summary>
        private void RecalculateAllStats()
        {
            cachedValues.Clear();

            foreach (StatType statType in Enum.GetValues(typeof(StatType)))
            {
                cachedValues[statType] = CalculateStat(statType);
            }

            isDirty = false;
        }

        /// <summary>
        /// 計算單個屬性的最終值
        ///
        /// 計算順序與公式:
        /// 1. 基礎值 (Base)
        /// 2. + 固定加成 (Flat) - 直接加算
        /// 3. × (1 + 百分比加成 (Percentage) / 100) - 基於前面結果的百分比加成
        /// 4. × (1 + 最終加成 (Final) / 100) - 最終乘算加成
        ///
        /// 範例:
        /// - 基礎攻擊力 = 100
        /// - Flat +20 攻擊
        /// - Percentage +50% 攻擊 (數值為 50)
        /// - Final +10% 傷害 (數值為 10)
        ///
        /// 計算: ((100 + 20) × (1 + 50/100)) × (1 + 10/100)
        ///     = (120 × 1.5) × 1.1
        ///     = 180 × 1.1
        ///     = 198
        /// </summary>
        /// <param name="statType">要計算的屬性類型</param>
        /// <returns>計算後的最終值</returns>
        private float CalculateStat(StatType statType)
        {
            float baseValue = GetBaseValue(statType);
            float flatBonus = 0f;
            float percentageBonus = 0f; // 範圍 0-100，代表百分比
            float finalBonus = 0f;       // 範圍 0-100，代表百分比

            if (!modifiers.TryGetValue(statType, out var modifierList))
                return baseValue;

            // ===== 第一階段：收集各類加成 =====
            foreach (var modifier in modifierList)
            {
                switch (modifier.modifierType)
                {
                    case ModifierType.Flat:
                        // 固定數值加成 (例如: +10 攻擊力)
                        flatBonus += modifier.value;
                        break;

                    case ModifierType.Percentage:
                        // 百分比加成 (例如: +15% 代表 modifier.value = 15)
                        percentageBonus += modifier.value;
                        break;

                    case ModifierType.Final:
                        // 最終加成 (例如: +10% 代表 modifier.value = 10)
                        finalBonus += modifier.value;
                        break;
                }
            }

            // ===== 第二階段：按順序計算最終值 =====
            // 1. 基礎值 + 固定加成
            float valueAfterFlat = baseValue + flatBonus;

            // 2. 套用百分比加成 (modifier.value 已經是 0-100，所以除以 100 轉換為比例)
            float valueAfterPercentage = valueAfterFlat * (1f + percentageBonus / 100f);

            // 3. 套用最終加成
            float finalValue = valueAfterPercentage * (1f + finalBonus / 100f);

            // ===== 第三階段：套用數值限制 (避免超出合理範圍) =====
            return ApplyStatLimits(statType, finalValue);
        }

        /// <summary>
        /// 套用屬性數值限制
        /// 避免屬性值超出合理範圍，保持遊戲平衡
        /// </summary>
        /// <param name="statType">屬性類型</param>
        /// <param name="value">計算後的原始值</param>
        /// <returns>限制後的最終值</returns>
        private float ApplyStatLimits(StatType statType, float value)
        {
            switch (statType)
            {
                // 百分比屬性限制在 0-100%
                case StatType.CritRate:    // 爆擊率
                case StatType.DodgeRate:   // 迴避率
                    return Mathf.Clamp(value, 0f, 100f);

                // 爆擊傷害: 最低 100% (1倍傷害)，最高 300% (3倍傷害)
                case StatType.CritDamage:
                    return Mathf.Clamp(value, 100f, 300f);

                // 技能急速上限 40% (避免冷卻時間過短)
                case StatType.SkillHaste:
                    return Mathf.Clamp(value, 0f, 40f);

                // 命中率上限 100%
                case StatType.HitRate:
                    return Mathf.Clamp(value, 0f, 100f);

                // 韌性和穿透上限 100
                case StatType.Toughness:   // 韌性 (抗爆擊)
                case StatType.Penetration: // 穿透 (無視防禦)
                    return Mathf.Clamp(value, 0f, 100f);

                // 其他屬性 (HP, MP, ATK, INT, DEF, SPD) 最小值為 0，無上限
                default:
                    return Mathf.Max(0f, value);
            }
        }

        /// <summary>
        /// 取得屬性上限值
        /// 用於 UI 顯示或檢查是否已達上限
        /// </summary>
        /// <param name="statType">屬性類型</param>
        /// <returns>該屬性的上限值，無上限則回傳 float.MaxValue</returns>
        public float GetStatCap(StatType statType)
        {
            switch (statType)
            {
                case StatType.CritRate: return 100f;
                case StatType.CritDamage: return 300f;
                case StatType.DodgeRate: return 100f;
                case StatType.Toughness: return 100f;
                case StatType.Penetration: return 100f;
                case StatType.SkillHaste: return 40f;
                case StatType.HitRate: return 100f;
                default: return float.MaxValue; // 無上限
            }
        }

        /// <summary>
        /// 取得所有主要屬性 (HP, MP, ATK, INT, DEF, SPD)
        /// 常用於 UI 顯示或戰鬥系統計算
        /// </summary>
        /// <returns>包含所有主要屬性及其最終值的字典</returns>
        public Dictionary<StatType, float> GetPrimaryStats()
        {
            return new Dictionary<StatType, float>
            {
                { StatType.HP, GetFinalValue(StatType.HP) },
                { StatType.MP, GetFinalValue(StatType.MP) },
                { StatType.ATK, GetFinalValue(StatType.ATK) },
                { StatType.Magic, GetFinalValue(StatType.Magic) },
                { StatType.DEF, GetFinalValue(StatType.DEF) },
                { StatType.SPD, GetFinalValue(StatType.SPD) },
                { StatType.SP, GetFinalValue(StatType.SP) }
            };
        }

        /// <summary>
        /// 取得所有隱藏稀有屬性 (爆擊、迴避、韌性等)
        /// 用於進階 UI 顯示或特殊計算
        /// </summary>
        /// <returns>包含所有稀有屬性及其最終值的字典</returns>
        public Dictionary<StatType, float> GetSecondaryStats()
        {
            return new Dictionary<StatType, float>
            {
                { StatType.CritRate, GetFinalValue(StatType.CritRate) },
                { StatType.CritDamage, GetFinalValue(StatType.CritDamage) },
                { StatType.DodgeRate, GetFinalValue(StatType.DodgeRate) },
                { StatType.Toughness, GetFinalValue(StatType.Toughness) },
                { StatType.Penetration, GetFinalValue(StatType.Penetration) },
                { StatType.SkillHaste, GetFinalValue(StatType.SkillHaste) },
                { StatType.HitRate, GetFinalValue(StatType.HitRate) }
            };
        }

        /// <summary>
        /// 取得特定屬性的所有修飾器
        /// 用於 UI 顯示或除錯
        /// </summary>
        /// <param name="statType">屬性類型</param>
        /// <returns>該屬性的所有修飾器列表</returns>
        public List<StatModifier> GetModifiers(StatType statType)
        {
            EnsureInitialized();

            if (modifiers.TryGetValue(statType, out var list))
            {
                // 回傳副本，避免外部修改
                return new List<StatModifier>(list);
            }

            return new List<StatModifier>();
        }

        /// <summary>
        /// 複製屬性資料
        /// 創建一個完全獨立的副本，包含所有基礎值和修飾器
        /// 注意: 修飾器不會深拷貝，而是引用相同的物件
        /// </summary>
        /// <returns>新的 CharacterStats 實例</returns>
        public CharacterStats Clone()
        {
            CharacterStats clone = new CharacterStats();

            // 複製所有基礎值
            foreach (StatType statType in Enum.GetValues(typeof(StatType)))
            {
                clone.SetBaseValue(statType, GetBaseValue(statType));

                // 複製所有修飾器 (注意: 這是淺拷貝，修飾器物件會被共享)
                if (modifiers.TryGetValue(statType, out var modifierList))
                {
                    foreach (var modifier in modifierList)
                    {
                        clone.AddModifier(modifier);
                    }
                }
            }

            return clone;
        }
    }
}
