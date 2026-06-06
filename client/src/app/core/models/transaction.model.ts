export interface Transaction {
  id: string;
  merchantId: string;
  amount: number;
  currency: string;
  status: 'Pending' | 'Processing' | 'Completed' | 'Failed' | 'Refunded';
  bank: string;
  accountNumber: string;
  reference: string;
  description: string;
  customerEmail: string;
  customerName: string;
  createdAt: string;
  updatedAt: string;
  completedAt?: string;
  failureReason?: string;
  paymentMethod?: string;
  metadata?: Record<string, any>;
}

export interface TransactionAnalytics {
  totalTransactions: number;
  totalVolume: number;
  successRate: number;
  activeRefunds: number;
  dailyVolumes: DailyVolume[];
  bankDistribution: { bank: string; volume: number; count: number }[];
  recentTransactions: Transaction[];
}

export interface DailyVolume {
  date: string;
  volume: number;
  count: number;
}

export interface PagedResult<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
