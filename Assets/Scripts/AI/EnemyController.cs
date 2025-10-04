using UnityEngine;
using Core;
using Stats;
using Combat;
using Companion;

namespace AI
{
    /// <summary>
    /// 敵人控制器 (重構版，使用狀態機)
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        [Header("引用 (自動初始化,無需手動設定)")]
        public CharacterStats stats;
        public HealthSystem healthSystem;
        public CombatSystem combatSystem;

        [Header("AI 配置")]
        public float detectionRange = 10f;
        public float attackRange = 2f;
        public float moveSpeed = 3f;
        public float attackCooldown = 2f; // 攻擊冷卻時間

        [Header("敵人屬性")]
        public Element.ElementType mainElement = Element.ElementType.Fire;
        public float maxHP = 500f;
        public float maxMP = 200f;
        public float attackPower = 30f;
        public float magicPower = 40f;
        public float defense = 20f;

        [Header("眷屬 (可選)")]
        public CompanionManager companionManager;

        // 狀態機
        private StateMachine<EnemyController> stateMachine;

        // 目標
        private Transform player;
        private Vector3 patrolTarget;

        // 攻擊計時器
        private float lastAttackTime;

        private void Start()
        {
            InitializeEnemy();
            InitializeStateMachine();
        }

        /// <summary>
        /// 初始化敵人屬性
        /// </summary>
        private void InitializeEnemy()
        {
            // 初始化屬性
            stats = new CharacterStats();
            stats.SetBaseValue(Stats.StatType.HP, maxHP);
            stats.SetBaseValue(Stats.StatType.MP, maxMP);
            stats.SetBaseValue(Stats.StatType.ATK, attackPower);
            stats.SetBaseValue(Stats.StatType.Magic, magicPower);
            stats.SetBaseValue(Stats.StatType.DEF, defense);
            stats.SetBaseValue(Stats.StatType.SPD, 80f);
            stats.SetBaseValue(Stats.StatType.CritRate, 15f);
            stats.SetBaseValue(Stats.StatType.CritDamage, 130f);
            stats.SetBaseValue(Stats.StatType.HitRate, 85f);

            // 初始化生命系統
            healthSystem = new HealthSystem(stats, true);

            // 初始化戰鬥系統
            combatSystem = new CombatSystem(stats, healthSystem);
            combatSystem.Element = mainElement;

            // 訂閱死亡事件
            healthSystem.OnDeath += HandleDeath;

            Debug.Log($"[EnemyController] {gameObject.name} 初始化完成 - HP: {maxHP}, ATK: {attackPower}");
        }

        /// <summary>
        /// 初始化狀態機
        /// </summary>
        private void InitializeStateMachine()
        {
            stateMachine = new StateMachine<EnemyController>(this);
            stateMachine.ChangeState(new IdleState());
        }

        private void Update()
        {
            stateMachine.Update(Time.deltaTime);
        }

        private void OnDestroy()
        {
            if (healthSystem != null)
            {
                healthSystem.OnDeath -= HandleDeath;
            }
        }

        // ===== 狀態機介面 =====

        public void ChangeState(State<EnemyController> newState)
        {
            stateMachine.ChangeState(newState);
        }

        public bool DetectPlayer()
        {
            if (player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                    player = playerObj.transform;
            }

            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.position);
                return distance <= detectionRange;
            }

            return false;
        }

        public bool InAttackRange()
        {
            if (player == null) return false;
            float distance = Vector3.Distance(transform.position, player.position);
            return distance <= attackRange;
        }

        public void SetRandomPatrolPoint()
        {
            Vector2 randomDir = Random.insideUnitCircle * 5f;
            patrolTarget = transform.position + new Vector3(randomDir.x, 0, randomDir.y);
        }

        public void MoveToTarget()
        {
            Vector3 direction = (patrolTarget - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        public bool ReachedTarget()
        {
            return Vector3.Distance(transform.position, patrolTarget) < 0.5f;
        }

        public void ChasePlayer()
        {
            if (player == null) return;

            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        public void Attack()
        {
            if (player == null) return;

            // 檢查攻擊冷卻
            if (Time.time - lastAttackTime < attackCooldown)
                return;

            // 取得目標 PlayerController
            var playerController = player.GetComponent<Player.PlayerController>();
            if (playerController == null)
            {
                Debug.LogWarning("[EnemyController] 找不到 PlayerController!");
                return;
            }

            // 執行攻擊
            if (combatSystem != null && playerController.healthSystem != null)
            {
                combatSystem.Attack(
                    playerController.healthSystem,
                    playerController.stats,
                    playerController.combatSystem,
                    damageMultiplier: 1.0f,
                    Combat.DamageType.Physical,
                    "敵人普攻"
                );

                lastAttackTime = Time.time;
                Debug.Log($"[{gameObject.name}] 攻擊玩家!");
            }
        }

        public void OnDeath()
        {
            // 召回所有眷屬
            if (companionManager != null)
            {
                companionManager.RecallAllCompanions();
            }

            // 發布死亡事件
            EventBus.Publish(new EntityDeathEvent { Entity = this });

            // 銷毀物件 (可改為物件池回收)
            Destroy(gameObject, 2f);
        }

        private void HandleDeath()
        {
            stateMachine.ChangeState(new DeadState());
        }

        // ===== Debug =====
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
