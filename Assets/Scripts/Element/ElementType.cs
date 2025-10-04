using System;

namespace Element
{
    /// <summary>
    /// 元素屬性類型
    /// </summary>
    public enum ElementType
    {
        // ===== 玩家可選屬性 =====
        /// <summary>火屬性 (Fire)</summary>
        Fire,

        /// <summary>水屬性 (Water)</summary>
        Water,

        /// <summary>地屬性 (Earth)</summary>
        Earth,

        /// <summary>風屬性 (Wind)</summary>
        Wind,

        // ===== 特殊屬性 (遊戲全屬性) =====
        /// <summary>無屬性 (None) - 不參與屬性克制</summary>
        None,

        /// <summary>光屬性 (Light)</summary>
        Light,

        /// <summary>闇屬性 (Dark)</summary>
        Dark,

        /// <summary>神屬性 (Divine) - 最高級屬性</summary>
        Divine
    }

    /// <summary>
    /// 元素分類
    /// </summary>
    public enum ElementCategory
    {
        /// <summary>基礎四元素 - 玩家可選</summary>
        Basic,

        /// <summary>特殊元素 - 遊戲限定</summary>
        Special
    }
}
