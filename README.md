# Unity ARPG 遊戲 - Enter World

> **專案狀態**: 開發中 | **Unity 版本**: 2022.3+ | **最後更新**: 2025-10-04

一款使用 Unity 開發的 2D ARPG 遊戲,具備完整的屬性系統、戰鬥系統、眷屬系統、技能天賦系統等核心功能。

---

## 📖 目錄

- [專案概述](#專案概述)
- [核心功能](#核心功能)
- [系統架構](#系統架構)
- [快速開始](#快速開始)
- [遊戲系統詳解](#遊戲系統詳解)
- [開發指南](#開發指南)
- [更新日誌](#更新日誌)

---

## 🎮 專案概述

### 遊戲特色

- **深度屬性系統**: 15+ 種角色屬性,支援複雜的加成計算
- **元素克制系統**: 8 大元素相互克制,策略性戰鬥
- **眷屬收集**: 捕捉並培養眷屬,組建最強隊伍
- **技能天賦樹**: 豐富的技能和天賦選擇,打造獨特角色
- **裝備強化**: 裝備強化、套裝系統、隱藏屬性
- **自動存檔**: 完整的存檔系統,支援多槽位和自動存檔

### 技術特色

- **服務導向架構**: 使用 ServiceLocator 管理全域服務
- **事件驅動通訊**: 透過 EventBus 實現系統解耦
- **資料驅動設計**: ScriptableObject 作為資料容器
- **物件池優化**: 減少 GC 壓力,提升運行效能
- **狀態機模式**: 清晰的 AI 和遊戲狀態管理

---

## ✅ 核心功能

### 已完成系統 (12/14 - 85.7%)

#### 🎯 核心架構
- ✅ **ServiceLocator** - 服務定位器,統一管理遊戲服務
- ✅ **EventBus** - 事件總線,解耦系統間通訊
- ✅ **ObjectPool** - 物件池,優化性能
- ✅ **StateMachine** - 狀態機,管理複雜邏輯

#### 📊 遊戲系統
1. ✅ **Stats 屬性系統** - 15+ 種屬性,三種加成類型
2. ✅ **Element 元素系統** - 8 大元素,完整克制表
3. ✅ **Combat 戰鬥系統** - 傷害計算、爆擊、閃避、穿透
4. ✅ **Skill 技能系統** - 技能樹、冷卻管理、跨元素消耗
5. ✅ **Talent 天賦系統** - 主屬性專屬天賦、三大類型
6. ✅ **Companion 眷屬系統** - SP 機制、重生系統、捕捉系統
7. ✅ **Item 物品系統** - 6 大類型、6 種稀有度
8. ✅ **Equipment 裝備系統** - 強化 +0~+9、套裝加成
9. ✅ **Card 卡片系統** - 星級、融合、真實時間冷卻
10. ✅ **Inventory 背包系統** - 50-200 格、分類、擴充
11. ✅ **Shop 商店系統** - 三種商店、刷新機制
12. ✅ **Level 等級系統** - 經驗值、升級獎勵

#### 💾 服務層
- ✅ **CombatService** - 戰鬥服務
- ✅ **InventoryService** - 背包服務
- ✅ **SaveService** - 存檔服務 (已完成序列化框架)

### 待完成功能 (2/14)

1. ⚠️ **SaveService** - 需要整合各系統的序列化邏輯
2. ❌ **Enemy Formation** - 敵人編隊配置系統

---

## 🏗️ 系統架構

### 三層架構設計

```
┌──────────────────────────────────────┐
│   Presentation Layer (表現層)         │
│   PlayerController, UI, EnemyAI      │
└──────────────────────────────────────┘
            ↓ 調用服務
┌──────────────────────────────────────┐
│   Service Layer (服務層)              │
│   ServiceLocator + EventBus          │
│   CombatService, InventoryService    │
└──────────────────────────────────────┘
            ↓ 操作資料
┌──────────────────────────────────────┐
│   Data Layer (資料層)                 │
│   ScriptableObject, Pure C# Classes  │
│   CharacterStats, SkillTree, etc.    │
└──────────────────────────────────────┘
```

### 目錄結構

```
Assets/Scripts/
├── Core/                    # 核心架構
│   ├── ServiceLocator.cs   # 服務定位器
│   ├── EventBus.cs         # 事件總線
│   ├── GameEvents.cs       # 預定義事件
│   ├── ObjectPool.cs       # 物件池
│   ├── StateMachine.cs     # 狀態機
│   └── GameManager.cs      # 遊戲管理器(自動存檔)
│
├── Services/                # 服務層
│   ├── ICombatService.cs
│   ├── CombatService.cs
│   ├── IInventoryService.cs
│   ├── InventoryService.cs
│   ├── ISaveService.cs
│   ├── SaveService.cs
│   ├── SaveData.cs
│   ├── ISkillService.cs
│   └── ICompanionService.cs
│
├── Stats/                   # 屬性系統
│   ├── StatType.cs
│   ├── StatModifier.cs
│   ├── CharacterStats.cs
│   └── HealthSystem.cs
│
├── Element/                 # 元素系統
│   ├── ElementType.cs
│   └── ElementalAffinityTable.cs
│
├── Combat/                  # 戰鬥系統
│   ├── CombatSystem.cs
│   ├── DamageInfo.cs
│   ├── SuperArmor.cs
│   ├── CrowdControl.cs
│   └── DamageNumber.cs
│
├── Skill/                   # 技能&天賦
│   ├── SkillData.cs
│   ├── SkillInstance.cs
│   ├── SkillTree.cs
│   ├── SkillCastSystem.cs
│   ├── TalentData.cs
│   ├── TalentNode.cs
│   └── TalentTree.cs
│
├── Companion/               # 眷屬系統
│   ├── CompanionData.cs
│   ├── CompanionInstance.cs
│   ├── CompanionManager.cs
│   ├── CompanionCaptureSystem.cs
│   └── CompanionRarity.cs
│
├── Item/                    # 物品系統
│   ├── ItemType.cs
│   ├── ItemData.cs
│   ├── EquipmentData.cs
│   ├── EquipmentInstance.cs
│   ├── CardData.cs
│   └── CardInstance.cs
│
├── Inventory/               # 背包系統
│   └── InventorySystem.cs
│
├── Shop/                    # 商店系統
│   └── ShopSystem.cs
│
├── Level/                   # 等級系統
│   └── PlayerLevel.cs
│
├── Player/                  # 玩家控制
│   ├── PlayerController.cs
│   └── PlayerMovement.cs
│
└── AI/                      # AI 系統
    ├── EnemyController.cs
    └── EnemyStates.cs
```

---

## 🚀 快速開始

### 環境需求

- Unity 2022.3 或更高版本
- .NET Standard 2.1
- Input System Package (新版輸入系統)

### 初始化步驟

1. **場景設置**

```csharp
// 1. 創建空物件 "GameServices"
// 2. 掛載 GameServices.cs 組件
// 3. 自動註冊所有服務
```

2. **創建玩家**

```csharp
// 1. 創建玩家物件
// 2. 掛載 PlayerController.cs
// 3. 配置初始屬性
```

3. **使用服務**

```csharp
// 獲取戰鬥服務
var combatService = ServiceLocator.Get<ICombatService>();

// 攻擊敵人
DamageInfo damage = combatService.CalculateDamage(
    playerStats,
    enemyStats,
    100f,  // 基礎傷害
    DamageType.Physical
);
```

### 基本使用範例

#### 屬性系統

```csharp
// 創建角色屬性
CharacterStats stats = new CharacterStats();
stats.SetBaseValue(StatType.HP, 100f);
stats.SetBaseValue(StatType.ATK, 20f);

// 添加裝備加成
var swordBonus = new StatModifier(
    StatType.ATK,
    ModifierType.Flat,
    15f,
    "iron_sword"
);
stats.AddModifier(swordBonus);

// 取得最終屬性
float finalATK = stats.GetFinalValue(StatType.ATK); // 35
```

#### 戰鬥系統

```csharp
// 創建戰鬥系統
CombatSystem combat = new CombatSystem(attackerStats, attackerHealth);
combat.Element = ElementType.Fire;

// 攻擊目標
DamageInfo damageInfo = combat.Attack(
    targetHealth,
    targetStats,
    baseDamage: 100f,
    damageType: DamageType.Magical,
    skillMultiplier: 1.5f
);

// 檢查結果
if (damageInfo.isCritical) Debug.Log("暴擊!");
Debug.Log($"造成 {damageInfo.finalDamage} 點傷害");
```

#### 眷屬系統

```csharp
// 捕捉眷屬
CaptureResult result = CompanionCaptureSystem.AttemptCapture(
    companionData,
    targetLevel: 5,
    targetHealth,
    targetStats,
    playerLevel: 10,
    netQuality: CaptureNetQuality.Good
);

if (result.success)
{
    // 新增到管理器
    companionManager.AddCompanion(result.capturedCompanion);
    
    // 部署眷屬
    companionManager.DeployCompanion(result.capturedCompanion);
}
```

#### 存檔系統

```csharp
// 獲取存檔服務
var saveService = ServiceLocator.Get<ISaveService>();

// 存檔
saveService.SaveGame(0);  // 存到槽位 0

// 讀檔
if (saveService.LoadGame(0))
{
    Debug.Log("載入成功!");
}

// 自動存檔由 GameManager 自動處理
// - 應用程式暫停時
// - 應用程式失焦時
// - 應用程式退出時
// - 定時自動存檔(預設 5 分鐘)
```

---

## 🎯 遊戲系統詳解

### 1. 屬性系統 (Stats)

#### 屬性分類

**主要屬性** (可升級分配):
- **HP** - 生命值
- **MP** - 魔力值
- **ATK** - 物理攻擊力
- **Magic** - 魔法力量
- **DEF** - 防禦力
- **SPD** - 移動速度
- **SP** - 精神力 (眷屬系統專用)

**稀有屬性** (裝備/Buff 獲得):
- **CritRate** - 爆擊率 (0-100%)
- **CritDamage** - 爆擊傷害 (100-300%)
- **DodgeRate** - 閃避率 (0-100%)
- **Toughness** - 韌性 (降低控場命中率)
- **Penetration** - 穿透 (無視防禦)
- **SkillHaste** - 技能急速 (0-40%)
- **HitRate** - 命中率 (0-100%)

#### 修飾器類型

- **Flat** - 固定數值 (+10 攻擊)
- **Percentage** - 百分比 (+15% HP)
- **Final** - 最終值 (+10% 最終傷害)

### 2. 元素系統 (Element)

#### 8 大元素

**基礎四元素**:
- **Fire** (火) - 克風 160%, 被水克 80%
- **Water** (水) - 克火 140%, 被地克 75%
- **Earth** (地) - 克水 130%, 被風克 85%
- **Wind** (風) - 克地 145%, 被火克 90%

**特殊元素**:
- **None** (無) - 對所有元素 80%
- **Light** (光) - 對闇 200%, 被闇 50%
- **Dark** (闇) - 對基礎元素 120%, 對光 50%
- **Divine** (神) - 對所有元素 150% (最強)

### 3. 戰鬥系統 (Combat)

#### 傷害計算流程

```
1. 命中判定 (攻擊者命中率 vs 目標閃避率)
2. 閃避判定 (如果命中)
3. 計算基礎傷害
   - 物理: ATK × 技能倍率
   - 魔法: Magic × 技能倍率
   - 真實: 直接傷害
4. 爆擊判定 (爆擊率 - 目標韌性)
5. 元素克制 (僅魔法傷害)
6. 防禦減免 (有效防禦 = DEF - 穿透)
7. 霸體減傷 (如果有)
```

#### 控場效果 (CC)

- **Stun** - 暈眩 (無法行動)
- **Freeze** - 冰凍 (無法移動和攻擊)
- **Slow** - 減速
- **Silence** - 沉默 (無法施法)
- **Root** - 定身
- **Knockback** - 擊飛
- **Knockdown** - 擊倒

#### 霸體系統

- 免疫所有控場效果
- 高額傷害減免
- 霸體值可被消耗
- 破壞後進入暈眩狀態

### 4. 眷屬系統 (Companion)

#### 稀有度等級

- **Common** (普通) - 重生 10 秒
- **Elite** (精英) - 重生 20 秒
- **Rare** (稀有) - 重生 40 秒
- **Legendary** (傳說) - 重生 60 秒

#### SP (精神力) 機制

```
重生時間公式:
基礎時間 × (2.0 - 1.0 × SP%) × (1 ± 10% 隨機)

範例:
- 稀有眷屬 (40秒)
- SP 50%: 40 × 1.5 = 60 秒
- SP 0%:  40 × 2.0 = 80 秒
- SP 100%: 40 × 1.0 = 40 秒
```

#### 捕捉系統

```
捕捉率 =
  基礎捕捉率
  + (1 - HP%) × 50%
  + (1 - SP%) × 30%
  + 等級差 × 2%
  + 網子品質加成
  + 技能加成

限制: 5% ~ 95%
```

### 5. 技能系統 (Skill)

#### 技能樹機制

- 每個元素 20 個技能
- 跨元素學習消耗雙倍技能點
- 技能等級 1-5 級
- 支援前置技能需求

#### 技能類型

- **主動技能** - 消耗 MP、造成傷害/治療
- **被動技能** - 永久屬性加成
- **控場技能** - 施加 CC 效果
- **Buff 技能** - 增強友軍

### 6. 天賦系統 (Talent)

#### 天賦類型

- **被動屬性** - 永久增加屬性
- **技能強化** - 增強特定技能
- **特殊機制** - 解鎖獨特機制

#### 限制

- 主元素專屬 (只能學習主屬性天賦)
- 需要前置天賦
- 需要角色等級
- 更換主屬性時重置

### 7. 物品系統 (Item)

#### 物品類型

- **Consumable** - 消耗品 (藥水、食物)
- **Equipment** - 裝備 (武器、防具)
- **Item** - 一般物品
- **Material** - 材料
- **QuestItem** - 任務道具
- **Card** - 卡片

#### 稀有度

- Common (灰) → Uncommon (綠) → Rare (藍) → Epic (紫) → Legendary (橙) → Mythic (紅)

### 8. 裝備系統 (Equipment)

#### 裝備槽位

- **Weapon** - 武器
- **Head** - 頭部
- **Chest** - 胸甲
- **Shoes** - 鞋子
- **Accessory** - 飾品

#### 強化系統

- 強化等級: +0 到 +9
- 隱藏屬性解鎖: +3, +6, +9
- 強化失敗降低耐久
- 套裝加成 (2/4/6 件)

### 9. 存檔系統 (Save)

#### 功能特色

- ✅ Base64 加密存檔
- ✅ 主檔案 + 備份機制
- ✅ 存檔版本控制
- ✅ 多槽位支援 (預設 3 個)
- ✅ 自動存檔機制

#### 自動存檔觸發

- 應用程式暫停時
- 應用程式失焦時
- 應用程式退出時
- 定時自動存檔 (可配置,預設 5 分鐘)

#### 存檔內容

- 玩家位置、場景、等級、經驗
- 角色屬性 (基礎值 + 修飾器)
- 血量/魔力狀態
- 背包資料 (容量 + 物品)
- 技能/天賦資料
- 眷屬資料
- 遊戲進度

---

## 💻 開發指南

### 新增遊戲內容

#### 創建眷屬

1. 右鍵 → Create → Game → Companion → Companion Data
2. 設定屬性、稀有度、元素等
3. 在遊戲中使用 `CompanionCaptureSystem` 捕捉

#### 創建技能

1. 右鍵 → Create → Game → Skill → Skill Data
2. 設定技能效果、冷卻、消耗等
3. 加入到 SkillTree

#### 創建裝備

1. 右鍵 → Create → Game → Item → Equipment Data
2. 設定屬性加成、套裝效果等
3. 透過 InventoryService 添加到背包

### 擴展服務層

#### 創建新服務

```csharp
// 1. 定義介面
public interface IQuestService : IService
{
    void StartQuest(string questId);
    void CompleteQuest(string questId);
}

// 2. 實作服務
public class QuestService : IQuestService
{
    public void Initialize() { }
    public void Shutdown() { }
    
    public void StartQuest(string questId)
    {
        // 實作邏輯
        EventBus.Publish(new QuestStartedEvent { QuestId = questId });
    }
}

// 3. 註冊服務 (在 GameServices.cs)
ServiceLocator.Register<IQuestService>(new QuestService());
```

### 事件系統使用

#### 定義自訂事件

```csharp
public struct CustomEvent : IGameEvent
{
    public string Message;
    public int Value;
}
```

#### 訂閱和發布

```csharp
// 訂閱
EventBus.Subscribe<CustomEvent>(OnCustomEvent);

void OnCustomEvent(CustomEvent evt)
{
    Debug.Log($"收到事件: {evt.Message}, 值: {evt.Value}");
}

// 發布
EventBus.Publish(new CustomEvent
{
    Message = "測試",
    Value = 100
});

// 取消訂閱 (重要!)
void OnDestroy()
{
    EventBus.Unsubscribe<CustomEvent>(OnCustomEvent);
}
```

### 效能優化建議

1. **使用物件池**
```csharp
// 創建物件池
ObjectPool<GameObject> bulletPool = new ObjectPool<GameObject>(
    () => Instantiate(bulletPrefab),
    bullet => bullet.SetActive(true),
    bullet => bullet.SetActive(false),
    maxSize: 100
);

// 取得物件
GameObject bullet = bulletPool.Get();

// 歸還物件
bulletPool.Release(bullet);
```

2. **事件訂閱管理**
- 永遠在 OnDestroy 中取消訂閱
- 避免 lambda 表達式訂閱 (難以取消訂閱)

3. **ScriptableObject 使用**
- 遊戲資料都用 ScriptableObject
- 避免在 MonoBehaviour 中硬編碼數值

---

## 📋 更新日誌

### [2025-10-04] - 存檔系統完整實作

#### 新增
- ✅ 完整的 SaveService 實作 (加密、備份、版本控制)
- ✅ GameManager 自動存檔機制
- ✅ SaveData 資料結構 (10+ 個序列化類別)
- ✅ InventorySystem.Clear() 和 SetCapacity() 方法
- ✅ ISkillService 和 ICompanionService 介面定義
- ✅ 背包完整序列化邏輯

#### 修正
- ✅ StatType 命名統一為 `Magic` (原 `INT`)
- ✅ CombatService 方法名稱修正
- ✅ Unity API 棄用警告修正

#### 文件
- ✅ CHANGELOG.md - 詳細變更記錄
- ✅ 存檔系統使用說明

### [2025-10-03] - 核心系統重構

#### 重構
- ✅ 引入 ServiceLocator 架構
- ✅ 引入 EventBus 事件系統
- ✅ 重構所有服務層
- ✅ 整合 ObjectPool 和 StateMachine

#### 移除
- ❌ 冗餘的 Player.cs (合併到 PlayerController)
- ❌ 舊版 SaveSystem (改為 SaveService)

---

## 📞 待整理項目

### 需要確認的設計決策

1. **霸體減傷數值** 
   - 問題: SuperArmor 的物理/魔法減傷百分比未確定
   - 建議值: 物理減傷 30%, 魔法減傷 20%?

2. **穿透對霸體的效果**
   - 問題: 穿透屬性是否能降低霸體減傷?
   - 選項 A: 穿透只影響防禦,不影響霸體
   - 選項 B: 穿透同時降低防禦和霸體減傷

3. **裝備強化失敗懲罰**
   - 問題: 強化失敗時耐久降低多少?
   - 建議: 每次失敗降低 10 點耐久?

4. **商店刷新週期**
   - Fixed Shop: 24 小時刷新?
   - Wandering Shop: 6 小時刷新?
   - BlackMarket: 72 小時刷新?

### 遺留的 TODO 項目

1. **SkillService 和 CompanionService 實作**
   - 介面已定義,需要實作具體類別
   - 註冊到 ServiceLocator

2. **存檔系統反序列化**
   - ApplySkills() - 恢復已學習技能
   - ApplyCompanions() - 恢復眷屬資料
   - ApplyInventory() - 恢復背包物品 (部分完成)

3. **任務系統整合**
   - ProgressData 已包含任務欄位
   - 需要實際的任務系統

4. **Enemy Formation 系統**
   - EnemyController 基礎完成
   - 需要編隊配置 ScriptableObject
   - 需要動態生成邏輯

### 文件整理建議

以下舊文件可能需要整併或刪除:
- `README.md` (舊版,內容過時)
- `BUGFIX_REPORT.md` (已修復,可歸檔)
- `SYSTEM_ARCHITECTURE.md` 與 `ARCHITECTURE.md` 內容重複

建議保留:
- 本 README.md (整合版)
- CHANGELOG.md (版本記錄)
- SAVE_SYSTEM_README.md (存檔系統詳細說明)

---

## 📄 授權

本專案為個人學習專案。

---

**最後更新**: 2025-10-04  
**維護者**: GitHub Copilot  
**版本**: 2.0
