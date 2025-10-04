using Core;
using Combat;
using Element;
using Stats;

namespace Services
{
    public interface ICombatService : IService
    {
        float CalculateDamage(
            CharacterStats attacker,
            CharacterStats defender,
            float baseDamage,
            DamageType damageType,
            ElementType attackerElement,
            ElementType defenderElement
        );

        bool RollHit(float attackerHitRate, float defenderEvasion);
        bool RollCritical(float critRate);
        bool RollDodge(float evasion);
    }
}
