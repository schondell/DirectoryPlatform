export interface User {
  id: string;
  username: string;
  email: string;
  role: string;
  firstName?: string;
  lastName?: string;
  phone?: string;
  profileImageUrl?: string;
  isEmailVerified: boolean;
  createdAt: string;
  lastLoginAt?: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  expiration: string;
  username: string;
  email: string;
  role: string;
  userId: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  firstName?: string;
  lastName?: string;
}

export interface Category {
  id: string;
  name: string;
  slug: string;
  parentId?: string;
  iconUrl?: string;
  displayOrder: number;
  metaTitle?: string;
  metaDescription?: string;
}

export interface CategoryWithChildren extends Category {
  children: CategoryWithChildren[];
}

export interface Region {
  id: string;
  name: string;
  slug: string;
  parentId?: string;
  countryCode?: string;
  displayOrder: number;
  metaTitle?: string;
  metaDescription?: string;
}

export interface RegionWithChildren extends Region {
  children: RegionWithChildren[];
}

export interface Listing {
  id: string;
  title: string;
  shortDescription?: string;
  description?: string;
  status: string;
  categoryId: string;
  category?: Category;
  regionId?: string;
  region?: Region;
  town?: string;
  weight: number;
  isFeatured: boolean;
  isPremium: boolean;
  viewCount: number;
  expiresAt?: string;
  userId: string;
  userName?: string;
  createdAt: string;
  updatedAt?: string;
  detail?: ListingDetail;
  attributes: ListingAttribute[];
  media: ListingMedia[];
}

export interface ListingDetail {
  address?: string;
  latitude?: number;
  longitude?: number;
  phone?: string;
  email?: string;
  website?: string;
  availabilityHours?: string;
  priceInfo?: string;
  paymentMethods?: string;
}

export interface ListingAttribute {
  id: string;
  attributeDefinitionId: string;
  attributeName: string;
  attributeSlug: string;
  value: string;
  displayOrder: number;
  unit?: string;
}

export interface ListingMedia {
  id: string;
  url: string;
  mediaType: string;
  displayOrder: number;
  isPrimary: boolean;
  altText?: string;
}

export interface AttributeDefinition {
  id: string;
  name: string;
  slug: string;
  type: 'Text' | 'Number' | 'Boolean' | 'Select' | 'MultiSelect' | 'Date';
  categoryId: string;
  categoryName?: string;
  options?: string[];
  isFilterable: boolean;
  isRequired: boolean;
  displayOrder: number;
  description?: string;
  unit?: string;
  minValue?: number;
  maxValue?: number;
}

export interface ListingFilterRequest {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  categoryId?: string;
  regionId?: string;
  sortBy?: string;
  ascending?: boolean;
  attributes?: Record<string, string>;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface Review {
  id: string;
  listingId: string;
  userId: string;
  userName?: string;
  rating: number;
  comment?: string;
  status: string;
  createdAt: string;
}

export interface Message {
  id: string;
  senderId: string;
  senderName?: string;
  recipientId: string;
  recipientName?: string;
  listingId?: string;
  subject: string;
  body: string;
  isRead: boolean;
  readAt?: string;
  createdAt: string;
}

export interface Notification {
  id: string;
  type: string;
  title: string;
  message: string;
  isRead: boolean;
  actionUrl?: string;
  createdAt: string;
}

export interface Subscription {
  id: string;
  userId: string;
  subscriptionTierId: string;
  tierName?: string;
  startDate: string;
  endDate: string;
  isActive: boolean;
  autoRenew: boolean;
  createdAt: string;
}

export interface SubscriptionTier {
  id: string;
  name: string;
  description?: string;
  monthlyPrice: number;
  annualPrice?: number;
  isActive: boolean;
  features: SubscriptionFeature[];
}

export interface SubscriptionFeature {
  id: string;
  name: string;
  value?: string;
  isEnabled: boolean;
}

export interface DashboardStats {
  totalUsers: number;
  totalListings: number;
  activeListings: number;
  pendingListings: number;
  totalCategories: number;
  totalRegions: number;
  totalReviews: number;
  activeSubscriptions: number;
}
