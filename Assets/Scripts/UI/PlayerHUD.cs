using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Stats;
using Player;

namespace UI
{
    /// <summary>
    /// 玩家 HUD 顯示器
    /// 顯示玩家的血量、魔力、等級等資訊
    /// </summary>
    public class PlayerHUD : MonoBehaviour
    {
        [Header("玩家引用")]
        [SerializeField, Tooltip("玩家控制器 (自動尋找或手動設定)")]
        private PlayerController playerController;

        [Header("血條/魔力條")]
        [SerializeField] private Image hpFillImage;
        [SerializeField] private Image mpFillImage;
        [SerializeField] private TMP_Text hpText;
        [SerializeField] private TMP_Text mpText;

        [Header("屬性顯示")]
        [SerializeField] private TMP_Text atkText;
        [SerializeField] private TMP_Text defText;
        [SerializeField] private TMP_Text magicText;
        [SerializeField] private TMP_Text spdText;

        [Header("進階屬性 (可選)")]
        [SerializeField] private TMP_Text critRateText;
        [SerializeField] private TMP_Text critDamageText;

        [Header("顯示設定")]
        [SerializeField, Tooltip("是否顯示百分比")]
        private bool showPercentage = true;

        [SerializeField, Tooltip("是否顯示數值")]
        private bool showNumbers = true;

        [Header("血量顏色分段")]
        [SerializeField, Tooltip("HP 高於此百分比時為綠色 (健康狀態)")]
        [Range(0f, 1f)] private float hpHealthyThreshold = 0.5f;

        [SerializeField, Tooltip("HP 高於此百分比時為黃色 (警告狀態)")]
        [Range(0f, 1f)] private float hpWarningThreshold = 0.2f;

        [SerializeField, Tooltip("HP 低於警告閾值時為紅色 (危險狀態)")]
        private bool enableHPColorChange = true;

        [Header("顏色設定")]
        [SerializeField, Tooltip("血量健康時的顏色 (>50%) - Tailwind Green 500")]
        private Color hpHealthyColor = new Color(34f/255f, 197f/255f, 94f/255f, 1f); // #22C55E

        [SerializeField, Tooltip("血量警告時的顏色 (20%-50%) - Tailwind Amber 400")]
        private Color hpCautionColor = new Color(251f/255f, 191f/255f, 36f/255f, 1f); // #FBBF24

        [SerializeField, Tooltip("血量危險時的顏色 (<20%) - Tailwind Red 500")]
        private Color hpDangerColor = new Color(239f/255f, 68f/255f, 68f/255f, 1f); // #EF4444

        [SerializeField] private Color mpColor = Color.blue;

        private HealthSystem healthSystem;
        private CharacterStats stats;

        private void Start()
        {
            StartCoroutine(DelayedInitialize());
        }

        private System.Collections.IEnumerator DelayedInitialize()
        {
            yield return null; // 等待一幀
            InitializeHUD();
        }

        /// <summary>
        /// 初始化 HUD
        /// </summary>
        private void InitializeHUD()
        {
            // 自動尋找玩家控制器
            if (playerController == null)
            {
                playerController = FindFirstObjectByType<PlayerController>();

                if (playerController == null)
                {
                    Debug.LogError("[PlayerHUD] 找不到 PlayerController!");
                    enabled = false;
                    return;
                }
            }

            // 等待玩家初始化完成 (戰鬥系統目前被註解,需要先啟用)
            // 現在先檢查是否有 stats 和 healthSystem
            if (playerController.stats == null || playerController.healthSystem == null)
            {
                Debug.LogWarning("[PlayerHUD] PlayerController 的戰鬥系統尚未初始化,HUD 將無法顯示資訊");
                Debug.LogWarning("[PlayerHUD] 請在 PlayerController.InitializePlayer() 中取消註解戰鬥系統初始化程式碼");
                enabled = false;
                return;
            }

            stats = playerController.stats;
            healthSystem = playerController.healthSystem;

            // 訂閱事件
            healthSystem.OnHPChanged += UpdateHP;
            healthSystem.OnMPChanged += UpdateMP;

            // 初始顯示
            UpdateHP(healthSystem.CurrentHP, healthSystem.MaxHP);
            UpdateMP(healthSystem.CurrentMP, healthSystem.MaxMP);
            UpdateStats();

            Debug.Log("[PlayerHUD] HUD 初始化完成");
        }

        private void Update()
        {
            // 每幀更新屬性 (以防裝備變化等)
            UpdateStats();
        }

        /// <summary>
        /// 更新 HP 顯示
        /// </summary>
        private void UpdateHP(float currentHP, float maxHP)
        {
            if (hpFillImage != null)
            {
                float hpPercent = maxHP > 0 ? currentHP / maxHP : 0;
                hpFillImage.fillAmount = hpPercent;

                // 分段顏色變化
                if (enableHPColorChange)
                {
                    if (hpPercent > hpHealthyThreshold)
                    {
                        // 健康狀態: 綠色 (>50%)
                        hpFillImage.color = hpHealthyColor;
                    }
                    else if (hpPercent > hpWarningThreshold)
                    {
                        // 警告狀態: 黃色 (20%-50%)
                        hpFillImage.color = hpCautionColor;
                    }
                    else
                    {
                        // 危險狀態: 紅色 (<20%)
                        hpFillImage.color = hpDangerColor;
                    }
                }
            }

            if (hpText != null)
            {
                UpdateText(hpText, currentHP, maxHP);
            }
        }

        /// <summary>
        /// 更新 MP 顯示
        /// </summary>
        private void UpdateMP(float currentMP, float maxMP)
        {
            if (mpFillImage != null)
            {
                float mpPercent = maxMP > 0 ? currentMP / maxMP : 0;
                mpFillImage.fillAmount = mpPercent;
                mpFillImage.color = mpColor;
            }

            if (mpText != null)
            {
                UpdateText(mpText, currentMP, maxMP);
            }
        }

        /// <summary>
        /// 更新文字顯示 (通用方法)
        /// </summary>
        private void UpdateText(TMP_Text textComponent, float current, float max)
        {
            if (showNumbers && showPercentage)
            {
                float percent = max > 0 ? (current / max) * 100f : 0;
                textComponent.text = $"{current:F0}/{max:F0} ({percent:F0}%)";
            }
            else if (showNumbers)
            {
                textComponent.text = $"{current:F0}/{max:F0}";
            }
            else if (showPercentage)
            {
                float percent = max > 0 ? (current / max) * 100f : 0;
                textComponent.text = $"{percent:F0}%";
            }
        }

        /// <summary>
        /// 更新屬性顯示
        /// </summary>
        private void UpdateStats()
        {
            if (stats == null) return;

            if (atkText != null)
                atkText.text = $"ATK: {stats.GetFinalValue(StatType.ATK):F0}";

            if (defText != null)
                defText.text = $"DEF: {stats.GetFinalValue(StatType.DEF):F0}";

            if (magicText != null)
                magicText.text = $"MAG: {stats.GetFinalValue(StatType.Magic):F0}";

            if (spdText != null)
                spdText.text = $"SPD: {stats.GetFinalValue(StatType.SPD):F0}";

            if (critRateText != null)
                critRateText.text = $"CRIT: {stats.GetFinalValue(StatType.CritRate):F1}%";

            if (critDamageText != null)
                critDamageText.text = $"C.DMG: {stats.GetFinalValue(StatType.CritDamage):F0}%";
        }

        private void OnDestroy()
        {
            // 取消訂閱事件
            if (healthSystem != null)
            {
                healthSystem.OnHPChanged -= UpdateHP;
                healthSystem.OnMPChanged -= UpdateMP;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // 確保閾值在合理範圍且順序正確
            hpHealthyThreshold = Mathf.Clamp01(hpHealthyThreshold);
            hpWarningThreshold = Mathf.Clamp01(hpWarningThreshold);

            // 確保健康閾值 > 警告閾值
            if (hpHealthyThreshold <= hpWarningThreshold)
            {
                hpHealthyThreshold = Mathf.Min(0.5f, hpWarningThreshold + 0.1f);
            }
        }
#endif
    }
}
