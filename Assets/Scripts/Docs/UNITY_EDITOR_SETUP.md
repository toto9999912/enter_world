# 🎯 Unity Editor 操作步驟

這份文件提供詳細的 Unity Editor 操作步驟,讓您快速設置戰鬥演示場景。

---

## 📋 前置準備

### 檢查清單
- [ ] Unity 編輯器已開啟 `enter_world` 專案
- [ ] 所有腳本已編譯完成,無錯誤
- [ ] TextMeshPro 已安裝 (如果沒有,在 Package Manager 中安裝)

---

## 🎬 步驟 1: 創建新場景

1. **File > New Scene** 或 `Ctrl+N`
2. 選擇 **Basic (Built-in)** 或 **3D (URP)**
3. **File > Save As** → `Assets/Scenes/CombatDemo.unity`

---

## 🛠️ 步驟 2: 設置遊戲服務

### 2.1 創建 GameServices 物件
1. 在 **Hierarchy** 右鍵 → **Create Empty**
2. 重命名為 `GameServices`
3. 在 **Inspector** → **Add Component** → 搜尋 `GameServicesInitializer`
4. 添加此組件

### 2.2 配置 GameServices
```
GameServicesInitializer 組件設定:
├─ Dont Destroy On Load: ✓ (勾選)
└─ Enable Debug Logs: ✓ (勾選)
```

---

## 🎮 步驟 3: 創建玩家

### 3.1 創建玩家物件
1. **Hierarchy** 右鍵 → **3D Object > Capsule**
2. 重命名為 `Player`
3. 設定 Transform:
   - Position: `(0, 1, 0)`
   - Rotation: `(0, 0, 0)`
   - Scale: `(1, 1, 1)`

### 3.2 設定玩家 Tag
1. 在 **Inspector** 頂部選擇 **Tag** 下拉選單
2. 如果沒有 `Player` Tag,點擊 **Add Tag...**
3. 展開 **Tags**,點擊 **+** 新增 Tag `Player`
4. 回到 Player 物件,設定 Tag 為 `Player`

### 3.3 添加 PlayerController 腳本
1. 在 **Inspector** → **Add Component**
2. 搜尋 `PlayerController`
3. 添加此組件

### 3.4 配置 PlayerController
```
PlayerController 組件設定:
├─ Main Element: Fire
├─ Move Speed: 5
└─ (stats, healthSystem, combatSystem 會自動初始化)
```

### 3.5 添加攝影機跟隨 (可選)
如果想要攝影機跟隨玩家:
1. 選擇 **Main Camera**
2. 設定 Position: `(0, 10, -10)`
3. 設定 Rotation: `(45, 0, 0)`
4. 建議添加 Cinemachine 虛擬攝影機以獲得更平滑的跟隨效果

---

## 👾 步驟 4: 創建敵人

### 4.1 創建敵人物件
1. **Hierarchy** 右鍵 → **3D Object > Cube**
2. 重命名為 `Enemy`
3. 設定 Transform:
   - Position: `(5, 0.5, 0)`
   - Rotation: `(0, 0, 0)`
   - Scale: `(1, 1, 1)`

### 4.2 設定敵人 Tag
1. 同樣方式創建 `Enemy` Tag (如果沒有的話)
2. 設定敵人物件的 Tag 為 `Enemy`

### 4.3 添加 EnemyController 腳本
1. 在 **Inspector** → **Add Component**
2. 搜尋 `EnemyController`
3. 添加此組件

### 4.4 配置 EnemyController
```
EnemyController 組件設定:
├─ Main Element: Fire (或其他元素)
├─ Max HP: 500
├─ Attack Power: 30
├─ Magic Power: 40
├─ Defense: 20
├─ Detection Range: 10
├─ Attack Range: 2
├─ Move Speed: 3
└─ Attack Cooldown: 2
```

### 4.5 更改敵人顏色 (方便辨識)
1. 選擇 Enemy 物件
2. 在 **Inspector** 找到 **Mesh Renderer** → **Materials**
3. 點擊 Material,在 **Albedo** 選擇紅色

---

## 🎨 步驟 5: 設置 UI

### 5.1 創建 Canvas
1. **Hierarchy** 右鍵 → **UI > Canvas**
2. Canvas 自動創建,名稱為 `Canvas`
3. Canvas 設定:
   - **Render Mode**: Screen Space - Overlay
   - **UI Scale Mode**: Scale With Screen Size
   - **Reference Resolution**: 1920 x 1080

### 5.2 創建玩家血量文字
1. 在 **Canvas** 下右鍵 → **UI > Text - TextMeshPro**
   - 如果第一次使用 TMP,會提示匯入資源,點擊 **Import TMP Essentials**
2. 重命名為 `PlayerHealthText`
3. 設定 **Rect Transform**:
   - **Anchor**: Top Left
   - **Pos X**: 20, **Pos Y**: -20
   - **Width**: 300, **Height**: 50
4. 設定 **TextMeshProUGUI**:
   - **Text**: "生命值: 1000 / 1000"
   - **Font Size**: 24
   - **Color**: White
   - **Alignment**: Left, Top

### 5.3 添加 HealthUI 腳本到 Canvas
1. 選擇 **Canvas** 物件
2. **Add Component** → 搜尋 `HealthUI`
3. 配置 HealthUI:
   - 拖拽 **Player** 物件到 `Player` 欄位
   - 拖拽 **PlayerHealthText** 到 `Health Text` 欄位
   - **Update Interval**: 0.1

---

## 🩸 步驟 6: 添加敵人血條 (可選但推薦)

### 6.1 創建世界空間 Canvas (為敵人血條)
1. **Hierarchy** 右鍵 → **UI > Canvas**
2. 重命名為 `EnemyHealthCanvas`
3. 設定 Canvas:
   - **Render Mode**: World Space
   - **Width**: 200, **Height**: 50
   - **Scale**: (0.01, 0.01, 0.01)

### 6.2 創建血條 Slider
1. 在 **EnemyHealthCanvas** 下右鍵 → **UI > Slider**
2. 重命名為 `HealthSlider`
3. 設定 Slider:
   - **Min Value**: 0
   - **Max Value**: 1
   - **Value**: 1
   - **Interactable**: 取消勾選

### 6.3 設定血條顏色
1. 展開 **HealthSlider** → 找到 **Fill** 子物件
2. 在 **Image** 組件中設定顏色為 **綠色**

### 6.4 調整血條位置
1. 選擇 **HealthSlider**
2. **Rect Transform**:
   - **Anchor**: Center
   - **Width**: 180, **Height**: 20
   - **Pos X**: 0, **Pos Y**: 0

### 6.5 將 Canvas 設為敵人的子物件
1. 拖拽 **EnemyHealthCanvas** 到 **Enemy** 物件上,使其成為子物件
2. 設定 **EnemyHealthCanvas** 的 **Position**: (0, 2, 0) (在敵人頭頂上方)

### 6.6 添加 EnemyHealthBar 腳本
1. 選擇 **EnemyHealthCanvas** 物件
2. **Add Component** → 搜尋 `EnemyHealthBar`
3. 配置:
   - **Enemy Controller**: (自動找到)
   - **Health Slider**: 拖拽 HealthSlider 到此欄位
   - **Offset**: (0, 2, 0)
   - **Billboard To Camera**: ✓

---

## 🎮 步驟 7: 測試場景

### 7.1 儲存場景
**File > Save** 或 `Ctrl+S`

### 7.2 按下 Play
點擊 **Play** 按鈕或按 `Ctrl+P`

### 7.3 測試操作
- **WASD / 方向鍵**: 移動玩家
- **靠近敵人** (10米內): 敵人應該開始追逐
- **空白鍵 (Space)**: 攻擊 3米內的敵人
- **H 鍵**: 自我治療 100 HP

### 7.4 觀察效果
- ✅ Console 顯示初始化訊息
- ✅ 玩家可以移動
- ✅ 敵人偵測到玩家後會追逐
- ✅ 按空白鍵攻擊時,Console 顯示傷害計算
- ✅ 敵人血條減少
- ✅ UI 顯示玩家血量變化
- ✅ 敵人靠近玩家時會攻擊
- ✅ 角色血量歸零時死亡

---

## 🐛 故障排除

### 問題 1: 按 Play 後報錯
**檢查**:
- Console 顯示什麼錯誤?
- GameServicesInitializer 是否在場景中?
- 所有腳本是否編譯完成?

### 問題 2: 玩家無法移動
**檢查**:
- PlayerController 組件是否已添加?
- 是否有其他腳本干擾?
- Input System 是否正確 (使用舊版 Input Manager)?

### 問題 3: 攻擊沒有效果
**檢查**:
- 敵人是否有 `Enemy` Tag?
- 玩家與敵人距離是否在 3 米內?
- Console 是否有錯誤訊息?

### 問題 4: UI 沒有顯示
**檢查**:
- Canvas 的 Render Mode 是否正確?
- HealthUI 的引用是否正確設定?
- TextMeshPro 是否已安裝?

### 問題 5: 敵人不會追逐玩家
**檢查**:
- 玩家是否有 `Player` Tag?
- Detection Range 是否足夠大?
- EnemyController 的狀態機是否正常初始化?

---

## 🎯 下一步優化建議

完成基礎戰鬥後,可以嘗試:

### 1. 添加更多敵人
複製 Enemy 物件,放置在不同位置

### 2. 測試不同元素相剋
- 修改 PlayerController 的 `Main Element`
- 修改 EnemyController 的 `Main Element`
- 觀察傷害變化

### 3. 添加地板和障礙物
- 創建 Plane 作為地板
- 添加 Cube 作為障礙物

### 4. 調整戰鬥參數
在 PlayerController.cs 的 `InitializePlayer()` 方法中修改:
```csharp
stats.SetBaseValue(StatType.HP, 2000f);    // 增加生命值
stats.SetBaseValue(StatType.ATK, 100f);    // 增加攻擊力
stats.SetBaseValue(StatType.CritRate, 50f); // 增加暴擊率
```

### 5. 添加粒子特效
- 在 Unity Asset Store 下載免費特效
- 在攻擊時實例化特效

---

## 📸 最終場景結構

```
Hierarchy
├─ GameServices (GameServicesInitializer)
├─ Main Camera
├─ Directional Light
├─ Player (PlayerController)
│   └─ (Capsule Model)
├─ Enemy (EnemyController)
│   ├─ (Cube Model)
│   └─ EnemyHealthCanvas (EnemyHealthBar)
│       └─ HealthSlider
└─ Canvas (HealthUI)
    └─ PlayerHealthText (TextMeshProUGUI)
```

---

**完成這些步驟後,您就有一個可以測試戰鬥系統的完整場景了!** 🎉
