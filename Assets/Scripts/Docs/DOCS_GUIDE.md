# 📚 文檔結構說明

**最後更新**: 2025-10-04

---

## 🎯 當前文檔結構

### 根目錄
```
README.md                           # 🎯 主要文檔 (整合所有系統說明)
```

### Assets/Scripts/
```
Assets/Scripts/
├── CHANGELOG.md                    # 📋 版本歷史與更新記錄
├── SAVE_SYSTEM_README.md          # 💾 存檔系統專門文檔
├── DOCS_CLEANUP_PLAN.md           # 📦 文檔清理計畫 (本次整理說明)
└── Docs/
    └── Archive/                    # 📦 歷史文檔 (僅供參考)
        ├── ARCHITECTURE_v1.0.md
        ├── SYSTEM_ARCHITECTURE_v1.0.md
        ├── FILE_STRUCTURE_v1.0.md
        ├── CORE_FEATURES_STATUS_2025-10-04.md
        └── BUGFIX_REPORT_2025-10-04.md
```

---

## 📖 文檔用途

### 主要文檔

#### 1. README.md (根目錄) 🎯
**位置**: `d:\unity_game\enter_world\README.md`

**內容**:
- 專案概述與核心功能 (12/14 完成度)
- 完整系統架構 (三層架構設計)
- 目錄結構說明
- 9 大核心系統詳解
- 快速開始指南
- 開發指南 (擴展系統、事件使用)
- 更新日誌
- 待整理項目

**適用對象**: 所有開發者、新加入成員、專案概覽

**何時閱讀**: 
- 第一次接觸專案
- 需要了解整體架構
- 查找特定系統的使用方法

---

### 持續更新文檔

#### 2. CHANGELOG.md 📋
**位置**: `Assets/Scripts/CHANGELOG.md`

**內容**:
- 詳細的版本變更記錄
- 每次更新的功能清單
- 技術實作細節
- Git commit 統計
- 後續建議

**何時更新**: 每次重大更新或功能完成後

**何時閱讀**:
- 查看最近的變更
- 了解功能演進歷程
- 追蹤 TODO 完成狀態

---

### 專門文檔

#### 3. SAVE_SYSTEM_README.md 💾
**位置**: `Assets/Scripts/SAVE_SYSTEM_README.md`

**內容**:
- 存檔系統完整說明
- SaveService、SaveData、GameManager 詳解
- 自動存檔機制
- 使用範例
- 待完成事項
- 技術細節 (加密、備份、版本控制)

**何時閱讀**:
- 需要使用存檔功能
- 擴展存檔內容
- 實作序列化邏輯
- 調試存檔問題

---

### 參考文檔

#### 4. DOCS_CLEANUP_PLAN.md 📦
**位置**: `Assets/Scripts/DOCS_CLEANUP_PLAN.md`

**內容**:
- 文檔整理計畫
- 清理前後對比
- 執行步驟記錄

**何時閱讀**: 需要了解文檔整理的原因和過程

---

## 📦 歷史檔案 (僅供參考)

**位置**: `Assets/Scripts/Docs/Archive/`

這些是已經整合到主 README.md 的舊版文檔,保留僅供歷史參考:

1. **ARCHITECTURE_v1.0.md** - 舊版架構說明
2. **SYSTEM_ARCHITECTURE_v1.0.md** - 舊版系統詳解
3. **FILE_STRUCTURE_v1.0.md** - 舊版檔案結構
4. **CORE_FEATURES_STATUS_2025-10-04.md** - 2025-10-04 功能狀態快照
5. **BUGFIX_REPORT_2025-10-04.md** - 已修復的錯誤報告

**注意**: 這些文檔的內容已過時或已整合,請優先參考主 README.md

---

## 🚀 快速導航

### 我想要...

**了解專案整體** → 閱讀 `README.md`

**查看最新更新** → 閱讀 `CHANGELOG.md`

**使用存檔系統** → 閱讀 `SAVE_SYSTEM_README.md`

**查看歷史版本** → 查閱 `Docs/Archive/`

**了解特定系統** → 
- Stats 屬性系統 → README.md § 遊戲系統詳解 § 1
- Element 元素系統 → README.md § 遊戲系統詳解 § 2
- Combat 戰鬥系統 → README.md § 遊戲系統詳解 § 3
- Companion 眷屬系統 → README.md § 遊戲系統詳解 § 4
- Skill 技能系統 → README.md § 遊戲系統詳解 § 5
- 其他系統 → README.md (完整索引)

**擴展系統功能** → README.md § 開發指南

**查看待辦事項** → README.md § 待整理項目

---

## 📝 文檔維護指南

### 何時更新文檔

1. **完成新功能** → 更新 CHANGELOG.md + README.md 相應章節
2. **修復重大問題** → 記錄在 CHANGELOG.md
3. **架構變更** → 更新 README.md 的系統架構部分
4. **新增系統** → 在 README.md 新增章節
5. **存檔相關變更** → 更新 SAVE_SYSTEM_README.md

### 文檔撰寫原則

1. **清晰簡潔** - 使用明確的標題和分段
2. **提供範例** - 每個功能都有程式碼範例
3. **保持最新** - 功能變更時同步更新文檔
4. **使用表情符號** - 提高可讀性 (適度使用)
5. **標註狀態** - 使用 ✅❌⚠️ 標示完成狀態

### 歷史文檔管理

- 重大改版時,將舊版文檔移至 `Docs/Archive/`
- 歸檔時加上版本號或日期: `XXX_v1.0.md` 或 `XXX_2025-10-04.md`
- 保留至少一個版本作為歷史參考
- 每季度檢視歷史文檔,刪除過時且無參考價值的內容

---

## 🎓 給新加入成員的建議

### 第一天
1. 閱讀 `README.md` 的專案概述和核心功能
2. 了解三層架構設計
3. 查看目錄結構

### 第一週
1. 詳細閱讀 9 大核心系統
2. 跑一遍快速開始指南
3. 嘗試創建簡單的遊戲內容 (眷屬、技能等)

### 第二週
1. 閱讀開發指南,學習如何擴展系統
2. 查看 CHANGELOG.md 了解專案演進
3. 開始實作小功能

---

## 📞 文檔問題反饋

如果您發現文檔有以下問題:
- ❌ 內容錯誤或過時
- ❓ 說明不清楚
- 📝 缺少重要資訊
- 🔗 連結失效

請:
1. 在 README.md 底部的 "待整理項目" 區塊記錄
2. 或直接修改並提交 PR

---

**維護者**: GitHub Copilot  
**最後整理**: 2025-10-04  
**下次檢視**: 2025-11-04 (建議每月檢視一次)
