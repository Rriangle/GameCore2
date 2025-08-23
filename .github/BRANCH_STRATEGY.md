# 🌿 GameCore 分支策略

## 📋 分支概述

GameCore 專案採用 Git Flow 分支策略，確保程式碼的穩定性和可維護性。

## 🌳 主要分支

### `main` 分支
- **用途**：生產環境程式碼
- **保護規則**：
  - 禁止直接推送
  - 必須通過 Pull Request
  - 必須通過所有 CI/CD 檢查
  - 必須有至少一個審核者批准
- **合併策略**：Squash and Merge
- **自動部署**：生產環境

### `develop` 分支
- **用途**：開發主分支，整合所有功能
- **保護規則**：
  - 禁止直接推送
  - 必須通過 Pull Request
  - 必須通過所有 CI/CD 檢查
- **合併策略**：Merge Commit
- **自動部署**：開發環境

## 🌿 功能分支

### `feature/*` 分支
- **命名規範**：`feature/功能名稱`
- **來源分支**：`develop`
- **目標分支**：`develop`
- **生命週期**：功能完成後合併並刪除

**範例**：
```bash
feature/user-authentication
feature/game-analytics
feature/pet-system
```

### `bugfix/*` 分支
- **命名規範**：`bugfix/問題描述`
- **來源分支**：`develop`
- **目標分支**：`develop`
- **生命週期**：修復完成後合併並刪除

**範例**：
```bash
bugfix/login-validation-error
bugfix/database-connection-timeout
bugfix/frontend-routing-issue
```

### `hotfix/*` 分支
- **命名規範**：`hotfix/緊急修復描述`
- **來源分支**：`main`
- **目標分支**：`main` 和 `develop`
- **生命週期**：修復完成後合併並刪除

**範例**：
```bash
hotfix/security-vulnerability
hotfix/critical-api-failure
hotfix/database-performance-issue
```

## 🔄 分支工作流程

### 1. 功能開發流程
```bash
# 1. 從 develop 分支建立功能分支
git checkout develop
git pull origin develop
git checkout -b feature/new-feature

# 2. 開發功能並提交
git add .
git commit -m "feat: 新增功能描述"

# 3. 推送到遠端
git push origin feature/new-feature

# 4. 建立 Pull Request 到 develop
# 5. 審核通過後合併
# 6. 刪除功能分支
```

### 2. 錯誤修復流程
```bash
# 1. 從 develop 分支建立修復分支
git checkout develop
git pull origin develop
git checkout -b bugfix/issue-description

# 2. 修復問題並提交
git add .
git commit -m "fix: 修復問題描述"

# 3. 推送到遠端
git push origin bugfix/issue-description

# 4. 建立 Pull Request 到 develop
# 5. 審核通過後合併
# 6. 刪除修復分支
```

### 3. 緊急修復流程
```bash
# 1. 從 main 分支建立緊急修復分支
git checkout main
git pull origin main
git checkout -b hotfix/critical-issue

# 2. 修復問題並提交
git add .
git commit -m "hotfix: 緊急修復描述"

# 3. 推送到遠端
git push origin hotfix/critical-issue

# 4. 建立 Pull Request 到 main
# 5. 審核通過後合併到 main
# 6. 合併到 develop
# 7. 刪除緊急修復分支
```

## 📝 提交規範

### 提交訊息格式
```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

### 類型說明
- **feat**: 新功能
- **fix**: 錯誤修復
- **docs**: 文件更新
- **style**: 程式碼格式調整
- **refactor**: 重構
- **test**: 測試相關
- **chore**: 建置或輔助工具變更

### 範例
```bash
feat(auth): 新增 Google OAuth 登入功能
fix(api): 修復用戶註冊驗證問題
docs(readme): 更新安裝說明
style(ui): 調整按鈕樣式
refactor(database): 重構資料庫查詢邏輯
test(api): 新增 API 整合測試
chore(deps): 更新依賴套件版本
```

## 🚀 發布流程

### 1. 準備發布
```bash
# 從 develop 分支建立發布分支
git checkout develop
git pull origin develop
git checkout -b release/v1.2.0

# 更新版本號和更新日誌
# 提交版本更新
git add .
git commit -m "chore: 準備發布 v1.2.0"
git push origin release/v1.2.0
```

### 2. 測試和修復
- 在發布分支上進行最終測試
- 修復發現的問題
- 確保所有測試通過

### 3. 發布
```bash
# 合併到 main 分支
git checkout main
git merge release/v1.2.0
git tag -a v1.2.0 -m "Release version 1.2.0"
git push origin main --tags

# 合併到 develop 分支
git checkout develop
git merge release/v1.2.0
git push origin develop

# 刪除發布分支
git branch -d release/v1.2.0
git push origin --delete release/v1.2.0
```

## 🔒 分支保護規則

### main 分支保護
```yaml
# .github/branch-protection/main.yml
protection_rules:
  - branch: main
    required_status_checks:
      - ci/backend-tests
      - ci/frontend-tests
      - ci/build
      - ci/quality-gate
    required_pull_request_reviews:
      required_approving_review_count: 1
      dismiss_stale_reviews: true
      require_code_owner_reviews: true
    enforce_admins: false
    required_linear_history: false
    allow_force_pushes: false
    allow_deletions: false
    block_creations: false
    required_conversation_resolution: true
```

### develop 分支保護
```yaml
# .github/branch-protection/develop.yml
protection_rules:
  - branch: develop
    required_status_checks:
      - ci/backend-tests
      - ci/frontend-tests
      - ci/build
    required_pull_request_reviews:
      required_approving_review_count: 1
      dismiss_stale_reviews: true
    enforce_admins: false
    required_linear_history: false
    allow_force_pushes: false
    allow_deletions: false
    block_creations: false
    required_conversation_resolution: true
```

## 📊 分支管理工具

### 自動化腳本
```bash
# 建立功能分支
./scripts/create-feature-branch.sh feature-name

# 建立修復分支
./scripts/create-bugfix-branch.sh issue-description

# 建立緊急修復分支
./scripts/create-hotfix-branch.sh critical-issue

# 清理已合併分支
./scripts/cleanup-branches.sh
```

### Git Hooks
```bash
# 預提交檢查
pre-commit:
  - 程式碼格式檢查
  - 基本語法檢查
  - 測試執行

# 提交訊息檢查
commit-msg:
  - 提交訊息格式驗證
  - 類型檢查

# 推送前檢查
pre-push:
  - 完整測試套件執行
  - 程式碼品質檢查
```

## 🚨 緊急情況處理

### 生產環境問題
1. **立即評估**：確定問題的嚴重程度
2. **建立 hotfix 分支**：從 main 分支建立
3. **快速修復**：專注於最小化修復
4. **測試驗證**：確保修復有效
5. **緊急部署**：快速合併到 main 分支
6. **後續處理**：將修復合併到 develop 分支

### 回滾策略
```bash
# 回滾到上一個穩定版本
git checkout main
git revert HEAD
git push origin main

# 或回滾到特定標籤
git checkout main
git revert v1.1.0..v1.2.0
git push origin main
```

## 📚 最佳實踐

### 1. 分支命名
- 使用小寫字母和連字號
- 描述性且簡潔
- 避免特殊字元

### 2. 提交頻率
- 經常提交，小量變更
- 每個提交專注於單一功能
- 保持提交歷史清晰

### 3. 分支壽命
- 功能分支：1-2 週
- 修復分支：1-3 天
- 緊急修復分支：1 天內

### 4. 合併策略
- 使用 Pull Request 進行合併
- 保持分支歷史清晰
- 及時刪除已合併分支

---

**注意**：此分支策略會根據專案需求進行調整，請定期檢視和更新。