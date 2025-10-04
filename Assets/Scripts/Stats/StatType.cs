using System;

namespace Stats
{
    /// <summary>
    /// 屬性類型枚舉
    /// </summary>
    public enum StatType
    {
        // ===== 主要屬性 =====
        /// <summary>生命值 (Health Points)</summary>
        HP,

        /// <summary>魔力值 (Mana Points)</summary>
        MP,

        /// <summary>攻擊力 (Attack Power)</summary>
        ATK,

        /// <summary>魔法力 (Magic Power - 影響技能傷害)</summary>
        Magic,

        /// <summary>防禦力 (Defense)</summary>
        DEF,

        /// <summary>移動速度 (Speed)</summary>
        SPD,

        /// <summary>精神力 (Spiritual Power) - 影響眷屬持續作戰與重生速度</summary>
        SP,

        // ===== 隱藏稀有屬性 =====
        /// <summary>爆擊率 (Critical Rate) - 百分比</summary>
        CritRate,

        /// <summary>爆擊傷害 (Critical Damage) - 百分比</summary>
        CritDamage,

        /// <summary>迴避率 (Dodge Rate) - 百分比</summary>
        DodgeRate,

        /// <summary>韌性 (Toughness) - 抗暴擊</summary>
        Toughness,

        /// <summary>穿透 (Penetration) - 無視防禦</summary>
        Penetration,

        /// <summary>技能急速 (Skill Haste) - 減少冷卻時間百分比</summary>
        SkillHaste,

        /// <summary>命中率 (Hit Rate) - 百分比</summary>
        HitRate
    }

    /// <summary>
    /// 屬性類別 - 用於分類顯示
    /// </summary>
    public enum StatCategory
    {
        /// <summary>主要屬性 - 基礎數值</summary>
        Primary,

        /// <summary>隱藏稀有屬性 - 進階數值</summary>
        Secondary
    }
}
