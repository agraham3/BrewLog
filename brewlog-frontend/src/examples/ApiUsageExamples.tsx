// Example component showing how to use the API services and React Query hooks
import React from 'react';
import {
    useCoffeeBeans,
    useCreateCoffeeBean,
    useBrewSessions,
    useCreateBrewSession,
    useDashboardStats
} from '@/hooks';
import { RoastLevel, BrewMethod } from '@/types';

export const ApiUsageExamples: React.FC = () => {
    // Example 1: Fetching coffee beans with filtering
    const {
        data: coffeeBeans,
        isLoading: beansLoading,
        error: beansError
    } = useCoffeeBeans({
        roastLevel: RoastLevel.Medium
    });

    // Example 2: Creating a new coffee bean
    const createBeanMutation = useCreateCoffeeBean();

    const handleCreateBean = () => {
        createBeanMutation.mutate({
            name: 'Ethiopian Yirgacheffe',
            brand: 'Blue Bottle Coffee',
            roastLevel: RoastLevel.Light,
            origin: 'Ethiopia, Yirgacheffe'
        });
    };

    // Example 3: Fetching brew sessions with complex filtering
    const {
        data: brewSessions,
        isLoading: sessionsLoading
    } = useBrewSessions({
        method: BrewMethod.PourOver,
        minRating: 7,
        isFavorite: true,
        createdAfter: '2024-01-01T00:00:00Z'
    });

    // Example 4: Creating a new brew session
    const createSessionMutation = useCreateBrewSession();

    const handleCreateSession = () => {
        createSessionMutation.mutate({
            method: BrewMethod.PourOver,
            waterTemperature: 92.5,
            brewTime: '00:04:30', // 4 minutes 30 seconds
            tastingNotes: 'Bright acidity with notes of citrus and chocolate',
            rating: 8,
            isFavorite: false,
            coffeeBeanId: 1,
            grindSettingId: 1,
            brewingEquipmentId: 1
        });
    };

    // Example 5: Fetching analytics data
    const {
        data: dashboardStats,
        isLoading: statsLoading
    } = useDashboardStats();

    if (beansLoading || sessionsLoading || statsLoading) {
        return <div>Loading...</div>;
    }

    if (beansError) {
        return <div>Error loading coffee beans: {beansError.message}</div>;
    }

    return (
        <div className="p-6 space-y-6">
            <h1 className="text-2xl font-bold">API Usage Examples</h1>

            {/* Coffee Beans Section */}
            <section>
                <h2 className="text-xl font-semibold mb-4">Coffee Beans</h2>
                <button
                    onClick={handleCreateBean}
                    disabled={createBeanMutation.isPending}
                    className="mb-4 px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 disabled:opacity-50"
                >
                    {createBeanMutation.isPending ? 'Creating...' : 'Create New Bean'}
                </button>

                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                    {coffeeBeans?.map((bean) => (
                        <div key={bean.id} className="p-4 border rounded-lg">
                            <h3 className="font-semibold">{bean.name}</h3>
                            <p className="text-sm text-gray-600">{bean.brand}</p>
                            <p className="text-sm">Roast: {bean.roastLevel}</p>
                            <p className="text-sm">Origin: {bean.origin}</p>
                        </div>
                    ))}
                </div>
            </section>

            {/* Brew Sessions Section */}
            <section>
                <h2 className="text-xl font-semibold mb-4">Recent Brew Sessions</h2>
                <button
                    onClick={handleCreateSession}
                    disabled={createSessionMutation.isPending}
                    className="mb-4 px-4 py-2 bg-green-500 text-white rounded hover:bg-green-600 disabled:opacity-50"
                >
                    {createSessionMutation.isPending ? 'Creating...' : 'Create New Session'}
                </button>

                <div className="space-y-4">
                    {brewSessions?.slice(0, 5).map((session) => (
                        <div key={session.id} className="p-4 border rounded-lg">
                            <div className="flex justify-between items-start">
                                <div>
                                    <h3 className="font-semibold">{session.coffeeBean.name}</h3>
                                    <p className="text-sm text-gray-600">
                                        {session.method} • {session.waterTemperature}°C • {session.brewTime}
                                    </p>
                                    {session.rating && (
                                        <p className="text-sm">Rating: {session.rating}/10</p>
                                    )}
                                </div>
                                {session.isFavorite && (
                                    <span className="text-yellow-500">⭐</span>
                                )}
                            </div>
                            {session.tastingNotes && (
                                <p className="text-sm mt-2 text-gray-700">{session.tastingNotes}</p>
                            )}
                        </div>
                    ))}
                </div>
            </section>

            {/* Dashboard Stats Section */}
            <section>
                <h2 className="text-xl font-semibold mb-4">Dashboard Statistics</h2>
                {dashboardStats && (
                    <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                        <div className="p-4 bg-blue-50 rounded-lg">
                            <h3 className="font-semibold text-blue-800">Total Brews</h3>
                            <p className="text-2xl font-bold text-blue-600">
                                {dashboardStats.totalBrewSessions}
                            </p>
                        </div>
                        <div className="p-4 bg-green-50 rounded-lg">
                            <h3 className="font-semibold text-green-800">Coffee Beans</h3>
                            <p className="text-2xl font-bold text-green-600">
                                {dashboardStats.totalCoffeeBeans}
                            </p>
                        </div>
                        <div className="p-4 bg-purple-50 rounded-lg">
                            <h3 className="font-semibold text-purple-800">Favorites</h3>
                            <p className="text-2xl font-bold text-purple-600">
                                {dashboardStats.favoriteBrews}
                            </p>
                        </div>
                        <div className="p-4 bg-orange-50 rounded-lg">
                            <h3 className="font-semibold text-orange-800">Avg Rating</h3>
                            <p className="text-2xl font-bold text-orange-600">
                                {dashboardStats.averageRating.toFixed(1)}
                            </p>
                        </div>
                    </div>
                )}
            </section>

            {/* Error Handling Examples */}
            <section>
                <h2 className="text-xl font-semibold mb-4">Error Handling</h2>
                {createBeanMutation.error && (
                    <div className="p-4 bg-red-50 border border-red-200 rounded-lg">
                        <h3 className="font-semibold text-red-800">Bean Creation Error</h3>
                        <p className="text-red-600">{createBeanMutation.error.message}</p>
                    </div>
                )}

                {createSessionMutation.error && (
                    <div className="p-4 bg-red-50 border border-red-200 rounded-lg">
                        <h3 className="font-semibold text-red-800">Session Creation Error</h3>
                        <p className="text-red-600">{createSessionMutation.error.message}</p>
                    </div>
                )}
            </section>
        </div>
    );
};