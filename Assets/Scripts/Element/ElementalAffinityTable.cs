using System.Collections.Generic;
using UnityEngine;

namespace Element
{
    /// <summary>
    /// 元素克制表 - 管理元素間的傷害倍率
    /// 根據圖片數據：八大屬性完整相剋陣列
    /// </summary>
    public static class ElementalAffinityTable
    {
        // 元素克制倍率表 (攻擊者元素 -> 防禦者元素 -> 傷害倍率%)
        private static readonly Dictionary<ElementType, Dictionary<ElementType, float>> affinityTable =
            new Dictionary<ElementType, Dictionary<ElementType, float>>
        {
            // 火屬性 🔥
            {
                ElementType.Fire, new Dictionary<ElementType, float>
                {
                    { ElementType.Fire, 100f },  // 火 vs 火 = 100%
                    { ElementType.Water, 80f },  // 火 vs 水 = 80%
                    { ElementType.Earth, 100f }, // 火 vs 地 = 100%
                    { ElementType.Wind, 160f },  // 火 vs 風 = 160%
                    { ElementType.None, 125f },  // 火 vs 無 = 125%
                    { ElementType.Light, 105f }, // 火 vs 光 = 105%
                    { ElementType.Dark, 95f },   // 火 vs 闇 = 95%
                    { ElementType.Divine, 75f }  // 火 vs 神 = 75%
                }
            },

            // 水屬性 💧
            {
                ElementType.Water, new Dictionary<ElementType, float>
                {
                    { ElementType.Fire, 140f },  // 水 vs 火 = 140%
                    { ElementType.Water, 100f }, // 水 vs 水 = 100%
                    { ElementType.Earth, 75f },  // 水 vs 地 = 75%
                    { ElementType.Wind, 100f },  // 水 vs 風 = 100%
                    { ElementType.None, 125f },  // 水 vs 無 = 125%
                    { ElementType.Light, 105f }, // 水 vs 光 = 105%
                    { ElementType.Dark, 95f },   // 水 vs 闇 = 95%
                    { ElementType.Divine, 75f }  // 水 vs 神 = 75%
                }
            },

            // 地屬性 🌍
            {
                ElementType.Earth, new Dictionary<ElementType, float>
                {
                    { ElementType.Fire, 100f },  // 地 vs 火 = 100%
                    { ElementType.Water, 130f }, // 地 vs 水 = 130%
                    { ElementType.Earth, 100f }, // 地 vs 地 = 100%
                    { ElementType.Wind, 85f },   // 地 vs 風 = 85%
                    { ElementType.None, 125f },  // 地 vs 無 = 125%
                    { ElementType.Light, 105f }, // 地 vs 光 = 105%
                    { ElementType.Dark, 95f },   // 地 vs 闇 = 95%
                    { ElementType.Divine, 75f }  // 地 vs 神 = 75%
                }
            },

            // 風屬性 🌪️
            {
                ElementType.Wind, new Dictionary<ElementType, float>
                {
                    { ElementType.Fire, 90f },   // 風 vs 火 = 90%
                    { ElementType.Water, 100f }, // 風 vs 水 = 100%
                    { ElementType.Earth, 145f }, // 風 vs 地 = 145%
                    { ElementType.Wind, 100f },  // 風 vs 風 = 100%
                    { ElementType.None, 125f },  // 風 vs 無 = 125%
                    { ElementType.Light, 105f }, // 風 vs 光 = 105%
                    { ElementType.Dark, 95f },   // 風 vs 闇 = 95%
                    { ElementType.Divine, 75f }  // 風 vs 神 = 75%
                }
            },

            // 無屬性 ⚫
            {
                ElementType.None, new Dictionary<ElementType, float>
                {
                    { ElementType.Fire, 80f },   // 無 vs 火 = 80%
                    { ElementType.Water, 80f },  // 無 vs 水 = 80%
                    { ElementType.Earth, 80f },  // 無 vs 地 = 80%
                    { ElementType.Wind, 80f },   // 無 vs 風 = 80%
                    { ElementType.None, 100f },  // 無 vs 無 = 100%
                    { ElementType.Light, 80f },  // 無 vs 光 = 80%
                    { ElementType.Dark, 80f },   // 無 vs 闇 = 80%
                    { ElementType.Divine, 80f }  // 無 vs 神 = 80%
                }
            },

            // 光屬性 ✨
            {
                ElementType.Light, new Dictionary<ElementType, float>
                {
                    { ElementType.Fire, 95f },   // 光 vs 火 = 95%
                    { ElementType.Water, 95f },  // 光 vs 水 = 95%
                    { ElementType.Earth, 95f },  // 光 vs 地 = 95%
                    { ElementType.Wind, 95f },   // 光 vs 風 = 95%
                    { ElementType.None, 125f },  // 光 vs 無 = 125%
                    { ElementType.Light, 100f }, // 光 vs 光 = 100%
                    { ElementType.Dark, 200f },  // 光 vs 闇 = 200%
                    { ElementType.Divine, 100f } // 光 vs 神 = 100%
                }
            },

            // 闇屬性 🌑
            {
                ElementType.Dark, new Dictionary<ElementType, float>
                {
                    { ElementType.Fire, 120f },  // 闇 vs 火 = 120%
                    { ElementType.Water, 120f }, // 闇 vs 水 = 120%
                    { ElementType.Earth, 120f }, // 闇 vs 地 = 120%
                    { ElementType.Wind, 120f },  // 闇 vs 風 = 120%
                    { ElementType.None, 125f },  // 闇 vs 無 = 125%
                    { ElementType.Light, 50f },  // 闇 vs 光 = 50%
                    { ElementType.Dark, 100f },  // 闇 vs 闇 = 100%
                    { ElementType.Divine, 75f }  // 闇 vs 神 = 75%
                }
            },

            // 神屬性 ⚡
            {
                ElementType.Divine, new Dictionary<ElementType, float>
                {
                    { ElementType.Fire, 150f },  // 神 vs 火 = 150%
                    { ElementType.Water, 150f }, // 神 vs 水 = 150%
                    { ElementType.Earth, 150f }, // 神 vs 地 = 150%
                    { ElementType.Wind, 150f },  // 神 vs 風 = 150%
                    { ElementType.None, 150f },  // 神 vs 無 = 150%
                    { ElementType.Light, 100f }, // 神 vs 光 = 100%
                    { ElementType.Dark, 150f },  // 神 vs 闇 = 150%
                    { ElementType.Divine, 100f } // 神 vs 神 = 100%
                }
            }
        };

        /// <summary>
        /// 取得元素克制倍率
        /// </summary>
        /// <param name="attackerElement">攻擊者元素</param>
        /// <param name="defenderElement">防禦者元素</param>
        /// <returns>傷害倍率 (百分比，100 = 100%)</returns>
        public static float GetAffinityMultiplier(ElementType attackerElement, ElementType defenderElement)
        {
            if (affinityTable.TryGetValue(attackerElement, out var defenderTable))
            {
                if (defenderTable.TryGetValue(defenderElement, out float multiplier))
                {
                    return multiplier / 100f; // 轉換為小數 (100% = 1.0)
                }
            }

            // 預設為 100% (無克制)
            Debug.LogWarning($"[ElementalAffinity] 找不到元素克制資料: {attackerElement} vs {defenderElement}，使用預設倍率 100%");
            return 1.0f;
        }

        /// <summary>
        /// 取得元素克制關係描述
        /// </summary>
        /// <param name="attackerElement">攻擊者元素</param>
        /// <param name="defenderElement">防禦者元素</param>
        /// <returns>克制關係文字</returns>
        public static string GetAffinityDescription(ElementType attackerElement, ElementType defenderElement)
        {
            float multiplier = GetAffinityMultiplier(attackerElement, defenderElement);

            if (multiplier > 1.3f)
                return "超強效果!";
            else if (multiplier > 1.1f)
                return "效果絕佳!";
            else if (multiplier > 0.9f)
                return "普通";
            else if (multiplier > 0.7f)
                return "效果不佳";
            else
                return "效果微弱...";
        }

        /// <summary>
        /// 檢查是否為玩家可選元素
        /// </summary>
        public static bool IsPlayerSelectableElement(ElementType element)
        {
            return element == ElementType.Fire ||
                   element == ElementType.Water ||
                   element == ElementType.Earth ||
                   element == ElementType.Wind;
        }

        /// <summary>
        /// 取得元素的中文名稱
        /// </summary>
        public static string GetElementName(ElementType element)
        {
            return element switch
            {
                ElementType.Fire => "火",
                ElementType.Water => "水",
                ElementType.Earth => "地",
                ElementType.Wind => "風",
                ElementType.None => "無",
                ElementType.Light => "光",
                ElementType.Dark => "闇",
                ElementType.Divine => "神",
                _ => "未知"
            };
        }

        /// <summary>
        /// 取得元素的顏色 (用於 UI 顯示)
        /// </summary>
        public static Color GetElementColor(ElementType element)
        {
            return element switch
            {
                ElementType.Fire => new Color(1f, 0.3f, 0f),      // 橘紅色
                ElementType.Water => new Color(0f, 0.5f, 1f),     // 藍色
                ElementType.Earth => new Color(0.6f, 0.4f, 0.2f), // 棕色
                ElementType.Wind => new Color(0.7f, 1f, 0.7f),    // 淺綠色
                ElementType.None => new Color(0.5f, 0.5f, 0.5f),  // 灰色
                ElementType.Light => new Color(1f, 1f, 0.8f),     // 金黃色
                ElementType.Dark => new Color(0.3f, 0f, 0.5f),    // 紫黑色
                ElementType.Divine => new Color(1f, 0.8f, 0f),    // 神聖金色
                _ => Color.white
            };
        }
    }
}
