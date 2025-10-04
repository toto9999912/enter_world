using UnityEngine;
using TMPro;
using Core;

namespace Combat
{
    /// <summary>
    /// 傷害數字顯示
    /// </summary>
    public class DamageNumber : MonoBehaviour
    {
        [Header("引用")]
        public TextMeshProUGUI damageText;

        [Header("動畫參數")]
        public float moveSpeed = 2f;
        public float fadeSpeed = 1f;
        public float lifetime = 1.5f;

        private float timer;
        private Color textColor;

        public void Setup(float damage, bool isCritical, bool isHeal)
        {
            if (damageText == null)
            {
                damageText = GetComponentInChildren<TextMeshProUGUI>();
            }

            // 設定文字
            if (isHeal)
            {
                damageText.text = $"+{Mathf.RoundToInt(damage)}";
                textColor = Color.green;
            }
            else
            {
                damageText.text = Mathf.RoundToInt(damage).ToString();
                textColor = isCritical ? Color.yellow : Color.white;
            }

            damageText.color = textColor;

            // 暴擊字體更大
            if (isCritical)
            {
                damageText.fontSize = 48;
            }
            else
            {
                damageText.fontSize = 36;
            }

            timer = 0f;
        }

        private void Update()
        {
            timer += Time.deltaTime;

            // 向上移動
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;

            // 淡出
            float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
            textColor.a = alpha;
            damageText.color = textColor;

            // 生命週期結束
            if (timer >= lifetime)
            {
                PoolManager.Instance.Return("DamageNumber", gameObject);
            }
        }
    }
}
