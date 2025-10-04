using System;
using UnityEngine;
using Stats;
using Combat;
using Core;

namespace Skill
{
    /// <summary>
    /// 技能施放結果
    /// </summary>
    public class SkillCastResult
    {
        public bool success;
        public string failureReason;
        public DamageInfo[] damageInfos; // 可能命中多個目標
        public GameObject[] hitTargets;
    }

    /// <summary>
    /// 技能施放系統
    /// 處理技能施放、效果計算、冷卻管理
    /// </summary>
    public class SkillCastSystem
    {
        // ===== 引用 =====
        private CharacterStats casterStats;
        private HealthSystem casterHealth;
        private CombatSystem casterCombat;
        private SkillTree skillTree;

        // ===== 事件 =====
        public event Action<SkillInstance, SkillCastResult> OnSkillCast;
        public event Action<SkillInstance> OnSkillCastFailed;

        /// <summary>
        /// 建構子
        /// </summary>
        public SkillCastSystem(
            CharacterStats stats,
            HealthSystem health,
            CombatSystem combat,
            SkillTree skillTree)
        {
            this.casterStats = stats;
            this.casterHealth = health;
            this.casterCombat = combat;
            this.skillTree = skillTree;
        }

        /// <summary>
        /// 施放技能
        /// </summary>
        public SkillCastResult CastSkill(string skillId, GameObject target = null, Vector3? targetPosition = null)
        {
            SkillCastResult result = new SkillCastResult();

            // 1. 取得技能
            SkillInstance skill = skillTree.GetSkill(skillId);
            if (skill == null)
            {
                result.success = false;
                result.failureReason = "技能未學習！";
                OnSkillCastFailed?.Invoke(skill);
                return result;
            }

            // 2. 檢查冷卻
            if (skill.IsOnCooldown)
            {
                result.success = false;
                result.failureReason = $"冷卻中 ({skill.CooldownRemaining:F1} 秒)";
                OnSkillCastFailed?.Invoke(skill);
                return result;
            }

            // 3. 檢查魔力
            float manaCost = skill.GetManaCost();
            if (!casterHealth.HasEnoughMana(manaCost))
            {
                result.success = false;
                result.failureReason = $"魔力不足 (需要 {manaCost:F0})";
                OnSkillCastFailed?.Invoke(skill);
                return result;
            }

            // 4. 檢查目標
            if (!ValidateTarget(skill.Data, target, targetPosition))
            {
                result.success = false;
                result.failureReason = "目標無效！";
                OnSkillCastFailed?.Invoke(skill);
                return result;
            }

            // 5. 消耗魔力
            casterHealth.ConsumeMana(manaCost);

            // 6. 施放效果
            result = ExecuteSkillEffect(skill, target, targetPosition);
            result.success = true;

            // 7. 開始冷卻
            float skillHaste = casterStats.GetFinalValue(StatType.SkillHaste);
            skill.StartCooldown(skillHaste);

            // 觸發本地事件
            OnSkillCast?.Invoke(skill, result);

            // 發布全域事件
            EventBus.Publish(new SkillCastEvent
            {
                Caster = casterCombat,
                SkillId = skillId,
                Target = target
            });

            Debug.Log($"[SkillCast] 施放技能: {skill.Data.skillName}");
            return result;
        }

        /// <summary>
        /// 驗證目標有效性
        /// </summary>
        private bool ValidateTarget(SkillData skillData, GameObject target, Vector3? targetPosition)
        {
            switch (skillData.targetType)
            {
                case SkillTargetType.Self:
                    return true;

                case SkillTargetType.SingleEnemy:
                case SkillTargetType.SingleAlly:
                    return target != null;

                case SkillTargetType.Ground:
                    return targetPosition.HasValue;

                case SkillTargetType.AllAllies:
                case SkillTargetType.AllEnemies:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// 執行技能效果
        /// </summary>
        private SkillCastResult ExecuteSkillEffect(SkillInstance skill, GameObject target, Vector3? targetPosition)
        {
            SkillCastResult result = new SkillCastResult();

            // 根據技能類型執行不同效果
            if (skill.Data.damageType != DamageType.True && !skill.Data.healsTarget)
            {
                // 傷害技能
                result.damageInfos = ExecuteDamageEffect(skill, target);
            }
            else if (skill.Data.healsTarget)
            {
                // 治療技能
                ExecuteHealEffect(skill, target);
            }

            // 控場效果
            if (skill.Data.appliesCrowdControl && target != null)
            {
                ExecuteCrowdControlEffect(skill, target);
            }

            // Buff 效果
            if (skill.Data.appliesBuff)
            {
                ExecuteBuffEffect(skill, target);
            }

            return result;
        }

        /// <summary>
        /// 執行傷害效果
        /// </summary>
        private DamageInfo[] ExecuteDamageEffect(SkillInstance skill, GameObject target)
        {
            if (target == null)
                return new DamageInfo[0];

            // 取得目標系統
            // 注意: 這裡需要你的遊戲物件有對應的組件
            // 這只是範例，實際需要根據你的架構調整

            var targetHealth = target.GetComponent<HealthSystem>();
            var targetStats = target.GetComponent<CharacterStats>();
            var targetCombat = target.GetComponent<CombatSystem>();

            if (targetHealth == null || targetStats == null)
            {
                Debug.LogWarning("[SkillCast] 目標缺少必要組件！");
                return new DamageInfo[0];
            }

            // 計算傷害
            float damageMultiplier = skill.GetDamageMultiplier();
            DamageInfo damageInfo = casterCombat.Attack(
                targetHealth,
                targetStats,
                targetCombat,
                damageMultiplier,
                skill.Data.damageType,
                skill.Data.skillName
            );

            return new DamageInfo[] { damageInfo };
        }

        /// <summary>
        /// 執行治療效果
        /// </summary>
        private void ExecuteHealEffect(SkillInstance skill, GameObject target)
        {
            if (target == null)
                return;

            var targetHealth = target.GetComponent<HealthSystem>();
            if (targetHealth == null)
                return;

            // 計算治療量
            float magicPower = casterStats.GetFinalValue(StatType.Magic);
            float healAmount = (magicPower * skill.Data.healMultiplier) + skill.Data.flatHeal;

            targetHealth.Heal(healAmount);
            Debug.Log($"[SkillCast] 治療 {healAmount:F0} HP");
        }

        /// <summary>
        /// 執行控場效果
        /// </summary>
        private void ExecuteCrowdControlEffect(SkillInstance skill, GameObject target)
        {
            var targetArmor = target.GetComponent<SuperArmor>();
            if (targetArmor == null)
            {
                Debug.LogWarning("[SkillCast] 目標沒有 SuperArmor 組件，無法施加控場！");
                return;
            }

            var targetStats = target.GetComponent<CharacterStats>();
            float toughness = targetStats != null ? targetStats.GetFinalValue(StatType.Toughness) : 0f;

            var ccEffect = new CrowdControlEffect(
                skill.Data.crowdControlType,
                skill.Data.crowdControlDuration,
                intensity: 1.0f,
                source: skill.Data.skillName
            );

            // 先嘗試消耗霸體
            if (targetArmor.HasSuperArmor)
            {
                targetArmor.TakeCrowdControlDamage(skill.Data.superArmorDamage);
            }
            else
            {
                // 套用控場
                targetArmor.TryApplyCrowdControl(
                    ccEffect,
                    toughness,
                    skill.Data.crowdControlHitRate
                );
            }
        }

        /// <summary>
        /// 執行 Buff 效果
        /// </summary>
        private void ExecuteBuffEffect(SkillInstance skill, GameObject target)
        {
            if (target == null || skill.Data.buffModifiers == null)
                return;

            var targetStats = target.GetComponent<CharacterStats>();
            if (targetStats == null)
                return;

            // 添加 Buff 修飾器
            foreach (var modData in skill.Data.buffModifiers)
            {
                var modifier = new StatModifier(
                    modData.statType,
                    modData.modifierType,
                    modData.value,
                    skill.Data.skillId + "_buff"
                );

                targetStats.AddModifier(modifier);
            }

            // TODO: 需要實作 Buff 計時系統，在持續時間後移除
            Debug.Log($"[SkillCast] 施加 Buff: {skill.Data.skillName} ({skill.Data.buffDuration} 秒)");
        }

        /// <summary>
        /// 強制中斷施法 (被控場時)
        /// </summary>
        public void InterruptCast()
        {
            // TODO: 如果正在施放 Channeled 技能，中斷施法
            Debug.Log("[SkillCast] 施法被中斷！");
        }
    }
}
