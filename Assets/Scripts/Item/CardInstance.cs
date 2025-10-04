using System;
using UnityEngine;

namespace Item
{
    /// <summary>
    /// 卡片實例
    /// 代表玩家擁有的一張具體卡片，包含等級與冷卻狀態
    /// </summary>
    [Serializable]
    public class CardInstance
    {
        // ===== 基礎資料 =====
        [SerializeField] private string instanceId;
        [SerializeField] private CardData data;
        [SerializeField] private int level;

        // ===== 冷卻狀態 (使用真實時間) =====
        [SerializeField] private long lastUsedTimestamp; // Unix 時間戳
        [SerializeField] private bool isEquipped = false;

        // ===== 事件 =====
        public event Action<int> OnLevelUp;
        public event Action OnUsed;
        public event Action OnCooldownComplete;

        // ===== 屬性 =====
        public string InstanceId => instanceId;
        public CardData Data => data;
        public int Level => level;
        public int MaxLevel => data.maxLevel;
        public bool IsMaxLevel => level >= MaxLevel;
        public bool IsEquipped => isEquipped;

        /// <summary>
        /// 當前冷卻剩餘時間 (秒)
        /// </summary>
        public float CooldownRemaining
        {
            get
            {
                if (lastUsedTimestamp == 0)
                    return 0f;

                long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                long elapsedTime = currentTime - lastUsedTimestamp;
                float cooldownSeconds = data.cooldownTime;

                float remaining = cooldownSeconds - elapsedTime;
                return Mathf.Max(0f, remaining);
            }
        }

        /// <summary>
        /// 是否在冷卻中
        /// </summary>
        public bool IsOnCooldown => CooldownRemaining > 0f;

        /// <summary>
        /// 建構子
        /// </summary>
        public CardInstance(CardData data, int initialLevel = 1)
        {
            this.instanceId = Guid.NewGuid().ToString();
            this.data = data;
            this.level = Mathf.Clamp(initialLevel, 1, data.maxLevel);
            this.lastUsedTimestamp = 0;
        }

        /// <summary>
        /// 使用卡片
        /// </summary>
        public bool Use()
        {
            if (IsOnCooldown)
            {
                Debug.LogWarning($"[Card] {data.itemName} 冷卻中！剩餘 {CooldownRemaining:F0} 秒");
                return false;
            }

            if (!isEquipped)
            {
                Debug.LogWarning($"[Card] {data.itemName} 未裝備！");
                return false;
            }

            // 記錄使用時間
            lastUsedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            OnUsed?.Invoke();
            Debug.Log($"[Card] 使用卡片: {data.itemName} (Lv.{level})");
            return true;
        }

        /// <summary>
        /// 融合升級
        /// </summary>
        /// <param name="successRoll">成功判定 (0-100)</param>
        /// <returns>是否成功</returns>
        public bool Fuse(float successRoll)
        {
            if (IsMaxLevel)
            {
                Debug.LogWarning($"[Card] {data.itemName} 已達最高等級！");
                return false;
            }

            bool success = successRoll < data.fusionSuccessRate;

            if (success)
            {
                level++;
                OnLevelUp?.Invoke(level);
                Debug.Log($"[Card] {data.itemName} 融合成功！Lv.{level}");
                return true;
            }
            else
            {
                Debug.LogWarning($"[Card] {data.itemName} 融合失敗！");
                return false;
            }
        }

        /// <summary>
        /// 裝備卡片
        /// </summary>
        public void Equip()
        {
            isEquipped = true;
        }

        /// <summary>
        /// 卸下卡片
        /// </summary>
        public void Unequip()
        {
            isEquipped = false;
        }

        /// <summary>
        /// 重置冷卻 (特殊道具)
        /// </summary>
        public void ResetCooldown()
        {
            lastUsedTimestamp = 0;
            OnCooldownComplete?.Invoke();
        }

        /// <summary>
        /// 取得當前效果強度
        /// </summary>
        public float GetEffectPower()
        {
            return data.GetEffectPower(level);
        }

        /// <summary>
        /// 取得顯示資訊
        /// </summary>
        public string GetDisplayInfo()
        {
            string info = $"{data.GetColoredName()} Lv.{level}\n";
            info += $"{(int)data.starRating}★ 卡片\n";

            if (IsOnCooldown)
            {
                float remainingMinutes = CooldownRemaining / 60f;
                info += $"冷卻中: {remainingMinutes:F1} 分鐘";
            }
            else
            {
                info += "可使用";
            }

            return info;
        }

        /// <summary>
        /// 取得冷卻結束時間 (顯示用)
        /// </summary>
        public DateTime GetCooldownEndTime()
        {
            if (lastUsedTimestamp == 0)
                return DateTime.MinValue;

            return DateTimeOffset.FromUnixTimeSeconds(lastUsedTimestamp)
                .AddSeconds(data.cooldownTime)
                .LocalDateTime;
        }
    }
}
