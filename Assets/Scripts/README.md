# Scripts 目錄結構說明

本專案採用模組化設計，將不同功能的程式碼分類存放。

## 📁 目錄結構

```
Assets/Scripts/
│
├── Player/                      # 玩家角色相關
│   ├── Player.cs               # 玩家主類別（整合所有系統）
│   └── PlayerMovement.cs       # 玩家移動控制器（輸入處理、移動邏輯）
│
├── Stats/                       # 屬性系統
│   ├── StatType.cs             # 屬性類型定義（HP, ATK, DEF 等）
│   ├── StatModifier.cs         # 屬性修飾器（裝備加成、Buff）
│   ├── CharacterStats.cs       # 角色屬性數據（基礎值、最終值計算）
│   └── HealthSystem.cs         # 生命值系統（當前 HP/MP 管理）
│
├── Combat/                      # 戰鬥系統
│   ├── DamageInfo.cs           # 傷害資訊結構（記錄傷害類型、爆擊等）
│   └── CombatSystem.cs         # 戰鬥計算邏輯（傷害、爆擊、防禦）
│
└── PlayerInputActions.cs        # Input System 自動生成檔案（請勿手動編輯）
```

## 🎮 系統架構說明

### Player 系統
- **Player.cs**: 角色的核心組件，整合屬性、生命值、戰鬥等所有系統
- **PlayerMovement.cs**: 獨立的移動控制器，處理輸入和物理移動

### Stats 系統（屬性系統）
角色的屬性數據管理，支援複雜的屬性計算：

1. **StatType**: 定義所有屬性類型
   - 主要屬性：HP, MP, ATK, INT, DEF, SPD
   - 稀有屬性：爆擊率、爆擊傷害、閃避率、韌性、穿透、技能急速

2. **StatModifier**: 屬性加成系統
   - Flat: 固定數值加成（+10 攻擊）
   - Percentage: 百分比加成（+15% HP）
   - Final: 最終值加成（+10% 最終傷害）

3. **CharacterStats**: 核心屬性計算
   - 管理基礎屬性值
   - 計算最終屬性值（基礎值 + 修飾器）
   - 支援裝備系統（新增/移除修飾器）

4. **HealthSystem**: 當前狀態管理
   - 管理當前 HP/MP
   - 處理傷害、治療、魔力消耗
   - 死亡/復活邏輯

### Combat 系統（戰鬥系統）
處理所有戰鬥相關計算：

1. **DamageInfo**: 傷害數據結構
   - 記錄傷害類型（物理/魔法/真實）
   - 追蹤戰鬥結果（爆擊、閃避、格擋）
   - 提供 UI 顯示資訊

2. **CombatSystem**: 戰鬥計算引擎
   - 傷害計算：基礎傷害 → 爆擊判定 → 防禦減免
   - 爆擊系統：考慮攻擊者爆擊率和目標韌性
   - 防禦系統：穿透降低有效防禦
   - 閃避系統：機率性完全迴避攻擊

## 🔧 使用方式

### 創建玩家角色

1. 在場景中創建空物件，命名為 "Player"
2. 添加 `Player.cs` 組件
3. 在 Inspector 設定初始屬性
4. （選擇性）添加 `PlayerMovement.cs` 用於移動

### 屬性系統範例

```csharp
// 獲取玩家
Player player = GetComponent<Player>();

// 查詢屬性
float maxHP = player.Stats.GetFinalValue(StatType.HP);
float currentHP = player.Health.CurrentHP;

// 裝備物品
var sword = new StatModifier(StatType.ATK, ModifierType.Flat, 20, "鐵劍");
player.EquipItem(sword);

// 卸下裝備
player.UnequipItem("鐵劍");
```

### 戰鬥系統範例

```csharp
// 攻擊敵人
DamageInfo damage = player.AttackTarget(enemy);

// 使用技能
DamageInfo skillDamage = player.UseSkill(
    enemy,
    manaCost: 20f,
    damageMultiplier: 2.5f,
    DamageType.Magical,
    "火球術"
);

// 治療
player.Health.Heal(50f);
```

## 📝 注意事項

1. **PlayerInputActions.cs** 是由 Unity Input System 自動生成的檔案
   - 請勿手動編輯此檔案
   - 需要修改輸入時，請編輯 `PlayerInputActions.inputactions` 檔案

2. **Namespace 使用**
   - Player 系統：`using Player;`
   - Stats 系統：`using Stats;`
   - Combat 系統：`using Combat;`

3. **擴展系統**
   - 新增技能系統：建議創建 `Skills/` 目錄
   - 新增 AI 系統：建議創建 `AI/` 目錄
   - 新增物品系統：建議創建 `Items/` 目錄

## 🎯 未來擴展方向

- [ ] 技能系統（Skill System）
- [ ] 物品/裝備系統（Item/Equipment System）
- [ ] 背包系統（Inventory System）
- [ ] Buff/Debuff 系統
- [ ] AI 系統（Enemy AI）
- [ ] 任務系統（Quest System）
- [ ] UI 系統（Health Bar, Damage Numbers）
