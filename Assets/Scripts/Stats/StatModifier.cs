using System;

namespace Stats
{
    /// <summary>
    /// 屬性加成類型
    /// </summary>
    public enum ModifierType
    {
        /// <summary>固定數值加成 (例：+10 ATK)</summary>
        Flat,

        /// <summary>百分比加成 (例：+15% HP)，數值範圍 0-100</summary>
        Percentage,

        /// <summary>最終值加成 (最後計算，例：+10% 最終傷害)，數值範圍 0-100</summary>
        Final
    }

    /// <summary>
    /// 屬性修飾器 - 描述單個屬性的加成效果
    /// 改用 class 以便透過引用追蹤和管理修飾器
    /// </summary>
    [Serializable]
    public class StatModifier
    {
        /// <summary>屬性類型</summary>
        public StatType statType;

        /// <summary>加成類型</summary>
        public ModifierType modifierType;

        /// <summary>
        /// 加成數值
        /// - Flat: 固定數值 (例：10 = +10 攻擊力)
        /// - Percentage/Final: 百分比 (例：15 = +15%)
        /// </summary>
        public float value;

        /// <summary>修飾器來源 (例如：裝備名稱、Buff名稱)</summary>
        public string source;

        /// <summary>修飾器唯一ID - 用於精確移除</summary>
        public string id;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="statType">屬性類型</param>
        /// <param name="modifierType">加成類型</param>
        /// <param name="value">加成數值</param>
        /// <param name="source">來源標識 (選填)</param>
        public StatModifier(StatType statType, ModifierType modifierType, float value, string source = "")
        {
            this.statType = statType;
            this.modifierType = modifierType;
            this.value = value;
            this.source = source;
            this.id = Guid.NewGuid().ToString(); // 自動生成唯一ID
        }

        /// <summary>
        /// 取得顯示文字
        /// </summary>
        public string GetDisplayText()
        {
            string prefix = value >= 0 ? "+" : "";

            switch (modifierType)
            {
                case ModifierType.Flat:
                    return $"{prefix}{value:F0} {GetStatName()}";

                case ModifierType.Percentage:
                case ModifierType.Final:
                    return $"{prefix}{value:F1}% {GetStatName()}";

                default:
                    return $"{prefix}{value} {GetStatName()}";
            }
        }

        /// <summary>
        /// 取得屬性中文名稱
        /// </summary>
        private string GetStatName()
        {
            switch (statType)
            {
                case StatType.HP: return "生命值";
                case StatType.MP: return "魔力值";
                case StatType.ATK: return "攻擊力";
                case StatType.Magic: return "魔法力";
                case StatType.DEF: return "防禦力";
                case StatType.SPD: return "移動速度";
                case StatType.CritRate: return "爆擊率";
                case StatType.CritDamage: return "爆擊傷害";
                case StatType.DodgeRate: return "迴避率";
                case StatType.Toughness: return "韌性";
                case StatType.Penetration: return "穿透";
                case StatType.SkillHaste: return "技能急速";
                default: return statType.ToString();
            }
        }
    }
}
