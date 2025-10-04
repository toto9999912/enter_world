# 🚀 快速開始指南

## 📋 在 Unity 中設置戰鬥系統

### 步驟 1: 創建服務初始化器

1. 在 Hierarchy 創建空物件,命名為 `GameServices`
2. 添加 `GameServices.cs` 腳本
3. 勾選 `Dont Destroy On Load` 和 `Enable Debug Logs`

### 步驟 2: 創建遊戲管理器 (可選)

1. 在 Hierarchy 創建空物件,命名為 `GameManager`
2. 添加 `GameManager.cs` 腳本
3. 配置自動存檔選項

### 步驟 3: 創建玩家

1. 創建 3D 物件 (Capsule),命名為 `Player`
2. 設定 Tag 為 `Player`
3. 添加 `PlayerController.cs` 腳本
4. 設定 Main Element (火/水/雷/土/風)

### 步驟 4: 創建敵人

1. 創建 3D 物件 (Cube),命名為 `Enemy`
2. 設定 Tag 為 `Enemy`
3. 添加 `EnemyController.cs` 腳本
4. 配置屬性 (HP, Attack, Defense 等)

### 步驟 5: 測試

按 Play,使用:
- **WASD**: 移動
- **Space**: 攻擊 (3米內)
- **H**: 治療

---

## 🏗️ 核心架構

```
GameServices (必須)
  └─ 初始化 ServiceLocator, EventBus
  └─ 註冊 CombatService, InventoryService, SaveService

GameManager (推薦)
  └─ 處理自動存檔

PlayerController
  └─ 使用 CharacterStats, HealthSystem, CombatSystem

EnemyController
  └─ 使用 CharacterStats, HealthSystem, CombatSystem, StateMachine
```

---

## 📚 現有功能

### Combat 資料夾
- `DamageNumber.cs` - 傷害數字顯示
- `DamageNumberSpawner.cs` - 傷害數字生成器
- `CombatSystem.cs` - 戰鬥系統核心

### 使用範例

**訂閱戰鬥事件**:
```csharp
EventBus.Subscribe<DamageDealtEvent>(evt => {
    Debug.Log($"傷害: {evt.Damage}, 暴擊: {evt.IsCritical}");
});
```

**獲取服務**:
```csharp
var saveService = ServiceLocator.Get<ISaveService>();
saveService.SaveGame(0);
```

---

完整文檔請參考: `README.md`
