export interface Refund {
  id: string;
  transactionId: string;
  merchantId: string;
  amount: number;
  currency: string;
  status: 'Pending' | 'Approved' | 'Processing' | 'Completed' | 'Rejected';
  reason: string;
  createdAt: string;
  updatedAt: string;
  completedAt?: string;
  rejectionReason?: string;
}
