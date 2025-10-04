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

        [Header("顯示")]
        public SpriteRenderer spriteRenderer;

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

            // 自動獲取 SpriteRenderer (如果沒有手動設定)
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        private void OnEnable()
        {
            inputActions.Player.Enable();

            // 訂閱輸入事件
            inputActions.Player.Move.performed += OnMove;
            inputActions.Player.Move.canceled += OnMove;
            // inputActions.Player.Attack.performed += OnAttack; // 暫時註解,先專注移動
        }

        private void OnDisable()
        {
            // 取消訂閱輸入事件
            inputActions.Player.Move.performed -= OnMove;
            inputActions.Player.Move.canceled -= OnMove;
            // inputActions.Player.Attack.performed -= OnAttack; // 暫時註解

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

            Debug.Log("[PlayerController] Player initialized with combat system");
        }

        private void Update()
        {
            HandleMovement();
            HandleTestKeys(); // HUD 測試按鍵
        }

        // ===== HUD 測試按鍵 =====
        
        private void HandleTestKeys()
        {
            if (healthSystem == null) return;

            // H 鍵: 受到 100 傷害
            if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
            {
                healthSystem.TakeDamage(100f);
                Debug.Log($"[Player] 受到傷害! HP: {healthSystem.CurrentHP}/{healthSystem.MaxHP}");
            }

            // J 鍵: 治療 100 HP
            if (Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame)
            {
                healthSystem.Heal(100f);
                Debug.Log($"[Player] 治療! HP: {healthSystem.CurrentHP}/{healthSystem.MaxHP}");
            }

            // K 鍵: 消耗 50 MP
            if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
            {
                bool success = healthSystem.ConsumeMana(50f);
                if (success)
                    Debug.Log($"[Player] 使用魔力! MP: {healthSystem.CurrentMP}/{healthSystem.MaxMP}");
                else
                    Debug.Log($"[Player] 魔力不足! MP: {healthSystem.CurrentMP}/{healthSystem.MaxMP}");
            }

            // L 鍵: 恢復 50 MP
            if (Keyboard.current != null && Keyboard.current.lKey.wasPressedThisFrame)
            {
                healthSystem.RestoreMana(50f);
                Debug.Log($"[Player] 恢復魔力! MP: {healthSystem.CurrentMP}/{healthSystem.MaxMP}");
            }
        }

        // ===== Input System 回調 =====

        private void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        // 暫時註解攻擊功能
        /*
        private void OnAttack(InputAction.CallbackContext context)
        {
            AttackNearestEnemy();
        }
        */

        // ===== 移動處理 =====

        private void HandleMovement()
        {
            // 計算移動方向 (2D Top-Down: X=左右, Y=上下)
            Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0).normalized;
            
            // 應用移動
            transform.position += movement * moveSpeed * Time.deltaTime;

            // 計算當前速度 (用於動畫)
            currentSpeed = movement.magnitude;

            // 更新角色朝向 (根據水平移動方向)
            if (spriteRenderer != null && Mathf.Abs(moveInput.x) > 0.1f)
            {
                // 向右移動時不翻轉,向左移動時翻轉
                spriteRenderer.flipX = moveInput.x < 0;
            }

            // 更新 Animator 參數
            if (animator != null)
            {
                animator.SetFloat("Speed", currentSpeed);
            }

            // 調試輸出
            if (currentSpeed > 0.1f)
            {
                Debug.Log($"[PlayerController] Moving - Speed: {currentSpeed:F2}, Direction: ({moveInput.x:F2}, {moveInput.y:F2})");
            }
        }

        // ===== 暫時註解戰鬥相關功能,先專注移動測試 =====
        
        /*
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
        */
    }
}
