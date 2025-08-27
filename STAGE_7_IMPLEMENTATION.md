# Stage 7: Mini-Game (Adventure) Implementation

## Overview
Stage 7 implements a comprehensive mini-game adventure system featuring adventure templates, monster encounters, battle mechanics, and reward systems. The system includes adventure management, progress tracking, statistics, and integration with the virtual pet system.

## Architecture

### Domain Layer
- **AdventureTemplate**: Defines available adventure types with difficulty, requirements, and rewards
- **Adventure**: User-specific adventure instances based on templates
- **AdventureLog**: Tracks adventure progress, status, and outcomes
- **MonsterEncounter**: Records monster battles with outcomes and rewards

### Service Layer
- **IAdventureService**: Contract for adventure operations
- **AdventureService**: Implementation with business logic for adventure management, monster encounters, and rewards

### API Layer
- **AdventureController**: RESTful endpoints for adventure management and gameplay

### Data Layer
- **GameCoreDbContext**: Updated with Stage 7 entities and relationships
- **FakeDataService**: Comprehensive fake data generation for testing

## Entity Design

### AdventureTemplate
- **Configuration**: Name, description, category, difficulty level
- **Requirements**: Minimum/maximum user level, energy cost, duration
- **Rewards**: Base experience, points, and gold rewards
- **Gameplay**: Success rate, maximum monster encounters
- **Management**: Active/inactive status for content control

### Adventure
- **Instance Data**: User-specific adventure based on template
- **Progress Tracking**: Current status and completion state
- **Reward Calculation**: Dynamic reward computation based on user level
- **Integration**: Links to user wallet and virtual pet systems

### AdventureLog
- **Status Management**: InProgress, Completed, Failed, Abandoned states
- **Timing**: Start, completion, and failure timestamps
- **Resource Tracking**: Energy spent, experience gained, points earned
- **Pet Effects**: Health, hunger, energy, happiness, and cleanliness changes
- **Notes**: User-provided adventure notes and failure reasons

### MonsterEncounter
- **Battle Data**: Monster stats, battle outcomes, damage calculations
- **Rewards**: Experience, points, and gold from encounters
- **Progression**: Links to adventure progress and completion
- **History**: Detailed battle notes and encounter timing

## Service Layer Implementation

### Adventure Management
- **StartAdventureAsync**: Validates requirements, deducts energy, creates adventure instance
- **GetAdventureAsync**: Retrieves current adventure status with progress information
- **CompleteAdventureAsync**: Processes successful completion with full rewards
- **FailAdventureAsync**: Handles failure scenarios with partial rewards
- **AbandonAdventureAsync**: Allows users to abandon in-progress adventures

### Adventure Templates
- **GetAvailableTemplatesAsync**: Returns templates with availability status
- **GetTemplateByIdAsync**: Retrieves specific template details
- **CanStartAdventureAsync**: Validates user eligibility for specific templates

### Adventure Progress
- **GetAdventureProgressAsync**: Real-time progress tracking and completion status
- **CalculateAdventureRewardsAsync**: Dynamic reward calculation with modifiers
- **ProcessMonsterEncounterAsync**: Monster generation and battle simulation

### Data Retrieval
- **GetUserAdventureLogsAsync**: Paginated adventure history
- **GetAdventureLogByIdAsync**: Detailed adventure log information
- **GetUserAdventureStatisticsAsync**: Comprehensive adventure analytics
- **GetAdventureMonsterEncountersAsync**: Monster encounter history

## DTOs and Data Transfer

### Request DTOs
- **StartAdventureRequestDto**: Template selection for adventure start
- **CompleteAdventureRequestDto**: Optional notes for adventure completion
- **FailAdventureRequestDto**: Failure reason documentation

### Response DTOs
- **AdventureTemplateDto**: Template information with availability status
- **AdventureResponseDto**: Complete adventure information with progress
- **AdventureProgressDto**: Real-time progress and completion status
- **AdventureLogResponseDto**: Paginated adventure history
- **MonsterEncounterResponseDto**: Battle outcomes with progress updates
- **AdventureStatisticsDto**: Comprehensive adventure analytics
- **AdventureRewardsDto**: Calculated rewards with bonus information

## API Endpoints

### Adventure Management
- `POST /api/adventure/start` - Start new adventure
- `GET /api/adventure/{adventureId}` - Get adventure status
- `POST /api/adventure/{adventureId}/complete` - Complete adventure
- `POST /api/adventure/{adventureId}/fail` - Fail adventure
- `POST /api/adventure/{adventureId}/abandon` - Abandon adventure

### Adventure Templates
- `GET /api/adventure/templates` - Get available templates
- `GET /api/adventure/templates/{templateId}` - Get template details
- `GET /api/adventure/templates/{templateId}/can-start` - Check availability
- `GET /api/adventure/templates/{templateId}/rewards` - Calculate rewards

### Adventure Progress
- `GET /api/adventure/{adventureId}/progress` - Get progress status
- `POST /api/adventure/logs/{logId}/encounter` - Process monster encounter
- `GET /api/adventure/logs/{logId}/encounters` - Get encounter history

### Adventure History
- `GET /api/adventure/logs` - Get user adventure logs
- `GET /api/adventure/logs/{logId}` - Get specific log details
- `GET /api/adventure/statistics` - Get user adventure statistics

## Business Logic

### Adventure Requirements
- **Level Validation**: User level must meet template requirements
- **Energy Management**: Sufficient energy balance required
- **Active Adventure Check**: Only one active adventure per user
- **Template Availability**: Active templates only

### Reward Calculation
- **Base Rewards**: Template-defined base values
- **Difficulty Multipliers**: Harder difficulties provide higher rewards
- **Level Bonuses**: Higher user levels increase rewards
- **Success Modifiers**: Failed adventures provide partial rewards
- **Bonus Rewards**: Special achievements and milestone bonuses

### Monster Encounter System
- **Dynamic Generation**: Monster stats based on difficulty and type
- **Battle Simulation**: Damage calculation with attack/defense mechanics
- **Outcome Determination**: Success rate influenced by difficulty
- **Reward Distribution**: Experience, points, and gold based on outcome

### Progress Tracking
- **Completion Requirements**: Monster encounters and time limits
- **Status Management**: Real-time status updates and transitions
- **Progress Calculation**: Percentage completion and remaining time
- **Action Validation**: Available actions based on current state

## Testing

### Unit Tests
- **Service Logic**: Comprehensive coverage of all business logic
- **Edge Cases**: Error conditions and boundary scenarios
- **Data Validation**: Input validation and business rule enforcement
- **Integration Points**: Database operations and external dependencies

### Test Coverage
- **Adventure Management**: Start, complete, fail, and abandon scenarios
- **Template Validation**: Level requirements and energy checks
- **Monster Encounters**: Battle simulation and reward calculation
- **Progress Tracking**: Status updates and completion logic
- **Data Retrieval**: Pagination, filtering, and error handling

## Database Configuration

### Entity Relationships
- **AdventureTemplate** ↔ **Adventure**: One-to-many relationship
- **Adventure** ↔ **AdventureLog**: One-to-many relationship
- **AdventureLog** ↔ **MonsterEncounter**: One-to-many relationship
- **User** ↔ **Adventure**: One-to-many relationship

### Indexing Strategy
- **Performance Indexes**: User-based queries and adventure progress
- **Composite Keys**: Efficient relationship lookups and status queries
- **Status Indexes**: Adventure state and completion tracking

### Data Integrity
- **Foreign Key Constraints**: Referential integrity enforcement
- **Cascade Deletion**: Automatic cleanup of related records
- **Validation Rules**: Business logic enforcement at database level

## Fake Data Generation

### Comprehensive Coverage
- **Adventure Templates**: Multiple categories and difficulty levels
- **Adventures**: User-specific adventure instances
- **Adventure Logs**: Various completion states and outcomes
- **Monster Encounters**: Diverse battle scenarios and results

### Realistic Scenarios
- **Difficulty Progression**: Easy to extreme challenge levels
- **Category Variety**: Exploration, combat, puzzle, and social adventures
- **Outcome Distribution**: Success, failure, and abandonment scenarios
- **Reward Scaling**: Level-appropriate rewards and bonuses

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
- **Business Rule Enforcement**: Adventure requirement validation
- **User Authorization**: Adventure ownership verification
- **Resource Protection**: Energy balance and level requirement checks

### Data Protection
- **User Isolation**: Adventure data privacy and access control
- **Action Verification**: Legitimate adventure action validation
- **Resource Management**: Energy consumption and reward distribution
- **Audit Logging**: Comprehensive adventure history tracking

## Integration Points

### Virtual Pet System
- **Stat Effects**: Adventure outcomes affect pet health and happiness
- **Reward Integration**: Pet care items and experience bonuses
- **Progress Tracking**: Pet level and achievement integration

### User Wallet System
- **Energy Management**: Adventure energy costs and rewards
- **Gold Rewards**: Adventure completion and monster encounter rewards
- **Balance Updates**: Real-time wallet balance modifications

### User Progression
- **Level Requirements**: Adventure template level restrictions
- **Experience Rewards**: Adventure and monster encounter experience
- **Achievement System**: Adventure completion milestones

## Future Expansion

### Planned Features
- **Advanced Combat**: Weapon systems and special abilities
- **Team Adventures**: Multi-user cooperative adventures
- **Seasonal Events**: Time-limited special adventures
- **Achievement System**: Adventure-specific accomplishments

### Technical Enhancements
- **Real-time Updates**: WebSocket integration for live progress
- **Mobile Optimization**: Touch-friendly adventure interface
- **Analytics Dashboard**: Advanced adventure insights and recommendations
- **Integration APIs**: Third-party adventure content

## Usage Examples

### Starting an Adventure
```csharp
var request = new StartAdventureRequestDto { TemplateId = 1 };
var adventure = await adventureService.StartAdventureAsync(userId, request.TemplateId);
// Returns: AdventureResponseDto with current status and progress
```

### Processing Monster Encounter
```csharp
var encounter = await adventureService.ProcessMonsterEncounterAsync(logId, userId, "Normal");
// Returns: MonsterEncounterResponseDto with battle results and progress
```

### Completing an Adventure
```csharp
var result = await adventureService.CompleteAdventureAsync(adventureId, userId);
// Returns: AdventureResponseDto with completion status and rewards
```

### Getting Adventure Statistics
```csharp
var stats = await adventureService.GetUserAdventureStatisticsAsync(userId);
// Returns: AdventureStatisticsDto with comprehensive analytics
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

This implementation provides a robust foundation for the mini-game adventure system, with comprehensive features, thorough testing, and clear documentation for future development and maintenance.