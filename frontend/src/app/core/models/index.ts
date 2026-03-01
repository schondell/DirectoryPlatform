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

// Engagement
export interface ListingEngagement {
  likeCount: number;
  followerCount: number;
  hasUserLiked: boolean;
  isUserFollowing: boolean;
}

export interface LikedListing {
  listingId: string;
  title: string;
  likedAt: string;
}

export interface FollowedListing {
  listingId: string;
  title: string;
  notifyOnUpdate: boolean;
  followedAt: string;
}

export interface PageViewStats {
  totalViews: number;
  dailyViews: DailyView[];
}

export interface DailyView {
  date: string;
  viewCount: number;
}

export interface VisitorStats {
  totalVisitors: number;
  uniqueVisitors: number;
  recentVisitors: Visitor[];
}

export interface Visitor {
  userId: string;
  userName?: string;
  visitedAt: string;
}

// Boost
export interface Boost {
  id: string;
  listingId: string;
  boostType: string;
  startsAt: string;
  expiresAt: string;
  multiplier: number;
  amountPaid: number;
  currency: string;
  isActive: boolean;
}

export interface BoostPricing {
  boostType: string;
  dailyRate: number;
  multiplier: number;
  description: string;
}

export interface CreateBoost {
  listingId: string;
  boostType: string;
  durationDays: number;
}

// Bookkeeping
export interface Invoice {
  id: string;
  invoiceNumber: string;
  userId: string;
  userName?: string;
  listingId?: string;
  subscriptionId?: string;
  subtotal: number;
  taxAmount: number;
  totalAmount: number;
  currency: string;
  status: string;
  issueDate: string;
  dueDate: string;
  paidDate?: string;
  notes?: string;
  lineItems: InvoiceLineItem[];
  payments: Payment[];
}

export interface InvoiceLineItem {
  id: string;
  description: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface Payment {
  id: string;
  invoiceId: string;
  amount: number;
  currency: string;
  paymentMethod: string;
  status: string;
  transactionReference?: string;
  notes?: string;
  processedAt?: string;
  createdAt: string;
}

// Admin Dashboard
export interface AdminDashboard {
  overview: OverviewMetrics;
  recentActivity: RecentActivity;
  systemHealth: SystemHealth;
}

export interface OverviewMetrics {
  totalUsers: number;
  totalListings: number;
  activeListings: number;
  pendingApprovals: number;
  monthlyRevenue: number;
  newUsersThisMonth: number;
  newListingsThisMonth: number;
}

export interface RecentActivity {
  recentUsers: RecentUser[];
  recentListings: RecentListing[];
}

export interface RecentUser {
  id: string;
  username: string;
  email: string;
  createdAt: string;
}

export interface RecentListing {
  id: string;
  title: string;
  categoryName?: string;
  status: string;
  createdAt: string;
}

export interface SystemHealth {
  cpuUsagePercent: number;
  memoryUsageMB: number;
  memoryTotalMB: number;
  dotNetVersion: string;
  osPlatform: string;
  uptime: string;
}

// User KPI
export interface UserKpiDashboard {
  summary: KpiSummary;
  viewsOverTime: KpiTimeSeries[];
  likesOverTime: KpiTimeSeries[];
  messagesOverTime: KpiTimeSeries[];
  revenueOverTime: KpiTimeSeries[];
  categoryPerformance: CategoryPerformance[];
}

export interface KpiSummary {
  totalListings: number;
  activeListings: number;
  totalViews: number;
  totalLikes: number;
  totalFollowers: number;
  totalMessages: number;
  averageRating: number;
  responseRate: number;
  averageResponseTime: string;
  totalRevenue: number;
  viewsTrend: number;
  likesTrend: number;
  messagesTrend: number;
}

export interface KpiTimeSeries {
  date: string;
  value: number;
}

export interface CategoryPerformance {
  categoryName: string;
  listingCount: number;
  totalViews: number;
  totalLikes: number;
  averageRating: number;
}
