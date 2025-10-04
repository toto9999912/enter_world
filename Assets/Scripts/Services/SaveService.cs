using System;
using System.IO;
using UnityEngine;
using Core;
using Level;

namespace Services
{
    public class SaveService : ISaveService
    {
        private readonly string saveDirectory;
        private const string SaveFileExtension = ".sav";
        private const int MaxSlots = 3;

        public SaveService()
        {
            saveDirectory = Application.persistentDataPath + "/Saves/";
        }

        public void Initialize()
        {
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
            Debug.Log($"[SaveService] Initialized. Save path: {saveDirectory}");
        }

        public void Shutdown()
        {
            Debug.Log("[SaveService] Shutdown");
        }

        public bool SaveGame(int slotIndex)
        {
            try
            {
                // 收集存檔數據
                var saveData = CollectSaveData();
                saveData.saveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                // 轉換為 JSON
                string json = JsonUtility.ToJson(saveData, true);

                // 寫入檔案
                string filePath = GetSaveFilePath(slotIndex);
                File.WriteAllText(filePath, json);

                // 發布事件
                EventBus.Publish(new GameSavedEvent { SlotIndex = slotIndex });

                Debug.Log($"[SaveService] Game saved to slot {slotIndex}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveService] Save failed: {e.Message}");
                return false;
            }
        }

        public bool LoadGame(int slotIndex)
        {
            try
            {
                string filePath = GetSaveFilePath(slotIndex);
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"[SaveService] Save file not found: {filePath}");
                    return false;
                }

                // 讀取 JSON
                string json = File.ReadAllText(filePath);
                PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(json);

                // 套用存檔數據
                ApplySaveData(saveData);

                // 發布事件
                EventBus.Publish(new GameLoadedEvent { SlotIndex = slotIndex });

                Debug.Log($"[SaveService] Game loaded from slot {slotIndex}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveService] Load failed: {e.Message}");
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
                    Debug.Log($"[SaveService] Deleted save slot {slotIndex}");
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveService] Delete failed: {e.Message}");
                return false;
            }
        }

        public bool SaveExists(int slotIndex)
        {
            return File.Exists(GetSaveFilePath(slotIndex));
        }

        public bool AutoSave()
        {
            return SaveGame(999); // 特殊槽位用於自動存檔
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
                        string json = File.ReadAllText(GetSaveFilePath(i));
                        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

                        slots[i].saveName = data.saveName;
                        slots[i].level = data.level;
                        slots[i].saveTimestamp = data.saveTimestamp;
                        slots[i].playTimeSeconds = data.playTimeSeconds;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[SaveService] Failed to read slot {i}: {e.Message}");
                    }
                }
            }

            return slots;
        }

        private string GetSaveFilePath(int slotIndex)
        {
            return saveDirectory + $"save_{slotIndex}" + SaveFileExtension;
        }

        private PlayerSaveData CollectSaveData()
        {
            // TODO: 從各個系統收集數據
            // 這裡需要與 PlayerLevel, InventoryService 等系統整合

            return new PlayerSaveData
            {
                saveName = "新遊戲",
                level = 1,
                experience = 0,
                playTimeSeconds = 0
            };
        }

        private void ApplySaveData(PlayerSaveData saveData)
        {
            // TODO: 套用存檔數據到各個系統
            Debug.Log($"[SaveService] Applying save data: Level {saveData.level}");
        }
    }

    [Serializable]
    public class PlayerSaveData
    {
        public string saveName;
        public long saveTimestamp;
        public int playTimeSeconds;
        public int level;
        public int experience;
    }
}
