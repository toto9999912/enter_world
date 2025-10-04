# Scripts 檔案結構清單

**最後更新**: 2025-10-04
**架構版本**: 2.0 (重構版 - ServiceLocator + EventBus)

---

## 📁 目錄結構

```
Assets/Scripts/
├── Core/                    # 核心架構 (NEW)
├── Services/                # 服務層 (NEW)
├── Stats/                   # 屬性系統
├── Combat/                  # 戰鬥系統
├── Element/                 # 元素系統
├── Skill/                   # 技能與天賦系統
├── Companion/               # 眷屬系統
├── Item/                    # 物品系統
├── Inventory/               # 背包系統
├── Shop/                    # 商店系統
├── Level/                   # 等級系統
├── Player/                  # 玩家控制
├── AI/                      # AI 系統
├── ARCHITECTURE.md          # 架構說明文件
├── SYSTEM_ARCHITECTURE.md   # 系統詳細文件
└── FILE_STRUCTURE.md        # 本檔案
```

---

## 📦 核心檔案清單 (58 個檔案)

### 🔧 Core/ (核心架構) - 6 個檔案
✅ **IService.cs** - 服務介面定義
✅ **ServiceLocator.cs** - 服務定位器 (全域服務管理)
✅ **EventBus.cs** - 事件總線 (解耦系統通訊)
✅ **GameEvents.cs** - 預定義遊戲事件 (戰鬥/物品/眷屬等)
✅ **ObjectPool.cs** - 物件池系統 (減少 GC)
✅ **StateMachine.cs** - 狀態機基類
✅ **GameServices.cs** - 服務初始化器

### 🎮 Services/ (服務層) - 6 個檔案
✅ **ICombatService.cs** - 戰鬥服務介面
✅ **CombatService.cs** - 戰鬥服務實作
✅ **IInventoryService.cs** - 背包服務介面
✅ **InventoryService.cs** - 背包服務實作
✅ **ISaveService.cs** - 存檔服務介面
✅ **SaveService.cs** - 存檔服務實作 (JSON)

### 📊 Stats/ (屬性系統) - 4 個檔案
✅ **StatType.cs** - 屬性類型枚舉 (HP/MP/ATK/DEF...)
✅ **StatModifier.cs** - 屬性修飾器 (裝備加成/Buff)
✅ **CharacterStats.cs** - 角色屬性管理
✅ **HealthSystem.cs** - 生命值/魔力管理 (已整合 EventBus)

### ⚔️ Combat/ (戰鬥系統) - 6 個檔案
✅ **CombatSystem.cs** - 戰鬥系統核心 (已整合 EventBus)
✅ **DamageInfo.cs** - 傷害資訊結構
✅ **DamageType.cs** - 傷害類型枚舉
✅ **SuperArmor.cs** - 霸體系統 (Boss 專用)
✅ **CrowdControl.cs** - 控場效果系統
✅ **DamageNumber.cs** - 傷害數字顯示
✅ **DamageNumberSpawner.cs** - 傷害數字生成器 (監聽 EventBus)

### 🔥 Element/ (元素系統) - 2 個檔案
✅ **ElementType.cs** - 8 大元素定義
✅ **ElementalAffinityTable.cs** - 元素克制表

### 🎯 Skill/ (技能與天賦) - 9 個檔案
✅ **SkillType.cs** - 技能類型定義
✅ **SkillData.cs** - 技能資料 (ScriptableObject)
✅ **SkillInstance.cs** - 技能實例 (含冷卻)
✅ **SkillTree.cs** - 技能樹管理
✅ **SkillCastSystem.cs** - 技能施放系統 (已整合 EventBus)
✅ **TalentData.cs** - 天賦資料 (ScriptableObject)
✅ **TalentNode.cs** - 天賦節點
✅ **TalentTree.cs** - 天賦樹管理

### 👾 Companion/ (眷屬系統) - 5 個檔案
✅ **CompanionRarity.cs** - 稀有度配置 (ScriptableObject)
✅ **CompanionData.cs** - 眷屬資料 (ScriptableObject)
✅ **CompanionInstance.cs** - 眷屬實例 (已整合 EventBus)
✅ **CompanionManager.cs** - 眷屬管理器 (SP/重生) (已整合 EventBus)
✅ **CompanionCaptureSystem.cs** - 捕獲系統

### 📦 Item/ (物品系統) - 6 個檔案
✅ **ItemType.cs** - 物品類型與稀有度
✅ **ItemData.cs** - 物品資料基類 (ScriptableObject)
✅ **EquipmentData.cs** - 裝備資料 (強化/套裝)
✅ **EquipmentInstance.cs** - 裝備實例 (耐久/強化等級)
✅ **CardData.cs** - 卡片資料 (星級/融合)
✅ **CardInstance.cs** - 卡片實例 (Unix 時間戳冷卻)

### 🎒 Inventory/ (背包系統) - 1 個檔案
✅ **InventorySystem.cs** - 背包系統 (50-200 格)

### 🛒 Shop/ (商店系統) - 1 個檔案
✅ **ShopSystem.cs** - 商店系統 (固定/流動/黑市)

### ⬆️ Level/ (等級系統) - 1 個檔案
✅ **PlayerLevel.cs** - 玩家等級/經驗/屬性點

### 🕹️ Player/ (玩家控制) - 1 個檔案
✅ **PlayerController.cs** - 玩家控制器 (整合所有系統)

### 🤖 AI/ (AI 系統) - 2 個檔案
✅ **EnemyController.cs** - 敵人控制器 (使用狀態機)
✅ **EnemyStates.cs** - 敵人 AI 狀態 (Idle/Patrol/Chase/Attack/Dead)

### 📄 文件 - 3 個檔案
✅ **ARCHITECTURE.md** - 架構說明與使用範例
✅ **SYSTEM_ARCHITECTURE.md** - 系統詳細說明
✅ **FILE_STRUCTURE.md** - 本檔案 (檔案結構清單)

---

## ❌ 已刪除的冗餘檔案

以下檔案已於重構時刪除：

1. ❌ **Player/Player.cs** - 與 PlayerController 功能重複
2. ❌ **Player/PlayerMovement.cs** - 已整合到 PlayerController
3. ❌ **PlayerInputActions.cs** - 自動生成檔案，無使用
4. ❌ **Element/ElementalDamage.cs** - 功能已整合到 CombatSystem
5. ❌ **AI/EnemyAI.cs** - 已重構為 EnemyController + 狀態機
6. ❌ **SaveSystem/** - 已重構為 SaveService
7. ❌ **IMPLEMENTATION_SUMMARY.md** - 已合併到 ARCHITECTURE.md

---

## 🔄 EventBus 整合狀態

以下系統已整合 EventBus 發布事件：

✅ **CombatSystem** - 發布 `DamageDealtEvent`
✅ **HealthSystem** - 發布 `HealingEvent`, `EntityDeathEvent`
✅ **InventoryService** - 發布 `ItemAddedEvent`, `ItemRemovedEvent`
✅ **SkillCastSystem** - 發布 `SkillCastEvent`
✅ **CompanionManager** - 發布 `CompanionDeployedEvent`
✅ **DamageNumberSpawner** - 訂閱 `DamageDealtEvent`, `HealingEvent`

---

## 📈 統計

| 類別 | 數量 |
|------|------|
| 總檔案數 | 58 |
| C# 腳本 | 55 |
| 文件 | 3 |
| 核心架構 | 7 |
| 服務層 | 6 |
| 遊戲系統 | 42 |
| 已刪除檔案 | 7 |

---

## 🎯 架構特色

1. **解耦設計**: 使用 ServiceLocator 管理服務，EventBus 處理事件
2. **可測試性**: 服務介面分離，便於單元測試
3. **可維護性**: 清晰的目錄結構，職責分明
4. **可擴展性**: ScriptableObject 資料驅動，易於新增內容
5. **效能優化**: ObjectPool 減少 GC，狀態機優化 AI

---

## 📝 使用建議

### 快速開始
1. 場景中新增 GameObject，掛載 `GameServices`
2. 玩家 GameObject 掛載 `PlayerController`
3. 敵人 GameObject 掛載 `EnemyController`
4. 透過 ServiceLocator 存取服務
5. 使用 EventBus 訂閱/發布事件

### 新增功能
- **新增物品**: 創建 ScriptableObject (ItemData/EquipmentData/CardData)
- **新增技能**: 創建 SkillData ScriptableObject
- **新增眷屬**: 創建 CompanionData ScriptableObject
- **新增服務**: 實作 IService 介面，註冊到 GameServices

---

**結論**: 經過重構後，專案結構清晰，無冗餘檔案，所有系統已整合 EventBus 和 ServiceLocator，達到高內聚低耦合的架構目標。
