using Core;

namespace Services
{
    public interface ISaveService : IService
    {
        bool SaveGame(int slotIndex);
        bool LoadGame(int slotIndex);
        bool DeleteSave(int slotIndex);
        bool SaveExists(int slotIndex);
        bool AutoSave();
        SaveSlotInfo[] GetAllSaveSlots();
    }

    [System.Serializable]
    public class SaveSlotInfo
    {
        public int slotIndex;
        public bool exists;
        public string saveName;
        public int level;
        public long saveTimestamp;
        public int playTimeSeconds;
    }
}
