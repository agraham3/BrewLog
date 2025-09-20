import { useAppStore } from './stores/app-store';

function App() {
  const { isLoading, error, setLoading, setError, clearError } = useAppStore();

  const handleTestLoading = () => {
    setLoading(true);
    setTimeout(() => setLoading(false), 2000);
  };

  const handleTestError = () => {
    setError('This is a test error message');
  };

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-4xl mx-auto px-4">
        <header className="text-center mb-8">
          <h1 className="text-4xl font-bold text-gray-900 mb-2">
            BrewLog Frontend
          </h1>
          <p className="text-lg text-gray-600">
            Coffee tracking application built with React, TypeScript, and Tailwind CSS
          </p>
        </header>

        <div className="bg-white rounded-lg shadow-md p-6 mb-6">
          <h2 className="text-2xl font-semibold text-gray-800 mb-4">
            Setup Verification
          </h2>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
            <div className="p-4 bg-green-50 rounded-lg">
              <h3 className="font-semibold text-green-800 mb-2">✅ Configured</h3>
              <ul className="text-sm text-green-700 space-y-1">
                <li>• React with TypeScript</li>
                <li>• Vite build tool</li>
                <li>• Tailwind CSS styling</li>
                <li>• React Query for server state</li>
                <li>• Zustand for client state</li>
                <li>• ESLint & Prettier</li>
                <li>• Folder structure</li>
              </ul>
            </div>
            
            <div className="p-4 bg-blue-50 rounded-lg">
              <h3 className="font-semibold text-blue-800 mb-2">🚀 Ready for</h3>
              <ul className="text-sm text-blue-700 space-y-1">
                <li>• Component development</li>
                <li>• API integration</li>
                <li>• State management</li>
                <li>• Form handling</li>
                <li>• Responsive design</li>
              </ul>
            </div>
          </div>

          <div className="space-y-4">
            <h3 className="text-lg font-semibold text-gray-800">State Management Test</h3>
            
            <div className="flex flex-wrap gap-3">
              <button
                onClick={handleTestLoading}
                disabled={isLoading}
                className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {isLoading ? 'Loading...' : 'Test Loading State'}
              </button>
              
              <button
                onClick={handleTestError}
                className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700"
              >
                Test Error State
              </button>
              
              {error && (
                <button
                  onClick={clearError}
                  className="px-4 py-2 bg-gray-600 text-white rounded-md hover:bg-gray-700"
                >
                  Clear Error
                </button>
              )}
            </div>

            {error && (
              <div className="p-4 bg-red-50 border border-red-200 rounded-md">
                <p className="text-red-800">{error}</p>
              </div>
            )}

            {isLoading && (
              <div className="p-4 bg-blue-50 border border-blue-200 rounded-md">
                <p className="text-blue-800">Loading state is active...</p>
              </div>
            )}
          </div>
        </div>

        <div className="bg-white rounded-lg shadow-md p-6">
          <h2 className="text-2xl font-semibold text-gray-800 mb-4">
            Project Structure
          </h2>
          <div className="text-sm text-gray-600 font-mono">
            <div>src/</div>
            <div>├── components/</div>
            <div>│   ├── common/</div>
            <div>│   ├── beans/</div>
            <div>│   ├── grind/</div>
            <div>│   ├── equipment/</div>
            <div>│   ├── sessions/</div>
            <div>│   └── analytics/</div>
            <div>├── hooks/</div>
            <div>├── services/</div>
            <div>├── stores/</div>
            <div>├── types/</div>
            <div>└── lib/</div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default App;
