using UnityEngine;
using Core;

namespace AI
{
    /// <summary>
    /// 閒置狀態
    /// </summary>
    public class IdleState : State<EnemyController>
    {
        private float idleTimer;
        private const float maxIdleTime = 2f;

        public override void Enter(EnemyController context)
        {
            base.Enter(context);
            idleTimer = 0f;
        }

        public override void Update(float deltaTime)
        {
            idleTimer += deltaTime;

            // 偵測玩家
            if (context.DetectPlayer())
            {
                context.ChangeState(new ChaseState());
                return;
            }

            // 隨機巡邏
            if (idleTimer >= maxIdleTime)
            {
                context.ChangeState(new PatrolState());
            }
        }
    }

    /// <summary>
    /// 巡邏狀態
    /// </summary>
    public class PatrolState : State<EnemyController>
    {
        private float patrolTimer;
        private const float maxPatrolTime = 5f;

        public override void Enter(EnemyController context)
        {
            base.Enter(context);
            patrolTimer = 0f;
            context.SetRandomPatrolPoint();
        }

        public override void Update(float deltaTime)
        {
            patrolTimer += deltaTime;

            // 偵測玩家
            if (context.DetectPlayer())
            {
                context.ChangeState(new ChaseState());
                return;
            }

            // 移動到巡邏點
            context.MoveToTarget();

            // 到達巡邏點或超時
            if (context.ReachedTarget() || patrolTimer >= maxPatrolTime)
            {
                context.ChangeState(new IdleState());
            }
        }
    }

    /// <summary>
    /// 追擊狀態
    /// </summary>
    public class ChaseState : State<EnemyController>
    {
        public override void Enter(EnemyController context)
        {
            base.Enter(context);
        }

        public override void Update(float deltaTime)
        {
            // 失去目標
            if (!context.DetectPlayer())
            {
                context.ChangeState(new IdleState());
                return;
            }

            // 進入攻擊範圍
            if (context.InAttackRange())
            {
                context.ChangeState(new AttackState());
                return;
            }

            // 追擊玩家
            context.ChasePlayer();
        }
    }

    /// <summary>
    /// 攻擊狀態
    /// </summary>
    public class AttackState : State<EnemyController>
    {
        private float attackCooldown;
        private const float attackInterval = 1.5f;

        public override void Enter(EnemyController context)
        {
            base.Enter(context);
            attackCooldown = 0f;
        }

        public override void Update(float deltaTime)
        {
            attackCooldown -= deltaTime;

            // 失去目標
            if (!context.DetectPlayer())
            {
                context.ChangeState(new IdleState());
                return;
            }

            // 目標超出範圍
            if (!context.InAttackRange())
            {
                context.ChangeState(new ChaseState());
                return;
            }

            // 執行攻擊
            if (attackCooldown <= 0f)
            {
                context.Attack();
                attackCooldown = attackInterval;
            }
        }
    }

    /// <summary>
    /// 死亡狀態
    /// </summary>
    public class DeadState : State<EnemyController>
    {
        public override void Enter(EnemyController context)
        {
            base.Enter(context);
            context.OnDeath();
        }

        public override void Update(float deltaTime)
        {
            // 死亡狀態不執行任何操作
        }
    }
}
