# Stage 6: Virtual Pet (Slime) Implementation

## Overview
Stage 6 implements a comprehensive virtual pet system featuring slime-like creatures that users can care for, interact with, and customize. The system includes pet management, care actions, achievements, items, and statistics tracking.

## Architecture

### Domain Layer
- **VirtualPet**: Core pet entity with stats, leveling, and customization
- **PetCareLog**: Tracks all care actions and their effects
- **PetAchievement**: Achievement system for milestones and accomplishments
- **PetItem**: Items for feeding, playing, cleaning, and customization

### Service Layer
- **IVirtualPetService**: Contract for virtual pet operations
- **VirtualPetService**: Implementation with business logic for pet care, leveling, and achievements

### API Layer
- **VirtualPetController**: RESTful endpoints for pet management and care actions

### Data Layer
- **GameCoreDbContext**: Updated with Stage 6 entities and relationships
- **FakeDataService**: Comprehensive fake data generation for testing

## Entity Design

### VirtualPet
- **Core Stats**: Health, Hunger, Energy, Happiness, Cleanliness
- **Leveling System**: Experience-based progression with increasing requirements
- **Customization**: Color and personality traits
- **Cooldown Management**: Prevents spam actions with time-based restrictions
- **Status Tracking**: Real-time status calculation and needs identification

### PetCareLog
- **Action Recording**: Detailed logs of all care activities
- **Effect Tracking**: Quantified changes to pet stats
- **Reward System**: Experience and points earned from actions
- **Timestamp Management**: Precise action timing for cooldowns

### PetAchievement
- **Milestone System**: Automatic unlocking based on pet progress
- **Category Organization**: Creation, Leveling, Care, Social achievements
- **Reward Integration**: Points and recognition for accomplishments

### PetItem
- **Type Classification**: Food, Toy, Cleaning, Accessory categories
- **Effect System**: Multi-dimensional stat modifications
- **Pricing**: In-game economy integration
- **Availability Control**: Active/inactive status management

## Service Layer Implementation

### Core Pet Management
- **CreatePetAsync**: One pet per user with automatic achievement creation
- **GetUserPetAsync**: Comprehensive pet status with calculated needs
- **ChangePetColorAsync**: Visual customization options

### Care Actions
- **FeedPetAsync**: Nutrition management with hunger satisfaction
- **PlayWithPetAsync**: Entertainment and happiness boosting
- **CleanPetAsync**: Hygiene maintenance and cleanliness restoration
- **RestPetAsync**: Energy recovery and health restoration

### Advanced Features
- **Cooldown Validation**: Prevents action spam with configurable intervals
- **Stat Balancing**: Ensures stats remain within valid ranges
- **Experience Calculation**: Dynamic leveling with increasing requirements
- **Achievement Checking**: Automatic milestone detection and unlocking

### Data Retrieval
- **Care History**: Paginated action logs with detailed information
- **Achievement Progress**: Current status and unlocked accomplishments
- **Item Catalog**: Available items organized by type and category
- **Statistics**: Comprehensive pet care analytics and metrics

## DTOs and Data Transfer

### Request DTOs
- **CreatePetRequestDto**: Pet creation with name, color, and personality
- **PetCareRequestDto**: Generic care action with item selection

### Response DTOs
- **VirtualPetResponseDto**: Complete pet information with calculated status
- **PetCareResponseDto**: Action results with stat changes and rewards
- **PetCareHistoryResponseDto**: Paginated care log with metadata
- **PetAchievementDto**: Achievement details and unlock status
- **PetItemDto**: Item information for selection and use
- **PetStatisticsDto**: Comprehensive care analytics and metrics

## API Endpoints

### Pet Management
- `POST /api/virtual-pet/create` - Create new pet
- `GET /api/virtual-pet/my-pet` - Get current pet status
- `PUT /api/virtual-pet/color` - Change pet color

### Care Actions
- `POST /api/virtual-pet/feed/{itemId}` - Feed pet with specific item
- `POST /api/virtual-pet/play/{itemId}` - Play with pet using toy
- `POST /api/virtual-pet/clean/{itemId}` - Clean pet with cleaning item
- `POST /api/virtual-pet/rest` - Allow pet to rest

### Information and History
- `GET /api/virtual-pet/history` - Care action history with pagination
- `GET /api/virtual-pet/achievements` - Achievement progress and status
- `GET /api/virtual-pet/items` - Available care items
- `GET /api/virtual-pet/statistics` - Pet care statistics and metrics

### Admin Functions
- `POST /api/virtual-pet/process-decay` - Process pet status decay (Admin only)

## Business Logic

### Pet Status Management
- **Health Rules**: Decreases with low cleanliness and hunger
- **Hunger System**: Gradual decrease with feeding cooldowns
- **Energy Management**: Consumed by play, restored by rest
- **Happiness Calculation**: Influenced by all care actions and conditions
- **Cleanliness Decay**: Gradual decrease requiring regular maintenance

### Leveling System
- **Experience Calculation**: Earned from all care actions
- **Level Progression**: Exponential experience requirements
- **Stat Scaling**: Maximum stats increase with level
- **Achievement Unlocking**: Automatic milestone detection

### Cooldown Management
- **Action Intervals**: Configurable cooldowns for each action type
- **User Experience**: Prevents action spam while maintaining engagement
- **Balance Control**: Ensures sustainable pet care patterns

### Achievement System
- **Automatic Detection**: Real-time milestone checking
- **Progressive Unlocking**: Sequential achievement progression
- **Reward Integration**: Points and recognition for accomplishments
- **Category Organization**: Logical grouping of achievements

## Testing

### Unit Tests
- **Service Logic**: Comprehensive coverage of all business logic
- **Edge Cases**: Error conditions and boundary scenarios
- **Data Validation**: Input validation and business rule enforcement
- **Integration Points**: Database operations and external dependencies

### Test Coverage
- **Pet Creation**: First-time creation and duplicate prevention
- **Care Actions**: Valid actions, cooldowns, and invalid scenarios
- **Status Management**: Stat calculations and boundary conditions
- **Achievement System**: Unlocking logic and progress tracking
- **Data Retrieval**: Pagination, filtering, and error handling

## Database Configuration

### Entity Relationships
- **VirtualPet** ↔ **User**: One-to-one relationship
- **VirtualPet** ↔ **PetCareLog**: One-to-many relationship
- **VirtualPet** ↔ **PetAchievement**: One-to-many relationship
- **PetCareLog** ↔ **PetItem**: Many-to-one relationship

### Indexing Strategy
- **Unique Constraints**: One pet per user
- **Performance Indexes**: User-based queries and action history
- **Composite Keys**: Efficient relationship lookups

### Data Integrity
- **Foreign Key Constraints**: Referential integrity enforcement
- **Cascade Deletion**: Automatic cleanup of related records
- **Validation Rules**: Business logic enforcement at database level

## Fake Data Generation

### Comprehensive Coverage
- **Pet Items**: Multiple categories with realistic effects and pricing
- **Virtual Pets**: Various levels and stat combinations
- **Care Logs**: Diverse action history with realistic timestamps
- **Achievements**: Progressive unlocking with milestone tracking

### Realistic Scenarios
- **Stat Variations**: Different pet conditions and needs
- **Action Patterns**: Realistic care schedules and frequencies
- **Progression Paths**: Level advancement and achievement unlocking
- **Edge Cases**: Boundary conditions and error scenarios

## Performance Considerations

### Query Optimization
- **Eager Loading**: Efficient relationship navigation
- **Pagination**: Scalable data retrieval for large datasets
- **Indexing**: Optimized database query performance
- **Caching**: Frequently accessed data caching strategies

### Scalability
- **Database Design**: Efficient schema for large user bases
- **API Design**: RESTful endpoints with proper HTTP methods
- **Error Handling**: Graceful degradation and user feedback
- **Monitoring**: Performance metrics and bottleneck identification

## Security Features

### Input Validation
- **Request Validation**: Comprehensive input sanitization
- **Business Rule Enforcement**: Action validation and cooldown checking
- **User Authorization**: Pet ownership verification
- **Admin Protection**: Restricted access to administrative functions

### Data Protection
- **User Isolation**: Pet data privacy and access control
- **Action Verification**: Legitimate care action validation
- **Rate Limiting**: Cooldown-based action prevention
- **Audit Logging**: Comprehensive action history tracking

## Future Expansion

### Planned Features
- **Pet Evolution**: Advanced growth and transformation systems
- **Social Features**: Pet interaction and community features
- **Advanced Items**: Specialized care items and accessories
- **Seasonal Events**: Time-limited activities and rewards

### Technical Enhancements
- **Real-time Updates**: WebSocket integration for live pet status
- **Mobile Optimization**: Responsive design and mobile-specific features
- **Analytics Dashboard**: Advanced pet care insights and recommendations
- **Integration APIs**: Third-party service connectivity

## Deployment and Operations

### Environment Setup
- **Database Migration**: Entity Framework Core migrations
- **Configuration Management**: Environment-specific settings
- **Dependency Management**: NuGet package version control
- **Build Process**: Automated build and deployment pipelines

### Monitoring and Maintenance
- **Health Checks**: System status and performance monitoring
- **Error Tracking**: Comprehensive logging and error reporting
- **Performance Metrics**: Response time and throughput monitoring
- **Backup Strategies**: Data protection and recovery procedures

## Usage Examples

### Creating a Pet
```csharp
var request = new CreatePetRequestDto
{
    Name = "MySlime",
    Color = "Blue",
    Personality = "Friendly"
};

var pet = await virtualPetService.CreatePetAsync(userId, request);
```

### Feeding a Pet
```csharp
var result = await virtualPetService.FeedPetAsync(userId, foodItemId);
// Returns: PetCareResponseDto with stat changes and rewards
```

### Getting Pet Status
```csharp
var pet = await virtualPetService.GetUserPetAsync(userId);
// Returns: VirtualPetResponseDto with current status and needs
```

### Viewing Care History
```csharp
var history = await virtualPetService.GetPetCareHistoryAsync(userId, page: 1, pageSize: 20);
// Returns: PetCareHistoryResponseDto with paginated results
```

## Quality Assurance

### Code Standards
- **Clean Architecture**: Separation of concerns and dependency injection
- **Error Handling**: Comprehensive exception management and user feedback
- **Documentation**: Inline code documentation and API specifications
- **Testing**: Unit tests with high coverage and realistic scenarios

### Performance Standards
- **Response Times**: Sub-second API response times
- **Database Efficiency**: Optimized queries and indexing
- **Memory Usage**: Efficient data structures and resource management
- **Scalability**: Support for large user bases and data volumes

This implementation provides a robust foundation for the virtual pet system, with comprehensive features, thorough testing, and clear documentation for future development and maintenance.