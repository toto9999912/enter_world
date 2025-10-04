# ARPG 系統架構完整文檔

## 📋 已實作系統概覽

本文檔記錄所有已實作的核心系統及其使用方法。

**架構版本**: 2.0 (重構版 - 使用 ServiceLocator + EventBus)

---

## ✅ 已完成系統 (10/12)

### 🔧 核心架構 (NEW)
- ServiceLocator - 服務定位器
- EventBus - 事件總線
- ObjectPool - 物件池系統
- StateMachine - 狀態機
- GameServices - 服務初始化器

### 🎮 服務層 (NEW)
- CombatService - 戰鬥服務
- InventoryService - 背包服務
- SaveService - 存檔服務

### 📦 核心系統 (已重構)

### 1. Stats 屬性系統 ⭐⭐⭐
**位置:** `Assets/Scripts/Stats/`

#### 核心文件
- `StatType.cs` - 屬性類型枚舉
- `StatModifier.cs` - 屬性修飾器 (裝備加成/Buff)
- `CharacterStats.cs` - 角色屬性數據管理
- `HealthSystem.cs` - 生命值/魔力管理

#### 屬性分類
```csharp
主要屬性 (可升級分配):
├── HP - 生命值
├── MP - 魔力值
├── ATK - 攻擊力
├── INT - 元素強度 (魔法傷害)
├── DEF - 防禦力
├── SPD - 移動速度
└── SP - 精神力 (眷屬系統專用) ⭐NEW

稀有屬性 (裝備/Buff/消耗品):
├── CritRate - 爆擊率 (0-100%)
├── CritDamage - 爆擊傷害 (100-300%)
├── DodgeRate - 閃避率 (0-100%)
├── Toughness - 韌性 (降低控場命中率) ⭐UPDATED
├── Penetration - 穿透 (無視防禦)
├── SkillHaste - 技能急速 (0-40%)
└── HitRate - 命中率 (0-100%) ⭐NEW
```

#### 使用範例
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
    "iron_sword",
    "鐵劍"
);
stats.AddModifier(swordBonus);

// 取得最終屬性
float finalATK = stats.GetFinalValue(StatType.ATK); // 35

// 移除裝備
stats.RemoveModifiersBySource("iron_sword");
```

---

### 2. Element 元素系統 ⭐⭐⭐
**位置:** `Assets/Scripts/Element/`

#### 核心文件
- `ElementType.cs` - 8 大元素定義
- `ElementalAffinityTable.cs` - 完整克制表
- `ElementalDamage.cs` - 元素傷害計算

#### 元素分類
```
基礎四元素 (玩家可選主屬性):
├── Fire (火) - 克風 160%, 被水克 80%
├── Water (水) - 克火 140%, 被地克 75%
├── Earth (地) - 克水 130%, 被風克 85%
└── Wind (風) - 克地 145%, 被火克 90%

特殊元素 (遊戲限定):
├── None (無) - 對所有元素 80%
├── Light (光) - 對闇 200%, 被闇 50%
├── Dark (闇) - 對基礎元素 120%, 對光 50%
└── Divine (神) - 對所有元素 150% (最強)
```

#### 使用範例
```csharp
// 取得元素克制倍率
float multiplier = ElementalAffinityTable.GetAffinityMultiplier(
    ElementType.Fire,    // 攻擊者
    ElementType.Wind     // 防禦者
); // 返回 1.6 (160%)

// 計算元素傷害
float baseDamage = 100f;
float finalDamage = ElementalDamage.CalculateElementalDamage(
    baseDamage,
    ElementType.Fire,
    ElementType.Wind
); // 160

// 取得元素顏色 (UI)
Color fireColor = ElementalAffinityTable.GetElementColor(ElementType.Fire);
```

---

### 3. Combat 戰鬥系統 ⭐⭐⭐
**位置:** `Assets/Scripts/Combat/`

#### 核心文件
- `CombatSystem.cs` - 戰鬥計算引擎
- `DamageInfo.cs` - 傷害資訊結構
- `SuperArmor.cs` - 霸體系統 (Boss 專用)
- `CrowdControl.cs` - 控場效果系統

#### 傷害計算流程
```
1. 命中判定 (HitRate)
2. 閃避判定 (DodgeRate)
3. 計算基礎傷害 (ATK/INT × 技能倍率)
4. 爆擊判定 (CritRate → CritDamage)
5. 元素克制 (僅魔法傷害)
6. 防禦減免 (DEF - Penetration)
7. 霸體減傷 (如果有霸體)
```

#### 使用範例
```csharp
// 創建戰鬥系統
CharacterStats attackerStats = new CharacterStats();
HealthSystem attackerHealth = new HealthSystem(attackerStats);
CombatSystem combat = new CombatSystem(attackerStats, attackerHealth);
combat.Element = ElementType.Fire;

// 攻擊目標
DamageInfo damageInfo = combat.Attack(
    targetHealth,        // 目標生命系統
    targetStats,         // 目標屬性
    targetCombat,        // 目標戰鬥系統 (取得元素)
    damageMultiplier: 2.5f,
    damageType: DamageType.Magical,
    sourceDescription: "火球術"
);

// 檢查結果
if (damageInfo.isCritical)
    Debug.Log("暴擊!");
if (damageInfo.isDodged)
    Debug.Log("閃避!");

Debug.Log($"造成傷害: {damageInfo.finalDamage}");
```

---

### 4. SuperArmor 霸體/控場系統 ⭐⭐⭐
**位置:** `Assets/Scripts/Combat/SuperArmor.cs`, `CrowdControl.cs`

#### 控場類型
```csharp
CCType:
├── Stun - 暈眩 (無法行動)
├── Freeze - 冰凍 (無法移動和攻擊)
├── Slow - 減速 (移動速度降低)
├── Silence - 沉默 (無法使用技能)
├── Root - 定身 (無法移動但可攻擊)
├── Knockback - 擊飛 (強制位移)
└── Knockdown - 擊倒 (倒地無法行動)
```

#### 霸體機制
```
霸體狀態:
├── 免疫所有控場效果
├── 攻擊不可打斷
├── 高額物理/魔法減傷 (可配置)
└── 霸體值可被消耗

霸體破壞後:
├── Boss 進入暈眩狀態 (可配置時長)
├── 開始緩慢回復霸體
├── 暈眩期間可被控場技能控制
└── 韌性仍會降低控場命中率
```

#### 韌性機制 ⭐UPDATED
```
韌性不再降低爆擊率！
新功能: 降低控場技能命中率

公式:
實際命中率 = 技能基礎命中率 × (1 - 韌性 / 200)

範例:
- Boss 韌性 100
- 玩家暈眩術命中率 80%
- 實際命中率 = 80% × (1 - 100/200) = 40%
```

#### 使用範例
```csharp
// 創建 Boss 霸體
SuperArmor bossArmor = new SuperArmor(
    maxArmor: 1000f,         // 霸體值
    recoveryRate: 50f,       // 每秒恢復 50
    ccResistance: 20f,       // 額外 20% 控場抵抗
    enabled: true
);

// 更新霸體 (每幀)
void Update()
{
    bossArmor.Update(Time.deltaTime);
}

// 玩家嘗試施放控場技能
var stunEffect = new CrowdControlEffect(
    CCType.Stun,
    duration: 3f,
    intensity: 1.0f,
    source: "玩家暈眩術"
);

// 判定控場 (整合韌性)
bool success = bossArmor.TryApplyCrowdControl(
    stunEffect,
    toughness: 100f,        // Boss 韌性
    skillHitRate: 80f       // 技能基礎命中率
);

// 消耗霸體
bool resisted = bossArmor.TakeCrowdControlDamage(50f);
```

---

### 5. Companion 眷屬系統 ⭐⭐⭐
**位置:** `Assets/Scripts/Companion/`

#### 核心文件
- `CompanionData.cs` - 眷屬資料模板 (ScriptableObject)
- `CompanionInstance.cs` - 眷屬實例 (含等級/經驗/狀態)
- `CompanionManager.cs` - 眷屬管理器 (SP/重生)
- `CompanionCaptureSystem.cs` - 捕捉系統
- `CompanionRarity.cs` - 稀有度配置

#### 眷屬稀有度 (可自定義)
```
預設:
├── Common (普通) - 基礎重生時間 10 秒
├── Elite (精英) - 基礎重生時間 20 秒
├── Rare (稀有) - 基礎重生時間 40 秒
└── Legendary (傳說) - 基礎重生時間 60 秒

可在 CompanionRarityConfig (ScriptableObject) 中自定義名稱與時間
```

#### 眷屬類型 (SP 佔用)
```
├── Light (輕型) - 佔用 10 SP
├── Medium (中型) - 佔用 20 SP
├── Heavy (重型) - 佔用 40 SP
└── Special (特殊) - 自定義 SP
```

#### 精神力 (SP) 機制
```
SP 用途:
├── 決定可同時出戰的眷屬數量
├── 影響眷屬重生速度
└── SP = 0 時眷屬無法重生

重生時間公式:
基礎時間(稀有度) × (2.0 - 1.0 × SP%) × (1 ± 10% 隨機)

範例:
- 稀有眷屬基礎重生 40 秒
- 玩家 SP 50% 時: 40 × (2.0 - 0.5) = 60 秒
- 玩家 SP 0% 時: 40 × 2.0 = 80 秒
- 玩家 SP 100% 時: 40 × 1.0 = 40 秒
```

#### 捕捉系統
```
捕捉率計算公式:
基礎捕捉率
+ (1 - HP%) × 50%        // HP 越低越好抓
+ (1 - SP%) × 30%        // SP 越低越好抓
+ 等級差 × 2%            // 玩家等級高於目標
+ 網子品質加成           // 0% ~ 100%
+ 技能/道具加成

限制: 5% ~ 95% (永遠有失敗可能)

網子品質:
├── Normal (普通) - +0%
├── Good (良好) - +10%
├── Excellent (優秀) - +25%
├── Perfect (完美) - +50%
└── Master (大師) - +100%
```

#### 使用範例
```csharp
// === 1. 創建眷屬資料 (ScriptableObject) ===
// 在 Unity 中: Create > Game > Companion > Companion Data
CompanionData goblinData = ScriptableObject.CreateInstance<CompanionData>();
goblinData.companionName = "哥布林";
goblinData.rarity = CompanionRarity.Common;
goblinData.type = CompanionType.Light;
goblinData.element = ElementType.Earth;
goblinData.baseHP = 80f;
goblinData.baseATK = 15f;

// === 2. 捕捉眷屬 ===
CaptureResult result = CompanionCaptureSystem.AttemptCapture(
    goblinData,
    targetLevel: 5,
    targetHealth,
    targetStats,
    playerLevel: 10,
    netQuality: CaptureNetQuality.Good,
    bonusCaptureRate: 10f  // 技能加成
);

if (result.success)
{
    Debug.Log($"捕捉成功！捕捉率: {result.finalCaptureRate}%");
    CompanionInstance newCompanion = result.capturedCompanion;
}

// === 3. 管理眷屬 ===
CompanionManager manager = new CompanionManager(playerStats);

// 新增眷屬
manager.AddCompanion(newCompanion);

// 部署眷屬
if (manager.CanDeployMore(newCompanion.Data.GetSPCost()))
{
    manager.DeployCompanion(newCompanion);
}

// 更新管理器 (每幀)
void Update()
{
    manager.Update(Time.deltaTime);
}

// === 4. 眷屬升級 ===
newCompanion.GainExp(100);  // 獲得經驗

// === 5. 眷屬重生 ===
// 當眷屬死亡時，自動開始重生 (如果 SP > 0)
// 重生時間由 SP% 決定

// 立即復活 (特殊道具)
newCompanion.InstantRevive(hpPercent: 1.0f, mpPercent: 1.0f);
```

---

## 🔧 可配置參數總覽

所有系統都提供預設值，你可以在程式碼中調整：

### CompanionManager (眷屬重生)
```csharp
CompanionManager.reviveTimeMultiplierA = 2.0f;  // SP=0% 時倍率
CompanionManager.reviveTimeMultiplierB = 1.0f;  // SP=100% 時倍率
CompanionManager.reviveTimeVariance = 0.1f;     // ±10% 隨機浮動
```

### CompanionCaptureSystem (捕捉)
```csharp
CompanionCaptureSystem.hpFactorWeight = 0.5f;       // HP 影響權重
CompanionCaptureSystem.spFactorWeight = 0.3f;       // SP 影響權重
CompanionCaptureSystem.levelDiffWeight = 2f;        // 等級差影響
CompanionCaptureSystem.maxLevelDiffBonus = 10;      // 最大等級差獎勵
```

### SuperArmor (霸體)
```csharp
// 在創建時配置
SuperArmor bossArmor = new SuperArmor(
    maxArmor: 1000f,        // 霸體值上限
    recoveryRate: 50f,      // 每秒恢復速度
    ccResistance: 20f,      // 控場抵抗率
    enabled: true           // 是否啟用
);

// 設定霸體參數
bossArmor.SetArmorParameters(
    maxArmor: 1500f,
    recoveryRate: 100f,
    breakCooldown: 10f      // 破壞後冷卻時間
);
```

---

## 📂 目前目錄結構

```
Assets/Scripts/
├── Stats/                 # 屬性系統
│   ├── StatType.cs
│   ├── StatModifier.cs
│   ├── CharacterStats.cs
│   └── HealthSystem.cs
│
├── Element/               # 元素系統
│   ├── ElementType.cs
│   ├── ElementalAffinityTable.cs
│   └── ElementalDamage.cs
│
├── Combat/                # 戰鬥系統
│   ├── CombatSystem.cs
│   ├── DamageInfo.cs
│   ├── SuperArmor.cs
│   └── CrowdControl.cs
│
├── Companion/             # 眷屬系統
│   ├── CompanionData.cs
│   ├── CompanionInstance.cs
│   ├── CompanionManager.cs
│   ├── CompanionCaptureSystem.cs
│   └── CompanionRarity.cs
│
└── Player/                # 玩家 (舊)
    ├── Player.cs
    └── PlayerMovement.cs
```

---

## 🚀 待實作系統

以下系統尚未實作，但架構已規劃：

1. **Skill 技能系統** - 技能樹、冷卻、跨系雙倍消耗
2. **Talent 天賦系統** - 主屬性專屬天賦樹
3. **Item 物品系統** - 6 大分類、稀有度
4. **Equipment 裝備系統** - 強化 +0~+9、耐久、套裝
5. **Card 卡片系統** - 星級、融合、真實時間 CD
6. **Inventory 背包系統** - 分頁、堆疊、擴充
7. **Shop 商店系統** - 多類型商人、刷新、折扣
8. **Level/Exp 系統** - 升級、屬性點分配
9. **Enemy AI 系統** - 編隊、召喚、SP 懲罰
10. **Save/Load 系統** - JSON 存檔
11. **UI 輔助** - 傷害飄字、小地圖

---

## 💡 使用建議

### 1. 創建 ScriptableObject 資料

眷屬、裝備、技能等都使用 ScriptableObject：

```
Unity 菜單:
Create > Game > Companion > Companion Data
Create > Game > Companion > Rarity Config
```

### 2. 事件監聽

所有系統都提供事件，用於 UI 更新：

```csharp
// 監聽生命值變化
healthSystem.OnHPChanged += (currentHP, maxHP) => {
    // 更新 UI
};

// 監聽眷屬死亡
companionManager.OnCompanionDied += (companion) => {
    // 播放死亡特效
};
```

### 3. 序列化與存檔

CompanionInstance、CharacterStats 都支援 [Serializable]，可直接序列化：

```csharp
string json = JsonUtility.ToJson(companionInstance);
```

---

## 🐛 已知限制

1. **Unity Serialization**: Dictionary 不可序列化，CharacterStats 需要手動初始化
2. **元素傷害**: 目前只有魔法傷害計算元素克制
3. **霸體減傷**: 尚未實作具體減傷數值 (等你確認)
4. **目錄結構**: 尚未重構為最終架構

---

## 📞 需要確認的設計

1. **霸體減傷數值** - 物理/魔法各減多少？
2. **穿透對霸體的效果** - 用哪個方案？
3. **裝備強化失敗懲罰** - 降低耐久的具體數值？
4. **商店刷新週期** - 固定商人 vs 流浪商人的刷新時間？

---

更新日期: 2025-10-04
作者: Claude AI
版本: 1.0
