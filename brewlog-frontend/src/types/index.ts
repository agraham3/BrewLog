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

// Entity types
export interface CoffeeBean {
  id: number;
  name: string;
  brand: string;
  roastLevel: RoastLevel;
  origin: string;
  createdDate: string;
  modifiedDate?: string;
}

export interface GrindSetting {
  id: number;
  grindSize: number; // 1-30 scale
  grindTime: string; // TimeSpan format
  grindWeight: number; // in grams
  grinderType: string;
  notes?: string;
  createdDate: string;
}

export interface BrewingEquipment {
  id: number;
  vendor: string;
  model: string;
  type: EquipmentType;
  specifications: Record<string, string>;
  createdDate: string;
}

export interface BrewSession {
  id: number;
  method: BrewMethod;
  waterTemperature: number;
  brewTime: string; // TimeSpan format
  tastingNotes?: string;
  rating?: number; // 1-10 scale
  isFavorite: boolean;
  createdDate: string;
  coffeeBeanId: number;
  grindSettingId: number;
  brewingEquipmentId?: number;
  // Navigation properties
  coffeeBean?: CoffeeBean;
  grindSetting?: GrindSetting;
  brewingEquipment?: BrewingEquipment;
}

// DTO types for forms
export interface CreateCoffeeBeanDto {
  name: string;
  brand: string;
  roastLevel: RoastLevel;
  origin: string;
}

export type UpdateCoffeeBeanDto = Partial<CreateCoffeeBeanDto>;

export interface CreateBrewSessionDto {
  method: BrewMethod;
  waterTemperature: number;
  brewTime: string;
  tastingNotes?: string;
  rating?: number;
  coffeeBeanId: number;
  grindSettingId: number;
  brewingEquipmentId?: number;
}

export type UpdateBrewSessionDto = Partial<CreateBrewSessionDto>;

// Filter types
export interface CoffeeBeanFilter {
  search?: string;
  roastLevel?: RoastLevel;
  brand?: string;
}

export interface BrewSessionFilter {
  dateFrom?: string;
  dateTo?: string;
  method?: BrewMethod;
  coffeeBeanId?: number;
  rating?: number;
  isFavorite?: boolean;
}

// Analytics types
export interface DashboardStats {
  totalBrews: number;
  averageRating: number;
  favoriteMethod: BrewMethod;
  topRatedBean: string;
}

export interface CorrelationAnalysis {
  grindSizeVsRating: Array<{
    grindSize: number;
    averageRating: number;
    brewCount: number;
  }>;
}

export interface Recommendation {
  type: 'bean' | 'grind' | 'equipment';
  title: string;
  description: string;
  confidence: number;
}