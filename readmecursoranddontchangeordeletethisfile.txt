////
////使用者基本資料 
///
Table Users {
User_ID int [pk, increment, note: '使用者編號']
User_name nvarchar [not null, unique, note: '使用者姓名']
User_Account nvarchar [not null, unique, note: '登入帳號（唯一）']
User_Password nvarchar [not null, note: '使用者密碼']
}

//使用者介紹 ()
Table User_Introduce{
User_ID int [pk, ref: > Users.User_ID, note: 'FK 到 Users']
User_NickName NVARCHAR [not null, unique, note: '使用者暱稱']
Gender CHAR [not null, note: '性別']
IdNumber varchar [not null,unique, note: '身分證字號']
Cellphone varchar [not null, unique, note: '聯繫電話']
Email nvarchar [not null, unique, note: '電子郵件（唯一）']
Address nvarchar [not null, note: '地址']
DateOfBirth date [not null, note: '出生年月日']
Create_Account datetime2 [not null, note: '創建帳號日期']
User_Picture varbinary(max) [null, note: '頭像圖片 URL'] //MAX
User_Introduce NVARCHAR(200) [null, note: '使用者自介']

}
//使用者權限
Table User_Rights {
User_Id int [pk, ref: > Users.User_ID, note: 'FK 到 Users']
User_Status bit [note: '使用者狀態']
ShoppingPermission bit [note: '購物權限']
MessagePermission bit [note: '留言權限']
SalesAuthority bit [note: '銷售權限']
}

//使用者錢包
Table User_wallet{
User_Id int [pk, ref: > Users.User_ID, note: 'FK 到 Users'] 
User_Point int [note: '使用者編號']
Coupon_Number VARCHAR [note: '優惠券編號']
}



//開通銷售功能
Table MemberSalesProfile {
User_Id int [pk, ref: > Users.User_ID, note: 'FK 到 Users'] 
BankCode int [note: '銀行代號']
BankAccountNumber varchar [note: '銀行帳號'] 
AccountCoverPhoto varbinary(max) [note: '帳戶封面照片']
}

//使用者銷售資訊
Table User_Sales_Information{
User_Id int [pk, ref: > Users.User_ID, note: 'FK 到 Users'] 
UserSales_Wallet int [note: '使用者銷售錢包']
}
//////////////////////////////////////////////////////////////////
////管理者
//////////////////////////////////////////////////////////////////
//管理者資料
// 管理者資料（主表）
Table ManagerData {
Manager_Id INT [pk, note: '管理者編號']
Manager_Name NVARCHAR [note: '管理者姓名']
Manager_Account VARCHAR [unique, note: '管理者帳號（建議唯一）']
Manager_Password NVARCHAR [note: '管理者密碼（實務請存雜湊）']
Administrator_registration_date datetime2 [note: '管理者註冊時間'] 
}

// 角色權限定義
Table ManagerRolePermission {
ManagerRole_Id INT [pk, note: '管理者角色編號']
role_name NVARCHAR [not null, note: '顯示名稱 舉例:商城管理員']
//以上為建議
AdministratorPrivilegesManagement bit [note: '管理者權限管理']
UserStatusManagement bit [note: '使用者狀態管理']
ShoppingPermissionManagement bit [note: '商城權限管理']
MessagePermissionManagement bit [note: '論壇權限管理']
Pet_Rights_Management bit [note: '寵物權限管理']
customer_service bit [note: '客服權限管理']
}

// 管理者 ↔ 角色 的指派（關聯表；多對多）
// ＝ 一個管理者可有多個角色；一個角色可被多位管理者指派
// （這張就是你同學的「管理者角色表」）
Table ManagerRole {
Manager_Id INT [ref: > ManagerData.Manager_Id, note: 'FK: 管理者']
ManagerRole_Id INT [ref: > ManagerRolePermission.ManagerRole_Id, note: 'FK: 角色']
ManagerRole NVARCHAR [note: '角色名稱']

// 複合主鍵避免重複指派同一角色
indexes {
(Manager_Id, ManagerRole_Id) [pk]
}
}


////////////////////////////////////////////////////////////
// A. 熱度數據層（每日指標 → 指數 → 榜單快照） 管理員:温傑揚
// 用途：管理遊戲清單、外部來源與每日指標數據
////////////////////////////////////////////////////////////
Table games {
game_id int [pk, increment] // 遊戲ID（主鍵）
name nvarchar // 遊戲名稱
genre nvarchar // 類型（FPS/MOBA…）
created_at datetime2 // 建立時間
Note: '遊戲主檔：列出平台所有遊戲'
}
//關於這個算是清洗完進來的東西但是目前都是先放假資料?
Table metric_sources {
source_id int [pk, increment] // 來源ID（主鍵）
name varchar // 來源名（Steam/Bahamut/YouTube…）
note nvarchar // 備註（抓法/限制）
created_at datetime2 // 建立時間
Note: '數據來源字典：定義要抓的外部平台'
}

Table metrics {
metric_id int [pk, increment] // 指標ID（主鍵）
source_id int [ref: > metric_sources.source_id] // 所屬來源
code varchar // 指標代碼（concurrent_users/forum_posts…）
unit varchar // 單位（users/posts/views）
description nvarchar // 指標中文說明
is_active bit // 是否啟用
created_at datetime2 // 建立時間
indexes {
(source_id, code) [unique] // 同一來源下代碼唯一
}
Note: '指標字典：來源底下的可用指標清單'
}

Table game_source_map {
id int [pk, increment] // 對應ID
game_id int [ref: > games.game_id] // 內部遊戲ID
source_id int [ref: > metric_sources.source_id] // 外部來源ID
external_key varchar // 外部ID（Steam appid / 巴哈 slug）
indexes {
(game_id, source_id) [unique] // 一遊戲在一來源只對應一次
}
Note: '外部ID對應表：把內部遊戲對應到各來源的外部鍵'
}

Table game_metric_daily {
id int [pk, increment] // 流水號
game_id int [ref: > games.game_id] // 遊戲ID
metric_id int [ref: > metrics.metric_id] // 指標ID
date date // 日期（日粒度）
value decimal(18,4) // 數值（清洗後）
agg_method varchar // 聚合方法（sum/avg/max）
quality varchar // 資料品質（real/estimate/seed）
created_at datetime2 // 建立時間
updated_at datetime2 // 更新時間
indexes {
(game_id, metric_id, date) [unique] // UPSERT 防重
(date, metric_id) // 查某日多指標
(game_id, date) // 查遊戲時間序列
}
Note: '每天每指標的原始值（清洗後），是計算指數的底稿'
}

Table popularity_index_daily {
id bigint [pk, increment] // 流水號
game_id int [ref: > games.game_id] // 遊戲ID
date date // 日期
index_value decimal(18,4) // 熱度指數（加權計算）
created_at datetime2 // 建立時間
indexes {
(game_id, date) [unique] // 每日每遊戲唯一
(date) // 查日榜
}
Note: '每日熱度指數（計算結果）'
}

Table leaderboard_snapshots {
snapshot_id bigint [pk, increment] // 快照ID
period varchar // 期間類型（daily/weekly）
ts datetime2 // 快照時間
rank int // 名次（1..N）
game_id int [ref: > games.game_id] // 遊戲ID
index_value decimal(18,4) // 當時指數值
created_at datetime2 // 建立時間
indexes {
(period, ts, rank)
(period, ts, rank, game_id) [unique]
(period, ts, game_id)
}
Note: '榜單快照：直接給前台讀，避免重算'
}

////////////////////////////////////////////////////////////
// B. 統一動態牆（管理員洞察；未來可擴 type="user"）
// 用途：管理洞察文章與發佈當下的數據快照
////////////////////////////////////////////////////////////
Table posts {
post_id int [pk, increment] // 文章ID
type nvarchar // 類型（insight/user）
game_id int [ref: > games.game_id] // 關聯遊戲（可為NULL）
title nvarchar // 標題
tldr nvarchar // 三行摘要（卡片用）
body_md nvarchar // 內文（Markdown）
visibility bit // public/hidden
status varchar // draft/published/hidden
pinned bit // 是否置頂
created_by int [ref: > Users.User_ID] // 作者ID
published_at datetime2 // 發佈時間
created_at datetime2 // 建立時間
updated_at datetime2 // 更新時間
indexes {
(type, created_at)
(game_id, created_at)
(status, created_at)
}
Note: '統一貼文表：洞察與未來UGC都走這'
}
//因為有當前指數跟發文前指數
Table post_metric_snapshot {
post_id bigint [pk, ref: > posts.post_id] // 文章ID（主鍵）
game_id int [ref: > games.game_id] // 當時的遊戲ID
date date // 拍照日期
index_value decimal(18,4) // 當日指數
details_json nvarchar // 當日各指標值/Δ%/權重（JSON）
created_at datetime2 // 建立時間
Note: '洞察發佈時的數據快照（固定展示）'
}

Table post_sources {
id bigint [pk, increment] // 流水號
post_id bigint [ref: > posts.post_id] // 文章ID
source_name varchar // 顯示名稱
url varchar // 外部連結
created_at datetime2 // 建立時間
indexes {
(post_id)
}
Note: '洞察引用來源清單'
}

////////////////////////////////////////////////////////////
// C. 論壇（每遊戲一版）＋ 互動（讚/收藏）
// 用途：遊戲討論區與互動功能
////////////////////////////////////////////////////////////
Table forums {
forum_id int [pk, increment] // 論壇版ID
game_id int [ref: > games.game_id] // 遊戲ID（一對一）
name varchar // 版名
description nvarchar // 版說明
created_at datetime2 // 建立時間
indexes {
(game_id) [unique]
}
Note: '論壇版主檔：每遊戲一個版'
}

Table threads {
thread_id bigint [pk, increment] // 主題ID
forum_id int [ref: > forums.forum_id] // 所屬版ID
author_user_id int [ref: > Users.User_ID] // 作者ID
title nvarchar // 標題
status varchar // normal/hidden/archived
created_at datetime2 // 建立時間
updated_at datetime2 // 更新時間
indexes {
(forum_id, updated_at)
}
Note: '版內主題（討論串）'
}

Table thread_posts {
id bigint [pk, increment] // 回覆ID
thread_id bigint [ref: > threads.thread_id] // 主題ID
author_user_id int [ref: > Users.User_ID] // 回覆者ID
content_md nvarchar // 內容（Markdown）
parent_post_id bigint [ref: > thread_posts.id] // 父回覆ID（支援二層）
status varchar // normal/hidden/deleted
created_at datetime2 // 建立時間
updated_at datetime2 // 更新時間
indexes {
(thread_id, created_at)
}
Note: '主題回覆（支援二層結構）'
}

Table reactions {
id bigint [pk, increment] // 流水號
user_id int [ref: > Users.User_ID] // 誰按的
target_type varchar // 目標類型（post/thread/thread_post）
target_id bigint // 目標ID（多型，不設FK）
kind varchar // 反應類型（like/emoji…）
created_at datetime2 // 建立時間
indexes {
(user_id, target_type, target_id, kind) [unique]
(target_type, target_id)
}
Note: '通用讚表（多型）'
}

Table bookmarks {
id bigint [pk, increment] // 流水號
user_id int [ref: > Users.User_ID] // 收藏者ID
target_type varchar // 目標類型'post' | 'thread' | 'game' | 'forum'
target_id bigint // 目標ID（多型，不設FK）
created_at datetime2 // 建立時間
indexes {
(user_id, target_type, target_id) [unique]
(target_type, target_id)
}
Note: '通用收藏表（多型）'
}
//////////////////////////////
//////管理者:鐘群能 小遊戲&每日登入
//////////////////////////////
Table UserSignInStats {
LogID int [pk, increment, note: '簽到記錄 ID，自動遞增'] 
SignTime datetime2 [not null, default: 'SYSUTCDATETIME()', note: '簽到時間（預設 UTC 當下時間）'] 
UserID int [not null, note: '會員 ID，外鍵參考 Users.UserID'] 
PointsChanged int [not null, default: 0, note: '此次簽到會員點數增減數量'] 
ExpGained int [not null, default: 0, note: '此次簽到寵物獲得經驗值'] 
PointsChangedTime datetime2 [not null, default: 'SYSUTCDATETIME()', note: '點數變動時間'] 
ExpGainedTime datetime2 [not null, default: 'SYSUTCDATETIME()', note: '寵物經驗值獲得時間'] 
}


// 寵物狀態表
Table Pet {
PetID int [pk, increment, note: '寵物 ID，自動遞增'] 
UserID int [not null, note: '寵物主人會員 ID，外鍵參考 Users.UserID'] 
PetName nvarchar(50) [not null, default: '小可愛', note: '寵物名稱，若未提供則預設'] 
Level int [not null, default: 0, note: '寵物當前等級'] 
LevelUpTime datetime2 [not null, default: 'SYSUTCDATETIME()', note: '寵物最後一次升級時間'] 
Experience int [not null, default: 0, note: '寵物累計總經驗值'] 
Hunger int [not null, default: 0, note: '飢餓值（0–100）'] 
Mood int [not null, default: 0, note: '心情值（0–100）'] 
Stamina int [not null, default: 0, note: '體力值（0–100）'] 
Cleanliness int [not null, default: 0, note: '清潔值（0–100）'] 
Health int [not null, default: 0, note: '健康度（0–100）'] 
SkinColor nvarchar(50) [not null, default: '#ADD8E6', note: '膚色十六進位'] 
ColorChangedTime datetime2 [not null, default: 'SYSUTCDATETIME()', note: '最後一次膚色更換時間'] 
BackgroundColor nvarchar(50) [not null, default: '粉藍', note: '背景色'] 
BackgroundColorChangedTime datetime2 [not null, default: 'SYSUTCDATETIME()', note: '最後一次被景色更換時間'] 
PointsChanged int [not null, default: 0, note: '最近一次幫寵物換色所花費之會員點數'] 
PointsChangedTime datetime2 [not null, default: 'SYSUTCDATETIME()', note: '幫寵物換色所花費之會員點數變動時間'] 
}

// 小冒險遊戲紀錄表
Table MiniGame {
PlayID int [pk, increment, note: '遊戲執行記錄 ID，自動遞增'] 
UserID int [not null, note: '玩家會員 ID，外鍵參考 Users.UserID'] 
PetID int [not null, note: '出戰寵物 ID，外鍵參考 Pet.PetID'] 
Level int [not null, default: 0, note: '遊戲關卡等級'] 
MonsterCount int [not null, default: 0, note: '需面對的怪物數量'] 
SpeedMultiplier decimal(5,2) [not null, default: 1.00, note: '怪物移動速度倍率'] 
Result nvarchar(10) [not null, default: 'Unknown', note: '遊戲結果: Win(贏)/Lose(輸)/Abort(中退)'] 
ExpGained int [not null, default: 0, note: '寵物本次獲得經驗值'] 
ExpGainedTime datetime2 [note: '寵物獲得經驗值時間'] 
PointsChanged int [not null, default: 0, note: '本次會員點數增減'] 
PointsChangedTime datetime2 [note: '本次會員點數變動時間'] 
HungerDelta int [not null, default: 0, note: '寵物飢餓值變化量'] 
MoodDelta int [not null, default: 0, note: '寵物心情值變化量'] 
StaminaDelta int [not null, default: 0, note: '寵物體力值變化量'] 
CleanlinessDelta int [not null, default: 0, note: '寵物清潔值變化量'] 
StartTime datetime2 [not null, default: 'SYSUTCDATETIME()', note: '遊戲開始時間'] 
EndTime datetime2 [note: '遊戲結束時間，若中退則為null'] 
Aborted bit [not null, default: 0, note: '是否中途放棄 (0=否,1=是)，預設為0(否)'] 
}

// 關聯定義
Ref: UserSignInStats.UserID > Users.User_ID // 簽到紀錄 → 會員
Ref: User_wallet.User_Point > Users.User_ID // 點數表 → 會員
Ref: Pet.UserID > Users.User_ID // 寵物 → 會員
Ref: MiniGame.UserID > Users.User_ID // 小遊戲 → 會員
Ref: MiniGame.PetID > Pet.PetID // 小遊戲 → 寵物


////////////////////////////////////////////////////////////
// 商城 管理員:房立堯&成博儒
////////////////////////////////////////////////////////////
Table Official_Store_Ranking as OSR { //官方商城排行榜
ranking_id INT [pk] // 排行榜流水號
period_type NVARCHAR //榜單型態(日、月、季、年)
ranking_date DATE // 榜單日期(計算日期)
product_ID INT [ref: > PI.product_id] // 指向排名目標
ranking_metric VARCHAR // 排名指標(交易額/量)
ranking_position TINYINT //名次
trading_amount DECIMAL //交易額
trading_volume INT //交易量
ranking_updated_at DATETIME2 //排行榜更新時間

}

Table Supplier as S {//供應商
supplier_id INT [pk] //廠商ID
supplier_name NVARCHAR //廠商名字

}

Table OrderInfo as O {//訂單資訊
order_id INT [pk] //訂單ID
user_id INT [ref: > Users.User_ID] //下訂會員ID
order_date DATETIME2 //下單日期
order_status NVARCHAR //訂單狀態(未付款-為出貨-已出貨-已完成)
payment_status NVARCHAR //付款狀態(下單-待付款-已付款)
order_total DECIMAL //訂單總額
payment_at DATETIME2 //付款時間
shipped_at DATETIME2 //出貨時間
completed_at DATETIME2 //完成時間
}

Table OrderItems as OD {//訂單詳細
item_id INT [pk] // 訂單詳細ID PK
order_id INT [ref: > O.order_id] //訂單ID指向訂單 
product_id INT [ref: > PI.product_id] // 商品ID指向商品.Product_ID] 
line_no INT //實際物品編號1.2.3…
unit_price DECIMAL //單價
quantity INT //下單數量
subtotal DECIMAL //小計
}

Table ProductInfo as PI{//商品資訊
product_id INT [pk] //商品ID
product_name NVARCHAR //商品名稱
product_type NVARCHAR //商品類型
price DECIMAL //售價
currency_code NVARCHAR //使用幣別
Shipment_Quantity INT //庫存
product_created_by NVARCHAR //創建者
product_created_at DATETIME2 //建立時間
product_updated_by NVARCHAR //最後修改者
product_updated_at DATETIME2 //更新時間
user_id INT //會員ID
}

Table GameProductDetails as GPD{//遊戲主檔商品資訊
product_id int [pk, ref: > PI.product_id] //商品ID 
product_name NVARCHAR //商品名稱
product_description NVARCHAR //商品描述
supplier_id int [ref:> S.supplier_id] //廠商ID
platform_id int //遊戲平台ID
game_id int //遊戲ID
game_name NVARCHAR //遊戲名稱
download_link NVARCHAR //下載連結
}

Table OtherProductDetails as OPD{ //非遊戲主檔商品資訊
product_id int [pk, ref: > PI.product_id] //商品ID
product_name NVARCHAR //商品名稱
product_description NVARCHAR //商品描述
supplier_id int [ref:> S.supplier_id] //廠商ID
platform_id int //遊戲平台ID
digital_code NVARCHAR //數位序號兌換碼
size NVARCHAR //尺寸
color NVARCHAR //顏色
weight NVARCHAR //重量
dimensions NVARCHAR //尺寸
material NVARCHAR //材質
stock_quantity NVARCHAR //庫存數量
}
Table ProductInfoAuditLog as PIAL{ //商品修改日誌
log_id BIGINT //日誌ID
product_id INT [ref:> PI.product_id] //商品ID
action_type NVARCHAR //動作類型
field_name NVARCHAR [ref:> S.supplier_id] //修改欄位名稱
old_value NVARCHAR //舊值
new_value NVARCHAR //新值
Manager_Id INT [ref:> ManagerData.Manager_Id] //操作人ID
changed_at DATETIME2 //修改時間
}

Table PlayerMarketRanking as PMR {//自由市場排行榜
p_ranking_id int [pk] //p排行榜ID
p_period_type varchar //p榜單型態
p_ranking_date date //p日榜
p_product_id int [ref:> PMP.p_product_id] //p商品ID
p_ranking_metric varchar //排名指標
p_ranking_position int //名次
p_trading_amount DECIMAL //交易額
p_trading_volume int //交易量
updated_at datetime2 //排行榜更新時間
}

Table PlayerMarketProductInfo as PMP{ //自由市場商品資訊
p_product_id int [pk] //p商品ID
p_product_type nvarchar //p商品類型
p_product_title nvarchar//商品標題(噱頭標語)
p_product_name nvarchar //p商品名稱
p_product_description nvarchar //p商品描述
product_id int [ref: < PI.product_id] //商品ID
seller_id int [ref: < Users.User_ID] //賣家ID
p_status nvarchar // p商品狀態
price decimal //售價
p_product_img_id nvarchar //p商品圖片ID
created_at datetime2 //建立時間
updated_at datetime2 //更新時間
} 

Table PlayerMarketProductImgs{//自由市場商品圖片
p_product_img_id int [pk] // p商品圖片ID
p_product_id int [ref: > PMP.p_product_id] // 指向自由市場商品
p_product_img_url varbinary(max) //p商品圖片網址
}

Table PlayerMarketOrderInfo as PO{ //自由市場訂單
p_order_id int [pk] //p訂單ID
p_product_id int [ref: > PMP.p_product_id] // 指向自由市場商品
seller_id int [ref :> Users.User_ID]
// 指向用戶（假設有 User 表）Seller 是用會員帳號
buyer_id int [ref :> Users.User_ID] 
// 指向用戶（假設有 User 表）Buyer 是用會員帳號
p_order_date datetime2 //p訂單日期
p_order_status nvarchar //訂單狀態 
p_payment_status nvarchar //付款狀態
p_unit_price int //p單價
p_quantity int //p 數量
p_order_total int //p 總額
p_order_created_at datetime2 // 建立時間
p_order_updated_at datetime2 //更新時間
}

// Ref: "RadingList_Of_officeMall"."Trading_Volume" < "RadingList_Of_officeMall"."Game_Platform_Code"

Table PlayerMarketOrderTradepage as POTP{ //交易中頁面
//Buyer和Seller都確認在遊戲平台交易完成，才金流
p_order_tradepage_id int [pk]//流水號 交易頁面ID
p_order_id int [ref: > PO.p_order_id] //p訂單ID
p_product_id int [ref: > PMP.p_product_id] //p商品ID
p_order_platform_fee int //平台抽成
seller_transferred_at datetime2//user1(seller) 賣家移交時間
buyer_received_at datetime2//user2(buyer) 買家接收時間
completed_at datetime2//交易完成時間
}

Table PlayerMarketTradeMsg{//自由市場交易頁面對話
trade_msg_id int [pk] //交易中雙方訊息ID
p_order_tradepage_id int [ref: > POTP.p_order_tradepage_id] //交易頁面ID
msg_from nvarchar //誰傳的訊息(seller/buyer)
message_text nvarchar //訊息內容
created_at datetime2 //傳訊時間
}






// ===== ManagerData（如果你已在別處定義，就刪掉這個 stub）=====
//
// ================== 字典 / 樣式 ==================
// ================== 後台管理員表 ==================
Table Admins {
manager_id int [pk, ref: > ManagerRole.Manager_Id, note: '管理員ID']
last_login datetime2 [note: '上次登入時間，用於後台登入追蹤']
}

// ================== 禁言/樣式池功能表 ==================
Table Mutes {
mute_id int [pk, increment, note: '禁言選項ID（字典）']
mute_name nvarchar [note: '禁言名稱']
created_at datetime2 [note: '建立時間']
is_active bit [not null, default: 1, note: '是否啟用']
manager_id int [ref: > ManagerRole.Manager_Id, note: '設置者ID']
}

Table Styles {
style_id int [pk, increment, note: '樣式ID']
style_name nvarchar [note: '樣式名稱']
effect_desc nvarchar [note: '效果說明']
created_at datetime2 [note: '建立時間']
manager_id int [ref: > ManagerRole.Manager_Id, note: '設置者ID']
}

// ================== 通知相關表 ==================
Table Notification_Sources {
source_id int [pk, increment, note: '來源類型ID']
source_name nvarchar [note: '來源名稱']
}


Table Notification_Actions {
action_id int [pk, increment, note: '行為類型ID']
action_name nvarchar [note: '行為名稱']
indexes {
(action_name) [unique]
}
}

// ================== 通知主表 ==================
Table Notifications {
notification_id int [pk, increment, note: '通知ID']
source_id int [not null, ref: > Notification_Sources.source_id, note: '來源類型ID']
action_id int [not null, ref: > Notification_Actions.action_id, note: '行為類型ID']
sender_id int [not null, ref: > Users.User_ID, note: '發送者ID']
sender_manager_id int [ref: > ManagerRole.Manager_Id, note: '發送者ID(管理員)']
notification_title nvarchar [note: '通知標題']
notification_message nvarchar [note: '通知內容']
created_at datetime2 [not null, note: '建立時間']
group_id int [ref: > Groups.group_id, note: '群組ID（若為群組相關）']
}

// ================== 通知接收者表 ==================
Table Notification_Recipients {
recipient_id int [pk, increment, note: '接收紀錄ID']
notification_id int [not null, ref: > Notifications.notification_id, note: '通知ID']
user_id int [not null, ref: > Users.User_ID, note: '使用者ID']
is_read bit [not null, default: 0, note: '是否已讀']
read_at datetime2 [note: '已讀時間']
indexes {
(notification_id, user_id) [unique, note: '同一通知不重複投遞給同一人']
(user_id, is_read, recipient_id) [name: 'IX_Inbox']
}
}

// ================== 聊天訊息表 ==================
Table Chat_Message {
message_id int [pk, increment, note: '訊息ID']
manager_id int [ref: > ManagerRole.Manager_Id, note: '管理員ID(客服)']
sender_id int [not null, ref: > Users.User_ID, note: '發送者ID']
receiver_id int [ref: > Users.User_ID, note: '接收者ID']
chat_content nvarchar [not null, note: '訊息內容']
sent_at datetime2 [not null, note: '發送時間']
is_read bit [not null, default: 0, note: '是否已讀']
is_sent bit [not null, default: 1, note: '是否寄送']
indexes {
(sent_at)
(receiver_id, sent_at)
}
}

// ================== 群組表 ==================
Table Groups {
group_id int [pk, increment, note: '群組ID']
group_name nvarchar [note: '群組名稱']
created_by int [ref: > Users.User_ID, note: '建立者ID']
created_at datetime2 [note: '建立時間']
}

// ================== 群組成員表 ==================
Table Group_Member {
group_id int [not null, ref: > Groups.group_id, note: '群組ID']
user_id int [not null, ref: > Users.User_ID, note: '使用者ID']
joined_at datetime2 [note: '加入時間']
is_admin bit [not null, default: 0, note: '是否為管理員']
indexes {
(group_id, user_id) [pk, note: '群組成員複合主鍵，不重複']
}
}

// ================== 群組專用聊天 ================== 複合pk group_chat_id+group_id
Table Group_Chat {
group_chat_id int [pk, increment, note: '群組聊天ID']
group_id int [ref: > Groups.group_id, note: '群組ID']
sender_id int [ref: > Users.User_ID, note: '發送者ID']
group_chat_content nvarchar [note: '訊息內容']
sent_at datetime2 [note: '發送時間']
is_sent bit [not null, default: 1, note: '是否寄送']
indexes {
group_id // 常用：查群組訊息
(group_id, sent_at) // 常用：依群組+時間排序撈歷史訊息
}
}

// ================== 封鎖表(群組專用) ================== 複合pk block_id+group_id
Table Group_Block {
block_id int [pk, increment, note: '封鎖ID']
group_id int [ref: > Groups.group_id, note: '群組ID']
user_id int [ref: > Users.User_ID, note: '被封鎖者ID']
blocked_by int [ref: > Users.User_ID, note: '封鎖者ID']
created_at datetime2 [note: '建立時間']
indexes {
(group_id, user_id) [unique] // 關鍵：同一群×同一人只能有一筆
group_id
user_id
}
}



你先閱讀
# GameCore 專案詳細規格（依最終定版資料庫）

> 本規格 **完全以你提供的最終資料庫定版** 為準（不新增資料表、不更改欄位），將前台/後台功能、流程、狀態機、API 規格、權限與驗證一次統整，作為研發與測試之依據。

---

## 1. 產品總覽

**定位**：結合遊戲熱度觀測、論壇社群、官方商城、玩家自由市場（C2C）、寵物養成與小遊戲、即時訊息/群組與通知的整合平台。

**主要角色**：
- **一般用戶（Users）**：註冊登入、完善個資（User_Introduce）、使用者權限（User_Rights）、點數錢包（User_wallet）、開通銷售（MemberSalesProfile）、銷售錢包（User_Sales_Information）。
- **管理者（ManagerData）**：透過角色權限（ManagerRolePermission）與指派（ManagerRole），管理各模組；輔助表：Admins、Mutes、Styles。

**核心模組與對應資料表（僅列主體）**：
- 使用者：`Users`、`User_Introduce`、`User_Rights`、`User_wallet`、`MemberSalesProfile`、`User_Sales_Information`
- 管理員/權限：`ManagerData`、`ManagerRolePermission`、`ManagerRole`、`Admins`、`Mutes`、`Styles`
- 熱度/排行榜：`games`、`metric_sources`、`metrics`、`game_source_map`、`game_metric_daily`、`popularity_index_daily`、`leaderboard_snapshots`
- 洞察貼文：`posts`、`post_metric_snapshot`、`post_sources`
- 論壇/互動：`forums`、`threads`、`thread_posts`、`reactions`、`bookmarks`
- 小遊戲/簽到/寵物：`UserSignInStats`、`Pet`、`MiniGame`
- 官方商城：`Supplier`、`ProductInfo(PI)`、`GameProductDetails(GPD)`、`OtherProductDetails(OPD)`、`OrderInfo(O)`、`OrderItems(OD)`、`Official_Store_Ranking(OSR)`、`ProductInfoAuditLog(PIAL)`
- 自由市場：`PlayerMarketProductInfo(PMP)`、`PlayerMarketProductImgs`、`PlayerMarketOrderInfo(PO)`、`PlayerMarketOrderTradepage(POTP)`、`PlayerMarketTradeMsg`、`PlayerMarketRanking(PMR)`
- 社交/通知/群組/聊天：`Notifications`、`Notification_Sources`、`Notification_Actions`、`Notification_Recipients`、`Chat_Message`、`Groups`、`Group_Member`、`Group_Chat`、`Group_Block`

---

## 2. 權限模型

### 2.1 用戶側（User_Rights）
- `User_Status`：是否啟用/停權（bit）。
- `ShoppingPermission`：是否可於官方商城/自由市場下單（bit）。
- `MessagePermission`：是否可發表/回覆論壇、聊天室訊息（bit）。
- `SalesAuthority`：是否可在**自由市場上架**（bit）。

> 前端所有會觸發上述行為的入口/操作，皆須在 UI 與 API 雙重檢查。

### 2.2 管理端（ManagerRolePermission + ManagerRole）
- 以 `ManagerRolePermission` 定義可指派的**職能開關**（例如 `AdministratorPrivilegesManagement`、`UserStatusManagement`、`ShoppingPermissionManagement`…）。
- 以 `ManagerRole` 完成**管理者 ↔ 角色**的多對多指派。
- 適用場景：後台頁面按鈕顯示、API 授權、審核/封禁/上下架/公告發佈/榜單產製。

---

## 3. 前台功能規格

### 3.1 帳號/登入/個資
**資料表**：`Users`、`User_Introduce`、`User_Rights`、`User_wallet`

**功能**：
1) 註冊/登入/登出
   - 註冊：寫入 `Users`（帳號唯一）、建立 `User_Rights`（預設允許留言，購物/銷售視策略）、建立 `User_wallet`（初始 `User_Point=0`）。
   - 登入：密碼以**雜湊**方式校驗；成功後建立會話/JWT（規格外層）。

2) 個資與介紹
   - 編輯 `User_Introduce`：暱稱唯一、Email/手機/身分證字號唯一，頭像 `User_Picture`（varbinary(max)）。
   - 檢視個人頁：聚合 Users & User_Introduce；展示點數、銷售狀態（是否已開通）。

3) 錢包
   - 顯示 `User_wallet.User_Point` 與 `Coupon_Number`（若有）
   - 點數變動來源：簽到（`UserSignInStats.PointsChanged`）、小遊戲（`MiniGame.PointsChanged`）、商城/市集訂單結算（應用邏輯變更 `User_Point`）。

> **驗證**：所有寫入欄位依唯一性與 not null 規則；圖片大小/型別在應用層限制；生日為 `date` 格式。

---

### 3.2 開通銷售與銷售錢包
**資料表**：`MemberSalesProfile`、`User_Sales_Information`

**功能**：
- 申請銷售功能：填寫銀行代碼/帳號、上傳 `AccountCoverPhoto`；審核通過 → 在 `User_Rights.SalesAuthority=1`。
- 銷售錢包顯示：`User_Sales_Information.UserSales_Wallet`（供市集成交入帳/提領申請 UI）。

---

### 3.3 官方商城（B2C）
**資料表**：`Supplier`、`ProductInfo(PI)`、`GameProductDetails(GPD)`、`OtherProductDetails(OPD)`、`OrderInfo(O)`、`OrderItems(OD)`、`Official_Store_Ranking(OSR)`、`ProductInfoAuditLog(PIAL)`

**功能**：
1) 商品瀏覽與詳情
   - 讀取 `PI` 基本資訊；若 `product_type` 為遊戲 → 讀 `GPD` 顯示平台/下載連結；否則讀 `OPD` 顯示尺寸/顏色/材質等。
   - 供應商 `Supplier` 基本資料顯示（名稱）。

2) 加入購物車/下單/付款
   - 建立 `OrderInfo`（`order_status`、`payment_status` 初始值見 §5 狀態機），寫入 `OrderItems` 明細與金額。
   - 支付成功：寫入 `payment_at`、更新 `payment_status` 與 `order_status`、檢核庫存 `Shipment_Quantity`。
   - 出貨：填入 `shipped_at`；完成：`completed_at`。

3) 排行榜
   - 依日/月/季/年產製 `OSR`（名次、交易額/量、指向 `PI.product_id`）。前台提供榜單瀏覽與商品連結。

4) 商品異動稽核
   - 後台異動商品時產生 `PIAL`（欄位、舊/新值、管理者、時間）。前台不顯示此表。

> **金額/幣別**：以 `PI.price + currency_code` 為售價基準；訂單小計/總額於 `OD.subtotal` / `O.order_total`。

---

### 3.4 自由市場（C2C）
**資料表**：`PMP`、`PlayerMarketProductImgs`、`PO`、`POTP`、`PlayerMarketTradeMsg`、`PMR`

**功能**：
1) 上架/瀏覽/搜尋
   - 上架：新增 `PMP`（標題/描述/售價/狀態 `p_status`、賣家 `seller_id`、可選關聯 `PI.product_id`）。
   - 圖片：`PlayerMarketProductImgs`（多張），`p_product_img_url` 為二進位存放。
   - 搜尋：依標題/名稱/價格區間/賣家。

2) 下單/交易頁/完成
   - 下單：新增 `PO`（含 `p_order_status`、`p_payment_status`、單價/數量/總額、買賣雙方）。
   - 交易頁：建立 `POTP` 一筆，雙方於 `seller_transferred_at` 與 `buyer_received_at` 確認道具交付；完成時填 `completed_at`，平台抽成 `p_order_platform_fee`。
   - 聊天：`PlayerMarketTradeMsg` 綁 `p_order_tradepage_id` 訊息流（`msg_from` = buyer/seller）。
   - 排行榜：`PMR` 依期間寫入商品交易額/量與排名。

> **入帳**：成交後應用層計算平台抽成 → 增加賣家 `User_Sales_Information.UserSales_Wallet`；扣買家 `User_wallet.User_Point`（若用點數）。

---

### 3.5 遊戲熱度/榜單與洞察
**資料表**：`games`、`metric_sources`、`metrics`、`game_source_map`、`game_metric_daily`、`popularity_index_daily`、`leaderboard_snapshots`、`posts`、`post_metric_snapshot`、`post_sources`

**功能**：
- 遊戲清單：`games`。
- 指標來源與指標字典：`metric_sources`、`metrics`，外鍵對應 `game_source_map`（各來源的外部鍵）。
- 每日指標：`game_metric_daily`；加權計算後寫入 `popularity_index_daily`。
- 榜單：定期產製 `leaderboard_snapshots(period, ts)`；前台提供日/週榜查詢。
- 洞察貼文：`posts`（狀態、置頂、可綁 `game_id`）；發佈時快照 `post_metric_snapshot`、引用來源 `post_sources`。

---

### 3.6 論壇與互動
**資料表**：`forums`、`threads`、`thread_posts`、`reactions`、`bookmarks`

**功能**：
- 版面（每遊戲一版）：`forums(game_id unique)`。
- 主題/回覆：`threads`、`thread_posts`（支援一層父回覆）。
- 互動：
  - 讚/表情：`reactions`（`target_type` = post/thread/thread_post，去重唯一鍵）。
  - 收藏：`bookmarks`（target 可為 post/thread/game/forum）。
- 內容狀態：`threads.status`、`thread_posts.status`（normal/hidden/archived/deleted）。

---

### 3.7 社交/通知/聊天/群組
**資料表**：`Notifications`、`Notification_Sources`、`Notification_Actions`、`Notification_Recipients`、`Chat_Message`、`Groups`、`Group_Member`、`Group_Chat`、`Group_Block`

**功能**：
- 通知：來源/行為字典建置 → 建立 `Notifications`，投遞至 `Notification_Recipients`；前台收件匣既讀回寫 `is_read / read_at`。
- 私訊：`Chat_Message`（user→user 或客服 `manager_id`）
- 群組：建立/加入/退出 `Groups` + `Group_Member`；群聊 `Group_Chat`；群封鎖 `Group_Block`（同群同人唯一）。

---

### 3.8 小遊戲/簽到/寵物
**資料表**：`UserSignInStats`、`Pet`、`MiniGame`

**功能**：
- 每日簽到：新增 `UserSignInStats` 一筆（含 `PointsChanged` / `ExpGained` 與時間戳）。
- 寵物：`Pet` 狀態面板（等級/各屬性/外觀顏色與異動時間）。
- 小冒險：`MiniGame` 記錄每局參數、結果、點數/屬性變化、時間（Start/End/Aborted）。

> **應用層邏輯**：簽到/小遊戲產生的 `PointsChanged` 需要同步更新 `User_wallet.User_Point`。

---

## 4. 後台功能規格

### 4.1 管理者/角色/登入追蹤
- 管理者 CRUD：`ManagerData`（含 `Administrator_registration_date`）。
- 角色權限定義：`ManagerRolePermission`；角色指派：`ManagerRole`。
- 後台登入追蹤：`Admins.last_login`。
- 禁言項/樣式池：`Mutes`、`Styles`（供論壇/社群工具使用）。

### 4.2 使用者治理
- 用戶檔案查詢：聯合 `Users`、`User_Introduce`、`User_Rights`、`User_wallet`、`MemberSalesProfile`、`User_Sales_Information`。
- 權限調整：寫入 `User_Rights`（停權/留言/購物/銷售）。
- 點數/銷售錢包調整（審批流）：系統操作頁面修改 `User_wallet.User_Point` / `User_Sales_Information.UserSales_Wallet`，並以**管理操作日誌**（系統層）留存。

### 4.3 官方商城營運
- 供應商：`Supplier` 新增/維護。
- 商品：`PI` + `GPD`/`OPD` 建立/上下架/價格、庫存維護；所有異動寫 `PIAL`（欄位名、舊新值、管理者、時間）。
- 訂單：`OrderInfo`、`OrderItems`（查詢/退款/出貨/完成）。
- 排行榜產製：寫 `OSR`；可設定期間與指標。

### 4.4 自由市場治理
- 商品管理：`PMP`（審核/狀態 `p_status` 調整）；圖片檢視 `PlayerMarketProductImgs`。
- 訂單/交易頁：`PO`、`POTP`（平台抽成、完成時間）；訊息稽核 `PlayerMarketTradeMsg`。
- 市場榜單：`PMR` 產製與查詢。

### 4.5 內容/社群
- 論壇：`forums`、`threads`、`thread_posts`（隱藏/封存/刪除）；置頂 `posts.pinned`（洞察）。
- 通知：字典維護、公告群發（建立 `Notifications`；批次寫入 `Notification_Recipients`）。
- 群組：`Groups` 與 `Group_Member` 管理、`Group_Block` 黑名單。

### 4.6 熱度與榜單
- 來源/指標配置：`metric_sources`、`metrics`、`game_source_map`。
- 清洗入庫：`game_metric_daily`；指數計算：`popularity_index_daily`。
- 榜單快照：`leaderboard_snapshots` 產製與回補。

---

## 5. 狀態機與流程

### 5.1 官方商城訂單（OrderInfo）
- `order_status`（NVARCHAR，建議值）：
  - **Created**（建立）→ **ToShip**（待出貨）→ **Shipped**（已出貨）→ **Completed**（已完成）
  - 取消/退款（選配：於應用層標記，資料表保留原狀態字串）
- `payment_status`：`Placed`（下單）→ `Pending`（待付款）→ `Paid`（已付款）
- 重要時間戳：`payment_at`、`shipped_at`、`completed_at`。
- **不更動資料表**前提下，狀態值由應用層**嚴格校驗**，避免任意文字。

**流程**：
1) 建立訂單 O + 明細 OD（`payment_status=Placed`，`order_status=Created`）。
2) 付款成功：`payment_status=Paid`、回寫 `payment_at`；若可出貨 → 將 `order_status=ToShip`。
3) 出貨：設 `shipped_at`，`order_status=Shipped`。
4) 完成：`completed_at`，`order_status=Completed`。

### 5.2 自由市場（PO + POTP）
- `p_order_status`（建議值）：`Created` → `Trading`（交易中） → `Completed` / `Cancelled`。
- `p_payment_status`：`Pending` → `Paid`（若採金流/點數）或 `N/A`（道具對交後才入帳）。
- 交易頁：`seller_transferred_at` 與 `buyer_received_at` 皆有值 → `completed_at`；平台抽成 `p_order_platform_fee`。

**流程**：
1) 下單（`PO` 建立）。
2) 交易頁建立（`POTP`），雙方對交 → 系統判斷雙方時間戳存在。
3) 完成 → 入帳：賣家加 `UserSales_Wallet`；買家扣點（如適用）。

### 5.3 貼文/論壇
- `posts.status`：`draft` → `published` → `hidden`（可逆）。
- `threads.status`、`thread_posts.status`：`normal` / `hidden` / `archived` / `deleted`（依後台操作與社群規範）。

### 5.4 通知
- 建立 `Notifications` → 批次 `Notification_Recipients`（每收件者一筆，`is_read` 初始 0）。
- 前台開啟通知列表/點開 → 更新 `is_read=1`、`read_at`。

### 5.5 簽到/小遊戲/寵物
- 簽到：新增 `UserSignInStats`，同時**應用層**把 `PointsChanged` 加總至 `User_wallet.User_Point`。
- 小遊戲：寫 `MiniGame` 一筆（含點數/屬性變化量與結束狀態）。
- 寵物升級：`Pet.Level` 與 `LevelUpTime` 由應用層依 `Experience` 與規則更新。

---

## 6. API 規格（重點端點）
> 命名以 REST 風格，回應一律 JSON；欄位沿用資料表命名為主，必要時在 DTO 層做別名。

### 6.1 Auth / Users
- `POST /api/auth/register` → 建立 `Users`、預設 `User_Rights`、`User_wallet`。
- `POST /api/auth/login` → 回 JWT。  
- `GET /api/users/me` → 讀取 `Users` + `User_Introduce` + `User_Rights` + `User_wallet`。
- `PUT /api/users/me/introduce` → 更新 `User_Introduce`（唯一鍵檢查）。

### 6.2 Wallet / Sales
- `GET /api/wallet` → 讀 `User_wallet`。
- `POST /api/sales/apply` → 建立/更新 `MemberSalesProfile`；後台審核改 `User_Rights.SalesAuthority`。
- `GET /api/sales/wallet` → 讀 `User_Sales_Information`。

### 6.3 Official Store
- `GET /api/store/products`（查 `PI`，依 `product_type` 裝配 `GPD/OPD`）
- `GET /api/store/products/{id}`
- `POST /api/store/orders` → 建立 `O` + `OD`
- `POST /api/store/orders/{orderId}/pay/callback` → 付款成功更新 `payment_status/at` + 狀態遷移
- `GET /api/store/orders/{orderId}`
- `GET /api/store/rankings?period=day|month|quarter|year` → 讀 `OSR`

### 6.4 Player Market
- `POST /api/market/products` → 建 `PMP`（需 `SalesAuthority=1`）
- `POST /api/market/products/{id}/images` → 新增 `PlayerMarketProductImgs`
- `GET /api/market/products`
- `POST /api/market/orders` → 建 `PO`
- `POST /api/market/tradepages` → 建 `POTP`
- `POST /api/market/tradepages/{id}/messages` → `PlayerMarketTradeMsg`
- `POST /api/market/tradepages/{id}/seller-transferred` → 回寫 `seller_transferred_at`
- `POST /api/market/tradepages/{id}/buyer-received` → 回寫 `buyer_received_at`（若兩端具備 → 設 `completed_at`、執行入出帳）
- `GET /api/market/rankings` → 讀 `PMR`

### 6.5 Forums & Posts
- `GET /api/forums`（`forums`）/ `GET /api/forums/{forumId}/threads`
- `POST /api/forums/{forumId}/threads` / `POST /api/threads/{threadId}/posts`
- `POST /api/reactions`（`user_id` + `target_type` + `target_id` + `kind`）
- `POST /api/bookmarks`

### 6.6 Insights / Leaderboards
- `GET /api/leaderboards?period=daily|weekly&ts=YYYY-MM-DD` → `leaderboard_snapshots`
- `GET /api/posts` / `GET /api/posts/{postId}`（含 `post_sources`、`post_metric_snapshot`）

### 6.7 Social / Notifications / Groups
- `GET /api/notifications`（收件匣）/ `POST /api/notifications/{id}/read`
- `GET /api/chat?peerId=` / `POST /api/chat`（`Chat_Message`）
- `POST /api/groups` / `POST /api/groups/{id}/join` / `POST /api/groups/{id}/leave`
- `GET /api/groups/{id}/messages` / `POST /api/groups/{id}/messages`
- `POST /api/groups/{id}/block` / `DELETE /api/groups/{id}/block/{userId}`

> **錯誤格式**：`{ code: string, message: string, details?: any }`

---

## 7. 驗證與商規

- **唯一值**：
  - `Users.User_Account`；`User_Introduce` 的 `User_NickName/IdNumber/Cellphone/Email`。
- **權限檢查**：
  - 留言：`User_Rights.MessagePermission=1`；購物：`ShoppingPermission=1`；上架：`SalesAuthority=1`。
- **圖片上限**：`varbinary(max)` 由應用層限 5–10MB；病毒掃描；格式白名單（JPEG/PNG/WebP）。
- **金流/點數**：
  - 官方商城：以 `O.order_total` 為準；付款完成再扣點或只是現金（依支付邏輯）。
  - 自由市場：完成交易頁雙方確認後入/出帳；平台抽成以 `POTP.p_order_platform_fee` 計算。
- **時區**：所有 `datetime2` 若無註明，採系統 UTC，前端顯示轉地區時間。

---

## 8. 安全與稽核

- 密碼僅存雜湊；登入錯誤次數限制；重要操作二次確認。
- API 授權：前台 JWT、後台角色政策（對應 `ManagerRolePermission`）。
- 稽核：商品異動 `PIAL`；管理者登入 `Admins.last_login`；通知與訊息皆可查表追溯。

---

## 9. 效能與索引（不更動表結構前提下）
- 建議對高頻查詢欄位加索引（若未建）：
  - 市集：`PMP(seller_id, p_status, created_at)`、`PO(buyer_id, seller_id, p_order_status)`、`POTP(p_order_id)`
  - 商城：`O(user_id, order_status, payment_status, order_date)`、`OD(order_id)`、`PI(product_type)`
  - 論壇：`threads(forum_id, updated_at)`、`thread_posts(thread_id, created_at)`
  - 通知：`Notification_Recipients(user_id, is_read)`
  - 熱度：已內建（`game_metric_daily`、`leaderboard_snapshots`）

---

## 10. 測試大綱（UAT 方向）
- 註冊/登入/個資唯一性/頭像上傳。
- 權限位元影響：停權/留言/購物/上架四種情境。
- 商城：完整下單→付款→出貨→完成；跨時區顯示；排行榜顯示。
- 市集：上架→下單→交易頁雙確認→抽成→入/出帳；交易訊息流；排行榜。
- 熱度與洞察：指數計算寫入、榜單快照、貼文快照。
- 論壇：建立主題/回覆、讚/收藏、狀態切換。
- 通知收件匣與已讀；群組建立/訊息/封鎖。
- 簽到/小遊戲：點數/屬性變化與寵物面板更新。

---

## 11. 交付物與里程碑（建議）
- **M1**：帳號/權限/個資/通知/私訊基礎；論壇只讀。
- **M2**：官方商城購物流（不含退款）；供應商/商品後台。
- **M3**：自由市場上/下架與交易頁；抽成入帳；訊息完成。
- **M4**：遊戲熱度/榜單與洞察；論壇完整互動。
- **M5**：寵物/小遊戲/簽到串點數；排行榜整合；群組。

> 本時程僅示例，實際依團隊調整。

---

### 附錄：狀態值建議（應用層校驗，不改表）
- `OrderInfo.order_status`：`Created|ToShip|Shipped|Completed|Cancelled`
- `OrderInfo.payment_status`：`Placed|Pending|Paid`
- `PMP.p_status`：`Draft|Active|Paused|Sold|Cancelled`
- `PO.p_order_status`：`Created|Trading|Completed|Cancelled`
- `PO.p_payment_status`：`Pending|Paid|N/A`
- `posts.status`：`draft|published|hidden`
- `threads.status` / `thread_posts.status`：`normal|hidden|archived|deleted`

> 上述值皆為**字串建議清單**，由程式端做 enum 校驗；若日後需要 i18n，於程式層做對應映射即可（不動資料庫）。


---

# ✅ 補強與最終規格合併（依你提供的**定版資料庫** + 備註規則）
> 下列內容 **不新增資料表**，嚴格以你提供的最終 Schema 為主，將備註中的細節（公式、規則）全面落稿到前台/後台需求與 API／流程規格中。若資料表預設值與業務規則不同，採用「**應用層預設** 覆蓋 DB 預設」的方式達成（不改 DDL）。所有每天 00:00 的重置與統計，皆以 **Asia/Taipei**（UTC+8）為基準。

## 0. 名詞對照與欄位映射
- **energy ≡ `Pet.Stamina`**、**hunger ≡ `Pet.Hunger`**、**cleanliness ≡ `Pet.Cleanliness`**、**health ≡ `Pet.Health`**、**mood ≡ `Pet.Mood`**。
- 會員點數餘額：`User_wallet.User_Point`；事件型點數變動：
  - 簽到：`UserSignInStats.PointsChanged`
  - 小遊戲：`MiniGame.PointsChanged`
  - 寵物換色：`Pet.PointsChanged`（最近一次）
- 由於 Schema 未設「完整點數流水」，**收支明細以「多來源彙整」產生**（詳見 1.3）。

---

## 1) 會員點數系統（Points）
### 1.1 前台（Client）
- **查看當前點數餘額**：顯示 `User_wallet.User_Point`。
- **查看收支明細**：以「事件彙整」呈現（簽到、小遊戲、寵物換色、管理調整），支援期間/事件類型/結果篩選。

### 1.2 後台（Server）
- **查看/調整會員點數**：
  - 調整行為：直接更新 `User_wallet.User_Point`。
  - **稽核留痕**：因無專屬流水表，**必填調整原因與管理者**，並同時：
    1) 對目標用戶發送一筆 `Notifications`（`source=system`、`action=points_adjustment`），作為**可查詢的帳務通知**。
    2) 於應用層審計日誌（外部 APM/Log）記錄（不可刪改）。

### 1.3 收支明細（Ledger）彙整規格（唯讀）
- API 以 `UNION ALL` 的觀念，自各來源**動態生成流水**：
  - 簽到：`UserSignInStats.LogID/SignTime/PointsChanged` → 類型 `signin`。
  - 小遊戲：`MiniGame.PlayID/StartTime/PointsChanged/Result` → 類型 `minigame`。
  - 寵物換色：`Pet.PointsChanged/ColorChangedTime`（僅最近一次）→ 類型 `pet_color`。
  - 後台調整：由 `Notifications`（`action=points_adjustment`）還原 → 類型 `adjustment`。
- 欄位：`id`、`ts`、`type`、`delta`、`balance_after`(可選) 、`meta`（JSON：來源鍵、說明）。
- **注意**：寵物換色僅能顯示最近一次（Schema 限制）；完整歷史由通知/外部稽核提供。

### 1.4 API（草案）
- `GET /api/wallet/balance` → `{ balance }`（讀 `User_wallet`）
- `GET /api/wallet/ledger?from=&to=&type=` → 分頁回傳彙整流水
- `POST /api/admin/wallet/adjust`（Admin）
  - Body: `{ userId, delta, reason }` → 更新餘額 + 建立通知 + 審計日誌

---

## 2) 會員簽到系統（每日 00:00 重置規則與獎勵）
### 2.1 前台（Client）
- **查看簽到簿**：顯示本月日曆、已簽到日、當前連續天數、曾獲額外獎勵標記。
- **簽到**：每日限一次（以 **Asia/Taipei** 的日界線判定）。

### 2.2 後台（Server）
- **簽到規則設定**（應用層設定，不改 DB）：
  - 平/假日獎勵、連 7 天加碼、全月加碼、是否開放補簽（本案=**不開放**）
- **查看/調整會員簽到紀錄**：
  - 查詢 `UserSignInStats`；必要時可新增「修正用」簽到紀錄（`PointsChanged=0`、`ExpGained=0`）並於通知中備註。

### 2.3 套用的獎勵表（固定版）
| 日期類型 | 點數 | 寵物經驗 |
|---|---:|---:|
| 平日(一~五) | **+20** | **0** |
| 假日(六、日) | **+30** | **+200** |
| 連續 7 天（當日額外） | **+40** | **+300** |
| 連續「當月全勤」（當月最後一日簽到時一次發） | **+200** | **+2000** |

> **無補簽**；**簽到回饋**：會員可消耗 **2000 點** 為寵物換膚色（見 §3.5）。

### 2.4 連續判定與重置
- **日界線**：每日 00:00（Asia/Taipei）。
- **唯一性**：同一用戶同一自然日僅允許一筆。以應用層約束（若需強制，於業務層加「`WHERE NOT EXISTS`」判斷）。
- **連續 7 天**：簽到當下回看連續區間長度==7 → 立即發放加碼。
- **當月全勤**：在當月最後一日簽到時檢核當月 1..最後日是否皆有紀錄 → 通過則立即加碼。

### 2.5 API（草案）
- `GET /api/signin/status` → `{ todaySigned, currentStreak, monthAttendance }`
- `POST /api/signin` → 依規則新增一筆 `UserSignInStats`，並回傳實際發放 `{ pointsDelta, expDelta, streakAfter }`

---

## 3) 虛擬寵物系統（史萊姆）
> **一人一寵**（業務規則）：每位會員僅可擁有 1 隻史萊姆。

### 3.1 初始設定（應用層預設，覆蓋 DB 預設值）
- 新建寵物時，以下欄位初始化為 **100**：`Hunger/Mood/Stamina/Cleanliness/Health`。
- `Level=1`、`Experience=0`、`PetName='小可愛'`（若未指定）。

### 3.2 屬性範圍與鉗位
- 所有屬性維持 **0..100**；寫入前一律 `min(max(value,0),100)`。

### 3.3 互動行為（Client 操作 → Server 原子更新）
| 行為 | 變化 |
|---|---|
| 餵食 | `Hunger += 10` |
| 洗澡 | `Cleanliness += 10` |
| 玩耍 | `Mood += 10` |
| 休息 | `Stamina += 10` |

> 互動後若 `Hunger/Mood/Stamina/Cleanliness` **均達 100**，則 `Health = 100`。

### 3.4 每日自動衰減（每日 00:00）
- `Hunger −20`、`Mood −30`、`Stamina −10`、`Cleanliness −20`、`Health −20`（完成後做 0..100 鉗位）。

### 3.5 健康度檢查與禁玩條件
- 互動或冒險結束後，依下列規則調整 `Health`：
  - 若 `Hunger < 30` → `Health −20`
  - 若 `Cleanliness < 30` → `Health −20`
  - 若 `Stamina < 30` → `Health −20`
- `Health` **最低 0**。若 `Health==0` 或任一屬性為 0，**禁止開始冒險**，需先恢復。

### 3.6 換色（扣會員點數）
- 費用：**2000 點**（後台可於「系統設定」調整；實作為應用層設定檔）。
- 成功時：
  - 扣 `User_wallet.User_Point`，寫入 `Pet.SkinColor`/`BackgroundColor`、`Pet.PointsChanged=2000`、`PointsChangedTime=NOW()`、`ColorChangedTime=NOW()`。
  - 發送 `Notifications`（source=system, action=pet_color_change）。
- 歷史查詢：因 Schema 僅存最近一次，完整歷程由通知列表彌補。

### 3.7 等級與經驗
- 上限：`Level ≤ 250`。
- **升級需求（下一級所需 EXP）**：
  1) `Level 1–10`：`EXP = 40 × level + 60`
  2) `Level 11–100`：`EXP = 0.8 × level^2 + 380`
  3) `Level ≥ 101`：`EXP = 285.69 × (1.06^level)`
- 經驗來源：互動（可選給少量）、小遊戲、簽到（假日/連續獎勵）。
- **升級獎勵**：給予會員點數（**後台可設定**；未設定則 0）。
- 流程：當 `Experience` ≥ 當前級別需求 → `Level += 1` 並扣除需求值（餘額保留，直到不滿足下一級）。

### 3.8 前台/後台功能清單
- **Client**：
  - 寵物基本資料修改（名字、外觀）
  - 互動（餵食/洗澡/玩耍/休息）
  - 換色（扣點二次確認）
- **Server**：
  - **全域規則設定**：升級公式（唯讀展示）、互動增益、可用外型與顏色、換色點數成本、是否開放互動冷卻等
  - **會員個別設定/查詢**：基本資料、五維狀態、等級/經驗、顏色；換色紀錄（取自通知 + `Pet.*ChangedTime`）

### 3.9 API（草案）
- `GET /api/pet` → 回傳 `Pet` 全量（含五維、顏色、等級/經驗）
- `PUT /api/pet/profile` → 改名/外觀（不含扣點）
- `POST /api/pet/actions/{feed|bathe|play|rest}` → 原子更新 + 鉗位 + 健康檢查
- `POST /api/pet/recolor` → 2000 點檢核/扣款 + 更新顏色 + 通知

---

## 4) 小遊戲：出發冒險（MiniGame）
> 使用 `MiniGame` 表完整記錄每次冒險（含屬性 Δ、點數/經驗、起迄時間、是否中退）。

### 4.1 每日次數限制
- 每名會員每日 **最多 3 次**；日界線：每日 00:00（Asia/Taipei）。
- 判定方式：統計當日（`StartTime` 落於本日）且 `Aborted=0` 的筆數。

### 4.2 起迄與進度規則
- 初次冒險 `Level=1`；若 **勝利** → 下次預設 `Level = min(Level+1, 上限)`；若 **失敗** → 保持原等級。

### 4.3 關卡與獎勵（預設表，可於後台系統設定調整）
| 關卡 | 怪物數 | 速度 | 獎勵 |
|---:|---:|---|---|
| 1 | 6 | 基礎 | `+100 exp, +10 點數` |
| 2 | 8 | 加快 | `+200 exp, +20 點數` |
| 3 | 10 | 再加快 | `+300 exp, +30 點數` |

> 對應欄位：`MiniGame.Level/MonsterCount/SpeedMultiplier/ExpGained/PointsChanged`。

### 4.4 結算屬性變化
| 結果 | hunger | mood | stamina | cleanliness |
|---|---:|---:|---:|---:|
| 輸 | −20 | −30 | −20 | −20 |
| 贏 | −20 | **+30** | −20 | −20 |

- 完成後執行 **健康度檢查**（§3.5），並做 0..100 鉗位。
- 若任一屬性或 `Health==0`，**禁止開始下一局**。

### 4.5 共同定時任務（每日 00:00）
- **重置冒險次數** + **執行寵物每日衰減**（§3.4）。

### 4.6 前台/後台功能清單
- **Client**：
  - 出發冒險（建立 `MiniGame` 記錄）
  - 查看遊戲紀錄（時間/輸贏/獲得獎勵）
- **Server**：
  - 規則設定：每關怪物數/速度倍率/獎勵、每日次數上限
  - 查看會員遊戲紀錄：可依使用者/日期/結果查詢

### 4.7 API（草案）
- `GET /api/minigame/records?from=&to=&result=` → 取 `MiniGame` 清單
- `POST /api/minigame/start` → 建立一筆進行中的記錄（檢查次數與寵物可玩條件）
- `POST /api/minigame/finish` → 結算（寫 `Result/ExpGained/PointsChanged/Δ屬性`，更新 `Pet` 五維/經驗/等級）

---

## 5) 與既有模組的銜接說明（重點）
- **通知（Notifications）**：
  - 點數調整（後台）→ 建立 `action=points_adjustment`。
  - 寵物換色 → `action=pet_color_change`，內含顏色與扣點資訊。
- **排行榜/熱門洞察**：
  - 與本次增修無耦合；既有 `popularity_index_daily/leaderboard_snapshots/posts` 按原規格。
- **商城/自由市場**：
  - 與會員點數邏輯分離（商城用金流/幣別，非平台點數）。

---

## 6) 邊界與一致性策略
- **交易/互動皆採原子更新**：以 DB 交易包住「讀取 → 檢核 → 更新」。
- **併發防護**：樂觀鎖（以 `updated_at` 或重新讀取校驗），避免多次點擊造成超發。
- **日界線一致性**：所有每日規則以 **Asia/Taipei** 為唯一時間基準。
- **資料限制不一致處理**：
  - DB 內 `Pet` 五維預設 0；**建立寵物流程**會以應用層將其改為 100。
  - 完整點數流水：Schema 不足，**以多來源彙整 + 通知 + 外部稽核**達到可追溯性。

---

## 7) 測試案例（精選）
1. **簽到平日**：週二簽到 → `+20` 點、`+0` EXP；當天重簽被拒。
2. **簽到假日**：週六簽到 → `+30` 點、`+200` EXP。
3. **連 7 天**：第 7 天簽到再加發 `+40` 點、`+300` EXP（單日可獲多筆獎勵）。
4. **當月全勤**：月底最後一天簽到加發 `+200` 點、`+2000` EXP。
5. **互動滿格回滿健康**：四維操作到 100 → `Health` 被設為 100。
6. **每日衰減**：00:00 後檢核五維按規則下降並鉗位不為負。
7. **冒險次數限制**：一天進 3 次後第 4 次被拒，隔天 00:00 回滿。
8. **冒險禁玩**：任一屬性為 0 時 `start` API 被拒。
9. **換色扣點**：餘額 ≥2000 成功，`Pet.*ChangedTime` 更新且產出通知；餘額不足被拒。
10. **後台調整**：Admin 調整 +100 → 餘額改變、通知生成、審計有紀錄。

---

> 若你要，我可以把這一段規則輸出為「後端設定檔樣板（appsettings.json）」與「Swagger YAML 範本」，方便團隊直接落地。



我之前傳給你的是我專案正式的資料庫(一個字都不會再更動了，已經定版了) ， 你先閱讀此.md檔所敘述的專案架構和我之前傳給你的專案正式的資料庫(一個字都不會再更動了，已經定版了)

補充: 使用ASP.NET (MVC) + C# +SQL Server + HTML / CSS / JavaScript 和一切衍伸套件例如bootstrap jquery（支援 Vue / Tailwind / shadcn）
登入機制：Email、自訂帳號 + OAuth（Google、Facebook、Discord） 需使用三層式架構設計（ASP.NET MVC）
Razor / HTML / Vue 組件設計頁面與互動
邏輯層：各功能模組控制器（例如 SignInController, PetController）
資料層：透過 EF Core 存取 SQL Server，如 PetRepository, UserRepository

補充: 並且要有 測試 (例如單元測試 整合測試 端對端測試 ....等等 可能會用到的測試)


全部一切包含前台 後台 資料庫 和 前台的前端畫面和後台的前端畫面 反正一切東西都要實做出來 讓我馬上可以用 而且要考慮到美觀性 我現在傳給你的範例就是我們要採用的前端風格 <!DOCTYPE html> <html lang="zh-Hant"> <head> <meta charset="UTF-8" /> <meta name="viewport" content="width=device-width, initial-scale=1.0"/> <title>GameCore｜合併最終版 v2.3（玻璃風＋彩色看板＋強化排行＋置頂＋查看更多＋我的史萊姆）</title> <style> :root{ --bg:#eef3f8; --bg2:#ffffff; --ink:#1f2937; --muted:#64748b; --line:#e5e7eb; --surface:rgba(255,255,255,.75); --glass:rgba(255,255,255,.45); --accent:#7557ff; --accent-2:#34d2ff; --accent-3:#22c55e; --radius:18px; --radius-sm:12px; --shadow:0 18px 40px rgba(17,24,39,.12); --blur:14px; /* 排行榜配色（參考檔風格） */ --ok:#22c55e; --down:#ff6b6b; --flat:#9aa8bf; --gold:#FFD700; --silver:#C0C0C0; --bronze:#CD7F32; --goldA:rgba(255,215,0,.18); --silverA:rgba(192,192,192,.18); --bronzeA:rgba(205,127,50,.18); } body.dark{ --bg:#0c111b; --bg2:#0a0f18; --ink:#e5edff; --muted:#9fb1c9; --line:#1e2b43; --surface:rgba(22,30,48,.65); --glass:rgba(18,24,39,.50); --shadow:0 18px 42px rgba(0,0,0,.35); } body.compact{ --radius:14px; --radius-sm:10px } *{box-sizing:border-box} html,body{height:100%} body{ margin:0; color:var(--ink); font:16px/1.65 system-ui,-apple-system,"Segoe UI",Roboto,"Noto Sans TC","PingFang TC","Microsoft JhengHei",sans-serif; background: radial-gradient(900px 500px at -10% -10%, rgba(117,87,255,.10), transparent 60%), radial-gradient(800px 460px at 110% 10%, rgba(52,210,255,.10), transparent 50%), linear-gradient(180deg, var(--bg), var(--bg2)); } a{color:inherit; text-decoration:none} .wrap{max-width:1380px; margin:0 auto; padding:0 16px} /* AppBar */ header.appbar{position:sticky; top:0; z-index:50; backdrop-filter:saturate(140%) blur(var(--blur)); background:var(--glass); border-bottom:1px solid var(--line)} .bar{display:flex; align-items:center; gap:12px; padding:10px 0} .logo{display:flex; align-items:center; gap:10px; font-weight:900} .logo-badge{width:36px; height:36px; border-radius:10px; background:linear-gradient(135deg,var(--accent), var(--accent-2)); color:#fff; display:grid; place-items:center; box-shadow:var(--shadow)} .search{flex:1; display:flex; gap:8px; background:var(--surface); border:1px solid var(--line); border-radius:12px; padding:8px 10px} .search input{flex:1; background:transparent; border:0; outline:0; color:inherit} .main-nav{display:flex; gap:8px; flex-wrap:wrap} .pill{display:inline-flex; align-items:center; gap:6px; border:1px solid var(--line); background:var(--surface); padding:8px 12px; border-radius:999px; font-weight:700} .pill.on{background:linear-gradient(90deg,#3b82f6,#a78bfa); color:#fff; border-color:transparent} .top-actions{display:flex; gap:8px; align-items:center} .btn{border:1px solid var(--line); background:var(--surface); padding:8px 12px; border-radius:12px; cursor:pointer} .btn.primary{border:0; background:linear-gradient(135deg,var(--accent),#3b82f6); color:#fff; box-shadow:var(--shadow)} .btn.link{background:transparent; border:1px dashed var(--line)} .switch{display:flex; align-items:center; gap:6px; font-size:13px} .dot{width:16px; height:16px; border-radius:50%; border:1px solid var(--line); cursor:pointer} .dot.c1{background:#7557ff}.dot.c2{background:#34d2ff}.dot.c3{background:#22c55e} /* Tiles（彩色看板） */ .tiles{padding:16px 0} .grid-tiles{display:grid; gap:12px; grid-template-columns:repeat(6, minmax(0,1fr))} @media (max-width:1100px){.grid-tiles{grid-template-columns:repeat(4, minmax(0,1fr))}} @media (max-width:740px){.grid-tiles{grid-template-columns:repeat(2, minmax(0,1fr))}} .tile{position:relative; min-height:120px; border-radius:16px; padding:14px; overflow:hidden; display:flex; flex-direction:column; justify-content:space-between; cursor:pointer; background:var(--glass); border:1px solid var(--line); backdrop-filter:blur(var(--blur)); transition:transform .16s ease, box-shadow .16s ease, filter .2s ease; box-shadow:var(--shadow)} .tile:hover{transform:translateY(-3px) scale(1.01)} .tile .name{font-weight:900} .tile .meta{color:var(--muted); font-size:12px} .mini{background:rgba(255,255,255,.65); padding:2px 8px; border-radius:999px; font-size:12px; border:1px solid var(--line)} body.dark .mini{background:rgba(0,0,0,.25); color:#e5edff} /* 彩色漸層（看板） */ .tile.colorful:nth-child(1){background:linear-gradient(135deg,#4f46e5,#22d3ee)} .tile.colorful:nth-child(2){background:linear-gradient(135deg,#f43f5e,#f59e0b)} .tile.colorful:nth-child(3){background:linear-gradient(135deg,#22c55e,#16a34a)} .tile.colorful:nth-child(4){background:linear-gradient(135deg,#8b5cf6,#06b6d4)} .tile.colorful:nth-child(5){background:linear-gradient(135deg,#f97316,#ef4444)} .tile.colorful:nth-child(6){background:linear-gradient(135deg,#06b6d4,#3b82f6)} .tile.colorful .name,.tile.colorful .meta,.tile.colorful .mini{color:#fff} .tile.colorful .mini{background:rgba(255,255,255,.25); border-color:rgba(255,255,255,.35)} /* Hot（橫向） */ .hot{background:var(--glass); border:1px solid var(--line); border-radius:16px; padding:14px; backdrop-filter:blur(var(--blur)); box-shadow:var(--shadow)} .head{display:flex; align-items:center; gap:10px; margin-bottom:10px} .kicker{font-size:12px; letter-spacing:.12em; color:#6366f1} .scroller{display:flex; gap:12px; overflow:auto; scroll-snap-type:x mandatory; padding-bottom:6px} .card{min-width:280px; background:var(--surface); border:1px solid var(--line); border-radius:16px; padding:12px; scroll-snap-align:start; transition:transform .16s ease, box-shadow .16s ease} .card:hover{transform:translateY(-3px) scale(1.01); box-shadow:var(--shadow)} /* Main layout */ main{margin:16px 0 28px} .layout{display:grid; grid-template-columns:1fr 360px; gap:16px} @media (max-width:980px){.layout{grid-template-columns:1fr}} .panel{background:var(--surface); border:1px solid var(--line); border-radius:16px; box-shadow:var(--shadow); padding:14px; backdrop-filter:blur(var(--blur))} .panel .hd{display:flex; align-items:center; gap:10px; margin-bottom:10px} .panel .title{font-weight:900} .seg{display:inline-flex; border:1px solid var(--line); border-radius:999px; overflow:hidden; margin-left:auto} .seg button{background:transparent; border:0; color:var(--muted); padding:6px 10px; cursor:pointer} .seg .on{background:#1f7ae0; color:#fff} .feed{display:grid; gap:10px} .row{display:grid; grid-template-columns:auto 1fr auto; gap:10px; align-items:center; border:1px solid var(--line); border-radius:14px; padding:10px; background:var(--bg2); transition:transform .16s ease, box-shadow .16s ease} .row:hover{transform:translateY(-3px) scale(1.01); box-shadow:var(--shadow)} .av{width:40px; height:40px; border-radius:12px; background:linear-gradient(135deg,#bae6fd,#e9d5ff); display:grid; place-items:center; font-weight:900; color:#334155} .meta{display:flex; gap:8px; flex-wrap:wrap; color:var(--muted); font-size:13px; margin-top:2px} .chip{border:1px solid var(--line); padding:2px 8px; border-radius:999px; background:var(--surface)} .ghost{border:1px solid var(--line); background:var(--surface); border-radius:10px; padding:6px 10px} /* 置頂區 */ .pinned{border:2px dashed #c4b5fd; background:rgba(196,181,253,.15); padding:10px; border-radius:12px} .pinned .row{background:transparent} /* 右側：原有黏性（保留）、個別卡片可再 sticky */ .right{align-self:start; position:sticky; top:calc(50vh - 320px)} .stack{display:flex; flex-direction:column; gap:16px} /* 跑馬燈（加速） */ .ticker{position:relative; overflow:hidden; border:1px solid var(--line); background:var(--surface); border-radius:12px} .ticker .track{display:flex; gap:32px; padding:10px 12px; white-space:nowrap; animation:ticker 10s linear infinite} @keyframes ticker{0%{transform:translateX(0)}100%{transform:translateX(-50%)}} /* 排行榜（通用） */ .list{display:flex; flex-direction:column; gap:8px} .rrow{position:relative; display:grid; grid-template-columns:40px 1fr auto; gap:10px; align-items:center; border:1px solid var(--line); background:var(--bg2); border-radius:12px; padding:10px; overflow:hidden} .rank{font-weight:900; text-align:center} .delta{text-align:right; min-width:64px; font-weight:800} .up{color:var(--ok)} .down{color:var(--down)} .flat{color:var(--flat)} .rrow.top::before{content:""; position:absolute; inset:0; pointer-events:none; z-index:0; opacity:.9; filter:blur(14px)} .rrow.top1::before{background:linear-gradient(90deg,var(--goldA),transparent 60%)} .rrow.top2::before{background:linear-gradient(90deg,var(--silverA),transparent 60%)} .rrow.top3::before{background:linear-gradient(90deg,var(--bronzeA),transparent 60%)} .rrow.top1 .rank, .rrow.top1 .title-2{color:var(--gold)} .rrow.top2 .rank, .rrow.top2 .title-2{color:var(--silver)} .rrow.top3 .rank, .rrow.top3 .title-2{color:var(--bronze)} .rrow > *{position:relative; z-index:1} .pulse{position:relative; overflow:hidden} .pulse::after{content:""; position:absolute; inset:-3px; border-radius:calc(var(--radius) + 6px); background:linear-gradient(90deg,var(--accent),var(--accent-2),var(--accent)); filter:blur(10px); opacity:.16; z-index:0; animation:pulseGlow 2.8s linear infinite} .pulse > *{position:relative; z-index:1} @keyframes pulseGlow{0%{transform:translateX(-20%)}50%{opacity:.26}100%{transform:translateX(20%)}} /* 類別排行網格（加寬） */ #cats2.grid-tiles{grid-template-columns:repeat(3,minmax(0,1fr))} @media (max-width:1100px){#cats2.grid-tiles{grid-template-columns:repeat(2,minmax(0,1fr))}} @media (max-width:740px){#cats2.grid-tiles{grid-template-columns:repeat(1,minmax(0,1fr))}} #cats2 .tile{min-height:auto} /* Modal（美化） */ .modal{position:fixed; inset:0; background:rgba(8,12,20,.85); display:none; align-items:center; justify-content:center; padding:24px; z-index:9999} .modal.show{display:flex} .modal .panel{position:relative; background:var(--surface); border:1px solid var(--line); border-radius:20px; max-width:980px; width:100%; max-height:86vh; overflow:auto; padding:18px 18px 22px; display:flex; flex-direction:column; gap:12px; backdrop-filter:blur(var(--blur))} .mh{display:flex; align-items:center; gap:10px; margin-bottom:6px} .mh .subtitle{color:var(--muted); font-size:13px} .closeX{position:absolute; top:12px; right:12px; border:1px solid var(--line); background:var(--surface); border-radius:50%; width:36px; height:36px; cursor:pointer} .modal .list .rrow{background:var(--surface)} .modal .list .rrow.top1::before, .modal .list .rrow.top2::before, .modal .list .rrow.top3::before{opacity:1; filter:blur(14px)} footer{color:var(--muted); text-align:center; padding:20px 0; border-top:1px solid var(--line)} /* ====== 右側 Sticky：我的史萊姆卡片（你提供的樣式，微調類名避免衝突） ====== */ .gc-right-sticky{ position: sticky; top:16px; } .gc-pet-card{ font-family: ui-monospace, SFMono-Regular, Menlo, Consolas, monospace; background:#1f2430; color:#e9ecf1; border:1px solid #0006; border-radius:12px; box-shadow:0 8px 24px #0004, inset 0 1px 0 #fff1; padding:12px; width:100%; } .gc-pet-head{ display:flex; align-items:center; justify-content:space-between; margin-bottom:8px; } .gc-pet-title{ font-weight:700; letter-spacing:.5px; } .gc-sub{ color:#aeb6d9; } .gc-coins{ background:#262d3d; border:1px solid #0006; padding:4px 8px; border-radius:999px; } .gc-pet-canvas-wrap{ display:flex; justify-content:center; margin:6px 0 10px; } #gc-pet-canvas{ image-rendering: pixelated; width:160px; height:160px; background:#9ee37d; border:1px solid #0006; border-radius:8px; } .gc-stats{ display:grid; grid-template-columns:1fr; gap:6px; margin-bottom:8px; } .gc-stat{ display:grid; grid-template-columns:auto 1fr auto; gap:8px; align-items:center; } .gc-bar{ height:10px; background:#2b3142; border:1px solid #0008; border-radius:6px; overflow:hidden; } .gc-bar i{ display:block; height:100%; width:0%; background:linear-gradient(90deg,#6de38d,#35c47e); transition:width .25s; } .gc-bar.warn i{ background:linear-gradient(90deg,#ffd159,#ffba4b); } .gc-bar.bad i{ background:linear-gradient(90deg,#ff7b72,#ff5d6a); } .gc-actions{ display:grid; grid-template-columns:repeat(5,1fr); gap:6px; margin-bottom:8px; } .gc-actions button{ background:#2e3440; color:#fff; border:1px solid #0006; border-radius:10px; padding:8px 6px; cursor:pointer; } .gc-actions .gc-accent{ background:#5661ff; } .gc-log{ list-style:none; padding:8px; margin:0; max-height:120px; overflow:auto; background:#262d3d; border:1px solid #0006; border-radius:10px; font-size:12px; color:#c7d0ff; } .gc-log li{ margin:0 0 6px; } .gc-log .warn{ color:#ffe08a; } .gc-log .bad{ color:#ff9b93; } </style> </head> <body> <!-- AppBar --> <header class="appbar"> <div class="wrap bar"> <a class="logo" href="#"><span class="logo-badge">GC</span><span>GameCore</span></a> <form class="search" onsubmit="event.preventDefault(); doSearch2();"> <input id="q2" type="search" placeholder="搜尋文章 / 分區 / 標籤…" aria-label="搜尋"/> <button type="submit" class="btn">搜尋</button> </form> <nav class="main-nav" aria-label="主導覽"> <a class="pill" href="#profile">個人中心</a> <a class="pill" href="#checkin">每日簽到</a> <a class="pill on" href="#forum">論壇</a> <a class="pill" href="#slime">我的史萊姆</a> <a class="pill" href="#shop">官方商城</a> <a class="pill" href="#market">玩家市集</a> <a class="pill" href="#ranks">排行榜</a> <a class="pill" href="#match">約玩配對</a> </nav> <div class="top-actions"> <label class="switch"><input id="t-dark" type="checkbox"/> 深色</label> <label class="switch"><input id="t-density" type="checkbox"/> 緊湊</label> <div class="switch" title="主色"> <span class="dot c1" data-accent="#7557ff"></span> <span class="dot c2" data-accent="#34d2ff"></span> <span class="dot c3" data-accent="#22c55e"></span> </div> <button class="btn primary" id="btnNew2">＋ 發表主題</button> </div> </div> </header> <!-- 看板 Tiles --> <section class="wrap tiles" aria-label="看板彩色小卡"> <div class="grid-tiles" id="tileGrid2"></div> </section> <!-- Hot Threads --> <section class="wrap"> <article class="hot"> <div class="head"><span class="kicker">Hot Threads</span><strong>熱門精選</strong></div> <div class="scroller" id="hotScroller2"></div> </article> </section> <!-- Main --> <main class="wrap layout" id="forum"> <!-- 中欄：文章 Feed --> <section class="panel"> <div class="hd"> <div class="title">最新文章</div> <span id="feedCount2" class="muted" style="margin-left:8px"></span> <div class="seg" role="tablist" aria-label="快速篩選"> <button id="all2" class="on" aria-selected="true">全部</button> <button id="lol2">LOL</button> <button id="steam2">Steam</button> <button id="mobile2">手遊</button> <button id="genshin2">原神</button> <button id="mood2">心情板</button> </div> </div> <!-- 置頂區 --> <div id="pinned" class="pinned" aria-label="置頂文章"></div> <div class="feed" id="feed2"></div> <!-- 改為查看更多按鈕（跳轉論壇主頁） --> <div style="text-align:center;margin-top:12px"> <a class="btn primary" href="forum.html" id="btnMoreFeed">查看更多</a> </div> </section> <!-- 右欄 --> <aside class="right"> <div class="stack"> <!-- ====== 右側 Sticky：我的史萊姆卡片 ====== --> <aside class="gc-right-sticky"> <div class="gc-pet-card" id="gc-pet-card"> <header class="gc-pet-head"> <div> <div class="gc-pet-title">我的史萊姆</div> <small class="gc-sub"> <span id="gc-pet-name">—</span> · Lv.<b id="gc-pet-lv">1</b> · XP <b id="gc-pet-xp">0</b>/<b id="gc-pet-xpmax">50</b> </small> </div> <div class="gc-coins">💰 <b id="gc-coins">0</b></div> </header> <div class="gc-pet-canvas-wrap"> <canvas id="gc-pet-canvas" width="120" height="120"></canvas> </div> <div class="gc-stats"> <div class="gc-stat"><label>飢餓</label><div class="gc-bar"><i id="bar-hunger"></i></div><span id="txt-hunger">0</span></div> <div class="gc-stat"><label>心情</label><div class="gc-bar"><i id="bar-mood"></i></div><span id="txt-mood">0</span></div> <div class="gc-stat"><label>體力</label><div class="gc-bar"><i id="bar-energy"></i></div><span id="txt-energy">0</span></div> <div class="gc-stat"><label>清潔</label><div class="gc-bar"><i id="bar-clean"></i></div><span id="txt-clean">0</span></div> <div class="gc-stat"><label>健康</label><div class="gc-bar"><i id="bar-health"></i></div><span id="txt-health">0</span></div> </div> <div class="gc-actions"> <button data-act="Feed">餵食</button> <button data-act="Bath">洗澡</button> <button data-act="Play">玩耍</button> <button data-act="Rest">休息</button> <button class="gc-accent" id="gc-adv">出發冒險（每日 3 次）</button> </div> <ul class="gc-log" id="gc-log"></ul> </div> </aside> <!-- ====== /我的史萊姆卡片 ====== --> <article class="ticker" aria-label="公告跑馬燈"><div class="track" id="ticker2"></div></article> <article class="panel pulse"> <div class="hd"> <div class="title">跨平台熱門（近 7 / 30 天）</div> <div class="seg" style="margin-left:auto"> <button id="mix7b" class="on">近 7 天</button> <button id="mix30b">近 30 天</button> </div> </div> <div id="mixList2" class="list"></div> <div style="display:flex;justify-content:flex-end;margin-top:8px"> <button class="btn" id="allMix2">查看完整綜合排行</button> </div> </article> <article class="panel"> <div class="hd"><div class="title">熱門標籤</div></div> <div class="meta" id="hotTags2" style="flex-wrap:wrap"></div> </article> <article class="panel"> <div class="hd"><div class="title">本日人氣作者</div></div> <div id="authors2" class="list"></div> </article> </div> </aside> </main> <!-- 類別排行（注意：已移除 moba 類別） --> <section class="wrap" id="ranks" style="margin-top:8px"> <article class="panel"> <div class="hd"> <div class="title">各類遊戲分區熱度排行</div> <div class="seg" style="margin-left:auto"> <button class="on" id="cats7b">近 7 天</button> <button id="cats30b">近 30 天</button> </div> </div> <div class="grid-tiles" id="cats2"></div> <div style="display:flex;justify-content:flex-end;margin-top:8px"> <a class="btn primary" id="allCats2" href="ranks.html">查看更多</a> </div> </article> </section> <!-- 浮動發文（行動） --> <button class="compose" id="fab2">＋ 發表主題</button> <!-- Modal --> <div class="modal" id="modal2"> <div class="panel" id="panel2"> <button class="closeX" id="x2" aria-label="關閉">×</button> <div class="mh"> <strong id="mtitle2">—</strong> <span class="subtitle" id="msub2"></span> </div> <div id="mlist2" class="list" style="min-height:0;"></div> </div> </div> <footer class="wrap">© 2025 GameCore — 假資料示範頁 v2.3（極簡玻璃風，無後端）</footer> <script> /* 假資料 */ const BOARDS2 = [ {key:'lol', name:'英雄聯盟', intro:'版本情報、電競賽事、教學攻略'}, {key:'genshin', name:'原神', intro:'角色配隊、抽卡心得、世界探索'}, {key:'steam', name:'Steam 綜合', intro:'促銷情報、遊戲心得、實況討論'}, {key:'mobile', name:'手機遊戲', intro:'Android / iOS 手遊討論'}, {key:'general', name:'綜合討論', intro:'硬體外設、雜談灌水、求助問答'}, {key:'mood', name:'心情板', intro:'日常、告白、碎碎念、抱怨'} ]; const BULLETINS2 = [ '【活動】投稿最佳攻略贏鍵盤滑鼠組', '【公告】站務規範更新，請勿張貼攻擊性言論', '【社群】本月達標 2,000 則優質回覆，感謝大家！', '【徵稿】實況主合作專題，開放報名', '【修復】行動端卡片重疊在 v2 已修正' ]; const HOT2 = Array.from({length:16}).map((_,i)=>({ t:`焦點話題 #${i+1}｜大改版重點彙整`, m:'包含角色強度與裝備配置、關卡練度門檻、常見 QA…', tag:i%2?'情報':'攻略', board:['LOL','原神','Steam','手機'][i%4] })); /* 熱門標籤 / 本日人氣作者（擴充版） */ const TAGS2_BASE = ['#新手求助','#攻略','#情報','#閒聊','#活動','#同人','#抽卡','#更新','#改版','#Bug','#心得','#PVP', '#PVE','#模擬器','#競速','#MOD','#版務','#公告','#深度解析','#開箱','#評測','#配裝','#地圖','#速刷','#角色','#培養', '#召喚','#副本','#社群','#實況剪輯','#梗圖','#密技','#省電','#設定','#懶人包','#問答','#心得文','#裝備','#天賦','#流派']; const TAGS2 = Array.from({length: (Math.floor(Math.random()*5)+24)}).map((_,i)=>{ const t = TAGS2_BASE[i % TAGS2_BASE.length]; return { t, c: Math.floor(Math.random()*316)+5 }; }); const AUTHORS2 = Array.from({length: 12}).map((_,i)=>{ const names = ['紙箱研究室','夜行貓','璃月曦光','阿傑攻略','老王不打野','低調小廢物','Nia','Klein','Nova','Aster','小K','神樂','阿筆','Rin','Zed']; const name = names[i % names.length] + (i>10 ? ('_' + (i-10)) : ''); return { name, posts: Math.floor(Math.random()*15)+4, likes: Math.floor(Math.random()*361)+60 }; }); const USERS2 = ['Miko','Gary','Kira','Lulu','阿筆','神樂','小K','Jerry','Nia','Zed','Klein','Rin','Nova','Aster']; const TITLES2 = [ '平民向武器替代表（附表格）','改版後坦克裝推薦','首抽角色 CP 分析','實測 120 抽紀錄','打野動線更新（S 賽季）','速刷日常路線（含地圖）','Steam 夏促清單精選','本周活動懶人包','入門三天上手指南','冷門角機體解構','手機省電設定大全' ]; function r(a,b){return Math.floor(Math.random()*(b-a+1))+a} function pick(arr){return arr[r(0,arr.length-1)]} const PINNED = Array.from({length:3}).map((_,i)=>({ title:`【置頂】站務公告與精華整理 #${i+1}`, author: 'Admin', board: 'general', likes: r(30,300), replies: r(10,150), views: r(500,6000), tags: ['#公告','#精華'], minsAgo: r(5,120) })); const POSTS2 = Array.from({length:120}).map((_,i)=>{ const board = pick(BOARDS2.map(b=>b.key)); const title = pick(TITLES2) + (Math.random()<0.22? '（含數據圖表）':'' ); const author = pick(USERS2); const likes = r(0,1200); const replies = r(0,520); const views = likes*3 + r(0,300); const tags = Array.from({length:r(1,3)}).map(()=>pick(TAGS2_BASE)); const minsAgo = r(2, 60*48); return {id:i+1, board, title, author, likes, replies, views, tags, minsAgo}; }); /* 綜合排行 */ const MIX7b = [ {name:'Elden Ring: Shadow', delta:+3}, {name:'Honkai: Star Rail', delta:+1}, {name:'Genshin Impact', delta:-1}, {name:'Valorant', delta:+2}, {name:'Monster Hunter Now', delta:+2}, {name:'League of Legends', delta:+1}, {name:"Baldur's Gate 3", delta:-2}, {name:'PUBG: BATTLEGROUNDS', delta:-1}, {name:'Fortnite', delta:+1}, {name:'Minecraft', delta:+1} ]; const MIX30b = [ {name:'League of Legends', delta:+2}, {name:'Genshin Impact', delta:0}, {name:'Honkai: Star Rail', delta:+2}, {name:'Elden Ring: Shadow', delta:+3}, {name:'Apex Legends', delta:-1}, {name:'Call of Duty', delta:+1}, {name:'PUBG: BATTLEGROUNDS', delta:-2}, {name:'Valorant', delta:+1}, {name:'Monster Hunter Now', delta:+1}, {name:'Stardew Valley', delta:+1}, {name:'Helldivers 2', delta:-1}, {name:'Fortnite', delta:-1}, {name:'Clash of Clans', delta:+1}, {name:"Baldur's Gate 3", delta:-2}, {name:'Overwatch 2', delta:-1} ]; /* 類別排行（移除 moba） */ const CATS7 = { action:['ELDEN RING','Monster Hunter Now','Helldivers 2','Fortnite','Sekiro'], rpg:["Genshin Impact","Baldur's Gate 3",'Honkai: Star Rail','Octopath II','Dragon Quest XI'], indie:['Hades II','Stardew Valley','Vampire Survivors','Dave the Diver','Hollow Knight'], mobile:['Clash of Clans','Arknights','FGO','NIKKE','Uma Musume'] }; const CATS30 = { action:['Monster Hunter Now','ELDEN RING','Fortnite','Helldivers 2','Armored Core VI'], rpg:['Honkai: Star Rail',"Genshin Impact","Baldur's Gate 3",'Starfield','Persona 5 Royal'], indie:['Stardew Valley','Hades II','Hollow Knight','Celeste','RimWorld'], mobile:['Clash of Clans','NIKKE','Arknights','FGO','Genshin Impact (Mobile)'] }; // DOM 參照 const tileGrid2 = document.getElementById('tileGrid2'); const hotScroller2 = document.getElementById('hotScroller2'); const feed2 = document.getElementById('feed2'); const feedCount2 = document.getElementById('feedCount2'); const pinnedEl = document.getElementById('pinned'); const ticker2 = document.getElementById('ticker2'); const mixList2 = document.getElementById('mixList2'); const modal2 = document.getElementById('modal2'); const mlist2 = document.getElementById('mlist2'); const mtitle2 = document.getElementById('mtitle2'); const msub2 = document.getElementById('msub2'); /* Tiles */ function initTiles2(){ tileGrid2.innerHTML = BOARDS2.map((b,i)=>` <a class="tile colorful" href="#" data-board="${b.key}"> <div class="name">${b.name}</div> <div class="meta">${b.intro}</div> <div style="display:flex; gap:6px; flex-wrap:wrap; margin-top:6px"> <span class="mini">今日新貼 ${r(6,40)}</span> <span class="mini">活躍 ${r(40,230)}</span> </div> </a> `).join(''); tileGrid2.querySelectorAll('.tile').forEach(el=>{ el.addEventListener('click', e=>{ e.preventDefault(); filter2(el.dataset.board); }); }); } /* Hot */ function initHot2(){ hotScroller2.innerHTML = HOT2.map(h=>` <article class="card"> <div style="font-weight:900">${h.t}</div> <div style="color:var(--muted); font-size:13px">${h.m}</div> <div style="display:flex; gap:8px; margin-top:8px"> <span class="chip">${h.board}</span> <span class="chip">${h.tag}</span> </div> </article> `).join(''); } /* Ticker */ function initTicker2(){ const twice = BULLETINS2.concat(BULLETINS2); ticker2.innerHTML = twice.map(t=>`<span>🔔 ${t}</span>`).join('<span>·</span>'); } /* Feed */ let cur2 = 'all'; function ago2(min){ if(min<60) return `${min} 分鐘前`; const h=Math.floor(min/60); const r=min%60; return `${h} 小時 ${r} 分鐘前`; } function renderPinned(){ pinnedEl.innerHTML = PINNED.map(p=>` <article class="row"> <div class="av">📌</div> <div> <div style="font-weight:900">${p.title}</div> <div class="meta"> <span>@${p.author}</span><span>｜</span> <span>分區：<strong>${BOARDS2.find(b=>b.key===p.board).name}</strong></span><span>｜</span> <span>${ago2(p.minsAgo)}</span> ${p.tags.map(t=>`<span class="chip">${t}</span>`).join('')} </div> </div> <div style="display:flex; gap:8px"> <span class="ghost">❤️ ${p.likes}</span> <span class="ghost">💬 ${p.replies}</span> <span class="ghost">👁️ ${p.views}</span> </div> </article> `).join(''); } function render2(){ feed2.innerHTML=''; const list = POSTS2.filter(x=> cur2==='all' ? true : x.board===cur2).slice(0,12); list.forEach(p=>{ const el = document.createElement('article'); el.className='row'; el.innerHTML = ` <div class="av">${p.author[0].toUpperCase()}</div> <div> <div style="font-weight:900">${p.title}</div> <div class="meta"> <span>@${p.author}</span> <span>｜</span> <span>分區：<strong>${BOARDS2.find(b=>b.key===p.board).name}</strong></span> <span>｜</span> <span>${ago2(p.minsAgo)}</span> ${p.tags.map(t=>`<span class="chip">${t}</span>`).join('')} </div> </div> <div style="display:flex; gap:8px"> <span class="ghost">❤️ ${p.likes}</span> <span class="ghost">💬 ${p.replies}</span> <span class="ghost">👁️ ${p.views}</span> </div>`; feed2.appendChild(el); }); feedCount2.textContent = `顯示 ${Math.min(12, POSTS2.length)} / ${POSTS2.length} 篇`; } function filter2(key){ cur2 = key==='all' ? 'all' : key; render2(); window.scrollTo({top: document.querySelector('.layout').offsetTop - 60, behavior:'smooth'}); ['all2','lol2','steam2','mobile2','genshin2','mood2'].forEach(id=> document.getElementById(id).classList.remove('on')); const map = { all:'all2', lol:'lol2', steam:'steam2', mobile:'mobile2', genshin:'genshin2', mood:'mood2' }; const btn = document.getElementById(map[cur2]||'all2'); if(btn) btn.classList.add('on'); } function feedCtrl2(){ document.getElementById('all2').onclick = ()=>filter2('all'); document.getElementById('lol2').onclick = ()=>filter2('lol'); document.getElementById('steam2').onclick = ()=>filter2('steam'); document.getElementById('mobile2').onclick = ()=>filter2('mobile'); document.getElementById('genshin2').onclick = ()=>filter2('genshin'); document.getElementById('mood2').onclick = ()=>filter2('mood'); } /* 發表主題 */ function openCompose2(){ showModal2('發表主題', composeForm2(), '送出後暫存於此頁示範（無後端）'); const f = document.getElementById('compose2'); f.addEventListener('submit', e=>{ e.preventDefault(); const title = f.title.value.trim(); const board = f.board.value; const tags = f.tags.value.split(',').map(s=>s.trim()).filter(Boolean).slice(0,3); if(!title){ alert('請輸入標題'); return; } const np = {id:Date.now(), board, title, author:pick(USERS2), likes:0, replies:0, views:0, tags, minsAgo:2}; POSTS2.unshift(np); close2(); filter2('all'); }); } function composeForm2(){ const opts = BOARDS2.map(b=>`<option value="${b.key}">${b.name}</option>`).join(''); return ` <form id="compose2" class="list"> <label>標題 <input name="title" placeholder="請輸入標題" style="width:100%; padding:10px; border-radius:10px; border:1px solid var(--line); background:var(--surface)"></label> <label>分區 <select name="board" style="width:100%; padding:10px; border-radius:10px; border:1px solid var(--line); background:var(--surface)">${opts}</select></label> <label>標籤（逗號分隔，最多 3 個） <input name="tags" placeholder="#攻略, #情報" style="width:100%; padding:10px; border-radius:10px; border:1px solid var(--line); background:var(--surface)"></label> <div style="display:flex; gap:8px; justify-content:flex-end"> <button class="btn link" type="button" onclick="close2()">取消</button> <button class="btn primary" type="submit">送出</button> </div> </form>`; } /* 綜合排行（右側） */ let curMix2 = '7'; function renderMix2(){ const data = curMix2==='7'? MIX7b : MIX30b; mixList2.innerHTML = data.slice(0,10).map((g,i)=>{ const cls = i<3 ? `top top${i+1}` : ''; const sign = g.delta>0? '▲' : g.delta<0? '▼' : '–'; const dcls = g.delta>0? 'up' : g.delta<0? 'down' : 'flat'; const n = g.delta===0? 0 : Math.abs(g.delta); return `<div class="rrow ${cls}"><div class="rank">${i+1}</div><div class="title-2">${g.name}</div><div class="delta ${dcls}">${sign} ${n}</div></div>`; }).join(''); } /* 類別排行（7/30 切換；無 moba） */ let curCatsPeriod = '7'; function renderCats2(){ const data = curCatsPeriod==='7' ? CATS7 : CATS30; const grid = document.getElementById('cats2'); const entries = Object.entries(data); grid.classList.add('grid-tiles'); grid.innerHTML = entries.map(([key,list])=>{ const items = list.map((n,idx)=>{ const cls = idx<3 ? `top top${idx+1}` : ''; const sign = idx<2 ? '▲' : idx===2 ? '–' : '▼'; const dcls = idx<2 ? 'up' : idx===2 ? 'flat' : 'down'; const delta = idx<2 ? r(1,4) : idx===2 ? 0 : r(1,3); return `<div class="rrow ${cls}"><div class="rank">${idx+1}</div><div class="title-2">${n}</div><div class="delta ${dcls}">${sign} ${delta}</div></div>`; }).join(''); return `<article class="tile" style="min-height:auto"> <div class="name" style="margin-bottom:6px">${key.toUpperCase()}</div> <div class="list">${items}</div> <div style="display:flex;justify-content:flex-end;margin-top:6px"> <button class="btn" data-cat="${key}">查看詳細</button> </div> </article>`; }).join(''); grid.querySelectorAll('.btn').forEach(b=>{ b.addEventListener('click',()=>{ const key = b.dataset.cat; const title = `${key.toUpperCase()} 類別完整排行（近 ${curCatsPeriod} 天）`; const base = (curCatsPeriod==='7' ? CATS7[key] : CATS30[key]) || []; const list = base.concat(Array.from({length:10}).map((_,i)=>`${key} extra #${i+1}`)); const html = list.map((n,i)=>{ const cls = i<3 ? `top top${i+1}` : ''; const sign = i<2 ? '▲' : i===2 ? '–' : '▼'; const dcls = i<2 ? 'up' : i===2 ? 'flat' : 'down'; const delta = i<2 ? r(1,4) : i===2 ? 0 : r(1,3); return `<div class="rrow ${cls}"><div class="rank">${i+1}</div><div class="title-2">${n}</div><div class="delta ${dcls}">${sign} ${delta}</div></div>`; }).join(''); showModal2(title, html, '此為示範資料（可串接真實 API）'); }); }); } /* Modal */ function showModal2(title, inner, sub=''){ mtitle2.textContent = title; msub2.textContent = sub || ''; mlist2.innerHTML = inner; modal2.classList.add('show'); document.body.classList.add('noscroll'); } function close2(){ modal2.classList.remove('show'); document.body.classList.remove('noscroll'); } /* 主題/密度/主色 */ document.getElementById('t-dark').addEventListener('change', e=>{ document.body.classList.toggle('dark', e.target.checked); }); document.getElementById('t-density').addEventListener('change', e=>{ document.body.classList.toggle('compact', e.target.checked); }); document.querySelectorAll('.dot').forEach(d=> d.addEventListener('click', ()=>{ document.documentElement.style.setProperty('--accent', d.dataset.accent); })); /* 搜尋 */ function doSearch2(){ const kw = (document.getElementById('q2').value||'').toLowerCase(); if(!kw){ filter2('all'); return } const filtered = POSTS2.filter(p=> p.title.toLowerCase().includes(kw) || p.tags.join(',').toLowerCase().includes(kw) || p.author.toLowerCase().includes(kw)); feed2.innerHTML=''; const tmp = filtered.slice(0,40); tmp.forEach(p=>{ const el = document.createElement('article'); el.className='row'; el.innerHTML = `<div class="av">${p.author[0].toUpperCase()}</div><div><div style="font-weight:900">${p.title}</div><div class="meta">@${p.author} ｜ 分區：<strong>${BOARDS2.find(b=>b.key===p.board).name}</strong> ｜ ${ago2(p.minsAgo)} ${p.tags.map(t=>`<span class="chip">${t}</span>`).join('')}</div></div><div style="display:flex;gap:8px"><span class="ghost">❤️ ${p.likes}</span><span class="ghost">💬 ${p.replies}</span><span class="ghost">👁️ ${p.views}</span></div>`; feed2.appendChild(el); }); feedCount2.textContent = `搜尋結果：${tmp.length} 篇（顯示前 40）`; window.scrollTo({top: document.querySelector('.layout').offsetTop - 60, behavior:'smooth'}); } /* 綁定 */ document.getElementById('mix7b').onclick = ()=>{ curMix2='7'; document.getElementById('mix7b').classList.add('on'); document.getElementById('mix30b').classList.remove('on'); renderMix2(); } document.getElementById('mix30b').onclick = ()=>{ curMix2='30'; document.getElementById('mix30b').classList.add('on'); document.getElementById('mix7b').classList.remove('on'); renderMix2(); } document.getElementById('allMix2').onclick = ()=>{ const data = curMix2==='7'? MIX7b : MIX30b; const html = data.concat(Array.from({length:5}).map((_,i)=>({name:`Extra Game #${i+1}`, delta: i%2? +1 : -1}))) .map((g,i)=>{ const sign = g.delta>0? '▲' : g.delta<0? '▼' : '–'; const cls = g.delta>0? 'up' : g.delta<0? 'down' : 'flat'; const n = g.delta===0? 0 : Math.abs(g.delta); const topCls = i<3 ? `top top${i+1}` : ''; return `<div class="rrow ${topCls}"><div class="rank">${i+1}</div><div class="title-2">${g.name}</div><div class="delta ${cls}">${sign} ${n}</div></div>`; }).join(''); showModal2(`綜合遊戲熱度排行（近 ${curMix2} 天）完整前 15 名`, html, '示範視圖（可改為跳轉頁 ranks.html）'); }; document.getElementById('x2').onclick = close2; modal2.addEventListener('click', e=>{ if(e.target===modal2) close2(); }); document.getElementById('btnNew2').onclick = openCompose2; document.getElementById('fab2').onclick = openCompose2; function renderTags2(){ const hot = TAGS2.sort((a,b)=>b.c-a.c).slice(0,24); document.getElementById('hotTags2').innerHTML = hot.map(x=>`<span class="chip" style="cursor:pointer">${x.t} (${x.c})</span>`).join(''); document.querySelectorAll('#hotTags2 .chip').forEach(ch=>{ ch.addEventListener('click', ()=>{ const text = ch.textContent.split(' ')[0].replace('#',''); document.getElementById('q2').value = text; doSearch2(); }); }); } function renderAuthors2(){ document.getElementById('authors2').innerHTML = AUTHORS2.map((a,i)=>{ const topCls = i<3 ? `top top${i+1}` : ''; return `<div class="rrow ${topCls}"> <div class="rank">${i+1}</div> <div><strong class="title-2">${a.name}</strong> <div class="meta">近 24 小時發文 ${a.posts} ｜ 獲讚 ${a.likes}</div> </div> <div class="delta up">+${r(1,12)}</div> </div>`; }).join(''); } /* 啟動 */ initTiles2(); initHot2(); initTicker2(); feedCtrl2(); renderPinned(); render2(); renderMix2(); renderCats2(); renderTags2(); renderAuthors2(); /* ====== 你的「我的史萊姆」卡片 JS（原樣整合） ====== */ (function(){ const api = { me: ()=>fetch('/api/pet/me'), care: (a)=>fetch('/api/pet/care',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({action:a})}), adv: ()=>fetch('/api/pet/adventure',{method:'POST'}), balance: ()=>fetch('/api/points/balance') }; const el = (sel)=>document.querySelector(sel); const clamp=(n)=>Math.max(0,Math.min(100,n)); const bars = { hunger: [el('#bar-hunger'), el('#txt-hunger')], mood: [el('#bar-mood'), el('#txt-mood')], energy: [el('#bar-energy'), el('#txt-energy')], clean: [el('#bar-clean'), el('#txt-clean')], health: [el('#bar-health'), el('#txt-health')] }; const logBox = el('#gc-log'); function log(t,cls){ const li=document.createElement('li'); if(cls) li.className=cls; li.textContent=t; logBox.prepend(li); if(logBox.childElementCount>12) logBox.removeChild(logBox.lastChild); } function setBar(key,val){ const [i,txt] = bars[key]; const v = clamp(val|0); txt.textContent = v; i.style.width = v+'%'; i.parentElement.classList.remove('warn','bad'); if(v<40) i.parentElement.classList.add('warn'); if(v<20) i.parentElement.classList.add('bad'); } // 畫像素史萊姆 const cvs = el('#gc-pet-canvas'); const ctx = cvs.getContext('2d',{alpha:false}); ctx.imageSmoothingEnabled=false; let tick=0; function drawPet(mood=80, clean=80, sleeping=false){ const W=cvs.width, H=cvs.height; tick++; ctx.fillStyle='#79b7ff'; ctx.fillRect(0,0,W,H); ctx.fillStyle='#91e27c'; ctx.fillRect(0,H-36,W,36); const y = H-36-44 + Math.sin(tick/12)*2; ctx.fillStyle='#f6d84a'; ctx.fillRect(40,y,40,34); ctx.fillStyle='#e1c146'; ctx.fillRect(40,y+4,40,3); ctx.fillStyle='#000'; ctx.fillRect(39,y,1,34); ctx.fillRect(80,y,1,34); ctx.fillRect(39,y-1,42,1); ctx.fillRect(39,y+34,42,1); const blink = (tick%120<6) ? 1:3; drawEye(52,y+10,blink); drawEye(66,y+10,blink); if(mood>=60){ ctx.fillRect(57,y+22,8,1); ctx.fillRect(59,y+23,4,1); } else if(mood>=30){ ctx.fillRect(57,y+22,8,1); } else{ ctx.fillRect(57,y+23,8,1); ctx.fillRect(59,y+22,4,1); } if(clean<50){ ctx.fillStyle='#7f6a2a'; ctx.fillRect(46,y+18,2,2); ctx.fillRect(72,y+14,1,2); } if(sleeping){ ctx.fillStyle='#fff'; ctx.font='10px monospace'; ctx.fillText('Z',86,y+6); ctx.fillText('z',92,y+12); } function drawEye(x,y,h){ ctx.fillStyle='#fff'; ctx.fillRect(x-1,y-1,5,h+2); ctx.fillStyle='#000'; ctx.fillRect(x-2,y-2,7,1); ctx.fillRect(x-2,y+h+1,7,1); ctx.fillRect(x-2,y-2,1,h+4); ctx.fillRect(x+4,y-2,1,h+4); ctx.fillRect(x+1,y,1,h); } } // 狀態與渲染 let pet = null, coins = 0; function syncUI(){ if(!pet) return; el('#gc-pet-name').textContent = pet.name || '史萊姆'; el('#gc-pet-lv').textContent = pet.level; el('#gc-pet-xp').textContent = pet.xp; el('#gc-pet-xpmax').textContent= pet.xpMax; el('#gc-coins').textContent = coins; setBar('hunger', pet.hunger); setBar('mood', pet.mood); setBar('energy', pet.energy); setBar('clean', pet.cleanliness ?? pet.clean); setBar('health', pet.health); drawPet(pet.mood, pet.cleanliness ?? pet.clean, false); } async function refresh(){ try{ const [p,b] = await Promise.all([ api.me().then(r=>r.json()), api.balance().then(r=>r.json()) ]); pet = p; coins = b.balance ?? 0; syncUI(); }catch(e){ log('尚未接上後端 API，卡片已載入（僅顯示範例畫面）','warn'); pet = {name:'史萊姆', level:1, xp:0, xpMax:50, hunger:80, mood:80, energy:80, clean:80, cleanliness:80, health:100}; coins = 0; syncUI(); } } document.querySelectorAll('.gc-actions button[data-act]').forEach(btn=>{ btn.addEventListener('click', async ()=>{ const act = btn.getAttribute('data-act'); try{ const res = await api.care(act); const data = await res.json(); pet = data.pet || pet; coins = (data.points ?? coins); syncUI(); log(data.message || `已執行：${act}`); }catch{ log('後端尚未連線，無法執行此動作。','warn'); } }); }); document.getElementById('gc-adv').addEventListener('click', async ()=>{ try{ const res = await api.adv(); const d = await res.json(); pet = d.pet || pet; coins = (d.points ?? coins); syncUI(); log(d.win ? `戰勝 ${d.monster}：+${d.rewardPoints} 點 · XP +${d.xp}` : `挑戰 ${d.monster} 失敗，休息一下！`); }catch{ log('後端尚未連線，無法冒險。','warn'); } }); refresh(); setInterval(()=>drawPet(pet?.mood ?? 80, pet?.cleanliness ?? pet?.clean ?? 80, false), 1000/12); })(); /* 基本互動綁定 */ document.getElementById('cats7b').onclick = ()=>{ curCatsPeriod='7'; document.getElementById('cats7b').classList.add('on'); document.getElementById('cats30b').classList.remove('on'); renderCats2(); }; document.getElementById('cats30b').onclick = ()=>{ curCatsPeriod='30'; document.getElementById('cats30b').classList.add('on'); document.getElementById('cats7b').classList.remove('on'); renderCats2(); }; </script> </body> </html>補充 : 寵物 的 狀態與門檻 狀態（0–100，整數）角色為可愛的史萊姆：hunger、mood、energy、cleanliness、health 門檻：低 ≤ 20｜正常 21–79｜高 ≥ 80 互動加成：feed→hunger +10｜bath→cleanliness +10｜play→mood +10｜rest→energy +10 夾限：所有值 clamp 到 0–100 回復規則：當 hunger/mood/energy/cleanliness 全=100 → health = 100 表情／動作對應（依最低狀態優先） Hunger 低：舔嘴、腹微凹、眼神期待、視線偏向餵食 高：揉肚滿足、嘴角上揚、眼神柔和 互動動作（餵食）：頭前傾→張口→咀嚼 2–3 次→吞嚥→滿足嘆氣 Mood 低：眉眼下垂、嘴角扁、身體略縮、偶發小嘆氣 高：開心眨眼、左右小舞（八拍）、愛心/星星粒子 互動動作（玩耍）：小跑步/轉圈/拋接玩具（隨機其一）→結尾喜悅小跳 Energy 低：打哈欠、眼皮半闔、重心下墜感 高：彈性增強、站姿挺、步伐輕快 互動動作（休息）：趴下→閉眼→規律呼吸→Z 粒子→伸懶腰 Cleanliness 低：頭頂/身上灰塵污點、略嫌棄表情 高：皮膚發亮、光點高光閃爍 互動動作（洗澡）：蓮蓬頭→泡泡生成/滑落→全身抖乾（後續抖動）→亮澤提升 Health 低：臉色灰、飽和度下降、站立不穩 回滿：光暈恢復特效 + 深呼吸 + 快樂眨眼 必備動畫（被動＋互動） 被動微動作（持續生命感） 呼吸：每 2.8–3.6s 輕微起伏（Squash & Stretch） 眨眼：每 3–6s 隨機一次，偶發雙連眨 眼球追視：跟隨游標/按鈕（限舞台區） 重心微移 / 身體微晃：緩慢左右擺 Idle 隨機：抓臉頰、抖身、歪頭、摸肚、左右輕晃 互動動畫原理 Anticipation（預備動作）：互動前微縮/後仰 Squash & Stretch（擠壓/拉伸）：跳躍、抖乾、咀嚼 Follow Through（後續）：抖乾尾勁、伸懶腰回彈 Ease-in / Ease-out：所有進出場與大位移 粒子特效：餵食蒸氣/點點、洗澡泡泡/水滴、玩耍愛心/星星、休息 Z 字粒子、健康回滿光暈 音效（建議用 Howler.js） 餵食：柔和嚼嚼 / 吞嚥「glug」 洗澡：水聲 / 泡泡「pop」 玩耍：輕快「ding／dong」 休息：哈欠 / 細微吐息 -（可選）按鈕回饋：輕點「tick」＋按鈕微縮放 技術要點（HTML/CSS/JS 為主） 動畫 主要以 CSS transform + keyframes（translate/scale/rotate）與 opacity，避免觸發重排 複雜序列可用 GSAP Timeline（彈性曲線、時間控制） 粒子可用 CSS 偽元素 或 Canvas / PixiJS（需要大量粒子時） 音效：Howler.js 管理音量、疊音、行動裝置解鎖播放 狀態管理：單一 store 物件 + updateStats(type)、render()、bindUI() 夾限 0–100、+10 邏輯與回滿判定在 updateStats 冷卻 300–600ms 防疊加與爆動畫；動作進行中設旗標避免重入 效能：60fps 目標，requestAnimationFrame 管理被動微動作；只動 transform/opacity；必要時加 will-change 可近用：按鈕 aria-label、Enter/Space 觸發、觸控大熱區 以上即為開發所需的最小且完整的「狀態 × 表情 × 動畫 × 音效 × 技術」精簡規格，總之寵物要看起來很真實很像活著的，而且要非常有互動性，而且要非常可愛和活潑。 都幫我全部一口氣做完 而且代碼要有詳細中文註釋和解說 因為我是非常初學者 並且務必要幫我生成多一點readme檔讓我看得懂 並且考慮到我要部署到github 並且須要幫我產生大量非常大量的假資料(為了展示用，讓我的主管閱覽)請讓網頁越真實越好 請大量參考外部真實網站的架構來幫我製作我的專案 並且注意 不可因為任何因素而省略任何假資料或代碼或中文註釋 幫我全部一口氣做完 我沒提到的規則和邏輯和假設你自行幫我規劃並實現 並且你可以使用你想使用的任何工具來完成我的專案 務必幫我塞入大量假資料(包含假圖片和假文字)(大量的定義: 1000筆以上) (儲存於資料庫sql server) 並且sqlserver也要幫我加上詳細中文註釋(但是結構不能修改) 最後 幫我做出一份介紹整份專案運用的技術、功能、實作、流程各種一切給主管看的專案報告書(可以是word檔) 以及 內容景美豐富的簡報檔(可以是ppt) 以流程圖 以及 時程規劃 請大量參考外部真實網站的架構來幫我製作我的專案 且考慮到我要部署到github 並且注意 不可因為任何因素而省略任何假資料或代碼或中文註釋 幫我全部一口氣做完




