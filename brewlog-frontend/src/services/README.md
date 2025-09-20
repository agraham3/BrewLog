# API Services and React Query Hooks

This directory contains the API service layer and React Query hooks for the BrewLog frontend application.

## Architecture Overview

The API layer is structured in three main parts:

1. **API Client** (`api.ts`) - Low-level HTTP client with error handling and interceptors
2. **Service Layer** - High-level service functions for each entity (coffeeBeans, grindSettings, etc.)
3. **React Query Hooks** - Custom hooks that wrap services with React Query for state management

## API Client Features

### Error Handling
- Custom `ApiClientError` class with structured error information
- Automatic parsing of backend error responses
- Support for validation errors with field-specific details

### Request/Response Interceptors
- Automatic JSON content-type headers
- Centralized error handling
- Support for custom interceptors

### Query Parameter Support
```typescript
// Automatically converts object to query string
apiClient.get('/coffeebeans', { 
  name: 'Ethiopian', 
  roastLevel: 'Medium' 
});
// Results in: GET /coffeebeans?name=Ethiopian&roastLevel=Medium
```

## Service Layer

Each entity has its own service file with CRUD operations:

- `coffeeBeans.ts` - Coffee bean management
- `grindSettings.ts` - Grind setting management  
- `brewingEquipment.ts` - Equipment management
- `brewSessions.ts` - Brew session management (includes favorite toggle)
- `analytics.ts` - Analytics and dashboard data

### Example Service Usage

```typescript
import { coffeeBeanService } from '@/services';

// Get all coffee beans with filtering
const beans = await coffeeBeanService.getAll({
  roastLevel: RoastLevel.Medium,
  brand: 'Blue Bottle'
});

// Create a new coffee bean
const newBean = await coffeeBeanService.create({
  name: 'Ethiopian Yirgacheffe',
  brand: 'Blue Bottle Coffee',
  roastLevel: RoastLevel.Light,
  origin: 'Ethiopia, Yirgacheffe'
});
```

## React Query Hooks

### Query Hooks
- `useCoffeeBeans(filter?)` - Fetch coffee beans with optional filtering
- `useCoffeeBean(id)` - Fetch single coffee bean
- `useBrewSessions(filter?)` - Fetch brew sessions with filtering
- `useDashboardStats()` - Fetch dashboard analytics

### Mutation Hooks
- `useCreateCoffeeBean()` - Create new coffee bean
- `useUpdateCoffeeBean()` - Update existing coffee bean
- `useDeleteCoffeeBean()` - Delete coffee bean
- `useToggleFavoriteBrewSession()` - Toggle favorite status

### Example Hook Usage

```typescript
import { useCoffeeBeans, useCreateCoffeeBean } from '@/hooks';

function CoffeeBeansComponent() {
  // Query with automatic loading states and error handling
  const { 
    data: coffeeBeans, 
    isLoading, 
    error 
  } = useCoffeeBeans({ roastLevel: RoastLevel.Medium });

  // Mutation with automatic cache updates
  const createBeanMutation = useCreateCoffeeBean();

  const handleCreate = () => {
    createBeanMutation.mutate({
      name: 'New Bean',
      brand: 'Local Roaster',
      roastLevel: RoastLevel.Dark,
      origin: 'Colombia'
    });
  };

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <div>
      <button 
        onClick={handleCreate}
        disabled={createBeanMutation.isPending}
      >
        {createBeanMutation.isPending ? 'Creating...' : 'Create Bean'}
      </button>
      
      {coffeeBeans?.map(bean => (
        <div key={bean.id}>{bean.name}</div>
      ))}
    </div>
  );
}
```

## Cache Management

React Query hooks automatically handle:

- **Cache Invalidation** - Lists are invalidated when items are created/updated/deleted
- **Optimistic Updates** - New items are immediately added to cache
- **Background Refetching** - Data is kept fresh automatically
- **Analytics Invalidation** - Analytics queries are invalidated when brew sessions change

### Query Keys Structure

```typescript
// Coffee Beans
['coffeeBeans'] // All coffee bean queries
['coffeeBeans', 'list'] // All list queries
['coffeeBeans', 'list', filter] // Specific filtered list
['coffeeBeans', 'detail', id] // Specific coffee bean

// Similar structure for other entities
['grindSettings', 'list', filter]
['brewSessions', 'detail', id]
['analytics', 'dashboard']
```

## Error Handling

### API Client Errors
```typescript
try {
  const result = await coffeeBeanService.getAll();
} catch (error) {
  if (error instanceof ApiClientError) {
    console.log('Error code:', error.code);
    console.log('HTTP status:', error.status);
    console.log('Validation details:', error.details);
  }
}
```

### React Query Error Handling
```typescript
const { data, error } = useCoffeeBeans();

if (error) {
  // error is automatically typed as ApiClientError
  return <div>Error: {error.message}</div>;
}
```

## TypeScript Types

All services and hooks are fully typed with TypeScript interfaces that match the backend DTOs exactly:

- `CoffeeBeanResponseDto` - Full coffee bean data from API
- `CreateCoffeeBeanDto` - Data required to create a coffee bean
- `UpdateCoffeeBeanDto` - Data for updating a coffee bean
- `CoffeeBeanFilterDto` - Filtering options for coffee bean queries

## Environment Configuration

Set the API base URL in your environment:

```env
VITE_API_URL=https://localhost:7001/api
```

If not set, defaults to `https://localhost:7001/api`.

## Testing

The API client includes test utilities for mocking responses:

```typescript
import { testApiClient } from '@/services/__tests__/api.test';

// Mock a successful response
testApiClient.mockFetch({ id: 1, name: 'Test Bean' });

// Mock an error response
testApiClient.mockFetch(
  { code: 'VALIDATION_ERROR', message: 'Invalid data' }, 
  false, 
  400
);
```