using UnityEngine;
using UnityEngine.InputSystem;
using Stats;
using Combat;
using Element;
using Core;

namespace Player
{
    /// <summary>
    /// 玩家控制器
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("屬性系統")]
        public CharacterStats stats;
        public HealthSystem healthSystem;
        public CombatSystem combatSystem;

        [Header("元素屬性")]
        public ElementType mainElement = ElementType.Fire;

        [Header("移動")]
        public float moveSpeed = 5f;

        [Header("動畫")]
        public Animator animator;

        // Input System
        private PlayerInputActions inputActions;
        private Vector2 moveInput;
        private float currentSpeed;

        private void Awake()
        {
            // 初始化 Input System
            inputActions = new PlayerInputActions();

            // 自動獲取 Animator (如果沒有手動設定)
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        private void OnEnable()
        {
            inputActions.Player.Enable();

            // 訂閱輸入事件
            inputActions.Player.Move.performed += OnMove;
            inputActions.Player.Move.canceled += OnMove;
            inputActions.Player.Attack.performed += OnAttack;
        }

        private void OnDisable()
        {
            // 取消訂閱輸入事件
            inputActions.Player.Move.performed -= OnMove;
            inputActions.Player.Move.canceled -= OnMove;
            inputActions.Player.Attack.performed -= OnAttack;

            inputActions.Player.Disable();
        }

        private void Start()
        {
            InitializePlayer();
        }

        private void InitializePlayer()
        {
            // 初始化屬性
            stats = new CharacterStats();
            stats.SetBaseValue(StatType.HP, 1000f);
            stats.SetBaseValue(StatType.MP, 500f);
            stats.SetBaseValue(StatType.ATK, 50f);
            stats.SetBaseValue(StatType.Magic, 60f);
            stats.SetBaseValue(StatType.DEF, 30f);
            stats.SetBaseValue(StatType.SPD, 100f);
            stats.SetBaseValue(StatType.CritRate, 25f);
            stats.SetBaseValue(StatType.CritDamage, 150f);
            stats.SetBaseValue(StatType.HitRate, 90f);

            // 初始化生命系統
            healthSystem = new HealthSystem(stats, true);

            // 初始化戰鬥系統
            combatSystem = new CombatSystem(stats, healthSystem);
            combatSystem.Element = mainElement;

            // 訂閱事件
            healthSystem.OnDeath += OnPlayerDeath;

            Debug.Log("[PlayerController] Player initialized");
        }

        private void Update()
        {
            HandleMovement();
        }

        // ===== Input System 回調 =====

        private void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            AttackNearestEnemy();
        }

        // ===== 移動處理 =====

        private void HandleMovement()
        {
            // 計算移動方向 (只使用 WASD 輸入)
            Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            
            // 應用移動
            transform.position += movement * moveSpeed * Time.deltaTime;

            // 計算當前速度 (用於動畫)
            currentSpeed = movement.magnitude;

            // 更新 Animator 參數
            if (animator != null)
            {
                animator.SetFloat("Speed", currentSpeed);
            }

            // 調試輸出
            if (currentSpeed > 0.1f)
            {
                Debug.Log($"[PlayerController] Moving - Speed: {currentSpeed:F2}");
            }
        }

        private void HandleCombat()
        {
            // 按下 H 鍵自我治療 (保留鍵盤快捷鍵)
            if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
            {
                healthSystem.Heal(100f);
                Debug.Log($"[Player] Healed! HP: {healthSystem.CurrentHP}/{healthSystem.MaxHP}");
            }
        }

        private void AttackNearestEnemy()
        {
            // 尋找最近的敵人
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0) return;

            GameObject nearestEnemy = null;
            float nearestDistance = float.MaxValue;

            foreach (var enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null && nearestDistance <= 3f)
            {
                var enemyController = nearestEnemy.GetComponent<AI.EnemyController>();
                if (enemyController != null)
                {
                    combatSystem.Attack(
                        enemyController.healthSystem,
                        enemyController.stats,
                        enemyController.combatSystem,
                        damageMultiplier: 1.5f,
                        DamageType.Physical,
                        "玩家普攻"
                    );
                }
            }
        }

        private void OnPlayerDeath()
        {
            Debug.Log("[PlayerController] Player died!");
            // TODO: 顯示死亡畫面
        }

        private void OnDestroy()
        {
            if (healthSystem != null)
            {
                healthSystem.OnDeath -= OnPlayerDeath;
            }
        }
    }
}
