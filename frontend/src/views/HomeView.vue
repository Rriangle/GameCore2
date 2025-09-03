<template>
  <div class="gc-home">
    <!-- 彩色看板區域 -->
    <section class="gc-tiles-section">
      <div class="gc-container">
        <div class="gc-tiles-grid">
          <div 
            v-for="(board, index) in boards" 
            :key="board.key"
            class="gc-tile gc-tile-colorful"
            :class="`gc-tile-${index + 1}`"
            @click="selectBoard(board.key)"
          >
            <div class="gc-tile-name">{{ board.name }}</div>
            <div class="gc-tile-meta">{{ board.intro }}</div>
            <div class="gc-tile-stats">
              <span class="gc-tile-stat">今日新貼 {{ board.todayPosts }}</span>
              <span class="gc-tile-stat">活躍 {{ board.activeUsers }}</span>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- 熱門文章區域 -->
    <section class="gc-hot-section">
      <div class="gc-container">
        <div class="gc-hot-panel">
          <div class="gc-panel-header">
            <span class="gc-kicker">🔥 Hot Threads</span>
            <h2 class="gc-panel-title">熱門精選</h2>
          </div>
          <div class="gc-hot-scroller">
            <div 
              v-for="(thread, index) in hotThreads" 
              :key="index"
              class="gc-hot-card"
            >
              <div class="gc-hot-title">{{ thread.title }}</div>
              <div class="gc-hot-meta">{{ thread.meta }}</div>
              <div class="gc-hot-tags">
                <span class="gc-tag">{{ thread.board }}</span>
                <span class="gc-tag">{{ thread.type }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- 主要內容區域 -->
    <main class="gc-main-content">
      <div class="gc-container">
        <div class="gc-layout">
          <!-- 中欄：文章列表 -->
          <section class="gc-feed-panel">
            <div class="gc-panel-header">
              <div class="gc-panel-title">最新文章</div>
              <span class="gc-feed-count">{{ feedCount }}</span>
              <div class="gc-segmented-control" role="tablist">
                <button 
                  v-for="filter in filters" 
                  :key="filter.key"
                  :class="['gc-seg-btn', { 'gc-seg-btn-active': activeFilter === filter.key }]"
                  @click="setFilter(filter.key)"
                  :aria-selected="activeFilter === filter.key"
                >
                  {{ filter.name }}
                </button>
              </div>
            </div>

            <!-- 置頂文章 -->
            <div v-if="pinnedPosts.length > 0" class="gc-pinned-section">
              <div 
                v-for="post in pinnedPosts" 
                :key="post.id"
                class="gc-post-row gc-post-pinned"
              >
                <div class="gc-post-avatar">📌</div>
                <div class="gc-post-content">
                  <div class="gc-post-title">{{ post.title }}</div>
                  <div class="gc-post-meta">
                    <span>@{{ post.author }}</span>
                    <span>｜</span>
                    <span>分區：<strong>{{ getBoardName(post.board) }}</strong></span>
                    <span>｜</span>
                    <span>{{ formatTime(post.minsAgo) }}</span>
                    <span 
                      v-for="tag in post.tags" 
                      :key="tag"
                      class="gc-tag"
                    >
                      {{ tag }}
                    </span>
                  </div>
                </div>
                <div class="gc-post-stats">
                  <span class="gc-stat">❤️ {{ post.likes }}</span>
                  <span class="gc-stat">💬 {{ post.replies }}</span>
                  <span class="gc-stat">👁️ {{ post.views }}</span>
                </div>
              </div>
            </div>

            <!-- 文章列表 -->
            <div class="gc-feed">
              <div 
                v-for="post in filteredPosts" 
                :key="post.id"
                class="gc-post-row"
              >
                <div class="gc-post-avatar">{{ post.author[0].toUpperCase() }}</div>
                <div class="gc-post-content">
                  <div class="gc-post-title">{{ post.title }}</div>
                  <div class="gc-post-meta">
                    <span>@{{ post.author }}</span>
                    <span>｜</span>
                    <span>分區：<strong>{{ getBoardName(post.board) }}</strong></span>
                    <span>｜</span>
                    <span>{{ formatTime(post.minsAgo) }}</span>
                    <span 
                      v-for="tag in post.tags" 
                      :key="tag"
                      class="gc-tag"
                    >
                      {{ tag }}
                    </span>
                  </div>
                </div>
                <div class="gc-post-stats">
                  <span class="gc-stat">❤️ {{ post.likes }}</span>
                  <span class="gc-stat">💬 {{ post.replies }}</span>
                  <span class="gc-stat">👁️ {{ post.views }}</span>
                </div>
              </div>
            </div>

            <!-- 查看更多按鈕 -->
            <div class="gc-more-section">
              <button class="gc-btn gc-btn-primary">查看更多</button>
            </div>
          </section>

          <!-- 右欄：側邊欄 -->
          <aside class="gc-sidebar">
            <div class="gc-sidebar-stack">
              <!-- 我的史萊姆卡片 -->
              <div class="gc-pet-card">
                <header class="gc-pet-header">
                  <div>
                    <div class="gc-pet-title">我的史萊姆</div>
                    <small class="gc-pet-subtitle">
                      <span>{{ pet.name }}</span> · Lv.<b>{{ pet.level }}</b> · XP <b>{{ pet.xp }}</b>/<b>{{ pet.xpMax }}</b>
                    </small>
                  </div>
                  <div class="gc-pet-coins">💰 <b>{{ pet.coins }}</b></div>
                </header>

                <div class="gc-pet-canvas-wrap">
                  <canvas ref="petCanvas" width="120" height="120" class="gc-pet-canvas"></canvas>
                </div>

                <div class="gc-pet-stats">
                  <div class="gc-pet-stat">
                    <label>飢餓</label>
                    <div class="gc-pet-bar" :class="{ 'gc-pet-bar-warn': pet.hunger < 40, 'gc-pet-bar-bad': pet.hunger < 20 }">
                      <div class="gc-pet-bar-fill" :style="{ width: pet.hunger + '%' }"></div>
                    </div>
                    <span>{{ pet.hunger }}</span>
                  </div>
                  <div class="gc-pet-stat">
                    <label>心情</label>
                    <div class="gc-pet-bar" :class="{ 'gc-pet-bar-warn': pet.mood < 40, 'gc-pet-bar-bad': pet.mood < 20 }">
                      <div class="gc-pet-bar-fill" :style="{ width: pet.mood + '%' }"></div>
                    </div>
                    <span>{{ pet.mood }}</span>
                  </div>
                  <div class="gc-pet-stat">
                    <label>體力</label>
                    <div class="gc-pet-bar" :class="{ 'gc-pet-bar-warn': pet.energy < 40, 'gc-pet-bar-bad': pet.energy < 20 }">
                      <div class="gc-pet-bar-fill" :style="{ width: pet.energy + '%' }"></div>
                    </div>
                    <span>{{ pet.energy }}</span>
                  </div>
                  <div class="gc-pet-stat">
                    <label>清潔</label>
                    <div class="gc-pet-bar" :class="{ 'gc-pet-bar-warn': pet.clean < 40, 'gc-pet-bar-bad': pet.clean < 20 }">
                      <div class="gc-pet-bar-fill" :style="{ width: pet.clean + '%' }"></div>
                    </div>
                    <span>{{ pet.clean }}</span>
                  </div>
                  <div class="gc-pet-stat">
                    <label>健康</label>
                    <div class="gc-pet-bar" :class="{ 'gc-pet-bar-warn': pet.health < 40, 'gc-pet-bar-bad': pet.health < 20 }">
                      <div class="gc-pet-bar-fill" :style="{ width: pet.health + '%' }"></div>
                    </div>
                    <span>{{ pet.health }}</span>
                  </div>
                </div>

                <div class="gc-pet-actions">
                  <button class="gc-pet-btn" @click="petAction('Feed')">餵食</button>
                  <button class="gc-pet-btn" @click="petAction('Bath')">洗澡</button>
                  <button class="gc-pet-btn" @click="petAction('Play')">玩耍</button>
                  <button class="gc-pet-btn" @click="petAction('Rest')">休息</button>
                  <button class="gc-pet-btn gc-pet-btn-accent" @click="petAdventure">出發冒險（每日 3 次）</button>
                </div>

                <ul class="gc-pet-log">
                  <li v-for="(log, index) in petLogs" :key="index" :class="log.type">{{ log.message }}</li>
                </ul>
              </div>

              <!-- 跑馬燈 -->
              <div class="gc-ticker">
                <div class="gc-ticker-track">
                  <span v-for="(bulletin, index) in bulletins" :key="index">🔔 {{ bulletin }}</span>
                  <span v-for="(bulletin, index) in bulletins" :key="`repeat-${index}`">🔔 {{ bulletin }}</span>
                </div>
              </div>

              <!-- 綜合排行榜 -->
              <div class="gc-panel gc-panel-pulse">
                <div class="gc-panel-header">
                  <div class="gc-panel-title">跨平台熱門（近 7 / 30 天）</div>
                  <div class="gc-segmented-control">
                    <button 
                      :class="['gc-seg-btn', { 'gc-seg-btn-active': mixPeriod === '7' }]"
                      @click="setMixPeriod('7')"
                    >
                      近 7 天
                    </button>
                    <button 
                      :class="['gc-seg-btn', { 'gc-seg-btn-active': mixPeriod === '30' }]"
                      @click="setMixPeriod('30')"
                    >
                      近 30 天
                    </button>
                  </div>
                </div>
                <div class="gc-rank-list">
                  <div 
                    v-for="(game, index) in mixRanking" 
                    :key="game.name"
                    class="gc-rank-row"
                    :class="{ 'gc-rank-row-top': index < 3, [`gc-rank-row-top${index + 1}`]: index < 3 }"
                  >
                    <div class="gc-rank-number">{{ index + 1 }}</div>
                    <div class="gc-rank-title">{{ game.name }}</div>
                    <div class="gc-rank-delta" :class="getDeltaClass(game.delta)">
                      {{ getDeltaSymbol(game.delta) }} {{ Math.abs(game.delta) }}
                    </div>
                  </div>
                </div>
                <div class="gc-panel-actions">
                  <button class="gc-btn">查看完整綜合排行</button>
                </div>
        </div>
        
              <!-- 熱門標籤 -->
              <div class="gc-panel">
                <div class="gc-panel-header">
                  <div class="gc-panel-title">熱門標籤</div>
                </div>
                <div class="gc-tags-grid">
                  <span 
                    v-for="tag in hotTags" 
                    :key="tag.name"
                    class="gc-tag gc-tag-clickable"
                    @click="searchTag(tag.name)"
                  >
                    {{ tag.name }} ({{ tag.count }})
                  </span>
                </div>
        </div>
        
              <!-- 本日人氣作者 -->
              <div class="gc-panel">
                <div class="gc-panel-header">
                  <div class="gc-panel-title">本日人氣作者</div>
                </div>
                <div class="gc-rank-list">
                  <div 
                    v-for="(author, index) in topAuthors" 
                    :key="author.name"
                    class="gc-rank-row"
                    :class="{ 'gc-rank-row-top': index < 3, [`gc-rank-row-top${index + 1}`]: index < 3 }"
                  >
                    <div class="gc-rank-number">{{ index + 1 }}</div>
                    <div class="gc-rank-content">
                      <strong class="gc-rank-title">{{ author.name }}</strong>
                      <div class="gc-rank-meta">近 24 小時發文 {{ author.posts }} ｜ 獲讚 {{ author.likes }}</div>
                    </div>
                    <div class="gc-rank-delta gc-rank-delta-up">+{{ author.trend }}</div>
                  </div>
                </div>
              </div>
            </div>
          </aside>
        </div>
      </div>
    </main>

    <!-- 類別排行榜 -->
    <section class="gc-cats-section">
      <div class="gc-container">
        <div class="gc-panel">
          <div class="gc-panel-header">
            <div class="gc-panel-title">各類遊戲分區熱度排行</div>
            <div class="gc-segmented-control">
              <button 
                :class="['gc-seg-btn', { 'gc-seg-btn-active': catsPeriod === '7' }]"
                @click="setCatsPeriod('7')"
              >
                近 7 天
              </button>
              <button 
                :class="['gc-seg-btn', { 'gc-seg-btn-active': catsPeriod === '30' }]"
                @click="setCatsPeriod('30')"
              >
                近 30 天
              </button>
            </div>
          </div>
          <div class="gc-cats-grid">
            <div 
              v-for="(category, key) in catsRanking" 
              :key="key"
              class="gc-cat-tile"
            >
              <div class="gc-cat-name">{{ key.toUpperCase() }}</div>
              <div class="gc-rank-list">
                <div 
                  v-for="(game, index) in category" 
                  :key="game"
                  class="gc-rank-row"
                  :class="{ 'gc-rank-row-top': index < 3, [`gc-rank-row-top${index + 1}`]: index < 3 }"
                >
                  <div class="gc-rank-number">{{ index + 1 }}</div>
                  <div class="gc-rank-title">{{ game }}</div>
                  <div class="gc-rank-delta" :class="getDeltaClass(getRandomDelta(index))">
                    {{ getDeltaSymbol(getRandomDelta(index)) }} {{ Math.abs(getRandomDelta(index)) }}
                  </div>
                </div>
              </div>
              <div class="gc-cat-actions">
                <button class="gc-btn">查看詳細</button>
              </div>
            </div>
          </div>
          <div class="gc-panel-actions">
            <button class="gc-btn gc-btn-primary">查看更多</button>
          </div>
        </div>
      </div>
    </section>

    <!-- 浮動發文按鈕 -->
    <button class="gc-fab" @click="openCompose">
      ＋ 發表主題
    </button>
  </div>
</template>

<script setup lang="ts">
// 首頁視圖元件
</script>

<style scoped>
/* 首頁玻璃風設計系統樣式 */

/* 主要容器 */
.gc-home {
  min-height: 100vh;
  background: 
    radial-gradient(900px 500px at -10% -10%, rgba(117, 87, 255, 0.10), transparent 60%),
    radial-gradient(800px 460px at 110% 10%, rgba(52, 210, 255, 0.10), transparent 50%),
    linear-gradient(180deg, var(--gc-bg), var(--gc-bg2));
}

/* 彩色看板區域 */
.gc-tiles-section {
  padding: var(--gc-space-8) 0;
  background: var(--gc-surface);
  backdrop-filter: blur(var(--gc-blur));
  border-bottom: 1px solid var(--gc-line);
}

.gc-tiles-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: var(--gc-space-4);
  margin-top: var(--gc-space-6);
}

.gc-tile {
  @apply gc-card p-6 cursor-pointer transition-all duration-300;
  background: var(--gc-surface);
  backdrop-filter: blur(var(--gc-blur));
  border: 1px solid var(--gc-line);
  position: relative;
  overflow: hidden;
}

.gc-tile::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, var(--gc-accent), var(--gc-accent-2));
}

.gc-tile:hover {
  transform: translateY(-4px);
  box-shadow: var(--gc-shadow-lg);
  border-color: var(--gc-accent);
}

.gc-tile-name {
  @apply text-xl font-black mb-2;
  color: var(--gc-ink);
}

.gc-tile-meta {
  @apply text-sm mb-4;
  color: var(--gc-muted);
}

.gc-tile-stats {
  display: flex;
  gap: var(--gc-space-3);
  flex-wrap: wrap;
}

.gc-tile-stat {
  @apply text-xs px-2 py-1 rounded-full;
  background: var(--gc-surface-2);
  color: var(--gc-muted);
  border: 1px solid var(--gc-line);
}

/* 熱門文章區域 */
.gc-hot-section {
  padding: var(--gc-space-8) 0;
}

.gc-hot-panel {
  @apply gc-card p-6;
  background: var(--gc-surface);
  backdrop-filter: blur(var(--gc-blur));
}

.gc-panel-header {
  display: flex;
  align-items: center;
  gap: var(--gc-space-4);
  margin-bottom: var(--gc-space-6);
}

.gc-kicker {
  @apply text-sm font-bold px-3 py-1 rounded-full;
  background: linear-gradient(135deg, var(--gc-accent), var(--gc-accent-2));
  color: white;
}

.gc-panel-title {
  @apply text-2xl font-black;
  color: var(--gc-ink);
}

.gc-hot-scroller {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: var(--gc-space-4);
}

.gc-hot-card {
  @apply p-4 rounded-xl border transition-all duration-200;
  background: var(--gc-surface-2);
  border-color: var(--gc-line);
}

.gc-hot-card:hover {
  border-color: var(--gc-accent);
  box-shadow: var(--gc-shadow);
}

.gc-hot-title {
  @apply font-bold mb-2;
  color: var(--gc-ink);
}

.gc-hot-meta {
  @apply text-sm mb-3;
  color: var(--gc-muted);
}

.gc-hot-tags {
  display: flex;
  gap: var(--gc-space-2);
  flex-wrap: wrap;
}

.gc-tag {
  @apply text-xs px-2 py-1 rounded-full;
  background: var(--gc-surface);
  color: var(--gc-muted);
  border: 1px solid var(--gc-line);
}

/* 主要內容區域 */
.gc-main-content {
  padding: var(--gc-space-8) 0;
}

.gc-layout {
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: var(--gc-space-8);
}

/* 文章列表面板 */
.gc-feed-panel {
  @apply gc-card p-6;
  background: var(--gc-surface);
  backdrop-filter: blur(var(--gc-blur));
}

.gc-feed-count {
  @apply text-sm px-2 py-1 rounded-full;
  background: var(--gc-surface-2);
  color: var(--gc-muted);
  border: 1px solid var(--gc-line);
}

.gc-segmented-control {
  display: flex;
  gap: var(--gc-space-1);
  padding: var(--gc-space-1);
  background: var(--gc-surface-2);
  border-radius: var(--gc-radius-lg);
  border: 1px solid var(--gc-line);
}

.gc-seg-btn {
  @apply px-4 py-2 text-sm font-medium rounded-lg transition-all duration-200;
  background: transparent;
  color: var(--gc-muted);
  border: none;
  cursor: pointer;
}

.gc-seg-btn:hover {
  color: var(--gc-ink);
}

.gc-seg-btn-active {
  background: var(--gc-accent);
  color: white;
}

/* 置頂文章 */
.gc-pinned-section {
  margin-bottom: var(--gc-space-6);
  padding: var(--gc-space-4);
  background: var(--gc-surface-2);
  border-radius: var(--gc-radius-lg);
  border: 1px solid var(--gc-accent);
}

.gc-post-row {
  display: flex;
  gap: var(--gc-space-4);
  padding: var(--gc-space-4) 0;
  border-bottom: 1px solid var(--gc-line);
}

.gc-post-row:last-child {
  border-bottom: none;
}

.gc-post-row.gc-post-pinned {
  background: var(--gc-surface-2);
  margin: 0 calc(-1 * var(--gc-space-4));
  padding: var(--gc-space-4);
  border-radius: var(--gc-radius-lg);
}

.gc-post-avatar {
  @apply w-10 h-10 rounded-full flex items-center justify-center text-lg font-bold;
  background: linear-gradient(135deg, var(--gc-accent), var(--gc-accent-2));
  color: white;
  flex-shrink: 0;
}

.gc-post-content {
  flex: 1;
  min-width: 0;
}

.gc-post-title {
  @apply font-bold mb-2;
  color: var(--gc-ink);
}

.gc-post-meta {
  @apply text-sm flex items-center gap-2 flex-wrap;
  color: var(--gc-muted);
}

.gc-post-stats {
  display: flex;
  gap: var(--gc-space-3);
  flex-shrink: 0;
}

.gc-stat {
  @apply text-sm;
  color: var(--gc-muted);
}

/* 文章列表 */
.gc-feed {
  margin-bottom: var(--gc-space-6);
}

/* 查看更多按鈕 */
.gc-more-section {
  text-align: center;
}

/* 側邊欄 */
.gc-sidebar {
  display: flex;
  flex-direction: column;
  gap: var(--gc-space-6);
}

.gc-sidebar-stack {
  display: flex;
  flex-direction: column;
  gap: var(--gc-space-6);
}

/* 史萊姆寵物卡片 */
.gc-pet-card {
  @apply gc-card p-6;
  background: var(--gc-surface);
  backdrop-filter: blur(var(--gc-blur));
}

.gc-pet-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--gc-space-4);
}

.gc-pet-title {
  @apply text-lg font-bold mb-1;
  color: var(--gc-ink);
}

.gc-pet-subtitle {
  @apply text-sm;
  color: var(--gc-muted);
}

.gc-pet-coins {
  @apply text-lg font-bold;
  color: var(--gc-accent);
}

.gc-pet-canvas-wrap {
  text-align: center;
  margin-bottom: var(--gc-space-4);
}

.gc-pet-canvas {
  border-radius: var(--gc-radius-lg);
  border: 2px solid var(--gc-line);
}

.gc-pet-stats {
  display: flex;
  flex-direction: column;
  gap: var(--gc-space-3);
  margin-bottom: var(--gc-space-4);
}

.gc-pet-stat {
  display: flex;
  align-items: center;
  gap: var(--gc-space-3);
}

.gc-pet-stat label {
  @apply text-sm font-medium w-16;
  color: var(--gc-ink);
}

.gc-pet-bar {
  @apply flex-1 h-2 rounded-full overflow-hidden;
  background: var(--gc-surface-2);
  border: 1px solid var(--gc-line);
}

.gc-pet-bar-fill {
  @apply h-full transition-all duration-300;
  background: linear-gradient(90deg, var(--gc-accent), var(--gc-accent-2));
}

.gc-pet-bar-warn .gc-pet-bar-fill {
  background: linear-gradient(90deg, #f59e0b, #f97316);
}

.gc-pet-bar-bad .gc-pet-bar-fill {
  background: linear-gradient(90deg, #ef4444, #dc2626);
}

.gc-pet-stat span {
  @apply text-sm font-medium w-8 text-right;
  color: var(--gc-ink);
}

.gc-pet-actions {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: var(--gc-space-2);
  margin-bottom: var(--gc-space-4);
}

.gc-pet-btn {
  @apply px-3 py-2 text-sm font-medium rounded-lg transition-all duration-200;
  background: var(--gc-surface-2);
  color: var(--gc-ink);
  border: 1px solid var(--gc-line);
  cursor: pointer;
}

.gc-pet-btn:hover {
  background: var(--gc-accent);
  color: white;
  border-color: var(--gc-accent);
}

.gc-pet-btn-accent {
  grid-column: 1 / -1;
  background: linear-gradient(135deg, var(--gc-accent), var(--gc-accent-2));
  color: white;
  border-color: transparent;
}

.gc-pet-btn-accent:hover {
  transform: translateY(-2px);
  box-shadow: var(--gc-shadow);
}

.gc-pet-log {
  @apply text-sm space-y-1;
  color: var(--gc-muted);
}

.gc-pet-log li {
  padding: var(--gc-space-1) 0;
}

.gc-pet-log li.feed {
  color: var(--gc-accent);
}

.gc-pet-log li.bath {
  color: var(--gc-accent-2);
}

.gc-pet-log li.play {
  color: #10b981;
}

.gc-pet-log li.rest {
  color: #8b5cf6;
}

/* 跑馬燈 */
.gc-ticker {
  @apply gc-card p-4 overflow-hidden;
  background: var(--gc-surface);
  backdrop-filter: blur(var(--gc-blur));
}

.gc-ticker-track {
  display: flex;
  gap: var(--gc-space-8);
  animation: gc-ticker-scroll 30s linear infinite;
  white-space: nowrap;
}

@keyframes gc-ticker-scroll {
  0% { transform: translateX(0); }
  100% { transform: translateX(-50%); }
}

.gc-ticker-track span {
  @apply text-sm;
  color: var(--gc-muted);
}

/* 面板樣式 */
.gc-panel {
  @apply gc-card p-6;
  background: var(--gc-surface);
  backdrop-filter: blur(var(--gc-blur));
}

.gc-panel-pulse {
  border: 1px solid var(--gc-accent);
  box-shadow: 0 0 20px rgba(117, 87, 255, 0.1);
}

.gc-panel-actions {
  margin-top: var(--gc-space-4);
  text-align: center;
}

/* 排行榜 */
.gc-rank-list {
  display: flex;
  flex-direction: column;
  gap: var(--gc-space-2);
  margin-top: var(--gc-space-4);
}

.gc-rank-row {
  display: flex;
  align-items: center;
  gap: var(--gc-space-3);
  padding: var(--gc-space-2);
  border-radius: var(--gc-radius-md);
  transition: all var(--gc-transition);
}

.gc-rank-row:hover {
  background: var(--gc-surface-2);
}

.gc-rank-row-top {
  background: var(--gc-surface-2);
}

.gc-rank-row-top1 {
  border: 1px solid #ffd700;
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.1), rgba(255, 215, 0, 0.05));
}

.gc-rank-row-top2 {
  border: 1px solid #c0c0c0;
  background: linear-gradient(135deg, rgba(192, 192, 192, 0.1), rgba(192, 192, 192, 0.05));
}

.gc-rank-row-top3 {
  border: 1px solid #cd7f32;
  background: linear-gradient(135deg, rgba(205, 127, 50, 0.1), rgba(205, 127, 50, 0.05));
}

.gc-rank-number {
  @apply w-6 h-6 rounded-full flex items-center justify-center text-sm font-bold;
  background: var(--gc-surface-2);
  color: var(--gc-muted);
  flex-shrink: 0;
}

.gc-rank-row-top1 .gc-rank-number {
  background: #ffd700;
  color: #000;
}

.gc-rank-row-top2 .gc-rank-number {
  background: #c0c0c0;
  color: #000;
}

.gc-rank-row-top3 .gc-rank-number {
  background: #cd7f32;
  color: #fff;
}

.gc-rank-title {
  @apply font-bold;
  color: var(--gc-ink);
}

.gc-rank-content {
  flex: 1;
  min-width: 0;
}

.gc-rank-meta {
  @apply text-sm;
  color: var(--gc-muted);
}

.gc-rank-delta {
  @apply text-sm font-medium;
  color: var(--gc-muted);
}

.gc-rank-delta-up {
  color: #10b981;
}

.gc-rank-delta-down {
  color: #ef4444;
}

/* 標籤網格 */
.gc-tags-grid {
  display: flex;
  flex-wrap: wrap;
  gap: var(--gc-space-2);
  margin-top: var(--gc-space-4);
}

.gc-tag-clickable {
  cursor: pointer;
  transition: all var(--gc-transition);
}

.gc-tag-clickable:hover {
  background: var(--gc-accent);
  color: white;
  border-color: var(--gc-accent);
}

/* 類別排行榜區域 */
.gc-cats-section {
  padding: var(--gc-space-8) 0;
  background: var(--gc-surface);
  backdrop-filter: blur(var(--gc-blur));
  border-top: 1px solid var(--gc-line);
}

.gc-cats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: var(--gc-space-6);
  margin-top: var(--gc-space-6);
}

.gc-cat-tile {
  @apply gc-card p-6;
  background: var(--gc-surface-2);
  backdrop-filter: blur(var(--gc-blur));
  border: 1px solid var(--gc-line);
}

.gc-cat-name {
  @apply text-lg font-bold mb-4 text-center;
  color: var(--gc-ink);
}

.gc-cat-actions {
  margin-top: var(--gc-space-4);
  text-align: center;
}

/* 浮動發文按鈕 */
.gc-fab {
  @apply fixed bottom-6 right-6 w-16 h-16 rounded-full text-white font-bold text-lg shadow-lg transition-all duration-300;
  background: linear-gradient(135deg, var(--gc-accent), var(--gc-accent-2));
  border: none;
  cursor: pointer;
  z-index: var(--gc-z-fixed);
}

.gc-fab:hover {
  transform: translateY(-4px);
  box-shadow: var(--gc-shadow-xl);
}

/* 響應式設計 */
@media (max-width: 1024px) {
  .gc-layout {
    grid-template-columns: 1fr;
    gap: var(--gc-space-6);
  }
  
  .gc-sidebar {
    order: -1;
  }
}

@media (max-width: 768px) {
  .gc-tiles-grid {
    grid-template-columns: 1fr;
  }
  
  .gc-hot-scroller {
    grid-template-columns: 1fr;
  }
  
  .gc-cats-grid {
    grid-template-columns: 1fr;
  }
  
  .gc-panel-header {
    flex-direction: column;
    align-items: flex-start;
    gap: var(--gc-space-3);
  }
  
  .gc-segmented-control {
    width: 100%;
    justify-content: center;
  }
}

@media (max-width: 480px) {
  .gc-tile {
    padding: var(--gc-space-4);
  }
  
  .gc-panel {
    padding: var(--gc-space-4);
  }
  
  .gc-post-row {
    flex-direction: column;
    gap: var(--gc-space-3);
  }
  
  .gc-post-stats {
    justify-content: flex-start;
  }
  
  .gc-fab {
    width: 56px;
    height: 56px;
    font-size: var(--gc-text-lg);
  }
}
</style>
