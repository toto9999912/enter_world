using System;
using System.Collections.Generic;
using UnityEngine;
using Stats;
using Companion;

namespace Services
{
    /// <summary>
    /// 完整的存檔資料結構
    /// </summary>
    [Serializable]
    public class CompleteSaveData
    {
        public int version = 1;
        public int slotIndex;
        public string saveName = "新遊戲";
        public long saveTimestamp;
        
        public PlayerData playerData;
        public StatsData statsData;
        public HealthData healthData;
        public InventoryData inventoryData;
        public SkillData skillData;
        public CompanionSaveData companionData;
        public ProgressData progressData;
    }

    [Serializable]
    public class PlayerData
    {
        public int level = 1;
        public int experience;
        public Vector3 position;
        public Vector3 rotation;
        public string currentScene;
    }

    [Serializable]
    public class StatsData
    {
        public List<StatValue> baseValues = new List<StatValue>();
        public List<ModifierData> modifiers = new List<ModifierData>();
    }

    [Serializable]
    public class StatValue
    {
        public StatType type;
        public float value;
    }

    [Serializable]
    public class ModifierData
    {
        public StatType statType;
        public ModifierType modifierType;
        public float value;
        public string source;
    }

    [Serializable]
    public class HealthData
    {
        public float currentHP;
        public float currentMP;
    }

    [Serializable]
    public class InventoryData
    {
        public int capacity;
        public List<ItemSaveData> items = new List<ItemSaveData>();
    }

    [Serializable]
    public class ItemSaveData
    {
        public string itemId;
        public int quantity;
        public int slotIndex;
    }

    [Serializable]
    public class SkillData
    {
        public int availableSkillPoints;
        public int availableTalentPoints;
        public List<LearnedSkill> learnedSkills = new List<LearnedSkill>();
        public List<LearnedTalent> learnedTalents = new List<LearnedTalent>();
    }

    [Serializable]
    public class LearnedSkill
    {
        public string skillId;
        public int level;
        public float cooldownRemaining;
    }

    [Serializable]
    public class LearnedTalent
    {
        public string talentId;
        public int level;
    }

    [Serializable]
    public class CompanionSaveData
    {
        public List<CompanionInstanceData> companions = new List<CompanionInstanceData>();
        public List<string> deployedCompanionIds = new List<string>();
    }

    [Serializable]
    public class CompanionInstanceData
    {
        public string companionId;
        public string instanceId;
        public int level;
        public int experience;
        public CompanionState state;
        public float currentHP;
        public float currentMP;
        public long reviveTimestamp;
    }

    [Serializable]
    public class ProgressData
    {
        public int playTimeSeconds;
        public List<string> completedQuests = new List<string>();
        public List<string> unlockedAchievements = new List<string>();
    }
}
