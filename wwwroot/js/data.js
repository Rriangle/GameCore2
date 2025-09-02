/**
 * GameCore 資料生成系統 JavaScript
 * 作者：GameCore 開發團隊
 * 版本：2.3
 * 描述：生成大量假資料用於展示和測試
 */

// 資料生成系統全域變數
let dataGenerator = {
    users: [],
    posts: [],
    products: [],
    orders: [],
    leaderboards: [],
    notifications: [],
    petData: []
};

// 資料統計
let dataStats = {
    totalUsers: 0,
    totalPosts: 0,
    totalProducts: 0,
    totalOrders: 0,
    totalNotifications: 0
};

/**
 * 初始化資料生成系統
 */
function initializeDataGenerator() {
    console.log('📊 初始化資料生成系統...');

    // 生成用戶資料
    generateUsers();

    // 生成文章資料
    generatePosts();

    // 生成商品資料
    generateProducts();

    // 生成訂單資料
    generateOrders();

    // 生成排行榜資料
    generateLeaderboards();

    // 生成通知資料
    generateNotifications();

    // 生成寵物資料
    generatePetData();

    // 更新統計
    updateDataStats();

    console.log('✅ 資料生成系統初始化完成');
}

/**
 * 生成用戶資料
 */
function generateUsers() {
    const firstNames = [
        '王', '李', '張', '劉', '陳', '楊', '黃', '趙', '吳', '周',
        '徐', '孫', '馬', '朱', '胡', '郭', '何', '高', '林', '羅',
        '鄭', '梁', '謝', '宋', '唐', '許', '韓', '馮', '鄧', '曹',
        '彭', '曾', '蕭', '田', '董', '袁', '潘', '於', '蔣', '蔡',
        '余', '杜', '葉', '程', '蘇', '魏', '呂', '丁', '任', '沈'
    ];

    const lastNames = [
        '小明', '小華', '小美', '小強', '小芳', '小偉', '小玲', '小傑', '小婷', '小豪',
        '小雅', '小凱', '小雯', '小宇', '小琪', '小翔', '小慧', '小龍', '小潔', '小峰',
        '小欣', '小志', '小琳', '小文', '小君', '小建', '小芬', '小德', '小如', '小安',
        '小萍', '小國', '小麗', '小民', '小英', '小輝', '小珍', '小軍', '小燕', '小偉',
        '小華', '小芳', '小傑', '小美', '小強', '小玲', '小偉', '小婷', '小豪', '小雅'
    ];

    const usernames = [
        'gamer123', 'player456', 'game_master', 'pro_gamer', 'casual_player',
        'hardcore_gamer', 'game_lover', 'play_station', 'xbox_fan', 'nintendo_lover',
        'pc_master', 'mobile_gamer', 'retro_gamer', 'indie_lover', 'rpg_fan',
        'fps_pro', 'moba_player', 'strategy_master', 'puzzle_solver', 'adventure_seeker',
        'arcade_king', 'fighting_champ', 'racing_driver', 'sports_star', 'simulation_fan',
        'horror_lover', 'comedy_gamer', 'action_hero', 'stealth_master', 'tactical_genius',
        'creative_builder', 'explorer', 'collector', 'achievement_hunter', 'speed_runner',
        'completionist', 'modder', 'streamer', 'content_creator', 'esports_pro',
        'tournament_winner', 'community_leader', 'game_reviewer', 'beta_tester',
        'early_adopter', 'veteran_player', 'newbie', 'returning_player', 'weekend_warrior',
        'night_owl', 'early_bird', 'all_day_gamer'
    ];

    const domains = ['gmail.com', 'yahoo.com', 'hotmail.com', 'outlook.com', 'qq.com'];

    dataGenerator.users = [];

    for (let i = 0; i < 1000; i++) {
        const firstName = firstNames[Math.floor(Math.random() * firstNames.length)];
        const lastName = lastNames[Math.floor(Math.random() * lastNames.length)];
        const username = usernames[Math.floor(Math.random() * usernames.length)] +
            (Math.random() > 0.7 ? Math.floor(Math.random() * 999) : '');
        const domain = domains[Math.floor(Math.random() * domains.length)];
        const email = `${username}@${domain}`;

        const user = {
            id: i + 1,
            username: username,
            email: email,
            firstName: firstName,
            lastName: lastName,
            fullName: `${firstName}${lastName}`,
            avatar: `images/avatars/avatar-${Math.floor(Math.random() * 50) + 1}.jpg`,
            joinDate: new Date(Date.now() - Math.random() * 365 * 24 * 60 * 60 * 1000),
            lastLogin: new Date(Date.now() - Math.random() * 7 * 24 * 60 * 60 * 1000),
            level: Math.floor(Math.random() * 100) + 1,
            experience: Math.floor(Math.random() * 10000),
            posts: Math.floor(Math.random() * 500),
            likes: Math.floor(Math.random() * 5000),
            followers: Math.floor(Math.random() * 1000),
            following: Math.floor(Math.random() * 500),
            isVerified: Math.random() > 0.9,
            isPremium: Math.random() > 0.95,
            status: Math.random() > 0.98 ? 'banned' : 'active'
        };

        dataGenerator.users.push(user);
    }

    console.log(`👥 生成了 ${dataGenerator.users.length} 個用戶`);
}

/**
 * 生成文章資料
 */
function generatePosts() {
    const titles = [
        '【攻略】新手入門完整指南',
        '【心得】遊戲體驗分享',
        '【情報】最新更新內容',
        '【討論】遊戲平衡性分析',
        '【開箱】新產品評測',
        '【教學】進階技巧分享',
        '【活動】官方活動情報',
        '【閒聊】日常遊戲心得',
        '【求助】遇到問題求解',
        '【分享】精彩遊戲截圖',
        '【推薦】值得一玩的遊戲',
        '【比較】同類型遊戲對比',
        '【回顧】經典遊戲懷舊',
        '【預測】未來發展趨勢',
        '【評論】遊戲優缺點分析',
        '【攻略】BOSS 戰技巧',
        '【心得】劇情體驗感受',
        '【情報】DLC 內容預告',
        '【討論】遊戲機制探討',
        '【分享】自製 MOD 作品'
    ];

    const categories = [
        'lol', 'genshin', 'steam', 'mobile', 'general', 'mood'
    ];

    const contentTemplates = [
        '這款遊戲真的很棒，推薦大家試試看！',
        '經過長時間的遊玩，我發現了一些有趣的技巧。',
        '官方這次的更新內容相當豐富，值得期待。',
        '遊戲的平衡性還需要調整，希望開發者能注意到。',
        '新產品的外觀設計很精美，功能也很實用。',
        '這個技巧可以幫助新手快速上手遊戲。',
        '官方舉辦的活動獎勵很豐富，大家不要錯過。',
        '今天玩遊戲時遇到了一些有趣的事情。',
        '有人知道如何解決這個問題嗎？',
        '分享一些遊戲中的精彩瞬間。',
        '這款遊戲雖然小眾，但品質很高。',
        '兩款遊戲各有特色，很難選擇。',
        '經典遊戲總是讓人懷念。',
        '根據目前的趨勢，未來可能會...',
        '遊戲的優點很多，但也有一些缺點。',
        '這個 BOSS 的攻擊模式很有規律。',
        '劇情發展出乎意料，讓人印象深刻。',
        'DLC 的內容看起來很豐富。',
        '這個遊戲機制設計得很巧妙。',
        '花了很多時間製作的 MOD，希望大家喜歡。'
    ];

    dataGenerator.posts = [];

    for (let i = 0; i < 2000; i++) {
        const title = titles[Math.floor(Math.random() * titles.length)];
        const category = categories[Math.floor(Math.random() * categories.length)];
        const author = dataGenerator.users[Math.floor(Math.random() * dataGenerator.users.length)];
        const content = contentTemplates[Math.floor(Math.random() * contentTemplates.length)];

        const post = {
            id: i + 1,
            title: title,
            content: content,
            category: category,
            author: author,
            authorId: author.id,
            createdAt: new Date(Date.now() - Math.random() * 30 * 24 * 60 * 60 * 1000),
            updatedAt: new Date(Date.now() - Math.random() * 7 * 24 * 60 * 60 * 1000),
            likes: Math.floor(Math.random() * 1000),
            dislikes: Math.floor(Math.random() * 100),
            views: Math.floor(Math.random() * 10000),
            comments: Math.floor(Math.random() * 500),
            shares: Math.floor(Math.random() * 100),
            isPinned: Math.random() > 0.99,
            isSticky: Math.random() > 0.995,
            isLocked: Math.random() > 0.998,
            tags: generateRandomTags(),
            images: generateRandomImages(),
            status: Math.random() > 0.99 ? 'hidden' : 'published'
        };

        dataGenerator.posts.push(post);
    }

    console.log(`📝 生成了 ${dataGenerator.posts.length} 篇文章`);
}

/**
 * 生成商品資料
 */
function generateProducts() {
    const productNames = [
        'Steam 遊戲序號合集',
        'PS5 手把 - 限定版',
        '電競滑鼠 - RGB 版本',
        '機械鍵盤 - 青軸',
        '遊戲耳機 - 7.1 聲道',
        '電競椅 - 人體工學設計',
        '遊戲墊 - 超大尺寸',
        '手把充電器 - 快充版',
        '遊戲收納盒 - 多功能',
        '鍵盤手托 - 記憶棉材質',
        '滑鼠墊 - 防滑設計',
        '耳機架 - 木質底座',
        '遊戲手把 - 無線版本',
        '鍵盤清潔套組',
        '滑鼠腳貼 - 特氟龍材質',
        '耳機線材 - 編織線',
        '遊戲收納袋 - 防水設計',
        '鍵盤防塵罩',
        '滑鼠防滑貼',
        '耳機防塵套'
    ];

    const categories = [
        'game', 'hardware', 'accessory', 'collectible', 'digital'
    ];

    const sellers = dataGenerator.users.filter(user => Math.random() > 0.8);

    dataGenerator.products = [];

    for (let i = 0; i < 1500; i++) {
        const name = productNames[Math.floor(Math.random() * productNames.length)];
        const category = categories[Math.floor(Math.random() * categories.length)];
        const seller = sellers[Math.floor(Math.random() * sellers.length)];

        const product = {
            id: i + 1,
            name: name,
            category: category,
            seller: seller,
            sellerId: seller.id,
            description: generateProductDescription(name),
            price: Math.floor(Math.random() * 5000) + 100,
            originalPrice: Math.floor(Math.random() * 5000) + 100,
            stock: Math.floor(Math.random() * 100) + 1,
            rating: (Math.random() * 2 + 3).toFixed(1),
            sales: Math.floor(Math.random() * 1000),
            views: Math.floor(Math.random() * 5000),
            images: generateProductImages(),
            tags: generateProductTags(category),
            createdAt: new Date(Date.now() - Math.random() * 30 * 24 * 60 * 60 * 1000),
            updatedAt: new Date(Date.now() - Math.random() * 7 * 24 * 60 * 60 * 1000),
            status: Math.random() > 0.95 ? 'sold' : 'active',
            isFeatured: Math.random() > 0.98,
            isVerified: Math.random() > 0.9
        };

        dataGenerator.products.push(product);
    }

    console.log(`🛒 生成了 ${dataGenerator.products.length} 個商品`);
}

/**
 * 生成訂單資料
 */
function generateOrders() {
    const orderStatuses = ['pending', 'paid', 'shipped', 'delivered', 'cancelled'];
    const paymentMethods = ['credit_card', 'bank_transfer', 'digital_wallet', 'cash_on_delivery'];

    dataGenerator.orders = [];

    for (let i = 0; i < 3000; i++) {
        const product = dataGenerator.products[Math.floor(Math.random() * dataGenerator.products.length)];
        const buyer = dataGenerator.users[Math.floor(Math.random() * dataGenerator.users.length)];
        const quantity = Math.floor(Math.random() * 3) + 1;
        const status = orderStatuses[Math.floor(Math.random() * orderStatuses.length)];
        const paymentMethod = paymentMethods[Math.floor(Math.random() * paymentMethods.length)];

        const order = {
            id: i + 1,
            productId: product.id,
            product: product,
            buyerId: buyer.id,
            buyer: buyer,
            sellerId: product.sellerId,
            seller: product.seller,
            quantity: quantity,
            unitPrice: product.price,
            totalPrice: product.price * quantity,
            status: status,
            paymentMethod: paymentMethod,
            createdAt: new Date(Date.now() - Math.random() * 30 * 24 * 60 * 60 * 1000),
            updatedAt: new Date(Date.now() - Math.random() * 7 * 24 * 60 * 60 * 1000),
            shippedAt: status === 'shipped' || status === 'delivered' ?
                new Date(Date.now() - Math.random() * 7 * 24 * 60 * 60 * 1000) : null,
            deliveredAt: status === 'delivered' ?
                new Date(Date.now() - Math.random() * 3 * 24 * 60 * 60 * 1000) : null,
            trackingNumber: status === 'shipped' || status === 'delivered' ?
                generateTrackingNumber() : null,
            notes: Math.random() > 0.8 ? generateOrderNotes() : null
        };

        dataGenerator.orders.push(order);
    }

    console.log(`📋 生成了 ${dataGenerator.orders.length} 個訂單`);
}

/**
 * 生成排行榜資料
 */
function generateLeaderboards() {
    const gameCategories = [
        'action', 'rpg', 'strategy', 'sports', 'racing', 'fighting',
        'shooter', 'adventure', 'simulation', 'puzzle', 'indie', 'mobile'
    ];

    const timePeriods = ['daily', 'weekly', 'monthly', 'yearly'];

    dataGenerator.leaderboards = [];

    gameCategories.forEach(category => {
        timePeriods.forEach(period => {
            const leaderboard = {
                category: category,
                period: period,
                games: []
            };

            // 為每個分類生成 20 個遊戲
            for (let i = 0; i < 20; i++) {
                const game = {
                    id: i + 1,
                    name: generateGameName(category),
                    category: category,
                    score: Math.floor(Math.random() * 10000) + 1000,
                    change: Math.floor(Math.random() * 10) - 5, // -5 到 +5
                    players: Math.floor(Math.random() * 100000) + 1000,
                    rating: (Math.random() * 2 + 3).toFixed(1),
                    releaseDate: new Date(Date.now() - Math.random() * 365 * 24 * 60 * 60 * 1000),
                    publisher: generatePublisherName(),
                    platform: generatePlatform(),
                    image: `images/games/${category}-${i + 1}.jpg`
                };

                leaderboard.games.push(game);
            }

            // 按分數排序
            leaderboard.games.sort((a, b) => b.score - a.score);

            dataGenerator.leaderboards.push(leaderboard);
        });
    });

    console.log(`🏆 生成了 ${dataGenerator.leaderboards.length} 個排行榜`);
}

/**
 * 生成通知資料
 */
function generateNotifications() {
    const notificationTypes = [
        'system', 'post', 'comment', 'like', 'follow', 'order', 'promotion'
    ];

    const notificationTitles = [
        '系統公告', '新文章通知', '評論回覆', '獲得讚數', '新粉絲',
        '訂單狀態', '促銷活動', '活動提醒', '系統維護', '功能更新'
    ];

    const notificationMessages = [
        '歡迎來到 GameCore！',
        '您關注的作者發布了新文章',
        '有人回覆了您的評論',
        '您的文章獲得了新的讚數',
        '有新的用戶關注了您',
        '您的訂單狀態已更新',
        '限時促銷活動開始了',
        '別忘了參加今天的活動',
        '系統將進行維護',
        '新功能已經上線'
    ];

    dataGenerator.notifications = [];

    for (let i = 0; i < 5000; i++) {
        const type = notificationTypes[Math.floor(Math.random() * notificationTypes.length)];
        const title = notificationTitles[Math.floor(Math.random() * notificationTitles.length)];
        const message = notificationMessages[Math.floor(Math.random() * notificationMessages.length)];
        const user = dataGenerator.users[Math.floor(Math.random() * dataGenerator.users.length)];

        const notification = {
            id: i + 1,
            type: type,
            title: title,
            message: message,
            userId: user.id,
            user: user,
            isRead: Math.random() > 0.3,
            createdAt: new Date(Date.now() - Math.random() * 7 * 24 * 60 * 60 * 1000),
            readAt: Math.random() > 0.3 ?
                new Date(Date.now() - Math.random() * 3 * 24 * 60 * 60 * 1000) : null,
            actionUrl: generateActionUrl(type),
            priority: Math.random() > 0.9 ? 'high' : 'normal'
        };

        dataGenerator.notifications.push(notification);
    }

    console.log(`🔔 生成了 ${dataGenerator.notifications.length} 個通知`);
}

/**
 * 生成寵物資料
 */
function generatePetData() {
    const petNames = [
        '小可愛', '史萊姆', '果凍', '布丁', '棉花糖', '泡泡', '彩虹', '星星',
        '月亮', '太陽', '雲朵', '雪花', '火焰', '雷電', '風暴', '海洋',
        '森林', '山脈', '河流', '沙漠', '草原', '叢林', '冰原', '火山'
    ];

    const skinColors = [
        '#79b7ff', '#ff6b6b', '#4ecdc4', '#45b7d1', '#96ceb4', '#feca57',
        '#ff9ff3', '#54a0ff', '#5f27cd', '#00d2d3', '#ff9f43', '#10ac84'
    ];

    dataGenerator.petData = [];

    for (let i = 0; i < 1000; i++) {
        const user = dataGenerator.users[Math.floor(Math.random() * dataGenerator.users.length)];
        const name = petNames[Math.floor(Math.random() * petNames.length)];
        const skinColor = skinColors[Math.floor(Math.random() * skinColors.length)];

        const pet = {
            id: i + 1,
            userId: user.id,
            user: user,
            name: name,
            level: Math.floor(Math.random() * 100) + 1,
            experience: Math.floor(Math.random() * 10000),
            hunger: Math.floor(Math.random() * 100),
            mood: Math.floor(Math.random() * 100),
            energy: Math.floor(Math.random() * 100),
            cleanliness: Math.floor(Math.random() * 100),
            health: Math.floor(Math.random() * 100),
            skinColor: skinColor,
            backgroundColor: generateBackgroundColor(),
            createdAt: new Date(Date.now() - Math.random() * 365 * 24 * 60 * 60 * 1000),
            lastFed: new Date(Date.now() - Math.random() * 24 * 60 * 60 * 1000),
            lastPlayed: new Date(Date.now() - Math.random() * 12 * 60 * 60 * 1000),
            totalActions: Math.floor(Math.random() * 1000),
            achievements: generatePetAchievements()
        };

        dataGenerator.petData.push(pet);
    }

    console.log(`🐾 生成了 ${dataGenerator.petData.length} 個寵物資料`);
}

/**
 * 生成隨機標籤
 */
function generateRandomTags() {
    const allTags = [
        '#攻略', '#心得', '#情報', '#閒聊', '#活動', '#同人', '#抽卡',
        '#更新', '#改版', '#Bug', '#PVP', '#PVE', '#模擬器', '#競速',
        '#MOD', '#版務', '#公告', '#深度解析', '#開箱', '#評測',
        '#配裝', '#地圖', '#速刷', '#角色', '#培養', '#召喚', '#副本'
    ];

    const tagCount = Math.floor(Math.random() * 4) + 1;
    const tags = [];

    for (let i = 0; i < tagCount; i++) {
        const tag = allTags[Math.floor(Math.random() * allTags.length)];
        if (!tags.includes(tag)) {
            tags.push(tag);
        }
    }

    return tags;
}

/**
 * 生成隨機圖片
 */
function generateRandomImages() {
    const imageCount = Math.floor(Math.random() * 4);
    const images = [];

    for (let i = 0; i < imageCount; i++) {
        images.push(`images/posts/post-${Math.floor(Math.random() * 100) + 1}.jpg`);
    }

    return images;
}

/**
 * 生成商品描述
 */
function generateProductDescription(productName) {
    const descriptions = [
        '全新未拆封，正版保證',
        '九成新，功能正常',
        '二手良品，性價比高',
        '限量版，收藏價值高',
        '原廠正品，品質保證',
        '特價促銷，數量有限',
        '熱門商品，搶購中',
        '獨家代理，品質優良',
        '玩家推薦，口碑良好',
        '新品上市，搶先體驗'
    ];

    return descriptions[Math.floor(Math.random() * descriptions.length)];
}

/**
 * 生成商品圖片
 */
function generateProductImages() {
    const imageCount = Math.floor(Math.random() * 4) + 1;
    const images = [];

    for (let i = 0; i < imageCount; i++) {
        images.push(`images/products/product-${Math.floor(Math.random() * 50) + 1}.jpg`);
    }

    return images;
}

/**
 * 生成商品標籤
 */
function generateProductTags(category) {
    const tagSets = {
        game: ['#Steam', '#PS5', '#Xbox', '#Switch', '#PC遊戲'],
        hardware: ['#電競', '#RGB', '#無線', '#藍牙', '#快充'],
        accessory: ['#周邊', '#配件', '#保護', '#收納', '#清潔'],
        collectible: ['#限量', '#收藏', '#紀念', '#典藏', '#稀有'],
        digital: ['#序號', '#兌換', '#下載', '#數位', '#虛擬']
    };

    const tags = tagSets[category] || ['#商品', '#熱門', '#推薦'];
    const selectedTags = [];

    const tagCount = Math.floor(Math.random() * 3) + 2;
    for (let i = 0; i < tagCount; i++) {
        const tag = tags[Math.floor(Math.random() * tags.length)];
        if (!selectedTags.includes(tag)) {
            selectedTags.push(tag);
        }
    }

    return selectedTags;
}

/**
 * 生成追蹤號碼
 */
function generateTrackingNumber() {
    const letters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';
    const numbers = '0123456789';
    let tracking = '';

    for (let i = 0; i < 2; i++) {
        tracking += letters[Math.floor(Math.random() * letters.length)];
    }

    for (let i = 0; i < 9; i++) {
        tracking += numbers[Math.floor(Math.random() * numbers.length)];
    }

    return tracking;
}

/**
 * 生成訂單備註
 */
function generateOrderNotes() {
    const notes = [
        '請小心包裝，謝謝',
        '希望能盡快收到',
        '請在下午送貨',
        '請打電話確認',
        '請放在管理室',
        '請按門鈴',
        '請在門口等',
        '請打電話通知',
        '請小心輕放',
        '請準時送達'
    ];

    return notes[Math.floor(Math.random() * notes.length)];
}

/**
 * 生成遊戲名稱
 */
function generateGameName(category) {
    const gameNames = {
        action: ['Action Hero', 'Combat Master', 'Fighting Spirit', 'Warrior Quest', 'Battle Royale'],
        rpg: ['Fantasy World', 'Epic Quest', 'Magic Kingdom', 'Hero Journey', 'Legend Quest'],
        strategy: ['Strategic Mind', 'Tactical Master', 'War Commander', 'Empire Builder', 'Kingdom Manager'],
        sports: ['Sports Champion', 'Athletic Star', 'Team Player', 'Champion League', 'Sports Master'],
        racing: ['Speed Demon', 'Racing Star', 'Fast Lane', 'Speed Master', 'Racing Champion'],
        fighting: ['Fighting Master', 'Combat King', 'Fight Club', 'Warrior King', 'Battle Master'],
        shooter: ['Shooting Star', 'Gun Master', 'Combat Elite', 'War Hero', 'Sniper Elite'],
        adventure: ['Adventure Quest', 'Explorer', 'Journey Master', 'Discovery Quest', 'Adventure King'],
        simulation: ['Sim Master', 'Life Simulator', 'City Builder', 'Farm Manager', 'Business Tycoon'],
        puzzle: ['Puzzle Master', 'Brain Teaser', 'Logic Quest', 'Mind Games', 'Puzzle King'],
        indie: ['Indie Gem', 'Hidden Treasure', 'Creative Master', 'Artistic Vision', 'Indie Star'],
        mobile: ['Mobile Master', 'Pocket Game', 'Touch Master', 'Mobile Star', 'Pocket Hero']
    };

    const names = gameNames[category] || gameNames.action;
    return names[Math.floor(Math.random() * names.length)];
}

/**
 * 生成發行商名稱
 */
function generatePublisherName() {
    const publishers = [
        'GameCore Studios', 'Digital Dreams', 'Pixel Perfect', 'Virtual Reality',
        'Epic Games', 'Electronic Arts', 'Ubisoft', 'Activision', 'Nintendo',
        'Sony Interactive', 'Microsoft Studios', 'Valve Corporation', 'Rockstar Games',
        'Bethesda Softworks', 'Square Enix', 'Capcom', 'Bandai Namco', 'Konami',
        'Sega', 'Atlus', 'Koei Tecmo', 'NIS America', 'Xseed Games'
    ];

    return publishers[Math.floor(Math.random() * publishers.length)];
}

/**
 * 生成平台
 */
function generatePlatform() {
    const platforms = ['PC', 'PS5', 'PS4', 'Xbox Series X', 'Xbox One', 'Switch', 'Mobile'];
    return platforms[Math.floor(Math.random() * platforms.length)];
}

/**
 * 生成背景顏色
 */
function generateBackgroundColor() {
    const colors = ['粉藍', '粉紅', '粉綠', '粉紫', '粉黃', '粉橙', '粉白', '粉灰'];
    return colors[Math.floor(Math.random() * colors.length)];
}

/**
 * 生成寵物成就
 */
function generatePetAchievements() {
    const achievements = [
        '第一次餵食', '第一次玩耍', '第一次洗澡', '第一次休息',
        '達到 10 級', '達到 50 級', '達到 100 級', '達到 200 級',
        '連續登入 7 天', '連續登入 30 天', '連續登入 100 天',
        '完成 100 次互動', '完成 500 次互動', '完成 1000 次互動',
        '獲得完美健康', '獲得完美心情', '獲得完美體力',
        '完成所有成就', '成為史萊姆大師'
    ];

    const earnedAchievements = [];
    const achievementCount = Math.floor(Math.random() * 10) + 1;

    for (let i = 0; i < achievementCount; i++) {
        const achievement = achievements[Math.floor(Math.random() * achievements.length)];
        if (!earnedAchievements.includes(achievement)) {
            earnedAchievements.push(achievement);
        }
    }

    return earnedAchievements;
}

/**
 * 生成動作 URL
 */
function generateActionUrl(type) {
    const urls = {
        system: '/system/announcement',
        post: '/forum/post',
        comment: '/forum/comment',
        like: '/forum/like',
        follow: '/user/profile',
        order: '/market/order',
        promotion: '/market/promotion'
    };

    return urls[type] || '/';
}

/**
 * 更新資料統計
 */
function updateDataStats() {
    dataStats.totalUsers = dataGenerator.users.length;
    dataStats.totalPosts = dataGenerator.posts.length;
    dataStats.totalProducts = dataGenerator.products.length;
    dataStats.totalOrders = dataGenerator.orders.length;
    dataStats.totalNotifications = dataGenerator.notifications.length;

    console.log('📊 資料統計更新完成:', dataStats);
}

/**
 * 獲取隨機用戶
 */
function getRandomUser() {
    return dataGenerator.users[Math.floor(Math.random() * dataGenerator.users.length)];
}

/**
 * 獲取隨機文章
 */
function getRandomPost() {
    return dataGenerator.posts[Math.floor(Math.random() * dataGenerator.posts.length)];
}

/**
 * 獲取隨機商品
 */
function getRandomProduct() {
    return dataGenerator.products[Math.floor(Math.random() * dataGenerator.products.length)];
}

/**
 * 獲取隨機訂單
 */
function getRandomOrder() {
    return dataGenerator.orders[Math.floor(Math.random() * dataGenerator.orders.length)];
}

/**
 * 獲取隨機通知
 */
function getRandomNotification() {
    return dataGenerator.notifications[Math.floor(Math.random() * dataGenerator.notifications.length)];
}

/**
 * 獲取隨機寵物資料
 */
function getRandomPetData() {
    return dataGenerator.petData[Math.floor(Math.random() * dataGenerator.petData.length)];
}

/**
 * 搜尋用戶
 */
function searchUsers(query) {
    const searchTerm = query.toLowerCase();
    return dataGenerator.users.filter(user =>
        user.username.toLowerCase().includes(searchTerm) ||
        user.fullName.toLowerCase().includes(searchTerm) ||
        user.email.toLowerCase().includes(searchTerm)
    );
}

/**
 * 搜尋文章
 */
function searchPosts(query) {
    const searchTerm = query.toLowerCase();
    return dataGenerator.posts.filter(post =>
        post.title.toLowerCase().includes(searchTerm) ||
        post.content.toLowerCase().includes(searchTerm) ||
        post.author.username.toLowerCase().includes(searchTerm) ||
        post.tags.some(tag => tag.toLowerCase().includes(searchTerm))
    );
}

/**
 * 搜尋商品
 */
function searchProducts(query) {
    const searchTerm = query.toLowerCase();
    return dataGenerator.products.filter(product =>
        product.name.toLowerCase().includes(searchTerm) ||
        product.description.toLowerCase().includes(searchTerm) ||
        product.seller.username.toLowerCase().includes(searchTerm) ||
        product.tags.some(tag => tag.toLowerCase().includes(searchTerm))
    );
}

// 全域函數（供其他模組調用）
window.initializeDataGenerator = initializeDataGenerator;
window.getRandomUser = getRandomUser;
window.getRandomPost = getRandomPost;
window.getRandomProduct = getRandomProduct;
window.getRandomOrder = getRandomOrder;
window.getRandomNotification = getRandomNotification;
window.getRandomPetData = getRandomPetData;
window.searchUsers = searchUsers;
window.searchPosts = searchPosts;
window.searchProducts = searchProducts;
window.dataGenerator = dataGenerator;
window.dataStats = dataStats; 