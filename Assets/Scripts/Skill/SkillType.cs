using System;
using Element;
using Combat;

namespace Skill
{
    /// <summary>
    /// 技能分類 (用於技能樹分支)
    /// </summary>
    public enum SkillCategory
    {
        /// <summary>傷害型 - 法師 (魔法傷害)</summary>
        DamageMage,

        /// <summary>傷害型 - 戰士 (物理傷害)</summary>
        DamageWarrior,

        /// <summary>輔助型 (增益/治療)</summary>
        Support,

        /// <summary>坦克型 (防禦/嘲諷)</summary>
        Tank,

        /// <summary>控場型 (限制技能)</summary>
        CrowdControl,

        /// <summary>通用 (無特定分類)</summary>
        General
    }

    /// <summary>
    /// 技能目標類型
    /// </summary>
    public enum SkillTargetType
    {
        /// <summary>自身</summary>
        Self,

        /// <summary>單一敵人</summary>
        SingleEnemy,

        /// <summary>範圍敵人</summary>
        AreaEnemy,

        /// <summary>單一友方</summary>
        SingleAlly,

        /// <summary>範圍友方</summary>
        AreaAlly,

        /// <summary>全體友方</summary>
        AllAllies,

        /// <summary>全體敵人</summary>
        AllEnemies,

        /// <summary>地面位置 (AOE)</summary>
        Ground
    }

    /// <summary>
    /// 技能施放類型
    /// </summary>
    public enum SkillCastType
    {
        /// <summary>瞬發</summary>
        Instant,

        /// <summary>吟唱 (需要施法時間)</summary>
        Channeled,

        /// <summary>蓄力 (可提前釋放)</summary>
        Charged
    }
}
