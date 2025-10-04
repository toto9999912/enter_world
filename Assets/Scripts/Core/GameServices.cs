using UnityEngine;

namespace Core
{
    /// <summary>
    /// 遊戲服務初始化器
    /// </summary>
    public class GameServices : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            InitializeServices();
        }

        private void InitializeServices()
        {
            // 註冊所有服務
            ServiceLocator.Register<Services.ICombatService>(new Services.CombatService());
            ServiceLocator.Register<Services.IInventoryService>(new Services.InventoryService());
            ServiceLocator.Register<Services.ISaveService>(new Services.SaveService());

            Debug.Log("[GameServices] All services initialized");
        }

        private void OnDestroy()
        {
            ServiceLocator.Clear();
            EventBus.Clear();
        }
    }
}
