// Common types for the BrewLog application

export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

export interface ApiError {
  code: string;
  message: string;
  details?: Array<{
    field: string;
    message: string;
  }>;
}

// Enums matching backend
export enum RoastLevel {
  Light = 'Light',
  MediumLight = 'MediumLight',
  Medium = 'Medium',
  MediumDark = 'MediumDark',
  Dark = 'Dark',
}

export enum BrewMethod {
  Espresso = 'Espresso',
  FrenchPress = 'FrenchPress',
  PourOver = 'PourOver',
  Drip = 'Drip',
  AeroPress = 'AeroPress',
  ColdBrew = 'ColdBrew',
}

export enum EquipmentType {
  EspressoMachine = 'EspressoMachine',
  Grinder = 'Grinder',
  FrenchPress = 'FrenchPress',
  PourOverSetup = 'PourOverSetup',
  DripMachine = 'DripMachine',
  AeroPress = 'AeroPress',
}

// Response DTOs (matching backend exactly)
export interface CoffeeBeanResponseDto {
  id: number;
  name: string;
  brand: string;
  roastLevel: RoastLevel;
  origin: string;
  createdDate: string;
  modifiedDate?: string;
}

export interface GrindSettingResponseDto {
  id: number;
  grindSize: number; // 1-30 scale
  grindTime: string; // TimeSpan format HH:MM:SS
  grindWeight: number; // in grams
  grinderType: string;
  notes: string;
  createdDate: string;
}

export interface BrewingEquipmentResponseDto {
  id: number;
  vendor: string;
  model: string;
  type: EquipmentType;
  specifications: Record<string, string>;
  createdDate: string;
}

export interface BrewSessionResponseDto {
  id: number;
  method: BrewMethod;
  waterTemperature: number;
  brewTime: string; // TimeSpan format HH:MM:SS
  tastingNotes: string;
  rating?: number; // 1-10 scale
  isFavorite: boolean;
  createdDate: string;
  coffeeBeanId: number;
  grindSettingId: number;
  brewingEquipmentId?: number;
  // Navigation properties
  coffeeBean: CoffeeBeanResponseDto;
  grindSetting: GrindSettingResponseDto;
  brewingEquipment?: BrewingEquipmentResponseDto;
}

// Create DTOs
export interface CreateCoffeeBeanDto {
  name: string;
  brand: string;
  roastLevel: RoastLevel;
  origin: string;
}

export interface CreateGrindSettingDto {
  grindSize: number; // 1-30 scale
  grindTime: string; // TimeSpan format HH:MM:SS
  grindWeight: number; // in grams
  grinderType: string;
  notes: string;
}

export interface CreateBrewingEquipmentDto {
  vendor: string;
  model: string;
  type: EquipmentType;
  specifications: Record<string, string>;
}

export interface CreateBrewSessionDto {
  method: BrewMethod;
  waterTemperature: number;
  brewTime: string; // TimeSpan format HH:MM:SS
  tastingNotes: string;
  rating?: number; // 1-10 scale
  isFavorite: boolean;
  coffeeBeanId: number;
  grindSettingId: number;
  brewingEquipmentId?: number;
}

// Update DTOs
export interface UpdateCoffeeBeanDto {
  name: string;
  brand: string;
  roastLevel: RoastLevel;
  origin: string;
}

export interface UpdateGrindSettingDto {
  grindSize: number; // 1-30 scale
  grindTime: string; // TimeSpan format HH:MM:SS
  grindWeight: number; // in grams
  grinderType: string;
  notes: string;
}

export interface UpdateBrewingEquipmentDto {
  vendor: string;
  model: string;
  type: EquipmentType;
  specifications: Record<string, string>;
}

export interface UpdateBrewSessionDto {
  method: BrewMethod;
  waterTemperature: number;
  brewTime: string; // TimeSpan format HH:MM:SS
  tastingNotes: string;
  rating?: number; // 1-10 scale
  isFavorite: boolean;
  coffeeBeanId: number;
  grindSettingId: number;
  brewingEquipmentId?: number;
}

// Filter DTOs
export interface CoffeeBeanFilterDto {
  name?: string;
  brand?: string;
  roastLevel?: RoastLevel;
  origin?: string;
  createdAfter?: string; // ISO 8601 format
  createdBefore?: string; // ISO 8601 format
}

export interface GrindSettingFilterDto {
  minGrindSize?: number; // 1-30 scale
  maxGrindSize?: number; // 1-30 scale
  grinderType?: string;
  minGrindWeight?: number; // in grams
  maxGrindWeight?: number; // in grams
  createdAfter?: string; // ISO 8601 format
  createdBefore?: string; // ISO 8601 format
}

export interface BrewingEquipmentFilterDto {
  vendor?: string;
  model?: string;
  type?: EquipmentType;
  createdAfter?: string; // ISO 8601 format
  createdBefore?: string; // ISO 8601 format
}

export interface BrewSessionFilterDto {
  method?: BrewMethod;
  coffeeBeanId?: number;
  grindSettingId?: number;
  brewingEquipmentId?: number;
  minWaterTemperature?: number;
  maxWaterTemperature?: number;
  minRating?: number; // 1-10 scale
  maxRating?: number; // 1-10 scale
  isFavorite?: boolean;
  createdAfter?: string; // ISO 8601 format
  createdBefore?: string; // ISO 8601 format
}

// Analytics DTOs
export interface DashboardStatsDto {
  totalBrewSessions: number;
  totalCoffeeBeans: number;
  totalGrindSettings: number;
  totalEquipment: number;
  favoriteBrews: number;
  averageRating: number;
  brewMethodStats: BrewMethodStatsDto[];
  equipmentStats: EquipmentStatsDto[];
  recentBrews: RecentBrewSessionDto[];
}

export interface BrewMethodStatsDto {
  method: BrewMethod;
  count: number;
  averageRating: number;
  favoriteCount: number;
}

export interface EquipmentStatsDto {
  equipmentId: number;
  equipmentName: string;
  type: EquipmentType;
  usageCount: number;
  averageRating: number;
  favoriteCount: number;
}

export interface RecentBrewSessionDto {
  id: number;
  method: BrewMethod;
  coffeeBeanName: string;
  rating?: number;
  isFavorite: boolean;
  createdDate: string;
}

export interface CorrelationAnalysisDto {
  grindSizeCorrelations: GrindSizeRatingCorrelationDto[];
  temperatureCorrelations: TemperatureRatingCorrelationDto[];
  brewTimeCorrelations: BrewTimeRatingCorrelationDto[];
  overallCorrelationStrength: number;
}

export interface GrindSizeRatingCorrelationDto {
  grindSize: number;
  averageRating: number;
  sampleCount: number;
}

export interface TemperatureRatingCorrelationDto {
  temperatureRange: number;
  averageRating: number;
  sampleCount: number;
}

export interface BrewTimeRatingCorrelationDto {
  brewTimeRange: string; // TimeSpan format
  averageRating: number;
  sampleCount: number;
}

export interface RecommendationDto {
  type: string;
  title: string;
  description: string;
  confidenceScore: number;
  parameters: Record<string, any>;
}

export interface EquipmentPerformanceDto {
  equipmentPerformance: EquipmentPerformanceItemDto[];
  bestPerformingEquipment?: EquipmentPerformanceItemDto;
  mostUsedEquipment?: EquipmentPerformanceItemDto;
}

export interface EquipmentPerformanceItemDto {
  equipmentId: number;
  vendor: string;
  model: string;
  type: EquipmentType;
  totalUses: number;
  averageRating: number;
  favoriteCount: number;
  performanceScore: number;
}

// Legacy aliases for backward compatibility
export type CoffeeBean = CoffeeBeanResponseDto;
export type GrindSetting = GrindSettingResponseDto;
export type BrewingEquipment = BrewingEquipmentResponseDto;
export type BrewSession = BrewSessionResponseDto;