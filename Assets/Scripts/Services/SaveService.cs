using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core;
using Level;
using Stats;
using Inventory;
using Skill;
using Companion;
using Item;

namespace Services
{
    /// <summary>
    /// 完整的存檔服務實作
    /// 支援加密、備份、自動存檔等功能
    /// </summary>
    public class SaveService : ISaveService
    {
        private readonly string saveDirectory;
        private const string SaveFileExtension = ".sav";
        private const int MaxSlots = 3;
        private const int AutoSaveSlot = 999;
        private const int SaveVersion = 1;

        public SaveService()
        {
            saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");
        }

        public void Initialize()
        {
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
            
            Debug.Log($"[SaveService] 初始化完成。存檔路徑: {saveDirectory}");
        }

        public void Shutdown()
        {
            // 關閉前自動存檔
            AutoSave();
            Debug.Log("[SaveService] 服務關閉");
        }

        #region Save Operations

        public bool SaveGame(int slotIndex)
        {
            try
            {
                var saveData = CollectSaveData();
                saveData.saveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                saveData.slotIndex = slotIndex;
                saveData.version = SaveVersion;

                // 轉換為 JSON
                string json = JsonUtility.ToJson(saveData, true);
                
                // 加密存檔
                string encrypted = EncryptSaveData(json);
                
                // 寫入主檔案
                string filePath = GetSaveFilePath(slotIndex);
                File.WriteAllText(filePath, encrypted);
                
                // 寫入備份
                string backupPath = GetBackupFilePath(slotIndex);
                File.WriteAllText(backupPath, encrypted);

                // 發布事件
                EventBus.Publish(new GameSavedEvent { SlotIndex = slotIndex });
                
                Debug.Log($"[SaveService] 遊戲已存檔至槽位 {slotIndex}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveService] 存檔失敗: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        public bool LoadGame(int slotIndex)
        {
            try
            {
                string filePath = GetSaveFilePath(slotIndex);
                
                // 如果主檔案不存在，嘗試載入備份
                if (!File.Exists(filePath))
                {
                    filePath = GetBackupFilePath(slotIndex);
                    if (!File.Exists(filePath))
                    {
                        Debug.LogWarning($"[SaveService] 找不到存檔檔案: 槽位 {slotIndex}");
                        return false;
                    }
                    Debug.LogWarning($"[SaveService] 主存檔損毀，正在載入備份...");
                }

                // 讀取並解密
                string encrypted = File.ReadAllText(filePath);
                string json = DecryptSaveData(encrypted);
                
                if (string.IsNullOrEmpty(json))
                {
                    Debug.LogError("[SaveService] 解密後的資料為空");
                    return false;
                }
                
                // 反序列化
                CompleteSaveData saveData = JsonUtility.FromJson<CompleteSaveData>(json);
                
                // 驗證存檔完整性
                if (!ValidateSaveData(saveData))
                {
                    Debug.LogError("[SaveService] 存檔資料驗證失敗！");
                    return false;
                }

                // 套用存檔數據
                ApplySaveData(saveData);

                // 發布事件
                EventBus.Publish(new GameLoadedEvent { SlotIndex = slotIndex });

                Debug.Log($"[SaveService] 遊戲已從槽位 {slotIndex} 載入");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveService] 載入失敗: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        public bool DeleteSave(int slotIndex)
        {
            try
            {
                string filePath = GetSaveFilePath(slotIndex);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                
                string backupPath = GetBackupFilePath(slotIndex);
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }
                
                Debug.Log($"[SaveService] 已刪除槽位 {slotIndex} 的存檔");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveService] 刪除失敗: {e.Message}");
                return false;
            }
        }

        public bool SaveExists(int slotIndex)
        {
            return File.Exists(GetSaveFilePath(slotIndex));
        }

        public bool AutoSave()
        {
            Debug.Log("[SaveService] 執行自動存檔...");
            return SaveGame(AutoSaveSlot);
        }

        public SaveSlotInfo[] GetAllSaveSlots()
        {
            SaveSlotInfo[] slots = new SaveSlotInfo[MaxSlots];

            for (int i = 0; i < MaxSlots; i++)
            {
                slots[i] = new SaveSlotInfo
                {
                    slotIndex = i,
                    exists = SaveExists(i)
                };

                if (slots[i].exists)
                {
                    try
                    {
                        string encrypted = File.ReadAllText(GetSaveFilePath(i));
                        string json = DecryptSaveData(encrypted);
                        
                        if (!string.IsNullOrEmpty(json))
                        {
                            var data = JsonUtility.FromJson<CompleteSaveData>(json);

                            slots[i].saveName = data.saveName;
                            slots[i].level = data.playerData?.level ?? 1;
                            slots[i].saveTimestamp = data.saveTimestamp;
                            slots[i].playTimeSeconds = data.progressData?.playTimeSeconds ?? 0;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[SaveService] 讀取槽位 {i} 資訊失敗: {e.Message}");
                    }
                }
            }

            return slots;
        }

        #endregion

        #region Data Collection

        /// <summary>
        /// 收集所有遊戲資料
        /// </summary>
        private CompleteSaveData CollectSaveData()
        {
            var saveData = new CompleteSaveData();
            
            // 收集玩家資料
            var player = UnityEngine.Object.FindFirstObjectByType<Player.PlayerController>();
            if (player != null)
            {
                saveData.playerData = new PlayerData
                {
                    position = player.transform.position,
                    rotation = player.transform.rotation.eulerAngles,
                    currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                };
                
                // 屬性資料
                if (player.stats != null)
                {
                    saveData.statsData = SerializeStats(player.stats);
                }
                
                // 血量/魔力
                if (player.healthSystem != null)
                {
                    saveData.healthData = new HealthData
                    {
                        currentHP = player.healthSystem.CurrentHP,
                        currentMP = player.healthSystem.CurrentMP
                    };
                }
            }

            // 收集背包資料
            var inventoryService = ServiceLocator.Get<IInventoryService>();
            if (inventoryService != null)
            {
                saveData.inventoryData = SerializeInventory(inventoryService);
            }

            // 收集技能資料
            saveData.skillData = SerializeSkills();
            
            // 收集眷屬資料
            saveData.companionData = SerializeCompanions();
            
            // 收集遊戲進度
            saveData.progressData = CollectProgressData();
            
            return saveData;
        }

        /// <summary>
        /// 序列化角色屬性
        /// </summary>
        private StatsData SerializeStats(CharacterStats stats)
        {
            var data = new StatsData();
            
            // 保存基礎值
            foreach (StatType statType in Enum.GetValues(typeof(StatType)))
            {
                data.baseValues.Add(new StatValue
                {
                    type = statType,
                    value = stats.GetBaseValue(statType)
                });
            }
            
            // 保存修飾器
            foreach (StatType statType in Enum.GetValues(typeof(StatType)))
            {
                var modifiers = stats.GetModifiers(statType);
                if (modifiers != null)
                {
                    foreach (var mod in modifiers)
                    {
                        data.modifiers.Add(new ModifierData
                        {
                            statType = mod.statType,
                            modifierType = mod.modifierType,
                            value = mod.value,
                            source = mod.source
                        });
                    }
                }
            }
            
            return data;
        }

        /// <summary>
        /// 序列化背包資料
        /// </summary>
        private InventoryData SerializeInventory(IInventoryService inventory)
        {
            var data = new InventoryData
            {
                capacity = inventory.GetCapacity(),
                items = new List<ItemSaveData>()
            };
            
            // 遍歷所有物品槽
            foreach (var slot in inventory.GetAllItems())
            {
                if (slot != null && slot.itemData != null)
                {
                    data.items.Add(new ItemSaveData
                    {
                        itemId = slot.itemData.itemId,
                        quantity = slot.quantity
                    });
                }
            }
            
            Debug.Log($"[SaveService] 序列化背包: {data.items.Count} 個物品");
            return data;
        }

        /// <summary>
        /// 序列化技能資料
        /// </summary>
        private SkillData SerializeSkills()
        {
            var data = new SkillData();
            
            // 目前 SkillTree 只是類別而非服務,需要從玩家物件獲取
            // 暫時返回空資料,等待 ISkillService 實作
            Debug.LogWarning("[SaveService] 技能服務尚未實作,無法序列化技能資料");
            
            return data;
        }

        /// <summary>
        /// 序列化眷屬資料
        /// </summary>
        private CompanionSaveData SerializeCompanions()
        {
            var data = new CompanionSaveData();
            
            // 目前 CompanionManager 只是類別而非服務,需要從玩家物件獲取
            // 暫時返回空資料,等待 ICompanionService 實作
            Debug.LogWarning("[SaveService] 眷屬服務尚未實作,無法序列化眷屬資料");
            
            return data;
        }

        /// <summary>
        /// 收集遊戲進度資料
        /// </summary>
        private ProgressData CollectProgressData()
        {
            return new ProgressData
            {
                playTimeSeconds = (int)Time.time,
                // TODO: 收集任務進度、成就等
            };
        }

        #endregion

        #region Data Application

        /// <summary>
        /// 套用存檔資料到遊戲
        /// </summary>
        private void ApplySaveData(CompleteSaveData saveData)
        {
            // 載入場景
            if (!string.IsNullOrEmpty(saveData.playerData?.currentScene))
            {
                // 監聽場景載入完成事件
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
                
                void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
                {
                    UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
                    RestorePlayerState(saveData);
                }
                
                UnityEngine.SceneManagement.SceneManager.LoadScene(saveData.playerData.currentScene);
            }
            else
            {
                // 如果沒有場景資訊,直接恢復玩家狀態
                RestorePlayerState(saveData);
            }
        }

        /// <summary>
        /// 恢復玩家狀態
        /// </summary>
        private void RestorePlayerState(CompleteSaveData saveData)
        {
            var player = UnityEngine.Object.FindFirstObjectByType<Player.PlayerController>();
            if (player != null && saveData.playerData != null)
            {
                player.transform.position = saveData.playerData.position;
                player.transform.rotation = Quaternion.Euler(saveData.playerData.rotation);
                
                // 恢復屬性
                if (saveData.statsData != null && player.stats != null)
                {
                    ApplyStats(player.stats, saveData.statsData);
                }
                
                // 恢復血量
                if (saveData.healthData != null && player.healthSystem != null)
                {
                    player.healthSystem.SetHP(saveData.healthData.currentHP);
                    player.healthSystem.SetMP(saveData.healthData.currentMP);
                }
            }
            
            // 恢復背包
            if (saveData.inventoryData != null)
            {
                ApplyInventory(saveData.inventoryData);
            }
            
            // TODO: 恢復其他系統...
        }

        /// <summary>
        /// 套用屬性資料
        /// </summary>
        private void ApplyStats(CharacterStats stats, StatsData data)
        {
            // 設定基礎值
            foreach (var statValue in data.baseValues)
            {
                stats.SetBaseValue(statValue.type, statValue.value);
            }
            
            // 清除並重新添加修飾器
            stats.ClearAllModifiers();
            foreach (var modData in data.modifiers)
            {
                var modifier = new StatModifier(
                    modData.statType,
                    modData.modifierType,
                    modData.value,
                    modData.source
                );
                stats.AddModifier(modifier);
            }
        }

        /// <summary>
        /// 套用背包資料
        /// </summary>
        private void ApplyInventory(InventoryData data)
        {
            // TODO: 實作背包恢復邏輯
            Debug.Log($"[SaveService] 恢復背包資料 (容量: {data.capacity})");
        }

        #endregion

        #region Utility

        /// <summary>
        /// 取得存檔檔案路徑
        /// </summary>
        private string GetSaveFilePath(int slotIndex)
        {
            return Path.Combine(saveDirectory, $"save_{slotIndex}{SaveFileExtension}");
        }

        /// <summary>
        /// 取得備份檔案路徑
        /// </summary>
        private string GetBackupFilePath(int slotIndex)
        {
            return Path.Combine(saveDirectory, $"save_{slotIndex}_backup{SaveFileExtension}");
        }

        /// <summary>
        /// 驗證存檔資料完整性
        /// </summary>
        private bool ValidateSaveData(CompleteSaveData data)
        {
            if (data == null)
            {
                Debug.LogError("[SaveService] 存檔資料為 null");
                return false;
            }
            
            if (data.version != SaveVersion)
            {
                Debug.LogWarning($"[SaveService] 存檔版本不符 (存檔: {data.version}, 當前: {SaveVersion})");
                // 未來可以在這裡做版本轉換
                return false;
            }
            
            if (data.playerData == null)
            {
                Debug.LogError("[SaveService] 玩家資料遺失");
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// 加密存檔資料 (簡單的 Base64 編碼)
        /// </summary>
        private string EncryptSaveData(string data)
        {
            // 簡單的 Base64 編碼，實際應使用 AES 等加密算法
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 解密存檔資料
        /// </summary>
        private string DecryptSaveData(string encrypted)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(encrypted);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveService] 解密失敗: {e.Message}");
                return null;
            }
        }

        #endregion
    }
}
