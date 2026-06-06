import { Transaction, TransactionAnalytics, Refund, Merchant, DailyVolume } from '../models';

const banks = ['StandardBank', 'FNB', 'Absa', 'Nedbank', 'Capitec'];
const statuses: Transaction['status'][] = ['Pending', 'Processing', 'Completed', 'Failed', 'Refunded'];
const descriptions = ['Payment for services', 'Monthly subscription', 'Product purchase', 'Invoice payment', 'Consultation fee'];
const names = ['John Doe', 'Jane Smith', 'Bob Williams', 'Alice Brown', 'Charlie Davis', 'Diana Evans', 'Frank Garcia', 'Grace Harris'];

function seededRandom(seed: number): number {
  const x = Math.sin(seed) * 10000;
  return x - Math.floor(x);
}

function makeDateStr(daysBack: number, seed: number): string {
  const d = new Date('2026-06-05T12:00:00Z');
  d.setDate(d.getDate() - Math.floor(seededRandom(seed) * daysBack));
  d.setHours(Math.floor(seededRandom(seed + 1) * 24));
  d.setMinutes(Math.floor(seededRandom(seed + 2) * 60));
  return d.toISOString();
}

export const MOCK_TRANSACTIONS: Transaction[] = Array.from({ length: 50 }, (_, i) => ({
  id: `txn_${String(i + 1).padStart(6, '0')}`,
  merchantId: 'merchant_001',
  amount: Math.round((seededRandom(i * 7 + 3) * 50000 + 100) * 100) / 100,
  currency: 'ZAR',
  status: statuses[Math.floor(seededRandom(i * 11 + 1) * statuses.length)],
  bank: banks[Math.floor(seededRandom(i * 13 + 5) * banks.length)],
  accountNumber: '****' + String(Math.floor(seededRandom(i * 17 + 9) * 10000)).padStart(4, '0'),
  reference: `REF-${String(i + 1).padStart(6, '0')}`,
  description: descriptions[Math.floor(seededRandom(i * 19 + 7) * descriptions.length)],
  customerEmail: `customer${i + 1}@example.com`,
  customerName: names[Math.floor(seededRandom(i * 23 + 11) * names.length)],
  createdAt: makeDateStr(30, i * 31),
  updatedAt: makeDateStr(15, i * 37),
  completedAt: seededRandom(i * 41) > 0.3 ? makeDateStr(10, i * 43) : undefined,
  failureReason: undefined,
  metadata: {}
}));

export const MOCK_DAILY_VOLUMES: DailyVolume[] = Array.from({ length: 30 }, (_, i) => {
  const d = new Date('2026-06-05');
  d.setDate(d.getDate() - (29 - i));
  return {
    date: d.toISOString().split('T')[0],
    volume: Math.round(seededRandom(i * 47 + 13) * 200000 + 50000),
    count: Math.floor(seededRandom(i * 53 + 17) * 100 + 20)
  };
});

export const MOCK_ANALYTICS: TransactionAnalytics = {
  totalTransactions: 1247,
  totalVolume: 2845670.50,
  successRate: 94.7,
  activeRefunds: 12,
  dailyVolumes: MOCK_DAILY_VOLUMES,
  bankDistribution: [
    { bank: 'StandardBank', volume: 980000, count: 420 },
    { bank: 'FNB', volume: 750000, count: 310 },
    { bank: 'Absa', volume: 620000, count: 270 },
    { bank: 'Nedbank', volume: 310000, count: 150 },
    { bank: 'Capitec', volume: 185670.50, count: 97 }
  ],
  recentTransactions: MOCK_TRANSACTIONS.slice(0, 10)
};

export const MOCK_REFUNDS: Refund[] = Array.from({ length: 12 }, (_, i) => ({
  id: `ref_${String(i + 1).padStart(6, '0')}`,
  transactionId: MOCK_TRANSACTIONS[i]?.id || `txn_${String(i + 1).padStart(6, '0')}`,
  merchantId: 'merchant_001',
  amount: Math.round((seededRandom(i * 59 + 19) * 5000 + 50) * 100) / 100,
  currency: 'ZAR',
  status: (['Pending', 'Approved', 'Processing', 'Completed', 'Rejected'] as Refund['status'][])[Math.floor(seededRandom(i * 61 + 23) * 5)],
  reason: ['Customer request', 'Duplicate payment', 'Service not rendered', 'Product returned', 'Billing error'][Math.floor(seededRandom(i * 67 + 29) * 5)],
  createdAt: makeDateStr(20, i * 71),
  updatedAt: makeDateStr(10, i * 73),
  completedAt: seededRandom(i * 79) > 0.5 ? makeDateStr(5, i * 83) : undefined,
  rejectionReason: undefined
}));

export const MOCK_MERCHANT: Merchant = {
  id: 'merchant_001',
  businessName: 'Acme Payments Ltd',
  email: 'admin@acmepayments.co.za',
  apiKey: 'bp_live_sk_xxxxxxxxxxxxxxxxxxxxxxxxxxxx',
  webhookUrl: 'https://acmepayments.co.za/webhooks/bridgepay',
  isActive: true,
  createdAt: '2024-01-15T08:00:00Z',
  updatedAt: '2024-06-01T12:00:00Z',
  settings: {
    notificationsEnabled: true,
    autoSettlement: true,
    settlementSchedule: 'daily'
  }
};
