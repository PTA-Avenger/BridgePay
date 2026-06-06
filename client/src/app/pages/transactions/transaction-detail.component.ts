import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { MOCK_TRANSACTIONS } from '../../core/data/mock-data';
import { Transaction } from '../../core/models';

@Component({
  selector: 'app-transaction-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="detail-page page-enter">
      <!-- Header Bar -->
      <div class="detail-header-bar">
        <a routerLink="/transactions" class="back-link">
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <line x1="19" y1="12" x2="5" y2="12"></line>
            <polyline points="12 19 5 12 12 5"></polyline>
          </svg>
          Back to transactions
        </a>
      </div>

      @if (transaction(); as tx) {
        <div class="detail-grid">
          <!-- Left Column: Details -->
          <div class="detail-column">
            <div class="detail-card card-glow">
              <div class="card-header-row">
                <div>
                  <span class="detail-id-label">Transaction ID</span>
                  <h2 class="detail-id-value">{{ tx.id }}</h2>
                </div>
                <span class="badge" [class]="'badge-' + tx.status.toLowerCase()">
                  {{ tx.status }}
                </span>
              </div>

              <div class="amount-hero">
                <span class="hero-label">Amount</span>
                <h1 class="hero-val">R {{ tx.amount | number:'1.2-2' }}</h1>
              </div>

              <div class="fields-grid">
                <div class="field-item">
                  <span class="field-label">Reference</span>
                  <span class="field-value">{{ tx.reference }}</span>
                </div>
                <div class="field-item">
                  <span class="field-label">Bank Provider</span>
                  <span class="field-value">{{ tx.bank }}</span>
                </div>
                <div class="field-item">
                  <span class="field-label">Account Number</span>
                  <span class="field-value">{{ tx.accountNumber || 'N/A' }}</span>
                </div>
                <div class="field-item">
                  <span class="field-label">Payment Method</span>
                  <span class="field-value">{{ tx.paymentMethod || 'CreditCard' }}</span>
                </div>
                <div class="field-item">
                  <span class="field-label">Customer Name</span>
                  <span class="field-value">{{ tx.customerName }}</span>
                </div>
                <div class="field-item">
                  <span class="field-label">Customer Email</span>
                  <span class="field-value">{{ tx.customerEmail }}</span>
                </div>
                <div class="field-item">
                  <span class="field-label">Created At</span>
                  <span class="field-value">{{ tx.createdAt | date:'MMM d, yyyy h:mm:55 a' }}</span>
                </div>
                <div class="field-item">
                  <span class="field-label">Processed At</span>
                  <span class="field-value">{{ (tx.completedAt || tx.updatedAt) | date:'MMM d, yyyy h:mm:55 a' }}</span>
                </div>
              </div>

              @if (tx.status === 'Failed' && tx.failureReason) {
                <div class="failure-panel">
                  <span class="fail-title">Failure Reason</span>
                  <p class="fail-desc">{{ tx.failureReason }}</p>
                </div>
              }

              <!-- Refund Action -->
              @if (tx.status === 'Completed') {
                <div class="card-action-row">
                  <button class="btn btn-primary" (click)="showRefundModal.set(true)">
                    Request Refund
                  </button>
                </div>
              }
            </div>
          </div>

          <!-- Right Column: Timeline -->
          <div class="detail-column">
            <div class="timeline-card card-glow">
              <h3 class="card-title">Transaction Lifecycle</h3>

              <div class="timeline">
                <div class="timeline-item active">
                  <div class="timeline-dot"></div>
                  <div class="timeline-content">
                    <span class="timeline-title">Submitted</span>
                    <span class="timeline-time">{{ tx.createdAt | date:'MMM d, yyyy h:mm a' }}</span>
                    <p class="timeline-desc">Transaction was submitted by merchant and placed in the processing queue.</p>
                  </div>
                </div>

                <div class="timeline-item" [class.active]="tx.status !== 'Pending'">
                  <div class="timeline-dot"></div>
                  <div class="timeline-content">
                    <span class="timeline-title">Sent to Bank</span>
                    <span class="timeline-time">{{ tx.createdAt | date:'MMM d, yyyy h:mm a' }}</span>
                    <p class="timeline-desc">Routed to {{ tx.bank }} API gateway via Bridge pattern.</p>
                  </div>
                </div>

                @if (tx.status === 'Completed') {
                  <div class="timeline-item active completed">
                    <div class="timeline-dot"></div>
                    <div class="timeline-content">
                      <span class="timeline-title">Settled</span>
                      <span class="timeline-time">{{ tx.completedAt | date:'MMM d, yyyy h:mm a' }}</span>
                      <p class="timeline-desc">Successfully cleared by {{ tx.bank }}. Funds added to settlement queue.</p>
                    </div>
                  </div>
                } @else if (tx.status === 'Failed') {
                  <div class="timeline-item active failed">
                    <div class="timeline-dot"></div>
                    <div class="timeline-content">
                      <span class="timeline-title">Failed</span>
                      <span class="timeline-time">{{ tx.updatedAt | date:'MMM d, yyyy h:mm a' }}</span>
                      <p class="timeline-desc">Payment failed: {{ tx.failureReason || 'Declined by bank' }}.</p>
                    </div>
                  </div>
                } @else if (tx.status === 'Refunded') {
                  <div class="timeline-item active refunded">
                    <div class="timeline-dot"></div>
                    <div class="timeline-content">
                      <span class="timeline-title">Refunded</span>
                      <span class="timeline-time">{{ tx.updatedAt | date:'MMM d, yyyy h:mm a' }}</span>
                      <p class="timeline-desc">Transaction has been fully refunded back to the customer card.</p>
                    </div>
                  </div>
                } @else {
                  <div class="timeline-item pending">
                    <div class="timeline-dot"></div>
                    <div class="timeline-content">
                      <span class="timeline-title">Processing</span>
                      <p class="timeline-desc">Awaiting bank clearing confirmation response...</p>
                    </div>
                  </div>
                }
              </div>
            </div>
          </div>
        </div>
      } @else {
        <div class="loading-state card-glow">
          <p>Loading transaction details...</p>
        </div>
      }

      <!-- Refund Modal -->
      @if (showRefundModal()) {
        <div class="modal-backdrop">
          <div class="modal-card card-glow animate-fade">
            <h3 class="modal-title">Request Transaction Refund</h3>
            <p class="modal-subtitle">Initiate a card refund for transaction {{ transaction()?.reference }}</p>

            <form (ngSubmit)="submitRefund()" class="modal-form">
              <div class="floating-input-group">
                <input type="number" step="0.01" class="floating-input" id="amount" [(ngModel)]="refundAmount" name="amount" placeholder=" " required />
                <label for="amount" class="floating-label">Refund Amount (ZAR)</label>
              </div>

              <div class="floating-input-group">
                <input type="text" class="floating-input" id="reason" [(ngModel)]="refundReason" name="reason" placeholder=" " required />
                <label for="reason" class="floating-label">Reason for Refund</label>
              </div>

              <div class="modal-actions">
                <button type="button" class="btn btn-secondary" (click)="showRefundModal.set(false)">Cancel</button>
                <button type="submit" class="btn btn-primary" [disabled]="submittingRefund()">
                  {{ submittingRefund() ? 'Processing...' : 'Confirm Refund' }}
                </button>
              </div>
            </form>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .detail-page {
      display: flex;
      flex-direction: column;
      gap: 20px;
    }
    .back-link {
      display: inline-flex;
      align-items: center;
      gap: 8px;
      color: var(--text-secondary);
      font-size: 13px;
      text-decoration: none;
      transition: color var(--transition-base);
    }
    .back-link:hover {
      color: var(--text-primary);
    }
    .detail-grid {
      display: grid;
      grid-template-columns: 3fr 2fr;
      gap: 24px;
    }
    .detail-card, .timeline-card {
      padding: 28px;
    }
    .card-header-row {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 24px;
    }
    .detail-id-label {
      font-size: 11px;
      color: var(--text-secondary);
      text-transform: uppercase;
      letter-spacing: 0.05em;
    }
    .detail-id-value {
      font-size: 18px;
      font-weight: 700;
      color: var(--text-primary);
      margin-top: 4px;
    }
    .amount-hero {
      background: rgba(255, 255, 255, 0.02);
      border: 1px solid var(--border-subtle);
      border-radius: 8px;
      padding: 20px;
      margin-bottom: 28px;
    }
    .hero-label {
      font-size: 12px;
      color: var(--text-secondary);
    }
    .hero-val {
      font-size: 36px;
      font-weight: 800;
      color: var(--text-primary);
      margin-top: 8px;
    }
    .fields-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 20px;
    }
    .field-item {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }
    .field-label {
      font-size: 12px;
      color: var(--text-tertiary);
    }
    .field-value {
      font-size: 14px;
      color: var(--text-primary);
      font-weight: 500;
    }
    .failure-panel {
      background: rgba(244, 63, 94, 0.05);
      border: 1px solid rgba(244, 63, 94, 0.2);
      border-radius: 6px;
      padding: 16px;
      margin-top: 24px;
    }
    .fail-title {
      font-size: 12px;
      color: var(--accent-rose);
      font-weight: 600;
      display: block;
    }
    .fail-desc {
      font-size: 13px;
      color: var(--text-secondary);
      margin-top: 4px;
    }
    .card-action-row {
      margin-top: 28px;
      display: flex;
      justify-content: flex-end;
    }
    .timeline {
      display: flex;
      flex-direction: column;
      gap: 32px;
      margin-top: 24px;
      position: relative;
    }
    .timeline::before {
      content: '';
      position: absolute;
      top: 6px; bottom: 6px; left: 6px;
      width: 2px;
      background: rgba(255, 255, 255, 0.05);
    }
    .timeline-item {
      display: flex;
      gap: 20px;
      position: relative;
    }
    .timeline-dot {
      width: 14px; height: 14px;
      border-radius: 50%;
      background: var(--bg-secondary);
      border: 2px solid var(--text-muted);
      z-index: 10;
      transition: all var(--transition-base);
    }
    .timeline-item.active .timeline-dot {
      background: var(--accent-indigo);
      border-color: var(--bg-primary);
      box-shadow: 0 0 8px var(--accent-indigo);
    }
    .timeline-item.completed .timeline-dot {
      background: var(--accent-emerald);
      box-shadow: 0 0 8px var(--accent-emerald);
    }
    .timeline-item.failed .timeline-dot {
      background: var(--accent-rose);
      box-shadow: 0 0 8px var(--accent-rose);
    }
    .timeline-item.refunded .timeline-dot {
      background: var(--accent-violet);
      box-shadow: 0 0 8px var(--accent-violet);
    }
    .timeline-content {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }
    .timeline-title {
      font-size: 14px;
      font-weight: 600;
      color: var(--text-primary);
    }
    .timeline-time {
      font-size: 11px;
      color: var(--text-tertiary);
    }
    .timeline-desc {
      font-size: 12px;
      color: var(--text-secondary);
      margin: 0;
    }
    .modal-backdrop {
      position: fixed;
      top: 0; left: 0; right: 0; bottom: 0;
      background: rgba(0, 0, 0, 0.75);
      backdrop-filter: blur(8px);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 100;
    }
    .modal-card {
      width: 100%;
      max-width: 440px;
      padding: 32px;
      background: var(--bg-card);
      border: 1px solid var(--border-default);
      border-radius: 12px;
    }
    .modal-title {
      margin-top: 0;
      font-size: 20px;
    }
    .modal-subtitle {
      font-size: 13px;
      color: var(--text-secondary);
      margin-bottom: 24px;
    }
    .modal-form {
      display: flex;
      flex-direction: column;
      gap: 20px;
    }
    .modal-actions {
      display: flex;
      justify-content: flex-end;
      gap: 12px;
      margin-top: 12px;
    }
    @media (max-width: 992px) {
      .detail-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class TransactionDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private apiService = inject(ApiService);

  transaction = signal<Transaction | null>(null);
  showRefundModal = signal(false);
  refundAmount = 0;
  refundReason = '';
  submittingRefund = signal(false);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      const match = MOCK_TRANSACTIONS.find(t => t.id === id);
      if (match) {
        this.transaction.set(match);
        this.refundAmount = match.amount;
      } else {
        this.router.navigate(['/transactions']);
      }
    }
  }

  submitRefund(): void {
    const tx = this.transaction();
    if (!tx) return;

    this.submittingRefund.set(true);

    // Simulate API delay
    setTimeout(() => {
      // Update local transaction state mock
      tx.status = 'Refunded';
      tx.updatedAt = new Date().toISOString();
      this.transaction.set({ ...tx });

      this.submittingRefund.set(false);
      this.showRefundModal.set(false);
    }, 1200);
  }
}
