using UnityEngine;
using TMPro;
using System.Collections;

namespace UI
{
    /// <summary>
    /// 傷害數字顯示
    /// 在受傷位置彈出傷害數字
    /// </summary>
    public class DamageNumber : MonoBehaviour
    {
        [Header("UI 設定")]
        public TextMeshProUGUI damageText;

        [Header("動畫設定")]
        public float lifetime = 1.5f;
        public float moveSpeed = 2f;
        public Vector3 moveDirection = new Vector3(0, 1, 0);
        public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [Header("顏色設定")]
        public Color normalDamageColor = Color.white;
        public Color criticalDamageColor = Color.red;
        public Color healColor = Color.green;

        private float timer;
        private Vector3 initialScale;
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            initialScale = transform.localScale;
        }

        private void Update()
        {
            timer += Time.deltaTime;

            // 移動
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            // 縮放動畫
            float scaleT = timer / lifetime;
            float scaleMultiplier = scaleCurve.Evaluate(scaleT);
            transform.localScale = initialScale * scaleMultiplier;

            // 淡出動畫
            if (damageText != null)
            {
                float fadeT = timer / lifetime;
                float alpha = fadeCurve.Evaluate(fadeT);
                Color color = damageText.color;
                color.a = alpha;
                damageText.color = color;
            }

            // 面向攝影機
            if (mainCamera != null)
            {
                transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                                 mainCamera.transform.rotation * Vector3.up);
            }

            // 生命週期結束
            if (timer >= lifetime)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 設定傷害數字
        /// </summary>
        public void SetDamage(float damage, bool isCritical = false)
        {
            if (damageText != null)
            {
                damageText.text = damage.ToString("F0");
                damageText.color = isCritical ? criticalDamageColor : normalDamageColor;

                if (isCritical)
                {
                    damageText.fontSize *= 1.3f;
                    damageText.fontStyle = FontStyles.Bold;
                }
            }
        }

        /// <summary>
        /// 設定治療數字
        /// </summary>
        public void SetHeal(float healAmount)
        {
            if (damageText != null)
            {
                damageText.text = $"+{healAmount:F0}";
                damageText.color = healColor;
            }
        }

        /// <summary>
        /// 靜態工廠方法: 創建傷害數字
        /// </summary>
        public static void Create(Vector3 position, float damage, bool isCritical = false, Transform parent = null)
        {
            // 需要預先創建 DamageNumber Prefab
            GameObject prefab = Resources.Load<GameObject>("UI/DamageNumber");
            if (prefab == null)
            {
                Debug.LogWarning("[DamageNumber] 找不到 Resources/UI/DamageNumber Prefab!");
                return;
            }

            GameObject obj = Instantiate(prefab, position, Quaternion.identity, parent);
            var damageNumber = obj.GetComponent<DamageNumber>();
            if (damageNumber != null)
            {
                damageNumber.SetDamage(damage, isCritical);
            }
        }

        /// <summary>
        /// 靜態工廠方法: 創建治療數字
        /// </summary>
        public static void CreateHeal(Vector3 position, float healAmount, Transform parent = null)
        {
            GameObject prefab = Resources.Load<GameObject>("UI/DamageNumber");
            if (prefab == null)
            {
                Debug.LogWarning("[DamageNumber] 找不到 Resources/UI/DamageNumber Prefab!");
                return;
            }

            GameObject obj = Instantiate(prefab, position, Quaternion.identity, parent);
            var damageNumber = obj.GetComponent<DamageNumber>();
            if (damageNumber != null)
            {
                damageNumber.SetHeal(healAmount);
            }
        }
    }
}
