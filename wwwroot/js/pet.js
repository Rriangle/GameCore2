/**
 * GameCore 虛擬寵物系統 JavaScript
 * 作者：GameCore 開發團隊
 * 版本：2.3
 * 描述：史萊姆虛擬寵物系統，包含互動、動畫、狀態管理
 */

// 虛擬寵物系統全域變數
let petState = {
    name: '小可愛',
    level: 1,
    experience: 0,
    experienceToNext: 50,
    hunger: 80,
    mood: 80,
    energy: 80,
    cleanliness: 80,
    health: 100,
    skinColor: '#79b7ff',
    backgroundColor: '#91e27c',
    lastAction: null,
    lastActionTime: null,
    dailyActions: 0,
    maxDailyActions: 3,
    lastResetDate: null
};

// 寵物動畫狀態
let animationState = {
    isAnimating: false,
    currentAnimation: null,
    animationFrame: null
};

// DOM 元素快取
const petElements = {
    canvas: document.getElementById('gc-pet-canvas'),
    name: document.getElementById('gc-pet-name'),
    level: document.getElementById('gc-pet-lv'),
    experience: document.getElementById('gc-pet-xp'),
    experienceMax: document.getElementById('gc-pet-xpmax'),
    coins: document.getElementById('gc-coins'),
    hungerBar: document.getElementById('bar-hunger'),
    hungerText: document.getElementById('txt-hunger'),
    moodBar: document.getElementById('bar-mood'),
    moodText: document.getElementById('txt-mood'),
    energyBar: document.getElementById('bar-energy'),
    energyText: document.getElementById('txt-energy'),
    cleanBar: document.getElementById('bar-clean'),
    cleanText: document.getElementById('txt-clean'),
    healthBar: document.getElementById('bar-health'),
    healthText: document.getElementById('txt-health'),
    log: document.getElementById('gc-log'),
    adventureBtn: document.getElementById('gc-adv')
};

// 經驗值升級表
const experienceTable = {
    1: 50,    // 1級升2級需要50經驗
    2: 100,   // 2級升3級需要100經驗
    3: 150,   // 3級升4級需要150經驗
    4: 200,   // 4級升5級需要200經驗
    5: 250,   // 5級升6級需要250經驗
    6: 300,   // 6級升7級需要300經驗
    7: 350,   // 7級升8級需要350經驗
    8: 400,   // 8級升9級需要400經驗
    9: 450,   // 9級升10級需要450經驗
    10: 500   // 10級升11級需要500經驗
};

/**
 * 初始化虛擬寵物系統
 */
function initializePetSystem() {
    console.log('🐾 初始化虛擬寵物系統...');
    
    // 載入寵物狀態
    loadPetState();
    
    // 檢查每日重置
    checkDailyReset();
    
    // 初始化 Canvas
    initializeCanvas();
    
    // 更新 UI
    updatePetUI();
    
    // 綁定事件監聽器
    bindPetEvents();
    
    // 開始動畫循環
    startAnimationLoop();
    
    // 開始自動衰減計時器
    startDecayTimer();
    
    console.log('✅ 虛擬寵物系統初始化完成');
}

/**
 * 載入寵物狀態
 */
function loadPetState() {
    try {
        const savedState = localStorage.getItem('gamecore_pet_state');
        if (savedState) {
            const loadedState = JSON.parse(savedState);
            petState = { ...petState, ...loadedState };
            console.log('📋 寵物狀態載入完成');
        }
    } catch (error) {
        console.error('❌ 載入寵物狀態失敗:', error);
    }
}

/**
 * 儲存寵物狀態
 */
function savePetState() {
    try {
        localStorage.setItem('gamecore_pet_state', JSON.stringify(petState));
        console.log('💾 寵物狀態已儲存');
    } catch (error) {
        console.error('❌ 儲存寵物狀態失敗:', error);
    }
}

/**
 * 檢查每日重置
 */
function checkDailyReset() {
    const today = new Date().toDateString();
    
    if (petState.lastResetDate !== today) {
        // 每日重置
        petState.dailyActions = 0;
        petState.lastResetDate = today;
        
        // 每日衰減
        applyDailyDecay();
        
        console.log('🔄 每日重置完成');
    }
}

/**
 * 應用每日衰減
 */
function applyDailyDecay() {
    petState.hunger = Math.max(0, petState.hunger - 20);
    petState.mood = Math.max(0, petState.mood - 30);
    petState.energy = Math.max(0, petState.energy - 10);
    petState.cleanliness = Math.max(0, petState.cleanliness - 20);
    petState.health = Math.max(0, petState.health - 20);
    
    // 更新健康度
    updateHealth();
    
    addPetLog('每日衰減：各項屬性有所下降', 'warn');
}

/**
 * 初始化 Canvas
 */
function initializeCanvas() {
    if (!petElements.canvas) {
        console.error('❌ 找不到寵物 Canvas 元素');
        return;
    }
    
    const ctx = petElements.canvas.getContext('2d');
    ctx.imageSmoothingEnabled = false; // 保持像素風格
    
    console.log('🎨 Canvas 初始化完成');
}

/**
 * 更新寵物 UI
 */
function updatePetUI() {
    // 更新基本資訊
    if (petElements.name) petElements.name.textContent = petState.name;
    if (petElements.level) petElements.level.textContent = petState.level;
    if (petElements.experience) petElements.experience.textContent = petState.experience;
    if (petElements.experienceMax) petElements.experienceMax.textContent = petState.experienceToNext;
    if (petElements.coins) petElements.coins.textContent = getCoins();
    
    // 更新狀態條
    updateStatusBar('hunger', petState.hunger);
    updateStatusBar('mood', petState.mood);
    updateStatusBar('energy', petState.energy);
    updateStatusBar('clean', petState.cleanliness);
    updateStatusBar('health', petState.health);
    
    // 更新冒險按鈕
    updateAdventureButton();
}

/**
 * 更新狀態條
 */
function updateStatusBar(type, value) {
    const bar = petElements[`${type}Bar`];
    const text = petElements[`${type}Text`];
    
    if (bar && text) {
        // 更新數值
        text.textContent = Math.round(value);
        
        // 更新進度條
        const percentage = Math.max(0, Math.min(100, value));
        bar.style.width = `${percentage}%`;
        
        // 更新顏色
        const barContainer = bar.parentElement;
        barContainer.classList.remove('warn', 'bad');
        
        if (percentage < 20) {
            barContainer.classList.add('bad');
        } else if (percentage < 40) {
            barContainer.classList.add('warn');
        }
    }
}

/**
 * 更新冒險按鈕
 */
function updateAdventureButton() {
    if (!petElements.adventureBtn) return;
    
    const canAdventure = canStartAdventure();
    const remainingActions = petState.maxDailyActions - petState.dailyActions;
    
    petElements.adventureBtn.disabled = !canAdventure;
    petElements.adventureBtn.textContent = `出發冒險（每日 ${remainingActions} 次）`;
    
    if (!canAdventure) {
        petElements.adventureBtn.style.opacity = '0.5';
    } else {
        petElements.adventureBtn.style.opacity = '1';
    }
}

/**
 * 檢查是否可以開始冒險
 */
function canStartAdventure() {
    return petState.dailyActions < petState.maxDailyActions &&
           petState.hunger > 0 &&
           petState.mood > 0 &&
           petState.energy > 0 &&
           petState.cleanliness > 0 &&
           petState.health > 0;
}

/**
 * 綁定寵物事件
 */
function bindPetEvents() {
    // 互動按鈕事件
    const actionButtons = document.querySelectorAll('.gc-actions button[data-act]');
    actionButtons.forEach(button => {
        button.addEventListener('click', () => {
            const action = button.getAttribute('data-act');
            performPetAction(action);
        });
    });
    
    // 冒險按鈕事件
    if (petElements.adventureBtn) {
        petElements.adventureBtn.addEventListener('click', startAdventure);
    }
    
    console.log('🎧 寵物事件綁定完成');
}

/**
 * 執行寵物互動
 */
function performPetAction(action) {
    if (animationState.isAnimating) {
        console.log('⏳ 寵物正在執行其他動作，請稍候');
        return;
    }
    
    console.log(`🐾 執行寵物動作: ${action}`);
    
    let message = '';
    let logClass = '';
    
    switch (action) {
        case 'Feed':
            petState.hunger = Math.min(100, petState.hunger + 10);
            message = '餵食成功！飢餓度 +10';
            logClass = 'success';
            playFeedAnimation();
            break;
            
        case 'Bath':
            petState.cleanliness = Math.min(100, petState.cleanliness + 10);
            message = '洗澡完成！清潔度 +10';
            logClass = 'success';
            playBathAnimation();
            break;
            
        case 'Play':
            petState.mood = Math.min(100, petState.mood + 10);
            message = '玩耍愉快！心情 +10';
            logClass = 'success';
            playPlayAnimation();
            break;
            
        case 'Rest':
            petState.energy = Math.min(100, petState.energy + 10);
            message = '休息充分！體力 +10';
            logClass = 'success';
            playRestAnimation();
            break;
            
        default:
            console.warn(`⚠️ 未知的寵物動作: ${action}`);
            return;
    }
    
    // 更新健康度
    updateHealth();
    
    // 更新 UI
    updatePetUI();
    
    // 儲存狀態
    savePetState();
    
    // 添加日誌
    addPetLog(message, logClass);
    
    // 檢查升級
    checkLevelUp();
}

/**
 * 更新健康度
 */
function updateHealth() {
    // 如果所有屬性都滿了，健康度設為100
    if (petState.hunger >= 100 && petState.mood >= 100 && 
        petState.energy >= 100 && petState.cleanliness >= 100) {
        petState.health = 100;
        return;
    }
    
    // 檢查低於30的屬性，降低健康度
    let healthPenalty = 0;
    
    if (petState.hunger < 30) healthPenalty += 20;
    if (petState.cleanliness < 30) healthPenalty += 20;
    if (petState.energy < 30) healthPenalty += 20;
    
    petState.health = Math.max(0, petState.health - healthPenalty);
}

/**
 * 開始冒險
 */
function startAdventure() {
    if (!canStartAdventure()) {
        addPetLog('無法開始冒險：屬性不足或次數用完', 'bad');
        return;
    }
    
    console.log('🗺️ 開始冒險...');
    
    // 增加每日動作次數
    petState.dailyActions++;
    
    // 模擬冒險結果
    const adventureResult = simulateAdventure();
    
    // 應用冒險結果
    applyAdventureResult(adventureResult);
    
    // 更新 UI
    updatePetUI();
    
    // 儲存狀態
    savePetState();
    
    // 播放冒險動畫
    playAdventureAnimation(adventureResult);
    
    console.log('✅ 冒險完成:', adventureResult);
}

/**
 * 模擬冒險結果
 */
function simulateAdventure() {
    const successRate = calculateAdventureSuccessRate();
    const isSuccess = Math.random() < successRate;
    
    const baseExp = 50;
    const baseCoins = 10;
    
    if (isSuccess) {
        return {
            success: true,
            exp: baseExp + Math.floor(Math.random() * 50),
            coins: baseCoins + Math.floor(Math.random() * 10),
            message: '冒險成功！獲得經驗和獎勵',
            logClass: 'success'
        };
    } else {
        return {
            success: false,
            exp: Math.floor(baseExp * 0.3),
            coins: Math.floor(baseCoins * 0.5),
            message: '冒險失敗，但仍有收穫',
            logClass: 'warn'
        };
    }
}

/**
 * 計算冒險成功率
 */
function calculateAdventureSuccessRate() {
    let baseRate = 0.6; // 基礎成功率60%
    
    // 根據屬性調整成功率
    const avgStats = (petState.hunger + petState.mood + petState.energy + petState.cleanliness) / 4;
    const statBonus = (avgStats - 50) / 100; // -0.5 到 0.5
    
    // 根據等級調整成功率
    const levelBonus = (petState.level - 1) * 0.02; // 每級+2%
    
    return Math.max(0.1, Math.min(0.95, baseRate + statBonus + levelBonus));
}

/**
 * 應用冒險結果
 */
function applyAdventureResult(result) {
    // 增加經驗值
    petState.experience += result.exp;
    
    // 增加金幣
    addCoins(result.coins);
    
    // 屬性變化
    if (result.success) {
        petState.mood = Math.min(100, petState.mood + 30);
        petState.hunger = Math.max(0, petState.hunger - 20);
        petState.energy = Math.max(0, petState.energy - 20);
        petState.cleanliness = Math.max(0, petState.cleanliness - 20);
    } else {
        petState.mood = Math.max(0, petState.mood - 30);
        petState.hunger = Math.max(0, petState.hunger - 20);
        petState.energy = Math.max(0, petState.energy - 20);
        petState.cleanliness = Math.max(0, petState.cleanliness - 20);
    }
    
    // 更新健康度
    updateHealth();
    
    // 添加日誌
    addPetLog(`${result.message} (經驗 +${result.exp}, 金幣 +${result.coins})`, result.logClass);
    
    // 檢查升級
    checkLevelUp();
}

/**
 * 檢查升級
 */
function checkLevelUp() {
    while (petState.experience >= petState.experienceToNext && petState.level < 250) {
        petState.level++;
        petState.experience -= petState.experienceToNext;
        
        // 計算下一級所需經驗
        petState.experienceToNext = calculateNextLevelExp();
        
        // 升級獎勵
        const levelReward = calculateLevelReward();
        addCoins(levelReward);
        
        // 添加升級日誌
        addPetLog(`🎉 升級到 ${petState.level} 級！獎勵 ${levelReward} 金幣`, 'success');
        
        // 播放升級動畫
        playLevelUpAnimation();
        
        console.log(`🎉 寵物升級到 ${petState.level} 級`);
    }
}

/**
 * 計算下一級所需經驗
 */
function calculateNextLevelExp() {
    if (petState.level <= 10) {
        return 40 * petState.level + 60;
    } else if (petState.level <= 100) {
        return Math.floor(0.8 * Math.pow(petState.level, 2) + 380);
    } else {
        return Math.floor(285.69 * Math.pow(1.06, petState.level));
    }
}

/**
 * 計算升級獎勵
 */
function calculateLevelReward() {
    return petState.level * 5; // 每級獎勵5金幣
}

/**
 * 添加寵物日誌
 */
function addPetLog(message, className = '') {
    if (!petElements.log) return;
    
    const logItem = document.createElement('li');
    logItem.textContent = `[${new Date().toLocaleTimeString()}] ${message}`;
    
    if (className) {
        logItem.className = className;
    }
    
    // 添加到頂部
    petElements.log.insertBefore(logItem, petElements.log.firstChild);
    
    // 限制日誌數量
    while (petElements.log.children.length > 12) {
        petElements.log.removeChild(petElements.log.lastChild);
    }
}

/**
 * 開始動畫循環
 */
function startAnimationLoop() {
    let lastTime = 0;
    
    function animate(currentTime) {
        if (currentTime - lastTime > 1000 / 12) { // 12 FPS
            drawPet();
            lastTime = currentTime;
        }
        animationState.animationFrame = requestAnimationFrame(animate);
    }
    
    animationState.animationFrame = requestAnimationFrame(animate);
}

/**
 * 繪製寵物
 */
function drawPet() {
    if (!petElements.canvas) return;
    
    const ctx = petElements.canvas.getContext('2d');
    const canvas = petElements.canvas;
    
    // 清空畫布
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    
    // 繪製背景
    ctx.fillStyle = petState.backgroundColor;
    ctx.fillRect(0, 0, canvas.width, canvas.height);
    
    // 繪製史萊姆
    drawSlime(ctx);
    
    // 繪製表情和狀態
    drawExpression(ctx);
    
    // 繪製動畫效果
    drawAnimationEffects(ctx);
}

/**
 * 繪製史萊姆主體
 */
function drawSlime(ctx) {
    const centerX = 60;
    const centerY = 60;
    const time = Date.now() * 0.001;
    
    // 史萊姆主體（橢圓形）
    ctx.fillStyle = petState.skinColor;
    ctx.beginPath();
    ctx.ellipse(centerX, centerY, 30, 25, 0, 0, Math.PI * 2);
    ctx.fill();
    
    // 添加高光
    ctx.fillStyle = 'rgba(255, 255, 255, 0.3)';
    ctx.beginPath();
    ctx.ellipse(centerX - 10, centerY - 10, 8, 6, 0, 0, Math.PI * 2);
    ctx.fill();
    
    // 呼吸動畫
    const breatheScale = 1 + Math.sin(time * 2) * 0.05;
    ctx.save();
    ctx.translate(centerX, centerY);
    ctx.scale(breatheScale, breatheScale);
    ctx.translate(-centerX, -centerY);
}

/**
 * 繪製表情
 */
function drawExpression(ctx) {
    const centerX = 60;
    const centerY = 60;
    const time = Date.now() * 0.001;
    
    // 眼睛
    const eyeY = centerY - 5;
    const blink = Math.sin(time * 3) > 0.8 ? 0.1 : 1;
    
    // 左眼
    ctx.fillStyle = '#000';
    ctx.beginPath();
    ctx.ellipse(centerX - 8, eyeY, 3, 3 * blink, 0, 0, Math.PI * 2);
    ctx.fill();
    
    // 右眼
    ctx.beginPath();
    ctx.ellipse(centerX + 8, eyeY, 3, 3 * blink, 0, 0, Math.PI * 2);
    ctx.fill();
    
    // 嘴巴（根據心情變化）
    ctx.strokeStyle = '#000';
    ctx.lineWidth = 2;
    ctx.beginPath();
    
    if (petState.mood > 70) {
        // 開心
        ctx.arc(centerX, centerY + 5, 5, 0, Math.PI);
    } else if (petState.mood > 30) {
        // 普通
        ctx.moveTo(centerX - 3, centerY + 5);
        ctx.lineTo(centerX + 3, centerY + 5);
    } else {
        // 難過
        ctx.arc(centerX, centerY + 10, 5, Math.PI, Math.PI * 2);
    }
    
    ctx.stroke();
    
    // 飢餓表情
    if (petState.hunger < 30) {
        ctx.fillStyle = '#ff6b6b';
        ctx.beginPath();
        ctx.arc(centerX + 15, centerY - 10, 2, 0, Math.PI * 2);
        ctx.fill();
    }
}

/**
 * 繪製動畫效果
 */
function drawAnimationEffects(ctx) {
    const time = Date.now() * 0.001;
    
    // 根據當前動畫狀態繪製效果
    if (animationState.currentAnimation === 'feed') {
        drawFeedEffects(ctx, time);
    } else if (animationState.currentAnimation === 'bath') {
        drawBathEffects(ctx, time);
    } else if (animationState.currentAnimation === 'play') {
        drawPlayEffects(ctx, time);
    } else if (animationState.currentAnimation === 'rest') {
        drawRestEffects(ctx, time);
    } else if (animationState.currentAnimation === 'levelup') {
        drawLevelUpEffects(ctx, time);
    }
}

/**
 * 繪製餵食效果
 */
function drawFeedEffects(ctx, time) {
    // 食物粒子
    for (let i = 0; i < 5; i++) {
        const x = 60 + Math.sin(time * 3 + i) * 20;
        const y = 40 + Math.cos(time * 2 + i) * 10;
        const size = Math.sin(time * 4 + i) * 2 + 2;
        
        ctx.fillStyle = `hsl(${30 + i * 20}, 70%, 60%)`;
        ctx.beginPath();
        ctx.arc(x, y, size, 0, Math.PI * 2);
        ctx.fill();
    }
}

/**
 * 繪製洗澡效果
 */
function drawBathEffects(ctx, time) {
    // 泡泡效果
    for (let i = 0; i < 8; i++) {
        const x = 60 + Math.sin(time * 2 + i) * 25;
        const y = 60 + Math.cos(time * 1.5 + i) * 15;
        const size = Math.sin(time * 3 + i) * 3 + 3;
        
        ctx.fillStyle = 'rgba(255, 255, 255, 0.6)';
        ctx.beginPath();
        ctx.arc(x, y, size, 0, Math.PI * 2);
        ctx.fill();
    }
}

/**
 * 繪製玩耍效果
 */
function drawPlayEffects(ctx, time) {
    // 愛心粒子
    for (let i = 0; i < 6; i++) {
        const x = 60 + Math.sin(time * 2 + i) * 30;
        const y = 40 + Math.cos(time * 1.5 + i) * 20;
        const size = Math.sin(time * 4 + i) * 2 + 2;
        
        ctx.fillStyle = `hsl(${340 + i * 30}, 70%, 60%)`;
        drawHeart(ctx, x, y, size);
    }
}

/**
 * 繪製休息效果
 */
function drawRestEffects(ctx, time) {
    // Z字粒子
    const zPositions = [
        { x: 80, y: 30 },
        { x: 85, y: 25 },
        { x: 90, y: 20 }
    ];
    
    zPositions.forEach((pos, i) => {
        const alpha = Math.sin(time * 2 + i) * 0.5 + 0.5;
        ctx.fillStyle = `rgba(255, 255, 255, ${alpha})`;
        ctx.font = '12px Arial';
        ctx.fillText('Z', pos.x, pos.y);
    });
}

/**
 * 繪製升級效果
 */
function drawLevelUpEffects(ctx, time) {
    // 星星效果
    for (let i = 0; i < 10; i++) {
        const x = 60 + Math.sin(time * 3 + i) * 40;
        const y = 60 + Math.cos(time * 2 + i) * 30;
        const size = Math.sin(time * 4 + i) * 3 + 3;
        
        ctx.fillStyle = `hsl(${60 + i * 36}, 100%, 60%)`;
        drawStar(ctx, x, y, size);
    }
}

/**
 * 繪製愛心
 */
function drawHeart(ctx, x, y, size) {
    ctx.save();
    ctx.translate(x, y);
    ctx.scale(size / 10, size / 10);
    
    ctx.beginPath();
    ctx.moveTo(0, 0);
    ctx.bezierCurveTo(-5, -5, -10, 0, 0, 10);
    ctx.bezierCurveTo(10, 0, 5, -5, 0, 0);
    ctx.fill();
    
    ctx.restore();
}

/**
 * 繪製星星
 */
function drawStar(ctx, x, y, size) {
    ctx.save();
    ctx.translate(x, y);
    ctx.scale(size / 10, size / 10);
    
    ctx.beginPath();
    for (let i = 0; i < 5; i++) {
        const angle = (i * 4 * Math.PI) / 5;
        const x1 = Math.cos(angle) * 5;
        const y1 = Math.sin(angle) * 5;
        
        if (i === 0) {
            ctx.moveTo(x1, y1);
        } else {
            ctx.lineTo(x1, y1);
        }
    }
    ctx.closePath();
    ctx.fill();
    
    ctx.restore();
}

/**
 * 播放餵食動畫
 */
function playFeedAnimation() {
    if (animationState.isAnimating) return;
    
    animationState.isAnimating = true;
    animationState.currentAnimation = 'feed';
    
    setTimeout(() => {
        animationState.isAnimating = false;
        animationState.currentAnimation = null;
    }, 2000);
}

/**
 * 播放洗澡動畫
 */
function playBathAnimation() {
    if (animationState.isAnimating) return;
    
    animationState.isAnimating = true;
    animationState.currentAnimation = 'bath';
    
    setTimeout(() => {
        animationState.isAnimating = false;
        animationState.currentAnimation = null;
    }, 2000);
}

/**
 * 播放玩耍動畫
 */
function playPlayAnimation() {
    if (animationState.isAnimating) return;
    
    animationState.isAnimating = true;
    animationState.currentAnimation = 'play';
    
    setTimeout(() => {
        animationState.isAnimating = false;
        animationState.currentAnimation = null;
    }, 2000);
}

/**
 * 播放休息動畫
 */
function playRestAnimation() {
    if (animationState.isAnimating) return;
    
    animationState.isAnimating = true;
    animationState.currentAnimation = 'rest';
    
    setTimeout(() => {
        animationState.isAnimating = false;
        animationState.currentAnimation = null;
    }, 2000);
}

/**
 * 播放冒險動畫
 */
function playAdventureAnimation(result) {
    if (animationState.isAnimating) return;
    
    animationState.isAnimating = true;
    animationState.currentAnimation = result.success ? 'play' : 'rest';
    
    setTimeout(() => {
        animationState.isAnimating = false;
        animationState.currentAnimation = null;
    }, 3000);
}

/**
 * 播放升級動畫
 */
function playLevelUpAnimation() {
    if (animationState.isAnimating) return;
    
    animationState.isAnimating = true;
    animationState.currentAnimation = 'levelup';
    
    setTimeout(() => {
        animationState.isAnimating = false;
        animationState.currentAnimation = null;
    }, 3000);
}

/**
 * 開始衰減計時器
 */
function startDecayTimer() {
    // 每小時檢查一次衰減
    setInterval(() => {
        // 輕微衰減
        petState.hunger = Math.max(0, petState.hunger - 1);
        petState.mood = Math.max(0, petState.mood - 1);
        petState.energy = Math.max(0, petState.energy - 1);
        petState.cleanliness = Math.max(0, petState.cleanliness - 1);
        
        updateHealth();
        updatePetUI();
        savePetState();
    }, 3600000); // 1小時
}

/**
 * 獲取金幣數量
 */
function getCoins() {
    return parseInt(localStorage.getItem('gamecore_coins') || '0');
}

/**
 * 增加金幣
 */
function addCoins(amount) {
    const currentCoins = getCoins();
    const newCoins = currentCoins + amount;
    localStorage.setItem('gamecore_coins', newCoins.toString());
    
    console.log(`💰 金幣變化: ${currentCoins} + ${amount} = ${newCoins}`);
}

/**
 * 消耗金幣
 */
function spendCoins(amount) {
    const currentCoins = getCoins();
    if (currentCoins >= amount) {
        const newCoins = currentCoins - amount;
        localStorage.setItem('gamecore_coins', newCoins.toString());
        
        console.log(`💰 金幣變化: ${currentCoins} - ${amount} = ${newCoins}`);
        return true;
    }
    return false;
}

// 全域函數（供其他模組調用）
window.initializePetSystem = initializePetSystem;
window.performPetAction = performPetAction;
window.startAdventure = startAdventure;
window.getPetState = () => petState;
window.getCoins = getCoins;
window.addCoins = addCoins;
window.spendCoins = spendCoins; 