using UnityEngine;
using Core;
using Services;
using Inventory;

namespace Core
{
    /// <summary>
    /// 遊戲服務初始化器
    /// 負責在場景載入時初始化所有核心服務
    /// </summary>
    public class GameServices : MonoBehaviour
    {
        [Header("服務配置")]
        [Tooltip("是否在每個場景都保留此物件")]
        public bool dontDestroyOnLoad = true;

        [Header("調試")]
        public bool enableDebugLogs = true;

        private static GameServices instance;

        private void Awake()
        {
            // 單例模式
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            InitializeServices();
        }

        /// <summary>
        /// 初始化所有服務
        /// </summary>
        private void InitializeServices()
        {
            LogDebug("=== 開始初始化遊戲服務 ===");

            // 1. 初始化 ServiceLocator
            InitializeServiceLocator();

            // 2. 初始化 EventBus
            InitializeEventBus();

            // 3. 註冊核心服務
            RegisterCoreServices();

            // 4. 訂閱全域事件
            SubscribeToGlobalEvents();

            LogDebug("=== 遊戲服務初始化完成 ===");
        }

        /// <summary>
        /// 初始化 ServiceLocator
        /// </summary>
        private void InitializeServiceLocator()
        {
            // ServiceLocator 是靜態類,不需要初始化
            LogDebug("[ServiceLocator] 已準備就緒");
        }

        /// <summary>
        /// 初始化 EventBus
        /// </summary>
        private void InitializeEventBus()
        {
            // EventBus 是靜態類,不需要初始化
            LogDebug("[EventBus] 已準備就緒");
        }

        /// <summary>
        /// 註冊核心服務
        /// </summary>
        private void RegisterCoreServices()
        {
            // 1. 戰鬥服務
            var combatService = new CombatService();
            ServiceLocator.Register<ICombatService>(combatService);
            LogDebug("[CombatService] 已註冊");

            // 2. 背包服務
            var inventoryService = new InventoryService();
            inventoryService.Initialize(); // 內部會創建 InventorySystem(50)
            ServiceLocator.Register<IInventoryService>(inventoryService);
            LogDebug("[InventoryService] 已註冊");

            // 3. 存檔服務
            var saveService = new SaveService();
            ServiceLocator.Register<ISaveService>(saveService);
            LogDebug("[SaveService] 已註冊");

            // TODO: 當 SkillService 和 CompanionService 實作完成後,在此註冊
            // var skillService = new SkillService();
            // ServiceLocator.Register<ISkillService>(skillService);
            // LogDebug("[SkillService] 已註冊");

            // var companionService = new CompanionService();
            // ServiceLocator.Register<ICompanionService>(companionService);
            // LogDebug("[CompanionService] 已註冊");
        }

        /// <summary>
        /// 訂閱全域事件
        /// </summary>
        private void SubscribeToGlobalEvents()
        {
            // 訂閱傷害事件 (用於全域傷害統計等)
            EventBus.Subscribe<DamageDealtEvent>(OnDamageDealt);
            LogDebug("[EventBus] 已訂閱 DamageDealtEvent");

            // 訂閱死亡事件
            EventBus.Subscribe<EntityDeathEvent>(OnEntityDeath);
            LogDebug("[EventBus] 已訂閱 EntityDeathEvent");

            // 訂閱物品新增事件
            EventBus.Subscribe<ItemAddedEvent>(OnItemAdded);
            LogDebug("[EventBus] 已訂閱 ItemAddedEvent");
        }

        // ===== 事件處理器 =====

        private void OnDamageDealt(DamageDealtEvent e)
        {
            LogDebug($"[全域事件] 造成傷害: {e.Damage:F1} (暴擊: {e.IsCritical})");
        }

        private void OnEntityDeath(EntityDeathEvent e)
        {
            var entityName = (e.Entity as UnityEngine.Object)?.name ?? "Unknown";
            LogDebug($"[全域事件] 實體死亡: {entityName}");
        }

        private void OnItemAdded(ItemAddedEvent e)
        {
            LogDebug($"[全域事件] 獲得物品: {e.ItemId} x{e.Quantity} (類型: {e.ItemType})");
        }

        // ===== 清理 =====

        private void OnDestroy()
        {
            // 取消訂閱事件
            EventBus.Unsubscribe<DamageDealtEvent>(OnDamageDealt);
            EventBus.Unsubscribe<EntityDeathEvent>(OnEntityDeath);
            EventBus.Unsubscribe<ItemAddedEvent>(OnItemAdded);

            LogDebug("[GameServicesInitializer] 已清理");
        }

        // ===== 工具方法 =====

        private void LogDebug(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"<color=cyan>[GameServices]</color> {message}");
            }
        }

        // ===== 公開 API =====

        /// <summary>
        /// 檢查服務是否已初始化
        /// </summary>
        public static bool IsInitialized()
        {
            return instance != null;
        }

        /// <summary>
        /// 重新初始化服務 (測試用)
        /// </summary>
        public static void Reinitialize()
        {
            if (instance != null)
            {
                instance.InitializeServices();
            }
        }
    }
}
