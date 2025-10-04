using Stats;
using Element;
using Item;

namespace Core
{
    // ===== 戰鬥事件 =====
    public struct DamageDealtEvent : IGameEvent
    {
        public object Attacker;
        public object Target;
        public float Damage;
        public bool IsCritical;
        public ElementType Element;
    }

    public struct EntityDeathEvent : IGameEvent
    {
        public object Entity;
    }

    public struct HealingEvent : IGameEvent
    {
        public object Target;
        public float Amount;
    }

    // ===== 屬性事件 =====
    public struct StatChangedEvent : IGameEvent
    {
        public object Entity;
        public StatType StatType;
        public float OldValue;
        public float NewValue;
    }

    public struct LevelUpEvent : IGameEvent
    {
        public object Entity;
        public int NewLevel;
        public int SkillPoints;
        public int TalentPoints;
        public int AttributePoints;
    }

    // ===== 技能事件 =====
    public struct SkillLearnedEvent : IGameEvent
    {
        public string SkillId;
        public int Level;
    }

    public struct SkillCastEvent : IGameEvent
    {
        public object Caster;
        public string SkillId;
        public object Target;
    }

    public struct TalentLearnedEvent : IGameEvent
    {
        public string TalentId;
        public int Level;
    }

    // ===== 物品事件 =====
    public struct ItemAddedEvent : IGameEvent
    {
        public ItemType ItemType;
        public string ItemId;
        public int Quantity;
    }

    public struct ItemRemovedEvent : IGameEvent
    {
        public ItemType ItemType;
        public string ItemId;
        public int Quantity;
    }

    public struct EquipmentEquippedEvent : IGameEvent
    {
        public string EquipmentId;
        public EquipmentSlot Slot;
    }

    public struct EquipmentUnequippedEvent : IGameEvent
    {
        public string EquipmentId;
        public EquipmentSlot Slot;
    }

    // ===== 眷屬事件 =====
    public struct CompanionDeployedEvent : IGameEvent
    {
        public string CompanionId;
    }

    public struct CompanionRecalledEvent : IGameEvent
    {
        public string CompanionId;
    }

    public struct CompanionDiedEvent : IGameEvent
    {
        public string CompanionId;
        public float ReviveTime;
    }

    public struct CompanionRevivedEvent : IGameEvent
    {
        public string CompanionId;
    }

    public struct CompanionCapturedEvent : IGameEvent
    {
        public string CompanionId;
        public bool Success;
    }

    // ===== 系統事件 =====
    public struct GameSavedEvent : IGameEvent
    {
        public int SlotIndex;
    }

    public struct GameLoadedEvent : IGameEvent
    {
        public int SlotIndex;
    }
}
