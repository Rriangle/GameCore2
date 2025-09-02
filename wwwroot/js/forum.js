/**
 * GameCore 論壇系統 JavaScript
 * 作者：GameCore 開發團隊
 * 版本：2.3
 * 描述：論壇功能、文章管理、分類篩選等
 */

// 論壇系統全域變數
let forumData = {
    boards: [],
    posts: [],
    hotPosts: [],
    pinnedPosts: [],
    currentFilter: 'all',
    currentPage: 1,
    postsPerPage: 12,
    searchQuery: ''
};

// 論壇統計資料
let forumStats = {
    totalPosts: 0,
    totalUsers: 0,
    activeUsers: 0,
    todayPosts: 0
};

// DOM 元素快取
const forumElements = {
    tileGrid: document.getElementById('tileGrid'),
    hotScroller: document.getElementById('hotScroller'),
    postFeed: document.getElementById('postFeed'),
    feedCount: document.getElementById('feedCount'),
    pinnedPosts: document.getElementById('pinnedPosts'),
    ticker: document.getElementById('ticker'),
    mixList: document.getElementById('mixList'),
    hotTags: document.getElementById('hotTags'),
    authors: document.getElementById('authors'),
    cats: document.getElementById('cats')
};

// 篩選按鈕
const filterButtons = {
    all: document.getElementById('filterAll'),
    lol: document.getElementById('filterLol'),
    steam: document.getElementById('filterSteam'),
    mobile: document.getElementById('filterMobile'),
    genshin: document.getElementById('filterGenshin'),
    mood: document.getElementById('filterMood')
};

/**
 * 初始化論壇內容
 */
function initializeForumContent() {
    console.log('📝 初始化論壇內容...');

    // 載入論壇資料
    loadForumData();

    // 初始化看板
    initializeBoards();

    // 初始化熱門文章
    initializeHotPosts();

    // 初始化文章列表
    initializePostFeed();

    // 初始化置頂文章
    initializePinnedPosts();

    // 初始化跑馬燈
    initializeTicker();

    // 初始化排行榜
    initializeLeaderboards();

    // 初始化熱門標籤
    initializeHotTags();

    // 初始化人氣作者
    initializeAuthors();

    // 綁定事件
    bindForumEvents();

    console.log('✅ 論壇內容初始化完成');
}

/**
 * 載入論壇資料
 */
function loadForumData() {
    // 看板資料
    forumData.boards = [
        {
            key: 'lol',
            name: '英雄聯盟',
            intro: '版本情報、電競賽事、教學攻略',
            color: 'linear-gradient(135deg, #4f46e5, #22d3ee)',
            todayPosts: Math.floor(Math.random() * 35) + 6,
            activeUsers: Math.floor(Math.random() * 200) + 40
        },
        {
            key: 'genshin',
            name: '原神',
            intro: '角色配隊、抽卡心得、世界探索',
            color: 'linear-gradient(135deg, #f43f5e, #f59e0b)',
            todayPosts: Math.floor(Math.random() * 35) + 6,
            activeUsers: Math.floor(Math.random() * 200) + 40
        },
        {
            key: 'steam',
            name: 'Steam 綜合',
            intro: '促銷情報、遊戲心得、實況討論',
            color: 'linear-gradient(135deg, #22c55e, #16a34a)',
            todayPosts: Math.floor(Math.random() * 35) + 6,
            activeUsers: Math.floor(Math.random() * 200) + 40
        },
        {
            key: 'mobile',
            name: '手機遊戲',
            intro: 'Android / iOS 手遊討論',
            color: 'linear-gradient(135deg, #8b5cf6, #06b6d4)',
            todayPosts: Math.floor(Math.random() * 35) + 6,
            activeUsers: Math.floor(Math.random() * 200) + 40
        },
        {
            key: 'general',
            name: '綜合討論',
            intro: '硬體外設、雜談灌水、求助問答',
            color: 'linear-gradient(135deg, #f97316, #ef4444)',
            todayPosts: Math.floor(Math.random() * 35) + 6,
            activeUsers: Math.floor(Math.random() * 200) + 40
        },
        {
            key: 'mood',
            name: '心情板',
            intro: '日常、告白、碎碎念、抱怨',
            color: 'linear-gradient(135deg, #06b6d4, #3b82f6)',
            todayPosts: Math.floor(Math.random() * 35) + 6,
            activeUsers: Math.floor(Math.random() * 200) + 40
        }
    ];

    // 生成文章資料
    generatePosts();

    // 生成熱門文章
    generateHotPosts();

    // 生成置頂文章
    generatePinnedPosts();

    console.log('📋 論壇資料載入完成');
}

/**
 * 生成文章資料
 */
function generatePosts() {
    const titles = [
        '平民向武器替代表（附表格）',
        '改版後坦克裝推薦',
        '首抽角色 CP 分析',
        '實測 120 抽紀錄',
        '打野動線更新（S 賽季）',
        '速刷日常路線（含地圖）',
        'Steam 夏促清單精選',
        '本周活動懶人包',
        '入門三天上手指南',
        '冷門角機體解構',
        '手機省電設定大全',
        '新版本更新內容整理',
        '角色配裝心得分享',
        '遊戲技巧與小知識',
        '活動攻略與獎勵分析',
        '裝備搭配建議',
        '新手常見問題解答',
        '遊戲平衡性討論',
        '競技場攻略分享',
        '副本通關技巧'
    ];

    const authors = [
        'Miko', 'Gary', 'Kira', 'Lulu', '阿筆', '神樂', '小K', 'Jerry',
        'Nia', 'Zed', 'Klein', 'Rin', 'Nova', 'Aster', '老王', '夜行貓',
        '璃月曦光', '阿傑攻略', '低調小廢物', '紙箱研究室'
    ];

    const tags = [
        '#新手求助', '#攻略', '#情報', '#閒聊', '#活動', '#同人', '#抽卡',
        '#更新', '#改版', '#Bug', '#心得', '#PVP', '#PVE', '#模擬器',
        '#競速', '#MOD', '#版務', '#公告', '#深度解析', '#開箱'
    ];

    forumData.posts = [];

    for (let i = 0; i < 120; i++) {
        const board = forumData.boards[Math.floor(Math.random() * forumData.boards.length)];
        const title = titles[Math.floor(Math.random() * titles.length)] +
            (Math.random() < 0.22 ? '（含數據圖表）' : '');
        const author = authors[Math.floor(Math.random() * authors.length)];
        const likes = Math.floor(Math.random() * 1200);
        const replies = Math.floor(Math.random() * 520);
        const views = likes * 3 + Math.floor(Math.random() * 300);
        const postTags = [];

        // 隨機選擇 1-3 個標籤
        const tagCount = Math.floor(Math.random() * 3) + 1;
        for (let j = 0; j < tagCount; j++) {
            const tag = tags[Math.floor(Math.random() * tags.length)];
            if (!postTags.includes(tag)) {
                postTags.push(tag);
            }
        }

        const minsAgo = Math.floor(Math.random() * 60 * 48) + 2;

        forumData.posts.push({
            id: i + 1,
            board: board.key,
            title: title,
            author: author,
            likes: likes,
            replies: replies,
            views: views,
            tags: postTags,
            minsAgo: minsAgo,
            boardName: board.name
        });
    }

    forumStats.totalPosts = forumData.posts.length;
    forumStats.totalUsers = authors.length;
    forumStats.activeUsers = Math.floor(Math.random() * 500) + 100;
    forumStats.todayPosts = Math.floor(Math.random() * 200) + 50;
}

/**
 * 生成熱門文章
 */
function generateHotPosts() {
    const hotTitles = [
        '焦點話題 #1｜大改版重點彙整',
        '焦點話題 #2｜新角色強度分析',
        '焦點話題 #3｜活動攻略完整版',
        '焦點話題 #4｜裝備搭配指南',
        '焦點話題 #5｜競技場心得分享',
        '焦點話題 #6｜新手入門教學',
        '焦點話題 #7｜版本更新詳解',
        '焦點話題 #8｜角色培養攻略',
        '焦點話題 #9｜副本通關技巧',
        '焦點話題 #10｜PVP 對戰心得',
        '焦點話題 #11｜活動獎勵分析',
        '焦點話題 #12｜遊戲平衡討論',
        '焦點話題 #13｜裝備評測報告',
        '焦點話題 #14｜角色配隊建議',
        '焦點話題 #15｜遊戲技巧分享',
        '焦點話題 #16｜更新內容預告'
    ];

    forumData.hotPosts = hotTitles.map((title, index) => ({
        title: title,
        description: '包含角色強度與裝備配置、關卡練度門檻、常見 QA…',
        tag: index % 2 ? '情報' : '攻略',
        board: ['LOL', '原神', 'Steam', '手機'][index % 4]
    }));
}

/**
 * 生成置頂文章
 */
function generatePinnedPosts() {
    forumData.pinnedPosts = [
        {
            title: '【置頂】站務公告與精華整理 #1',
            author: 'Admin',
            board: 'general',
            likes: Math.floor(Math.random() * 270) + 30,
            replies: Math.floor(Math.random() * 140) + 10,
            views: Math.floor(Math.random() * 5500) + 500,
            tags: ['#公告', '#精華'],
            minsAgo: Math.floor(Math.random() * 115) + 5
        },
        {
            title: '【置頂】新手指南與常見問題 #2',
            author: 'Admin',
            board: 'general',
            likes: Math.floor(Math.random() * 270) + 30,
            replies: Math.floor(Math.random() * 140) + 10,
            views: Math.floor(Math.random() * 5500) + 500,
            tags: ['#公告', '#新手'],
            minsAgo: Math.floor(Math.random() * 115) + 5
        },
        {
            title: '【置頂】版規與發文規範 #3',
            author: 'Admin',
            board: 'general',
            likes: Math.floor(Math.random() * 270) + 30,
            replies: Math.floor(Math.random() * 140) + 10,
            views: Math.floor(Math.random() * 5500) + 500,
            tags: ['#公告', '#版規'],
            minsAgo: Math.floor(Math.random() * 115) + 5
        }
    ];
}

/**
 * 初始化看板
 */
function initializeBoards() {
    if (!forumElements.tileGrid) return;

    forumElements.tileGrid.innerHTML = forumData.boards.map((board, index) => `
        <a class="tile colorful" href="#" data-board="${board.key}" style="background: ${board.color}">
            <div class="name">${board.name}</div>
            <div class="meta">${board.intro}</div>
            <div style="display:flex; gap:6px; flex-wrap:wrap; margin-top:6px">
                <span class="mini">今日新貼 ${board.todayPosts}</span>
                <span class="mini">活躍 ${board.activeUsers}</span>
            </div>
        </a>
    `).join('');

    // 綁定看板點擊事件
    forumElements.tileGrid.querySelectorAll('.tile').forEach(tile => {
        tile.addEventListener('click', (e) => {
            e.preventDefault();
            const boardKey = tile.dataset.board;
            filterPosts(boardKey);
        });
    });

    console.log('📋 看板初始化完成');
}

/**
 * 初始化熱門文章
 */
function initializeHotPosts() {
    if (!forumElements.hotScroller) return;

    forumElements.hotScroller.innerHTML = forumData.hotPosts.map(post => `
        <article class="card">
            <div style="font-weight:900">${post.title}</div>
            <div style="color:var(--muted); font-size:13px">${post.description}</div>
            <div style="display:flex; gap:8px; margin-top:8px">
                <span class="chip">${post.board}</span>
                <span class="chip">${post.tag}</span>
            </div>
        </article>
    `).join('');

    console.log('🔥 熱門文章初始化完成');
}

/**
 * 初始化文章列表
 */
function initializePostFeed() {
    renderPosts();
    updateFeedCount();
}

/**
 * 渲染文章列表
 */
function renderPosts() {
    if (!forumElements.postFeed) return;

    const filteredPosts = getFilteredPosts();
    const startIndex = (forumData.currentPage - 1) * forumData.postsPerPage;
    const endIndex = startIndex + forumData.postsPerPage;
    const postsToShow = filteredPosts.slice(startIndex, endIndex);

    forumElements.postFeed.innerHTML = postsToShow.map(post => `
        <article class="row">
            <div class="av">${post.author[0].toUpperCase()}</div>
            <div>
                <div style="font-weight:900">${post.title}</div>
                <div class="meta">
                    <span>@${post.author}</span>
                    <span>｜</span>
                    <span>分區：<strong>${post.boardName}</strong></span>
                    <span>｜</span>
                    <span>${formatTimeAgo(post.minsAgo)}</span>
                    ${post.tags.map(tag => `<span class="chip">${tag}</span>`).join('')}
                </div>
            </div>
            <div style="display:flex; gap:8px">
                <span class="ghost">❤️ ${post.likes}</span>
                <span class="ghost">💬 ${post.replies}</span>
                <span class="ghost">👁️ ${post.views}</span>
            </div>
        </article>
    `).join('');

    console.log(`📄 渲染了 ${postsToShow.length} 篇文章`);
}

/**
 * 獲取篩選後的文章
 */
function getFilteredPosts() {
    let filtered = forumData.posts;

    // 按看板篩選
    if (forumData.currentFilter !== 'all') {
        filtered = filtered.filter(post => post.board === forumData.currentFilter);
    }

    // 按搜尋關鍵字篩選
    if (forumData.searchQuery) {
        const query = forumData.searchQuery.toLowerCase();
        filtered = filtered.filter(post =>
            post.title.toLowerCase().includes(query) ||
            post.author.toLowerCase().includes(query) ||
            post.tags.some(tag => tag.toLowerCase().includes(query))
        );
    }

    return filtered;
}

/**
 * 更新文章計數
 */
function updateFeedCount() {
    if (!forumElements.feedCount) return;

    const filteredPosts = getFilteredPosts();
    const totalPosts = filteredPosts.length;
    const showingPosts = Math.min(forumData.postsPerPage, totalPosts);

    forumElements.feedCount.textContent = `顯示 ${showingPosts} / ${totalPosts} 篇`;
}

/**
 * 篩選文章
 */
function filterPosts(boardKey) {
    forumData.currentFilter = boardKey;
    forumData.currentPage = 1;

    // 更新篩選按鈕狀態
    Object.keys(filterButtons).forEach(key => {
        if (filterButtons[key]) {
            filterButtons[key].classList.remove('on');
        }
    });

    const filterMap = {
        'all': 'filterAll',
        'lol': 'filterLol',
        'steam': 'filterSteam',
        'mobile': 'filterMobile',
        'genshin': 'filterGenshin',
        'mood': 'filterMood'
    };

    const buttonKey = filterMap[boardKey] || 'filterAll';
    if (filterButtons[buttonKey]) {
        filterButtons[buttonKey].classList.add('on');
    }

    // 重新渲染文章
    renderPosts();
    updateFeedCount();

    // 滾動到文章區域
    const layout = document.querySelector('.layout');
    if (layout) {
        layout.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }

    console.log(`🔍 篩選文章: ${boardKey}`);
}

/**
 * 初始化置頂文章
 */
function initializePinnedPosts() {
    if (!forumElements.pinnedPosts) return;

    forumElements.pinnedPosts.innerHTML = forumData.pinnedPosts.map(post => `
        <article class="row">
            <div class="av">📌</div>
            <div>
                <div style="font-weight:900">${post.title}</div>
                <div class="meta">
                    <span>@${post.author}</span>
                    <span>｜</span>
                    <span>分區：<strong>${post.boardName || '綜合討論'}</strong></span>
                    <span>｜</span>
                    <span>${formatTimeAgo(post.minsAgo)}</span>
                    ${post.tags.map(tag => `<span class="chip">${tag}</span>`).join('')}
                </div>
            </div>
            <div style="display:flex; gap:8px">
                <span class="ghost">❤️ ${post.likes}</span>
                <span class="ghost">💬 ${post.replies}</span>
                <span class="ghost">👁️ ${post.views}</span>
            </div>
        </article>
    `).join('');

    console.log('📌 置頂文章初始化完成');
}

/**
 * 初始化跑馬燈
 */
function initializeTicker() {
    if (!forumElements.ticker) return;

    const bulletins = [
        '【活動】投稿最佳攻略贏鍵盤滑鼠組',
        '【公告】站務規範更新，請勿張貼攻擊性言論',
        '【社群】本月達標 2,000 則優質回覆，感謝大家！',
        '【徵稿】實況主合作專題，開放報名',
        '【修復】行動端卡片重疊在 v2 已修正'
    ];

    const twice = bulletins.concat(bulletins);
    forumElements.ticker.innerHTML = twice.map(bulletin =>
        `<span>🔔 ${bulletin}</span>`
    ).join('<span>·</span>');

    console.log('📢 跑馬燈初始化完成');
}

/**
 * 初始化排行榜
 */
function initializeLeaderboards() {
    initializeMixLeaderboard();
    initializeCatsLeaderboard();
}

/**
 * 初始化綜合排行榜
 */
function initializeMixLeaderboard() {
    if (!forumElements.mixList) return;

    const mix7Data = [
        { name: 'Elden Ring: Shadow', delta: +3 },
        { name: 'Honkai: Star Rail', delta: +1 },
        { name: 'Genshin Impact', delta: -1 },
        { name: 'Valorant', delta: +2 },
        { name: 'Monster Hunter Now', delta: +2 },
        { name: 'League of Legends', delta: +1 },
        { name: "Baldur's Gate 3", delta: -2 },
        { name: 'PUBG: BATTLEGROUNDS', delta: -1 },
        { name: 'Fortnite', delta: +1 },
        { name: 'Minecraft', delta: +1 }
    ];

    renderMixLeaderboard(mix7Data);

    // 綁定切換事件
    const mix7b = document.getElementById('mix7b');
    const mix30b = document.getElementById('mix30b');

    if (mix7b) {
        mix7b.addEventListener('click', () => {
            mix7b.classList.add('on');
            mix30b.classList.remove('on');
            renderMixLeaderboard(mix7Data);
        });
    }

    if (mix30b) {
        mix30b.addEventListener('click', () => {
            mix30b.classList.add('on');
            mix7b.classList.remove('on');
            const mix30Data = mix7Data.map(item => ({
                ...item,
                delta: item.delta + (Math.random() > 0.5 ? 1 : -1)
            }));
            renderMixLeaderboard(mix30Data);
        });
    }

    console.log('🏆 綜合排行榜初始化完成');
}

/**
 * 渲染綜合排行榜
 */
function renderMixLeaderboard(data) {
    if (!forumElements.mixList) return;

    forumElements.mixList.innerHTML = data.slice(0, 10).map((game, index) => {
        const topClass = index < 3 ? `top top${index + 1}` : '';
        const sign = game.delta > 0 ? '▲' : game.delta < 0 ? '▼' : '–';
        const deltaClass = game.delta > 0 ? 'up' : game.delta < 0 ? 'down' : 'flat';
        const deltaValue = game.delta === 0 ? 0 : Math.abs(game.delta);

        return `
            <div class="rrow ${topClass}">
                <div class="rank">${index + 1}</div>
                <div class="title-2">${game.name}</div>
                <div class="delta ${deltaClass}">${sign} ${deltaValue}</div>
            </div>
        `;
    }).join('');
}

/**
 * 初始化類別排行榜
 */
function initializeCatsLeaderboard() {
    if (!forumElements.cats) return;

    const cats7Data = {
        action: ['ELDEN RING', 'Monster Hunter Now', 'Helldivers 2', 'Fortnite', 'Sekiro'],
        rpg: ["Genshin Impact", "Baldur's Gate 3", 'Honkai: Star Rail', 'Octopath II', 'Dragon Quest XI'],
        indie: ['Hades II', 'Stardew Valley', 'Vampire Survivors', 'Dave the Diver', 'Hollow Knight'],
        mobile: ['Clash of Clans', 'Arknights', 'FGO', 'NIKKE', 'Uma Musume']
    };

    renderCatsLeaderboard(cats7Data);

    // 綁定切換事件
    const cats7b = document.getElementById('cats7b');
    const cats30b = document.getElementById('cats30b');

    if (cats7b) {
        cats7b.addEventListener('click', () => {
            cats7b.classList.add('on');
            cats30b.classList.remove('on');
            renderCatsLeaderboard(cats7Data);
        });
    }

    if (cats30b) {
        cats30b.addEventListener('click', () => {
            cats30b.classList.add('on');
            cats7b.classList.remove('on');
            const cats30Data = {
                action: ['Monster Hunter Now', 'ELDEN RING', 'Fortnite', 'Helldivers 2', 'Armored Core VI'],
                rpg: ['Honkai: Star Rail', "Genshin Impact", "Baldur's Gate 3", 'Starfield', 'Persona 5 Royal'],
                indie: ['Stardew Valley', 'Hades II', 'Hollow Knight', 'Celeste', 'RimWorld'],
                mobile: ['Clash of Clans', 'NIKKE', 'Arknights', 'FGO', 'Genshin Impact (Mobile)']
            };
            renderCatsLeaderboard(cats30Data);
        });
    }

    console.log('🏆 類別排行榜初始化完成');
}

/**
 * 渲染類別排行榜
 */
function renderCatsLeaderboard(data) {
    if (!forumElements.cats) return;

    forumElements.cats.innerHTML = Object.entries(data).map(([category, games]) => {
        const items = games.map((game, index) => {
            const topClass = index < 3 ? `top top${index + 1}` : '';
            const sign = index < 2 ? '▲' : index === 2 ? '–' : '▼';
            const deltaClass = index < 2 ? 'up' : index === 2 ? 'flat' : 'down';
            const delta = index < 2 ? Math.floor(Math.random() * 4) + 1 :
                index === 2 ? 0 : Math.floor(Math.random() * 3) + 1;

            return `
                <div class="rrow ${topClass}">
                    <div class="rank">${index + 1}</div>
                    <div class="title-2">${game}</div>
                    <div class="delta ${deltaClass}">${sign} ${delta}</div>
                </div>
            `;
        }).join('');

        return `
            <article class="tile" style="min-height:auto">
                <div class="name" style="margin-bottom:6px">${category.toUpperCase()}</div>
                <div class="list">${items}</div>
                <div style="display:flex;justify-content:flex-end;margin-top:6px">
                    <button class="btn" data-cat="${category}">查看詳細</button>
                </div>
            </article>
        `;
    }).join('');

    // 綁定查看詳細按鈕
    forumElements.cats.querySelectorAll('.btn').forEach(btn => {
        btn.addEventListener('click', () => {
            const category = btn.dataset.cat;
            showCategoryDetail(category);
        });
    });
}

/**
 * 顯示類別詳細
 */
function showCategoryDetail(category) {
    const title = `${category.toUpperCase()} 類別完整排行`;
    const games = ['Game 1', 'Game 2', 'Game 3', 'Game 4', 'Game 5', 'Game 6', 'Game 7', 'Game 8', 'Game 9', 'Game 10'];

    const content = games.map((game, index) => {
        const topClass = index < 3 ? `top top${index + 1}` : '';
        const sign = index < 2 ? '▲' : index === 2 ? '–' : '▼';
        const deltaClass = index < 2 ? 'up' : index === 2 ? 'flat' : 'down';
        const delta = index < 2 ? Math.floor(Math.random() * 4) + 1 :
            index === 2 ? 0 : Math.floor(Math.random() * 3) + 1;

        return `
            <div class="rrow ${topClass}">
                <div class="rank">${index + 1}</div>
                <div class="title-2">${game}</div>
                <div class="delta ${deltaClass}">${sign} ${delta}</div>
            </div>
        `;
    }).join('');

    showModal(title, content, '此為示範資料（可串接真實 API）');
}

/**
 * 初始化熱門標籤
 */
function initializeHotTags() {
    if (!forumElements.hotTags) return;

    const tags = [
        '#新手求助', '#攻略', '#情報', '#閒聊', '#活動', '#同人', '#抽卡',
        '#更新', '#改版', '#Bug', '#心得', '#PVP', '#PVE', '#模擬器',
        '#競速', '#MOD', '#版務', '#公告', '#深度解析', '#開箱', '#評測',
        '#配裝', '#地圖', '#速刷', '#角色', '#培養', '#召喚', '#副本'
    ];

    const hotTags = tags.map(tag => ({
        tag: tag,
        count: Math.floor(Math.random() * 316) + 5
    })).sort((a, b) => b.count - a.count).slice(0, 24);

    forumElements.hotTags.innerHTML = hotTags.map(tag =>
        `<span class="chip" style="cursor:pointer">${tag.tag} (${tag.count})</span>`
    ).join('');

    // 綁定標籤點擊事件
    forumElements.hotTags.querySelectorAll('.chip').forEach(chip => {
        chip.addEventListener('click', () => {
            const tagText = chip.textContent.split(' ')[0].replace('#', '');
            const searchInput = document.getElementById('searchInput');
            if (searchInput) {
                searchInput.value = tagText;
                handleSearch();
            }
        });
    });

    console.log('🏷️ 熱門標籤初始化完成');
}

/**
 * 初始化人氣作者
 */
function initializeAuthors() {
    if (!forumElements.authors) return;

    const authors = [
        '紙箱研究室', '夜行貓', '璃月曦光', '阿傑攻略', '老王不打野',
        '低調小廢物', 'Nia', 'Klein', 'Nova', 'Aster', '小K', '神樂'
    ];

    const authorData = authors.map((name, index) => ({
        name: name + (index > 10 ? `_${index - 10}` : ''),
        posts: Math.floor(Math.random() * 15) + 4,
        likes: Math.floor(Math.random() * 361) + 60
    }));

    forumElements.authors.innerHTML = authorData.map((author, index) => {
        const topClass = index < 3 ? `top top${index + 1}` : '';
        const delta = Math.floor(Math.random() * 12) + 1;

        return `
            <div class="rrow ${topClass}">
                <div class="rank">${index + 1}</div>
                <div>
                    <strong class="title-2">${author.name}</strong>
                    <div class="meta">近 24 小時發文 ${author.posts} ｜ 獲讚 ${author.likes}</div>
                </div>
                <div class="delta up">+${delta}</div>
            </div>
        `;
    }).join('');

    console.log('👥 人氣作者初始化完成');
}

/**
 * 綁定論壇事件
 */
function bindForumEvents() {
    // 篩選按鈕事件
    Object.keys(filterButtons).forEach(key => {
        if (filterButtons[key]) {
            filterButtons[key].addEventListener('click', () => {
                filterPosts(key);
            });
        }
    });

    // 查看更多按鈕
    const btnMoreFeed = document.getElementById('btnMoreFeed');
    if (btnMoreFeed) {
        btnMoreFeed.addEventListener('click', () => {
            console.log('📄 跳轉到完整論壇頁面');
        });
    }

    // 查看完整排行榜按鈕
    const allMix = document.getElementById('allMix');
    if (allMix) {
        allMix.addEventListener('click', () => {
            showFullLeaderboard();
        });
    }

    // 查看更多排行榜按鈕
    const allCats = document.getElementById('allCats');
    if (allCats) {
        allCats.addEventListener('click', () => {
            console.log('🏆 跳轉到完整排行榜頁面');
        });
    }

    console.log('🎧 論壇事件綁定完成');
}

/**
 * 顯示完整排行榜
 */
function showFullLeaderboard() {
    const games = [
        'Elden Ring: Shadow', 'Honkai: Star Rail', 'Genshin Impact', 'Valorant',
        'Monster Hunter Now', 'League of Legends', "Baldur's Gate 3", 'PUBG: BATTLEGROUNDS',
        'Fortnite', 'Minecraft', 'Extra Game #1', 'Extra Game #2', 'Extra Game #3',
        'Extra Game #4', 'Extra Game #5'
    ];

    const content = games.map((game, index) => {
        const delta = Math.random() > 0.5 ? 1 : -1;
        const sign = delta > 0 ? '▲' : '▼';
        const deltaClass = delta > 0 ? 'up' : 'down';
        const topClass = index < 3 ? `top top${index + 1}` : '';

        return `
            <div class="rrow ${topClass}">
                <div class="rank">${index + 1}</div>
                <div class="title-2">${game}</div>
                <div class="delta ${deltaClass}">${sign} ${Math.abs(delta)}</div>
            </div>
        `;
    }).join('');

    showModal('綜合遊戲熱度排行完整前 15 名', content, '示範視圖（可改為跳轉頁 ranks.html）');
}

/**
 * 處理搜尋
 */
function handleSearch() {
    const searchInput = document.getElementById('searchInput');
    if (!searchInput) return;

    forumData.searchQuery = searchInput.value.trim();
    forumData.currentPage = 1;

    if (forumData.searchQuery.length === 0) {
        // 清空搜尋，顯示預設內容
        renderPosts();
        updateFeedCount();
        return;
    }

    // 執行搜尋
    const results = getFilteredPosts();

    if (forumElements.postFeed) {
        forumElements.postFeed.innerHTML = results.slice(0, 40).map(post => `
            <article class="row">
                <div class="av">${post.author[0].toUpperCase()}</div>
                <div>
                    <div style="font-weight:900">${post.title}</div>
                    <div class="meta">
                        @${post.author} ｜ 分區：<strong>${post.boardName}</strong> ｜ ${formatTimeAgo(post.minsAgo)}
                        ${post.tags.map(tag => `<span class="chip">${tag}</span>`).join('')}
                    </div>
                </div>
                <div style="display:flex; gap:8px">
                    <span class="ghost">❤️ ${post.likes}</span>
                    <span class="ghost">💬 ${post.replies}</span>
                    <span class="ghost">👁️ ${post.views}</span>
                </div>
            </article>
        `).join('');
    }

    if (forumElements.feedCount) {
        forumElements.feedCount.textContent = `搜尋結果：${Math.min(40, results.length)} 篇（顯示前 40）`;
    }

    // 滾動到文章區域
    const layout = document.querySelector('.layout');
    if (layout) {
        layout.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }

    console.log(`🔍 搜尋: ${forumData.searchQuery}, 結果: ${results.length} 篇`);
}

/**
 * 格式化時間
 */
function formatTimeAgo(minutes) {
    if (minutes < 60) {
        return `${minutes} 分鐘前`;
    }

    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;

    if (hours < 24) {
        return `${hours} 小時 ${remainingMinutes} 分鐘前`;
    }

    const days = Math.floor(hours / 24);
    return `${days} 天前`;
}

// 全域函數（供其他模組調用）
window.initializeForumContent = initializeForumContent;
window.handleSearch = handleSearch;
window.filterPosts = filterPosts;
window.showCategoryDetail = showCategoryDetail;
window.showFullLeaderboard = showFullLeaderboard; 