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
