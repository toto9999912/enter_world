using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Stats;

namespace UI
{
    /// <summary>
    /// 簡單的血量 UI 顯示
    /// 顯示玩家和敵人的生命值
    /// </summary>
    public class HealthUI : MonoBehaviour
    {
        [Header("引用")]
        [Tooltip("玩家物件")]
        public GameObject player;

        [Tooltip("顯示玩家血量的 Text (使用 TextMeshPro 或 UI.Text)")]
        public TextMeshProUGUI healthText;

        [Header("可選: 使用 UI.Text 替代 TextMeshPro")]
        public Text healthTextLegacy;

        [Header("更新設定")]
        [Tooltip("UI 更新頻率 (秒)")]
        public float updateInterval = 0.1f;

        private Player.PlayerController playerController;
        private float updateTimer;

        private void Start()
        {
            // 獲取玩家控制器
            if (player != null)
            {
                playerController = player.GetComponent<Player.PlayerController>();
            }

            if (playerController == null)
            {
                Debug.LogWarning("[HealthUI] 找不到 PlayerController,嘗試自動尋找...");
                var playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    playerController = playerObj.GetComponent<Player.PlayerController>();
                    player = playerObj;
                }
            }

            if (playerController == null)
            {
                Debug.LogError("[HealthUI] 無法找到 PlayerController!");
            }
        }

        private void Update()
        {
            updateTimer += Time.deltaTime;

            if (updateTimer >= updateInterval)
            {
                updateTimer = 0f;
                UpdateHealthDisplay();
            }
        }

        /// <summary>
        /// 更新血量顯示
        /// </summary>
        private void UpdateHealthDisplay()
        {
            if (playerController == null || playerController.healthSystem == null)
                return;

            var health = playerController.healthSystem;
            string displayText = $"生命值: {health.CurrentHP:F0} / {health.MaxHP:F0}";

            // 添加顏色 (血量低於 30% 顯示紅色)
            float healthPercent = health.CurrentHP / health.MaxHP;
            if (healthPercent <= 0.3f)
            {
                displayText = $"<color=red>生命值: {health.CurrentHP:F0} / {health.MaxHP:F0}</color>";
            }
            else if (healthPercent <= 0.6f)
            {
                displayText = $"<color=yellow>生命值: {health.CurrentHP:F0} / {health.MaxHP:F0}</color>";
            }
            else
            {
                displayText = $"<color=green>生命值: {health.CurrentHP:F0} / {health.MaxHP:F0}</color>";
            }

            // 設定文字
            if (healthText != null)
            {
                healthText.text = displayText;
            }
            else if (healthTextLegacy != null)
            {
                healthTextLegacy.text = displayText;
            }
        }

        /// <summary>
        /// 手動設定玩家引用
        /// </summary>
        public void SetPlayer(GameObject playerObj)
        {
            player = playerObj;
            if (player != null)
            {
                playerController = player.GetComponent<Player.PlayerController>();
            }
        }
    }
}
