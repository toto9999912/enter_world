using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 玩家移動控制器 - 處理 top-down 2D 遊戲的 WASD 移動
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField, Tooltip("角色的水平與垂直移動速度 (單位/秒)")]
    private float moveSpeed = 5f;
    
    [Header("動畫設定")]
    [SerializeField, Tooltip("判定為移動狀態的最小速度閾值")]
    private float movementThreshold = 0.1f;
    
    [Header("角色翻轉設定")]
    [SerializeField, Tooltip("是否啟用角色水平翻轉")]
    private bool enableFlipping = true;
    [SerializeField, Tooltip("角色預設面向右邊（true為右，false為左）")]
    private bool facingRight = true;

    private PlayerInputActions inputActions;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private float currentSpeed;

    private void Awake()
    {
        // 初始化輸入系統
        inputActions = new PlayerInputActions();
        
        // 取得必要元件
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // 設定 Rigidbody2D 為 2D top-down 模式
        rb.gravityScale = 0f;
    }

    private void OnEnable()
    {
        // 啟用玩家輸入
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        // 移除輸入事件監聽
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.Disable();

        // 重置移動狀態
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        currentSpeed = 0f;
        
        // 重置動畫狀態為靜止
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    private void FixedUpdate()
    {
        // 應用移動速度到剛體
        rb.linearVelocity = moveInput * moveSpeed;
        
        // 計算當前速度大小
        currentSpeed = rb.linearVelocity.magnitude;
        
        // 處理角色翻轉
        HandleCharacterFlipping();
        
        // 更新動畫器的速度參數
        if (animator != null)
        {
            animator.SetFloat("Speed", currentSpeed);
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        // 讀取輸入值並正規化（避免對角移動過快）
        Vector2 rawInput = context.ReadValue<Vector2>();
        moveInput = rawInput.sqrMagnitude > 1f ? rawInput.normalized : rawInput;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        // 停止移動輸入
        moveInput = Vector2.zero;
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }

    /// <summary>
    /// 取得當前移動速度（供外部腳本查詢使用）
    /// </summary>
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    
    /// <summary>
    /// 檢查角色是否正在移動
    /// </summary>
    public bool IsMoving()
    {
        return currentSpeed > movementThreshold;
    }
    
    /// <summary>
    /// 處理角色的水平翻轉
    /// </summary>
    private void HandleCharacterFlipping()
    {
        // 如果未啟用翻轉功能或沒有水平移動輸入，則跳過
        if (!enableFlipping || Mathf.Abs(moveInput.x) < 0.01f)
            return;
        
        // 判斷是否需要翻轉
        bool shouldFaceRight = moveInput.x > 0f;
        
        // 如果當前面向與應該面向的方向不同，則進行翻轉
        if (shouldFaceRight != facingRight)
        {
            FlipCharacter();
        }
    }
    
    /// <summary>
    /// 翻轉角色（使用 Scale 的方式）
    /// </summary>
    private void FlipCharacter()
    {
        // 更新面向狀態
        facingRight = !facingRight;
        
        // 透過改變 X 軸的縮放來翻轉角色
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }
    
    /// <summary>
    /// 取得角色當前面向方向（供外部腳本查詢使用）
    /// </summary>
    public bool IsFacingRight()
    {
        return facingRight;
    }
    
    /// <summary>
    /// 手動設定角色面向方向
    /// </summary>
    /// <param name="faceRight">true為面向右邊，false為面向左邊</param>
    public void SetFacingDirection(bool faceRight)
    {
        if (faceRight != facingRight)
        {
            FlipCharacter();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 確保移動速度不為負值
        if (moveSpeed < 0f)
        {
            moveSpeed = 0f;
        }
        
        // 確保移動閾值不為負值
        if (movementThreshold < 0f)
        {
            movementThreshold = 0f;
        }
        
        // 如果角色預設不是面向右邊，確保初始縮放正確
        if (Application.isPlaying && !facingRight && transform.localScale.x > 0f)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }
#endif
}
