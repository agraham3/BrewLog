# DTOs and AutoMapper Configuration

This directory contains all Data Transfer Objects (DTOs) and AutoMapper configuration for the BrewLog API.

## Structure

### DTO Files
- **CoffeeBeanDtos.cs** - DTOs for coffee bean operations (Create, Update, Response, Filter)
- **GrindSettingDtos.cs** - DTOs for grind setting operations (Create, Update, Response, Filter)
- **BrewingEquipmentDtos.cs** - DTOs for brewing equipment operations (Create, Update, Response, Filter)
- **BrewSessionDtos.cs** - DTOs for brew session operations (Create, Update, Response, Filter)
- **AnalyticsDtos.cs** - DTOs for analytics and dashboard operations

### Mapping Configuration
- **MappingProfiles.cs** - AutoMapper profiles for all entity-to-DTO mappings

## DTO Types

### Response DTOs
Used for returning data from API endpoints. Include all relevant entity properties.

### Create DTOs
Used for creating new entities. Exclude system-generated fields like Id, CreatedDate.

### Update DTOs
Used for updating existing entities. Exclude system-generated fields and set ModifiedDate automatically.

### Filter DTOs
Used for complex query operations with optional filtering parameters.

### Analytics DTOs
Specialized DTOs for dashboard statistics, correlations, recommendations, and performance metrics.

## AutoMapper Configuration

The mapping profiles handle:
- Entity to Response DTO mapping
- Create/Update DTO to Entity mapping
- Automatic timestamp management (CreatedDate, ModifiedDate)
- Navigation property exclusion for create/update operations
- Complex mappings for analytics DTOs

## Testing

Unit tests are provided in `Tests/DTOs/` to verify:
- All mapping configurations are valid
- Mappings work correctly for all DTO types
- Timestamp handling works as expected
- Complex mappings for analytics work properly

## Usage

These DTOs should be used in:
- API Controllers for request/response handling
- Service layer for business logic operations
- Repository layer for complex filtering
- Analytics services for dashboard data