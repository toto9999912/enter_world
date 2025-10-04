# Unity ARPG 遊戲架構文件

## 📋 目錄
- [架構概覽](#架構概覽)
- [核心系統](#核心系統)
- [服務層](#服務層)
- [資料層](#資料層)
- [使用範例](#使用範例)
- [最佳實踐](#最佳實踐)

---

## 🏗️ 架構概覽

### 設計原則
1. **模組化**: 各系統獨立，低耦合
2. **服務導向**: 使用 ServiceLocator 管理全域服務
3. **事件驅動**: 透過 EventBus 解耦系統間通訊
4. **資料導向**: ScriptableObject 作為資料容器
5. **狀態模式**: 使用狀態機管理複雜邏輯

### 三層架構

```
┌─────────────────────────────────────┐
│   Presentation Layer (表現層)        │
│   MonoBehaviour, UI, VFX            │
└─────────────────────────────────────┘
           ↓ 使用服務
┌─────────────────────────────────────┐
│   Service Layer (服務層)             │
│   CombatService, InventoryService   │
└─────────────────────────────────────┘
           ↓ 操作資料
┌─────────────────────────────────────┐
│   Data Layer (資料層)                │
│   ScriptableObject, Pure C# Classes │
└─────────────────────────────────────┘
```

---

## 🔧 核心系統

### 1. ServiceLocator (服務定位器)

**位置**: `Assets/Scripts/Core/ServiceLocator.cs`

**用途**: 全域服務管理，避免 Singleton 濫用

```csharp
// 註冊服務
ServiceLocator.Register<ICombatService>(new CombatService());

// 取得服務
var combatService = ServiceLocator.Get<ICombatService>();

// 清除所有服務
ServiceLocator.Clear();
```

### 2. EventBus (事件總線)

**位置**: `Assets/Scripts/Core/EventBus.cs`

**用途**: 解耦系統間通訊，避免直接引用

```csharp
// 定義事件
public struct DamageDealtEvent : IGameEvent
{
    public object Attacker;
    public float Damage;
}

// 訂閱事件
EventBus.Subscribe<DamageDealtEvent>(OnDamageDealt);

// 發布事件
EventBus.Publish(new DamageDealtEvent
{
    Attacker = this,
    Damage = 100f
});

// 取消訂閱
EventBus.Unsubscribe<DamageDealtEvent>(OnDamageDealt);
```

**內建事件** (`Core/GameEvents.cs`):
- 戰鬥事件: `DamageDealtEvent`, `EntityDeathEvent`, `HealingEvent`
- 屬性事件: `StatChangedEvent`, `LevelUpEvent`
- 技能事件: `SkillLearnedEvent`, `SkillCastEvent`, `TalentLearnedEvent`
- 物品事件: `ItemAddedEvent`, `ItemRemovedEvent`, `EquipmentEquippedEvent`
- 眷屬事件: `CompanionDeployedEvent`, `CompanionDiedEvent`, `CompanionCapturedEvent`

### 3. ObjectPool (物件池)

**位置**: `Assets/Scripts/Core/ObjectPool.cs`

**用途**: 減少 GC 壓力，提升效能

```csharp
// 建立物件池
PoolManager.Instance.CreatePool("Projectile", projectilePrefab, 20);

// 從池中取得
GameObject obj = PoolManager.Instance.Get("Projectile");

// 歸還到池
PoolManager.Instance.Return("Projectile", obj);
```

### 4. StateMachine (狀態機)

**位置**: `Assets/Scripts/Core/StateMachine.cs`

**用途**: 管理複雜狀態邏輯 (AI, 技能, UI)

```csharp
// 定義狀態
public class IdleState : State<EnemyController>
{
    public override void Enter(EnemyController context) { }
    public override void Update(float deltaTime) { }
    public override void Exit() { }
}

// 使用狀態機
var stateMachine = new StateMachine<EnemyController>(this);
stateMachine.ChangeState(new IdleState());
stateMachine.Update(Time.deltaTime);
```

---

## 🎮 服務層

### ICombatService

**位置**: `Assets/Scripts/Services/CombatService.cs`

**功能**:
- 傷害計算 (元素克制、防禦減免)
- 命中判定
- 暴擊判定
- 閃避判定

```csharp
var combatService = ServiceLocator.Get<ICombatService>();

float damage = combatService.CalculateDamage(
    attacker: attackerStats,
    defender: defenderStats,
    baseDamage: 100f,
    damageType: DamageType.Magical,
    attackerElement: ElementType.Fire,
    defenderElement: ElementType.Wind
);

bool isHit = combatService.RollHit(attackerHitRate, defenderEvasion);
bool isCrit = combatService.RollCritical(critRate);
```

### IInventoryService

**位置**: `Assets/Scripts/Services/InventoryService.cs`

**功能**:
- 物品新增/移除
- 數量查詢
- 背包擴充

```csharp
var inventoryService = ServiceLocator.Get<IInventoryService>();

// 新增物品 (自動發布 ItemAddedEvent)
inventoryService.AddItem(itemData, quantity: 5);

// 移除物品 (自動發布 ItemRemovedEvent)
inventoryService.RemoveItem(itemId, quantity: 2);

// 檢查物品
bool hasItem = inventoryService.HasItem(itemId, quantity: 3);
int count = inventoryService.GetItemCount(itemId);
```

---

## 📦 資料層

### ScriptableObject 資料結構

所有遊戲資料使用 ScriptableObject 儲存，便於設計師編輯：

```
Assets/Data/
├── Items/
│   ├── Consumables/  (ConsumableData)
│   ├── Equipment/    (EquipmentData)
│   ├── Materials/    (MaterialData)
│   └── Cards/        (CardData)
├── Skills/           (SkillData)
├── Talents/          (TalentData)
├── Companions/       (CompanionData)
└── Config/
    ├── ElementalAffinityTable
    └── CompanionRarity
```

### 核心資料類別

| 類別 | 路徑 | 用途 |
|------|------|------|
| `ItemData` | `Item/ItemData.cs` | 物品基類 |
| `EquipmentData` | `Item/EquipmentData.cs` | 裝備資料 (強化、套裝) |
| `SkillData` | `Skill/SkillData.cs` | 技能資料 |
| `TalentData` | `Skill/TalentData.cs` | 天賦資料 |
| `CompanionData` | `Companion/CompanionData.cs` | 眷屬資料 |

---

## 💡 使用範例

### 範例 1: 完整戰鬥流程

```csharp
public class Player : MonoBehaviour
{
    private ICombatService combatService;

    private void Start()
    {
        combatService = ServiceLocator.Get<ICombatService>();
        EventBus.Subscribe<DamageDealtEvent>(OnDamageDealt);
    }

    public void AttackEnemy(EnemyController enemy)
    {
        // 計算傷害
        float damage = combatService.CalculateDamage(
            myStats, enemyStats,
            baseDamage: myStats.GetStat(StatType.ATK),
            DamageType.Physical,
            myElement, enemyElement
        );

        // 命中判定
        if (combatService.RollHit(myStats.GetStat(StatType.HitRate),
                                  enemyStats.GetStat(StatType.Evasion)))
        {
            // 暴擊判定
            bool isCrit = combatService.RollCritical(myStats.GetStat(StatType.CritRate));
            if (isCrit)
                damage *= myStats.GetStat(StatType.CritDamage) / 100f;

            // 造成傷害
            enemy.healthSystem.TakeDamage(damage);

            // 發布事件
            EventBus.Publish(new DamageDealtEvent
            {
                Attacker = this,
                Target = enemy,
                Damage = damage,
                IsCritical = isCrit
            });
        }
    }

    private void OnDamageDealt(DamageDealtEvent evt)
    {
        // 顯示傷害數字、播放特效等
        Debug.Log($"Dealt {evt.Damage} damage!");
    }
}
```

### 範例 2: AI 狀態機

```csharp
public class BossController : MonoBehaviour
{
    private StateMachine<BossController> stateMachine;

    private void Start()
    {
        stateMachine = new StateMachine<BossController>(this);
        stateMachine.ChangeState(new BossIdleState());
    }

    private void Update()
    {
        stateMachine.Update(Time.deltaTime);
    }

    public void OnHealthBelow50Percent()
    {
        stateMachine.ChangeState(new BossRageState());
    }
}
```

### 範例 3: 物品使用

```csharp
public class ItemUser : MonoBehaviour
{
    private IInventoryService inventoryService;

    private void Start()
    {
        inventoryService = ServiceLocator.Get<IInventoryService>();
        EventBus.Subscribe<ItemAddedEvent>(OnItemAdded);
    }

    public void UsePotion(ConsumableData potion)
    {
        if (inventoryService.HasItem(potion.itemId))
        {
            // 使用物品
            healthSystem.Heal(potion.healHP);

            // 移除物品 (自動發布 ItemRemovedEvent)
            inventoryService.RemoveItem(potion.itemId, 1);
        }
    }

    private void OnItemAdded(ItemAddedEvent evt)
    {
        Debug.Log($"獲得 {evt.ItemId} x{evt.Quantity}");
    }
}
```

---

## ✅ 最佳實踐

### 1. 服務使用原則

✅ **正確**: 在 `Awake` 或 `Start` 中快取服務
```csharp
private ICombatService combatService;
void Start()
{
    combatService = ServiceLocator.Get<ICombatService>();
}
```

❌ **錯誤**: 每次使用都查詢
```csharp
void Update()
{
    var service = ServiceLocator.Get<ICombatService>(); // 不要這樣做！
}
```

### 2. 事件訂閱原則

✅ **正確**: 在 `OnDestroy` 中取消訂閱
```csharp
void Start()
{
    EventBus.Subscribe<DamageDealtEvent>(OnDamage);
}

void OnDestroy()
{
    EventBus.Unsubscribe<DamageDealtEvent>(OnDamage);
}
```

❌ **錯誤**: 忘記取消訂閱 (記憶體洩漏！)

### 3. 物件池使用原則

✅ **正確**: 頻繁生成的物件使用物件池
```csharp
GameObject projectile = PoolManager.Instance.Get("Projectile");
// 使用完畢後
PoolManager.Instance.Return("Projectile", projectile);
```

❌ **錯誤**: 頻繁 Instantiate/Destroy (GC 壓力大)

### 4. 狀態機使用原則

✅ **正確**: 複雜邏輯使用狀態機
- AI 行為 (閒置/巡邏/追擊/攻擊)
- 技能施放 (吟唱/引導/冷卻)
- UI 流程 (主選單/遊戲中/暫停)

❌ **錯誤**: 簡單開關用狀態機 (過度設計)

### 5. ScriptableObject 原則

✅ **正確**: 唯讀資料使用 ScriptableObject
- 技能資料、物品資料、配置表

❌ **錯誤**: 需要存檔的狀態用 ScriptableObject
- 玩家背包、角色屬性 (應使用序列化類別)

---

## 🔄 從舊架構遷移

### 已移除的檔案
- ❌ `SaveSystem/SaveSystem.cs` (過於龐大，待重新設計)
- ❌ `AI/EnemyAI.cs` (已重構為 `EnemyController` + 狀態機)
- ❌ `IMPLEMENTATION_SUMMARY.md` (已合併到本文件)

### 已重構的系統
- ✅ 戰鬥系統: `CombatSystem` → `CombatService`
- ✅ 背包系統: `InventorySystem` → `InventoryService`
- ✅ 敵人 AI: `EnemyAI` → `EnemyController` + 狀態機

---

## 📚 延伸閱讀

- [Unity 設計模式](https://github.com/QianMo/Unity-Design-Pattern)
- [Game Programming Patterns](https://gameprogrammingpatterns.com/)
- [Unity Best Practices](https://unity.com/how-to/build-modular-codebase-architecture)

---

**最後更新**: 2025-10-04
**架構版本**: 2.0 (重構版)
