export interface Merchant {
  id: string;
  businessName: string;
  email: string;
  apiKey: string;
  webhookUrl?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  settings?: MerchantSettings;
}

export interface MerchantSettings {
  notificationsEnabled: boolean;
  autoSettlement: boolean;
  settlementSchedule: 'daily' | 'weekly' | 'monthly';
}
