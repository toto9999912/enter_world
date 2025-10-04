# 🎮 戰鬥系統整合指南

## 📋 目標
讓玩家和怪物在遊戲場景中可以戰鬥,並看到戰鬥效果。

---

## 🚀 快速開始 (5步驟)

### 步驟 1: 創建服務初始化器

**目的**: 確保所有服務在場景載入時正確初始化。

#### 在 Unity Editor 中操作:
1. 在 Hierarchy 中創建空物件,命名為 `GameServices`
2. 添加我下面創建的 `GameServicesInitializer.cs` 腳本到這個物件上
3. **重要**: 這個物件應該存在於每個場景中

> 💡 **提示**: 稍後我會創建這個腳本檔案

---

### 步驟 2: 設定玩家角色

#### 在 Unity Editor 中操作:
1. 在 Hierarchy 創建一個 3D 物件 (例如: Capsule)
2. 重命名為 `Player`
3. 設置 Tag 為 `Player` (如果沒有此 Tag,請在 Inspector 中創建)
4. 添加 `PlayerController.cs` 腳本
5. **不需要在 Inspector 中設定任何屬性** - 腳本會自動初始化

#### PlayerController 配置:
```
PlayerController 組件設定:
├─ Main Element: Fire (或您想要的元素)
├─ Move Speed: 5
└─ (其他屬性會在程式碼中自動設定)
```

---

### 步驟 3: 設定敵人

#### 在 Unity Editor 中操作:
1. 創建 3D 物件 (例如: Cube)
2. 重命名為 `Enemy`
3. 設置 Tag 為 `Enemy` (如果沒有此 Tag,請創建)
4. 添加我下面會優化的 `EnemyController.cs` 腳本

#### EnemyController 配置:
```
EnemyController 組件設定:
├─ Detection Range: 10 (偵測範圍)
├─ Attack Range: 2 (攻擊範圍)
├─ Move Speed: 3
└─ (stats, healthSystem, combatSystem 會自動初始化)
```

---

### 步驟 4: 添加簡單的 UI (血量顯示)

#### 在 Unity Editor 中操作:
1. 在 Hierarchy 中創建 `Canvas` (如果沒有的話)
2. 在 Canvas 下創建 `Text - TextMeshPro` 或 `Text`,命名為 `PlayerHealthText`
3. 添加我下面創建的 `HealthUI.cs` 腳本到 Canvas 上
4. 在 Inspector 中拖拽:
   - Player 物件到 `Player` 欄位
   - PlayerHealthText 到 `Health Text` 欄位

---

### 步驟 5: 測試戰鬥

#### 操作說明:
- **WASD / 方向鍵**: 移動玩家
- **空白鍵 (Space)**: 攻擊 3米內最近的敵人
- **H 鍵**: 自我治療 100 HP

#### 預期結果:
1. 玩家可以移動
2. 按空白鍵攻擊敵人時,Console 會顯示傷害計算
3. 敵人偵測到玩家後會追逐並攻擊
4. UI 會顯示玩家當前血量
5. 血量歸零時,角色死亡

---

## 📁 需要創建的新腳本

### 1. GameServicesInitializer.cs
**位置**: `Assets/Scripts/Core/GameServicesInitializer.cs`

這個腳本負責初始化所有服務 (ServiceLocator, EventBus, CombatService 等)。

### 2. HealthUI.cs
**位置**: `Assets/Scripts/UI/HealthUI.cs`

簡單的 UI 顯示玩家和敵人的血量。

### 3. 優化 EnemyController.cs
讓敵人自動初始化戰鬥系統,不需要手動設定。

---

## 🎯 戰鬥系統流程圖

```
遊戲開始
    ↓
GameServicesInitializer 初始化所有服務
    ↓
PlayerController.Start() → 初始化 stats, healthSystem, combatSystem
    ↓
EnemyController.Start() → 初始化 stats, healthSystem, combatSystem, 狀態機
    ↓
═══════════════════════════════════════
遊戲循環
    ↓
玩家移動 (Update)
    ↓
玩家按空白鍵
    ↓
AttackNearestEnemy() → 找最近敵人
    ↓
CombatSystem.Attack() → 計算傷害
    ↓
傷害計算流程:
  1. 基礎傷害 = ATK * damageMultiplier
  2. 元素加成 (元素相剋表)
  3. 暴擊判定
  4. 命中判定
  5. 防禦減免
    ↓
敵人 HealthSystem.TakeDamage()
    ↓
EventBus 發布 DamageDealtEvent
    ↓
UI 更新血量顯示
    ↓
如果血量 ≤ 0 → OnDeath 事件
═══════════════════════════════════════
```

---

## 🔧 進階配置 (可選)

### 元素屬性設定
在 PlayerController 或 EnemyController 的 Inspector 中可以設定:
- **Fire (火)**: 克制冰、弱點水
- **Water (水)**: 克制火、弱點雷
- **Thunder (雷)**: 克制水、弱點土
- **Earth (土)**: 克制雷、弱點風
- **Wind (風)**: 克制土、弱點火

### 調整戰鬥參數
在 `PlayerController.InitializePlayer()` 中可以調整:
```csharp
stats.SetBaseValue(StatType.HP, 1000f);      // 生命值
stats.SetBaseValue(StatType.ATK, 50f);       // 物理攻擊
stats.SetBaseValue(StatType.Magic, 60f);     // 魔法攻擊
stats.SetBaseValue(StatType.DEF, 30f);       // 防禦力
stats.SetBaseValue(StatType.CritRate, 25f);  // 暴擊率 (%)
stats.SetBaseValue(StatType.CritDamage, 150f); // 暴擊傷害 (%)
```

---

## 🐛 常見問題

### Q1: 攻擊沒有傷害?
**檢查**:
- 敵人是否有 Tag "Enemy"
- 距離是否在 3 米內
- Console 是否有錯誤訊息

### Q2: UI 沒有顯示?
**檢查**:
- HealthUI 腳本的 Player 欄位是否已設定
- Canvas 的 Render Mode 是否正確
- TextMeshPro 是否已安裝 (Package Manager)

### Q3: 敵人不會移動?
**檢查**:
- EnemyController 的狀態機是否正常初始化
- Detection Range 設定值
- Player Tag 是否正確

### Q4: 服務找不到?
**檢查**:
- GameServicesInitializer 是否在場景中
- 是否在 Awake() 中就初始化了服務
- ServiceLocator.Get<>() 的呼叫時機

---

## 📊 測試檢查清單

- [ ] GameServices 物件存在於場景中
- [ ] 玩家物件有 "Player" Tag
- [ ] 敵人物件有 "Enemy" Tag
- [ ] 按 Play 後 Console 顯示初始化訊息
- [ ] 玩家可以移動 (WASD)
- [ ] 按空白鍵可以攻擊
- [ ] Console 顯示傷害計算
- [ ] 敵人血量減少
- [ ] UI 顯示玩家血量
- [ ] 血量歸零時角色死亡

---

## 🎓 下一步建議

完成基礎戰鬥後,可以考慮:

1. **添加傷害數字特效**: 在敵人頭上顯示傷害數字
2. **添加血條**: 為玩家和敵人添加頭頂血條
3. **添加技能系統**: 整合 SkillTree 讓玩家可以施放技能
4. **添加掉落系統**: 敵人死亡時掉落物品
5. **添加經驗系統**: 整合 LevelSystem
6. **添加音效**: 攻擊、受傷、死亡音效
7. **添加粒子特效**: 攻擊特效、元素特效

---

## 📖 相關文件

- [系統架構說明](../../README.md)
- [存檔系統說明](./SAVE_SYSTEM_README.md)
- [CharacterStats 系統](../Stats/CharacterStats.cs)
- [CombatSystem 系統](../Combat/CombatSystem.cs)
- [ElementalAffinityTable](../Element/ElementalAffinityTable.cs)

---

**提示**: 我會立即創建所需的新腳本檔案,您只需要按照上面的步驟在 Unity Editor 中操作即可!
