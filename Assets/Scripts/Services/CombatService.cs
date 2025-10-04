using UnityEngine;
using Core;
using Combat;
using Element;
using Stats;

namespace Services
{
    public class CombatService : ICombatService
    {
        public void Initialize()
        {
            Debug.Log("[CombatService] Initialized");
        }

        public void Shutdown()
        {
            Debug.Log("[CombatService] Shutdown");
        }

        public float CalculateDamage(
            CharacterStats attacker,
            CharacterStats defender,
            float baseDamage,
            DamageType damageType,
            ElementType attackerElement,
            ElementType defenderElement)
        {
            float finalDamage = baseDamage;

            // 1. 元素加成 (僅魔法傷害)
            if (damageType == DamageType.Magical)
            {
                float elementalMultiplier = ElementalAffinityTable.GetAffinityMultiplier(attackerElement, defenderElement);
                finalDamage *= elementalMultiplier;
            }

            // 2. 防禦減免
            float defense = damageType == DamageType.Physical
                ? defender.GetFinalValue(StatType.DEF)
                : defender.GetFinalValue(StatType.Magic);

            float defenseMultiplier = 100f / (100f + defense);
            finalDamage *= defenseMultiplier;

            return Mathf.Max(1f, finalDamage);
        }

        public bool RollHit(float attackerHitRate, float defenderEvasion)
        {
            float hitChance = Mathf.Clamp(attackerHitRate - defenderEvasion, 5f, 95f);
            return Random.Range(0f, 100f) <= hitChance;
        }

        public bool RollCritical(float critRate)
        {
            return Random.Range(0f, 100f) <= critRate;
        }

        public bool RollDodge(float evasion)
        {
            return Random.Range(0f, 100f) <= evasion;
        }
    }
}
