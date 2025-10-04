using System;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// 控場效果類型
    /// </summary>
    public enum CCType
    {
        /// <summary>暈眩 - 無法行動</summary>
        Stun,

        /// <summary>冰凍 - 無法移動和攻擊</summary>
        Freeze,

        /// <summary>減速 - 移動速度降低</summary>
        Slow,

        /// <summary>沉默 - 無法使用技能</summary>
        Silence,

        /// <summary>定身 - 無法移動但可攻擊</summary>
        Root,

        /// <summary>擊飛 - 強制位移</summary>
        Knockback,

        /// <summary>擊倒 - 倒地無法行動</summary>
        Knockdown
    }

    /// <summary>
    /// 控場效果數據
    /// </summary>
    [Serializable]
    public class CrowdControlEffect
    {
        /// <summary>控場類型</summary>
        public CCType type;

        /// <summary>持續時間 (秒)</summary>
        public float duration;

        /// <summary>效果強度 (例如減速百分比)</summary>
        public float intensity;

        /// <summary>剩餘時間</summary>
        public float remainingTime;

        /// <summary>效果來源</summary>
        public string source;

        /// <summary>是否還在生效</summary>
        public bool IsActive => remainingTime > 0f;

        public CrowdControlEffect(CCType type, float duration, float intensity = 1.0f, string source = "")
        {
            this.type = type;
            this.duration = duration;
            this.intensity = intensity;
            this.remainingTime = duration;
            this.source = source;
        }

        /// <summary>
        /// 更新效果 (每幀調用)
        /// </summary>
        /// <param name="deltaTime">時間增量</param>
        /// <returns>是否仍然生效</returns>
        public bool Update(float deltaTime)
        {
            remainingTime -= deltaTime;
            return IsActive;
        }

        /// <summary>
        /// 重置效果時間
        /// </summary>
        public void Refresh()
        {
            remainingTime = duration;
        }

        /// <summary>
        /// 取得顯示文字
        /// </summary>
        public string GetDisplayText()
        {
            string typeName = type switch
            {
                CCType.Stun => "暈眩",
                CCType.Freeze => "冰凍",
                CCType.Slow => "減速",
                CCType.Silence => "沉默",
                CCType.Root => "定身",
                CCType.Knockback => "擊飛",
                CCType.Knockdown => "擊倒",
                _ => "未知"
            };

            return $"{typeName} ({remainingTime:F1}秒)";
        }
    }
}
