using System;
using UnityEngine;
using Element;

namespace Combat
{
    /// <summary>
    /// 傷害類型
    /// </summary>
    public enum DamageType
    {
        /// <summary>物理傷害 (基於 ATK)</summary>
        Physical,

        /// <summary>魔法傷害 (基於 INT)</summary>
        Magical,

        /// <summary>真實傷害 (無視防禦)</summary>
        True
    }

    /// <summary>
    /// 傷害資訊 - 記錄一次傷害的完整數據
    /// 用於戰鬥日誌、UI 顯示、傷害飄字等
    /// </summary>
    [Serializable]
    public class DamageInfo
    {
        /// <summary>傷害類型</summary>
        public DamageType damageType;

        /// <summary>基礎傷害值 (計算前)</summary>
        public float baseDamage;

        /// <summary>最終傷害值 (計算後)</summary>
        public float finalDamage;

        /// <summary>是否爆擊</summary>
        public bool isCritical;

        /// <summary>是否被閃避</summary>
        public bool isDodged;

        /// <summary>是否被格擋</summary>
        public bool isBlocked;

        /// <summary>攻擊者元素屬性</summary>
        public ElementType attackerElement;

        /// <summary>防禦者元素屬性</summary>
        public ElementType defenderElement;

        /// <summary>元素克制倍率</summary>
        public float elementalMultiplier = 1.0f;

        /// <summary>傷害來源 (攻擊者)</summary>
        public GameObject source;

        /// <summary>傷害目標 (受擊者)</summary>
        public GameObject target;

        /// <summary>傷害來源描述 (例如：技能名稱)</summary>
        public string sourceDescription;

        /// <summary>
        /// 建構子 - 基礎傷害資訊
        /// </summary>
        public DamageInfo(float damage, DamageType damageType = DamageType.Physical)
        {
            this.baseDamage = damage;
            this.finalDamage = damage;
            this.damageType = damageType;
            this.isCritical = false;
            this.isDodged = false;
            this.isBlocked = false;
        }

        /// <summary>
        /// 建構子 - 完整傷害資訊
        /// </summary>
        public DamageInfo(
            float damage,
            DamageType damageType,
            GameObject source,
            GameObject target,
            string sourceDescription = "")
        {
            this.baseDamage = damage;
            this.finalDamage = damage;
            this.damageType = damageType;
            this.source = source;
            this.target = target;
            this.sourceDescription = sourceDescription;
            this.isCritical = false;
            this.isDodged = false;
            this.isBlocked = false;
        }

        /// <summary>
        /// 取得傷害顯示文字 (用於戰鬥日誌)
        /// </summary>
        public string GetDisplayText()
        {
            string damageText = $"{finalDamage:F0}";

            if (isDodged)
                return "閃避!";

            if (isBlocked)
                damageText = $"{damageText} (格擋)";

            if (isCritical)
                damageText = $"{damageText} 暴擊!";

            string typeText = damageType switch
            {
                DamageType.Physical => "物理",
                DamageType.Magical => "魔法",
                DamageType.True => "真實",
                _ => ""
            };

            // 加入元素資訊
            string elementText = "";
            if (elementalMultiplier != 1.0f)
            {
                string affinity = ElementalAffinityTable.GetAffinityDescription(attackerElement, defenderElement);
                elementText = $" [{affinity}]";
            }

            return $"{typeText}傷害: {damageText}{elementText}";
        }

        /// <summary>
        /// 取得傷害顏色 (用於飄字 UI)
        /// </summary>
        public Color GetDamageColor()
        {
            if (isDodged)
                return Color.gray; // 閃避 - 灰色

            if (isCritical)
                return new Color(1f, 0.3f, 0f); // 爆擊 - 橘紅色

            return damageType switch
            {
                DamageType.Physical => Color.white,     // 物理 - 白色
                DamageType.Magical => Color.cyan,       // 魔法 - 青色
                DamageType.True => Color.yellow,        // 真實 - 黃色
                _ => Color.white
            };
        }

        /// <summary>
        /// 複製傷害資訊
        /// </summary>
        public DamageInfo Clone()
        {
            return new DamageInfo(baseDamage, damageType, source, target, sourceDescription)
            {
                finalDamage = this.finalDamage,
                isCritical = this.isCritical,
                isDodged = this.isDodged,
                isBlocked = this.isBlocked
            };
        }
    }
}
