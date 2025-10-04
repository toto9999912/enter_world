using UnityEngine;
using Core;
using Services;

namespace Core
{
    /// <summary>
    /// 遊戲管理器
    /// 處理應用程式生命週期事件和自動存檔
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("自動存檔設定")]
        [SerializeField, Tooltip("是否啟用暫停時自動存檔")]
        private bool autoSaveOnPause = true;
        
        [SerializeField, Tooltip("是否啟用失焦時自動存檔")]
        private bool autoSaveOnFocusLost = true;
        
        [SerializeField, Tooltip("是否啟用退出時自動存檔")]
        private bool autoSaveOnQuit = true;
        
        [SerializeField, Tooltip("自動存檔間隔(秒)")]
        private float autoSaveInterval = 300f; // 5分鐘
        
        private float autoSaveTimer = 0f;
        private ISaveService saveService;

        private void Awake()
        {
            // 確保只有一個 GameManager 實例
            if (FindObjectsByType<GameManager>(FindObjectsSortMode.None).Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // 獲取存檔服務
            saveService = ServiceLocator.Get<ISaveService>();
            
            if (saveService == null)
            {
                Debug.LogWarning("[GameManager] 找不到存檔服務！");
            }
        }

        private void Update()
        {
            // 定時自動存檔
            if (autoSaveInterval > 0f)
            {
                autoSaveTimer += Time.deltaTime;
                
                if (autoSaveTimer >= autoSaveInterval)
                {
                    autoSaveTimer = 0f;
                    PerformAutoSave("定時自動存檔");
                }
            }
        }

        /// <summary>
        /// 應用程式暫停時觸發
        /// </summary>
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && autoSaveOnPause)
            {
                PerformAutoSave("應用程式暫停");
            }
        }

        /// <summary>
        /// 應用程式失焦時觸發
        /// </summary>
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && autoSaveOnFocusLost)
            {
                PerformAutoSave("應用程式失焦");
            }
        }

        /// <summary>
        /// 應用程式退出時觸發
        /// </summary>
        private void OnApplicationQuit()
        {
            if (autoSaveOnQuit)
            {
                PerformAutoSave("應用程式退出");
            }
        }

        /// <summary>
        /// 執行自動存檔
        /// </summary>
        private void PerformAutoSave(string reason)
        {
            if (saveService != null)
            {
                Debug.Log($"[GameManager] {reason} - 執行自動存檔");
                saveService.AutoSave();
            }
        }

        /// <summary>
        /// 手動觸發自動存檔 (供外部呼叫)
        /// </summary>
        public void TriggerAutoSave()
        {
            PerformAutoSave("手動觸發");
        }

        /// <summary>
        /// 重置自動存檔計時器
        /// </summary>
        public void ResetAutoSaveTimer()
        {
            autoSaveTimer = 0f;
        }

        private void OnDestroy()
        {
            // 清理事件訂閱
            saveService = null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (autoSaveInterval < 0f)
            {
                autoSaveInterval = 0f;
            }
        }
#endif
    }
}
