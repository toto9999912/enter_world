using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    /// <summary>
    /// 敵人頭頂血條
    /// 自動跟隨敵人並顯示血量
    /// </summary>
    public class EnemyHealthBar : MonoBehaviour
    {
        [Header("引用")]
        [Tooltip("敵人控制器 (自動查找)")]
        public AI.EnemyController enemyController;

        [Header("UI 元件")]
        [Tooltip("血條 Slider (使用 UI > Slider)")]
        public Slider healthSlider;

        [Tooltip("血量文字 (可選,使用 TextMeshPro)")]
        public TextMeshProUGUI healthText;

        [Header("顯示設定")]
        [Tooltip("血條在敵人上方的偏移")]
        public Vector3 offset = new Vector3(0, 2f, 0);

        [Tooltip("是否始終面向攝影機")]
        public bool billboardToCamera = true;

        [Header("顏色設定")]
        public Color healthyColor = Color.green;
        public Color damagedColor = Color.yellow;
        public Color criticalColor = Color.red;

        [Range(0f, 1f)]
        public float damagedThreshold = 0.6f; // 血量低於此值顯示黃色

        [Range(0f, 1f)]
        public float criticalThreshold = 0.3f; // 血量低於此值顯示紅色

        private Camera mainCamera;
        private Image fillImage;

        private void Start()
        {
            mainCamera = Camera.main;

            // 自動查找敵人控制器
            if (enemyController == null)
            {
                enemyController = GetComponentInParent<AI.EnemyController>();
            }

            if (enemyController == null)
            {
                Debug.LogError("[EnemyHealthBar] 找不到 EnemyController!");
                enabled = false;
                return;
            }

            // 獲取 Slider 的 Fill Image
            if (healthSlider != null)
            {
                fillImage = healthSlider.fillRect?.GetComponent<Image>();
            }

            InitializeHealthBar();
        }

        private void LateUpdate()
        {
            UpdateHealthBar();
            UpdatePosition();
            UpdateRotation();
        }

        /// <summary>
        /// 初始化血條
        /// </summary>
        private void InitializeHealthBar()
        {
            if (healthSlider != null)
            {
                healthSlider.minValue = 0f;
                healthSlider.maxValue = 1f;
                healthSlider.value = 1f;
            }
        }

        /// <summary>
        /// 更新血條數值
        /// </summary>
        private void UpdateHealthBar()
        {
            if (enemyController?.healthSystem == null) return;

            var health = enemyController.healthSystem;
            float healthPercent = health.CurrentHP / health.MaxHP;

            // 更新 Slider
            if (healthSlider != null)
            {
                healthSlider.value = healthPercent;

                // 更新顏色
                if (fillImage != null)
                {
                    if (healthPercent <= criticalThreshold)
                    {
                        fillImage.color = criticalColor;
                    }
                    else if (healthPercent <= damagedThreshold)
                    {
                        fillImage.color = damagedColor;
                    }
                    else
                    {
                        fillImage.color = healthyColor;
                    }
                }
            }

            // 更新文字
            if (healthText != null)
            {
                healthText.text = $"{health.CurrentHP:F0} / {health.MaxHP:F0}";
            }
        }

        /// <summary>
        /// 更新位置 (跟隨敵人)
        /// </summary>
        private void UpdatePosition()
        {
            if (enemyController == null) return;

            transform.position = enemyController.transform.position + offset;
        }

        /// <summary>
        /// 更新旋轉 (面向攝影機)
        /// </summary>
        private void UpdateRotation()
        {
            if (billboardToCamera && mainCamera != null)
            {
                transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                                 mainCamera.transform.rotation * Vector3.up);
            }
        }

        /// <summary>
        /// 在 Scene 視圖中顯示偏移位置
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + offset, 0.1f);
        }
    }
}
