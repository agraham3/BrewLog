# BrewLog Frontend

A modern React frontend for the BrewLog coffee tracking application.

## Tech Stack

- **React 19** with TypeScript
- **Vite** for fast development and building
- **Tailwind CSS** for styling
- **React Query (TanStack Query)** for server state management
- **Zustand** for client state management
- **React Hook Form** for form handling
- **ESLint & Prettier** for code quality

## Project Structure

```
src/
├── components/          # React components
│   ├── common/         # Shared components
│   ├── beans/          # Coffee bean components
│   ├── grind/          # Grind settings components
│   ├── equipment/      # Equipment components
│   ├── sessions/       # Brew session components
│   └── analytics/      # Analytics components
├── hooks/              # Custom React hooks
├── services/           # API service layer
├── stores/             # Zustand stores
├── types/              # TypeScript type definitions
└── lib/                # Configuration files
```

## Getting Started

### Prerequisites

- Node.js 18+ 
- npm or yarn

### Installation

1. Install dependencies:
   ```bash
   npm install
   ```

2. Start the development server:
   ```bash
   npm run dev
   ```

3. Open [http://localhost:5173](http://localhost:5173) in your browser

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint
- `npm run lint:fix` - Fix ESLint errors
- `npm run format` - Format code with Prettier
- `npm run format:check` - Check code formatting
- `npm run type-check` - Run TypeScript type checking

## Features

- ✅ Modern React with TypeScript
- ✅ Fast development with Vite
- ✅ Responsive design with Tailwind CSS
- ✅ Server state management with React Query
- ✅ Client state management with Zustand
- ✅ Form handling with React Hook Form
- ✅ Code quality with ESLint & Prettier
- ✅ Path aliases for clean imports
- ✅ Development tools and debugging

## API Integration

The frontend is designed to work with the BrewLog .NET API. API services are located in the `src/services/` directory and use React Query for caching and synchronization.

## Development

### Code Style

This project uses ESLint and Prettier for consistent code formatting. Run `npm run format` to format your code and `npm run lint` to check for issues.

### Path Aliases

The project is configured with path aliases for cleaner imports:

- `@/*` - src directory
- `@/components/*` - components directory
- `@/hooks/*` - hooks directory
- `@/services/*` - services directory
- `@/types/*` - types directory

### State Management

- **Server State**: React Query handles API data, caching, and synchronization
- **Client State**: Zustand manages local application state
- **Form State**: React Hook Form handles form data and validation

## Next Steps

1. Implement API service layer
2. Create reusable UI components
3. Build feature-specific components
4. Add form validation
5. Implement routing
6. Add error boundaries
7. Write tests