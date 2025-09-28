using UnityEngine;

/// <summary>
/// 玩家移動控制器 - 處理玩家的上下左右移動功能
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 5f; // 移動速度

    // 輸入系統相關變數
    private PlayerInputActions playerInputActions; // 玩家輸入動作系統
    private Vector2 moveInput; // 移動輸入向量 (x: 左右, y: 上下)

    /// <summary>
    /// 在物件喚醒時初始化輸入系統
    /// </summary>
    private void Awake()
    {
        // 建立新的玩家輸入動作實例
        playerInputActions = new PlayerInputActions();

        // 綁定移動輸入事件 - 當玩家按下移動鍵時觸發
        playerInputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        // 綁定移動輸入取消事件 - 當玩家放開移動鍵時觸發
        playerInputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    /// <summary>
    /// 當物件啟用時啟動輸入系統
    /// </summary>
    private void OnEnable()
    {
        playerInputActions.Player.Enable();
    }

    /// <summary>
    /// 當物件禁用時停用輸入系統，避免記憶體洩漏
    /// </summary>
    private void OnDisable()
    {
        playerInputActions.Player.Disable();
    }

    /// <summary>
    /// 遊戲開始時執行的初始化方法
    /// </summary>
    void Start()
    {
        // 可以在這裡放置額外的初始化邏輯
    }

    /// <summary>
    /// 每一幀更新玩家移動
    /// </summary>
    void Update()
    {
        HandleMovement();
    }

    /// <summary>
    /// 處理玩家移動邏輯
    /// </summary>
    private void HandleMovement()
    {
        // 如果沒有輸入，直接返回
        if (moveInput == Vector2.zero) return;

        // 計算移動方向 (將2D輸入轉換為3D世界座標)
        // x軸對應左右移動，z軸對應前後移動（在Unity中y軸是上下）
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        // 正規化移動向量，確保斜向移動不會比直線移動更快
        moveDirection.Normalize();

        // 應用移動速度和時間增量來移動物件
        // Time.deltaTime 確保移動不受幀率影響
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 當物件被銷毀時清理資源
    /// </summary>
    private void OnDestroy()
    {
        // 如果輸入系統存在，則進行清理
        if (playerInputActions != null)
        {
            playerInputActions.Dispose();
        }
    }
}
