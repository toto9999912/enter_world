using UnityEngine;
using Core;

namespace Combat
{
    /// <summary>
    /// 傷害數字生成器
    /// </summary>
    public class DamageNumberSpawner : MonoBehaviour
    {
        [Header("預製體")]
        public GameObject damageNumberPrefab;

        [Header("生成位置偏移")]
        public Vector3 spawnOffset = new Vector3(0, 2f, 0);
        public float randomOffset = 0.5f;

        private void Start()
        {
            // 初始化物件池
            if (damageNumberPrefab != null)
            {
                PoolManager.Instance.CreatePool("DamageNumber", damageNumberPrefab, 20);
            }

            // 訂閱傷害事件
            EventBus.Subscribe<DamageDealtEvent>(OnDamageDealt);
            EventBus.Subscribe<HealingEvent>(OnHealing);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<DamageDealtEvent>(OnDamageDealt);
            EventBus.Unsubscribe<HealingEvent>(OnHealing);
        }

        private void OnDamageDealt(DamageDealtEvent evt)
        {
            if (evt.Target is MonoBehaviour targetMono)
            {
                SpawnDamageNumber(
                    targetMono.transform.position,
                    evt.Damage,
                    evt.IsCritical,
                    false
                );
            }
        }

        private void OnHealing(HealingEvent evt)
        {
            if (evt.Target is MonoBehaviour targetMono)
            {
                SpawnDamageNumber(
                    targetMono.transform.position,
                    evt.Amount,
                    false,
                    true
                );
            }
        }

        private void SpawnDamageNumber(Vector3 position, float value, bool isCritical, bool isHeal)
        {
            // 從物件池取得
            GameObject obj = PoolManager.Instance.Get("DamageNumber");
            if (obj == null) return;

            // 設定位置 (加上隨機偏移)
            Vector3 randomPos = new Vector3(
                Random.Range(-randomOffset, randomOffset),
                0,
                Random.Range(-randomOffset, randomOffset)
            );
            obj.transform.position = position + spawnOffset + randomPos;

            // 設定數值
            DamageNumber damageNum = obj.GetComponent<DamageNumber>();
            if (damageNum != null)
            {
                damageNum.Setup(value, isCritical, isHeal);
            }
        }
    }
}
