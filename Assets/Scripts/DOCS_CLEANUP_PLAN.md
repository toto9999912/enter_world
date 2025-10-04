# 文件清理計畫

**日期**: 2025-10-04  
**目的**: 整理重複和過時的文檔,保持文檔結構清晰

---

## 📋 文件狀態分析

### Assets/Scripts/ 目錄下的 Markdown 文件

| 文件名 | 大小 | 最後修改 | 狀態 | 建議操作 |
|--------|------|----------|------|----------|
| README.md (舊) | 4.3 KB | 2025-10-02 | ❌ 過時 | **刪除** |
| ARCHITECTURE.md | 11 KB | 2025-10-04 | ⚠️ 重複 | **移動到歷史** |
| SYSTEM_ARCHITECTURE.md | 14 KB | 2025-10-04 | ⚠️ 重複 | **移動到歷史** |
| BUGFIX_REPORT.md | 5 KB | 2025-10-04 | ✅ 已修復 | **移動到歷史** |
| FILE_STRUCTURE.md | 6.9 KB | 2025-10-04 | ⚠️ 重複 | **移動到歷史** |
| CORE_FEATURES_STATUS.md | 7 KB | 2025-10-04 | ⚠️ 重複 | **移動到歷史** |
| CHANGELOG.md | 7.2 KB | 2025-10-04 | ✅ 保留 | **保留** |
| SAVE_SYSTEM_README.md | 5.8 KB | 2025-10-04 | ✅ 保留 | **保留** |

### 根目錄的 Markdown 文件

| 文件名 | 狀態 | 說明 |
|--------|------|------|
| README.md (新) | ✅ 主文檔 | 已整合所有文檔內容 |

---

## 🎯 清理策略

### 方案 A: 完整清理 (推薦)

**保留文件** (在 Assets/Scripts/):
1. ✅ **CHANGELOG.md** - 版本歷史,持續更新
2. ✅ **SAVE_SYSTEM_README.md** - 存檔系統專門文檔,內容詳細

**移動到 Assets/Scripts/Docs/Archive/** (歷史文檔):
1. ARCHITECTURE.md → Archive/ARCHITECTURE_v1.0.md
2. SYSTEM_ARCHITECTURE.md → Archive/SYSTEM_ARCHITECTURE_v1.0.md
3. FILE_STRUCTURE.md → Archive/FILE_STRUCTURE_v1.0.md
4. CORE_FEATURES_STATUS.md → Archive/CORE_FEATURES_STATUS_2025-10-04.md
5. BUGFIX_REPORT.md → Archive/BUGFIX_REPORT_2025-10-04.md

**直接刪除**:
1. ❌ Assets/Scripts/README.md (舊版,已被根目錄的新版取代)

**保留在根目錄**:
1. ✅ README.md (主要文檔,已整合所有內容)

---

### 方案 B: 保守清理

如果您想保留更多歷史文檔作為參考:

**保留所有文件,但組織結構**:
```
Assets/Scripts/
├── CHANGELOG.md                    (當前版本記錄)
├── SAVE_SYSTEM_README.md          (專門文檔)
└── Docs/
    ├── Archive/                    (歷史文檔)
    │   ├── ARCHITECTURE_v1.0.md
    │   ├── SYSTEM_ARCHITECTURE_v1.0.md
    │   ├── FILE_STRUCTURE_v1.0.md
    │   ├── CORE_FEATURES_STATUS_2025-10-04.md
    │   └── BUGFIX_REPORT_2025-10-04.md
    └── README_OLD.md               (舊版參考)
```

---

## 📝 執行步驟

### 步驟 1: 創建歷史檔案目錄
```powershell
New-Item -ItemType Directory -Path "Assets/Scripts/Docs/Archive" -Force
```

### 步驟 2: 移動文件到歷史檔案
```powershell
# 移動並重命名
Move-Item "Assets/Scripts/ARCHITECTURE.md" "Assets/Scripts/Docs/Archive/ARCHITECTURE_v1.0.md"
Move-Item "Assets/Scripts/SYSTEM_ARCHITECTURE.md" "Assets/Scripts/Docs/Archive/SYSTEM_ARCHITECTURE_v1.0.md"
Move-Item "Assets/Scripts/FILE_STRUCTURE.md" "Assets/Scripts/Docs/Archive/FILE_STRUCTURE_v1.0.md"
Move-Item "Assets/Scripts/CORE_FEATURES_STATUS.md" "Assets/Scripts/Docs/Archive/CORE_FEATURES_STATUS_2025-10-04.md"
Move-Item "Assets/Scripts/BUGFIX_REPORT.md" "Assets/Scripts/Docs/Archive/BUGFIX_REPORT_2025-10-04.md"
```

### 步驟 3: 刪除過時文件
```powershell
Remove-Item "Assets/Scripts/README.md" -Force
```

### 步驟 4: 提交變更
```powershell
git add .
git commit -m "docs: 整理文檔結構,歸檔歷史文件

變更:
- 刪除過時的 Assets/Scripts/README.md
- 歸檔 5 個已整合的文檔到 Docs/Archive/
- 保留 CHANGELOG.md 和 SAVE_SYSTEM_README.md
- 主文檔統一為根目錄的 README.md"
```

---

## ✅ 清理後的文檔結構

### 根目錄
```
README.md                           # 🎯 主要文檔 (整合版)
```

### Assets/Scripts/
```
Assets/Scripts/
├── CHANGELOG.md                    # 📋 版本歷史
├── SAVE_SYSTEM_README.md          # 💾 存檔系統專門文檔
└── Docs/
    └── Archive/                    # 📦 歷史文檔 (參考用)
        ├── ARCHITECTURE_v1.0.md
        ├── SYSTEM_ARCHITECTURE_v1.0.md
        ├── FILE_STRUCTURE_v1.0.md
        ├── CORE_FEATURES_STATUS_2025-10-04.md
        └── BUGFIX_REPORT_2025-10-04.md
```

---

## 🎯 優點

1. **清晰的文檔結構** - 主文檔在根目錄,專門文檔和歷史文檔分開
2. **保留歷史** - 所有文檔都被保存在 Archive,方便日後參考
3. **避免混淆** - 不會有多個 README.md 造成混淆
4. **版本標記** - 歷史文檔加上版本號和日期,清楚知道時間點
5. **Git 友好** - 文件移動 Git 可以追蹤,不會丟失歷史

---

## ⚠️ 注意事項

1. **Unity meta 檔案**: 移動文件後 Unity 會自動更新 .meta 檔案
2. **Git 追蹤**: 使用 `git mv` 或 `Move-Item` 後記得 `git add`
3. **連結失效**: 如果有其他文檔連結到這些文件,需要更新連結
4. **備份**: 執行前建議先提交當前變更或創建備份

---

## 🚀 快速執行 (方案 A)

如果您同意方案 A,可以直接執行以下命令:

```powershell
# 進入目錄
cd d:\unity_game\enter_world\Assets\Scripts

# 創建歷史檔案目錄
New-Item -ItemType Directory -Path "Docs/Archive" -Force

# 移動文件
Move-Item "ARCHITECTURE.md" "Docs/Archive/ARCHITECTURE_v1.0.md"
Move-Item "SYSTEM_ARCHITECTURE.md" "Docs/Archive/SYSTEM_ARCHITECTURE_v1.0.md"
Move-Item "FILE_STRUCTURE.md" "Docs/Archive/FILE_STRUCTURE_v1.0.md"
Move-Item "CORE_FEATURES_STATUS.md" "Docs/Archive/CORE_FEATURES_STATUS_2025-10-04.md"
Move-Item "BUGFIX_REPORT.md" "Docs/Archive/BUGFIX_REPORT_2025-10-04.md"

# 刪除舊 README
Remove-Item "README.md" -Force

# 回到根目錄
cd ../..

# 提交變更
git add .
git commit -m "docs: 整理文檔結構,歸檔歷史文件"
```

---

**建議**: 使用方案 A (完整清理),保持文檔結構清晰簡潔。
