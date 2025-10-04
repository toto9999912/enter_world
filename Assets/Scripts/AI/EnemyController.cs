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
        [Header("引用")]
        public CharacterStats stats;
        public HealthSystem healthSystem;
        public CombatSystem combatSystem;

        [Header("AI 配置")]
        public float detectionRange = 10f;
        public float attackRange = 2f;
        public float moveSpeed = 3f;

        [Header("眷屬")]
        public CompanionManager companionManager;

        // 狀態機
        private StateMachine<EnemyController> stateMachine;

        // 目標
        private Transform player;
        private Vector3 patrolTarget;

        private void Start()
        {
            // 初始化狀態機
            stateMachine = new StateMachine<EnemyController>(this);
            stateMachine.ChangeState(new IdleState());

            // 訂閱死亡事件
            if (healthSystem != null)
            {
                healthSystem.OnDeath += HandleDeath;
            }
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

            // 取得目標組件
            var targetHealth = player.GetComponent<HealthSystem>();
            var targetStats = player.GetComponent<CharacterStats>();
            var targetCombat = player.GetComponent<CombatSystem>();

            if (targetHealth != null && targetStats != null && combatSystem != null)
            {
                combatSystem.Attack(targetHealth, targetStats, targetCombat);
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
