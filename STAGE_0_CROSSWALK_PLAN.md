# Stage 0 — Crosswalk Plan & Gap Analysis

## Gap Table

| Module | Spec Requirements | Current State | Missing | Files to Touch |
|--------|------------------|---------------|---------|----------------|
| **Auth/Users** | Users, User_Introduce, User_Rights, User_wallet with JWT auth | Partial: Basic User + UserWallet entities, basic auth controller | User_Introduce, User_Rights tables; OAuth providers; proper JWT integration | Domain/Entities/, Infrastructure/Data/, Api/Controllers/, frontend auth |
| **Wallet/Sales** | User_wallet, MemberSalesProfile, User_Sales_Information, points system | Basic UserWallet entity only | MemberSalesProfile, User_Sales_Information entities; points transaction system | Domain/Entities/, Infrastructure/, Api/Controllers/ |
| **Official Store** | ProductInfo, Supplier, OrderInfo, OrderItems, GameProductDetails, OtherProductDetails, Official_Store_Ranking | None | Complete B2C store system with products, orders, rankings | Domain/Entities/, Infrastructure/, Api/Controllers/, frontend store |
| **Player Market** | PlayerMarketProductInfo, PlayerMarketOrderInfo, PlayerMarketOrderTradepage, PlayerMarketTradeMsg, PlayerMarketRanking | None | Complete C2C marketplace with trading system | Domain/Entities/, Infrastructure/, Api/Controllers/, frontend market |
| **Popularity/Insights** | games, metric_sources, metrics, game_metric_daily, popularity_index_daily, leaderboard_snapshots, posts | None | Complete popularity tracking and insights system | Domain/Entities/, Infrastructure/, Api/Controllers/, frontend insights |
| **Forums/Social** | forums, threads, thread_posts, reactions, bookmarks | None | Complete forum system with interactions | Domain/Entities/, Infrastructure/, Api/Controllers/, frontend forum |
| **Notifications/Social** | Notifications, Notification_Sources, Notification_Actions, Notification_Recipients, Chat_Message, Groups, Group_Member, Group_Chat, Group_Block | None | Complete notification and social messaging system | Domain/Entities/, Infrastructure/, Api/Controllers/, frontend social |
| **Daily Sign-In** | UserSignInStats with daily rewards system | None | Complete daily sign-in with reward calculations | Domain/Entities/, Infrastructure/, Api/Controllers/, frontend signin |
| **Virtual Pet** | Pet table with stat management, health rules, interactions | None | Complete virtual pet (slime) system with animations | Domain/Entities/, Infrastructure/, Api/Controllers/, frontend pet |
| **Mini-Game** | MiniGame table with adventure system | None | Complete mini-game adventure system | Domain/Entities/, Infrastructure/, Api/Controllers/, frontend game |
| **Admin Backoffice** | ManagerData, ManagerRolePermission, ManagerRole, Admins, Mutes, Styles | None | Complete admin management system | Domain/Entities/, Infrastructure/, Api/Controllers/, admin frontend |

## Ordered Stage Plan

### Stage 1 — Auth/Users Foundation (Priority: Critical)
**Scope**: Complete user system with JWT authentication, user profiles, rights management
- Implement missing entities: User_Introduce, User_Rights  
- OAuth integration (Google, Facebook, Discord)
- JWT authentication middleware
- User profile management API
- Frontend registration/login pages
- **Tests**: Unit + Integration tests for auth flows
- **Seeds**: 500+ fake users with realistic data
- **Docs**: API documentation, setup instructions

### Stage 2 — Wallet/Sales System (Priority: High)  
**Scope**: Points system and sales registration
- Implement MemberSalesProfile, User_Sales_Information entities
- Points transaction system with audit trail
- Sales registration workflow
- Bank account verification
- **Tests**: Points transactions, sales registration
- **Seeds**: Points transactions, sales profiles
- **Docs**: Wallet API documentation

### Stage 3 — Official Store (B2C) (Priority: High)
**Scope**: Complete B2C e-commerce system
- Product catalog (games/other items)
- Shopping cart and checkout
- Order management with status workflow  
- Inventory management
- Store rankings system
- **Tests**: End-to-end purchase flows
- **Seeds**: 200+ products, orders, suppliers
- **Docs**: Store API, order workflow

### Stage 4 — Player Market (C2C) (Priority: High)
**Scope**: C2C marketplace with secure trading
- Player product listings
- Order and trading system
- Trade confirmation workflow
- Platform fee calculation
- Market rankings
- **Tests**: Complete trading flows
- **Seeds**: Player listings, trade history
- **Docs**: Trading API, safety guidelines

### Stage 5 — Popularity/Insights (Priority: Medium)
**Scope**: Game popularity tracking and insights
- Game metrics collection
- Popularity index calculation
- Leaderboards generation
- Insight post system with snapshots
- **Tests**: Metrics calculation, rankings
- **Seeds**: Game data, metrics history
- **Docs**: Metrics API, calculation formulas

### Stage 6 — Forums/Social Interactions (Priority: Medium)
**Scope**: Community forum with social features
- Forum structure (per-game boards)
- Thread and post system
- Reactions and bookmarks
- Content moderation
- **Tests**: Forum interactions, moderation
- **Seeds**: Forum posts, threads, reactions
- **Docs**: Forum API, moderation guide

### Stage 7 — Notifications/Messaging (Priority: Medium)
**Scope**: Real-time notifications and messaging
- Notification system
- Direct messaging
- Group chat functionality
- Block/unblock system
- **Tests**: Message delivery, notifications
- **Seeds**: Message history, groups
- **Docs**: Messaging API, real-time features

### Stage 8 — Daily Sign-In (Priority: Low)
**Scope**: Daily rewards system
- Sign-in tracking with streaks
- Reward calculation (points/pet XP)
- Monthly perfect attendance bonuses
- **Tests**: Reward calculations, streaks
- **Seeds**: Sign-in history
- **Docs**: Reward system documentation

### Stage 9 — Virtual Pet (Slime) (Priority: Low)
**Scope**: Interactive pet system with animations
- Pet stat management (hunger, mood, energy, cleanliness, health)
- Interactive animations and expressions
- Color customization system
- Health rules and restrictions
- **Tests**: Pet interactions, stat calculations
- **Seeds**: Pet data with various states
- **Docs**: Pet system guide, animation specs

### Stage 10 — Mini-Game Adventure (Priority: Low)
**Scope**: Adventure mini-game with rewards
- Level-based adventure system
- Monster encounters and difficulty scaling
- Reward system (XP, points)
- Daily play limits
- **Tests**: Game mechanics, reward calculations
- **Seeds**: Game history, various outcomes
- **Docs**: Game rules, balancing guide

### Stage 11 — Admin Backoffice (Priority: Medium)
**Scope**: Complete administrative interface
- Manager role and permission system
- User management and moderation
- Content management
- System monitoring and analytics
- **Tests**: Admin operations, permissions
- **Seeds**: Admin accounts, audit logs
- **Docs**: Admin guide, permission matrix

## Completion Criteria per Stage

### Quality Gate (Must pass before advancing):
1. **Build**: Green build with zero errors
2. **Tests**: All tests passing with meaningful assertions (unit + integration/API)
3. **Seeds**: Realistic fake data queryable with edge/failure cases
4. **End-to-End Flow**: At least 1 complete user journey demonstrated
5. **Documentation**: Updated README/module docs with run/test steps and examples

### Final Acceptance Evidence Required:
- **Registration/Login**: Email/username uniqueness, JWT tokens working, OAuth flows
- **Wallet**: Sign-in + mini-game + pet color change → correct points/balance updates
- **Official Store**: order→pay→ship→complete workflow + ranking updates
- **Player Market**: list→order→trade confirmation→platform fee→wallet updates
- **Forums & Reactions**: thread/reply/like/bookmark with status changes
- **Notifications**: send→inbox→mark read workflows
- **Daily Sign-In**: daily limits, streak bonuses, monthly rewards
- **Virtual Pet**: interaction stats + health rules + visual animations
- **Mini-Game**: monster encounters + win/lose + stat/point changes
- **Admin Backoffice**: role permissions + user management + audit trails

## Technical Architecture

### Backend (.NET 8.0)
- **Domain Layer**: Entity models matching exact database schema
- **Infrastructure Layer**: EF Core repositories, DbContext with all tables
- **API Layer**: RESTful controllers with proper HTTP status codes
- **Shared Layer**: DTOs, common utilities, validation
- **Testing**: xUnit with test fixtures, mocking, integration tests

### Frontend (Vue 3 + TypeScript)
- **Component-based architecture** with reusable UI components
- **State management** with Pinia stores
- **Routing** with Vue Router for SPA navigation
- **Styling** with Tailwind CSS following design system
- **TypeScript** for type safety and better DX

### Database (SQL Server)
- **Exact schema compliance** with provided specification
- **No schema modifications** allowed - use existing tables/columns only
- **Comprehensive seed data** for realistic testing and demo
- **Proper indexing** for performance optimization

### DevOps
- **Docker containers** for consistent environments
- **GitHub Actions** for CI/CD pipeline
- **Azure deployment** ready configuration
- **Automated testing** in pipeline

This plan ensures systematic delivery of all modules while maintaining quality standards and building a production-ready gaming platform.