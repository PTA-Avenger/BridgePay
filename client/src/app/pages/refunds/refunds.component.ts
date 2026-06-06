import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { MOCK_TRANSACTIONS } from '../../core/data/mock-data';
import { Refund } from '../../core/models';

@Component({
  selector: 'app-refunds',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="refunds-page page-enter">
      <!-- Filter Card -->
      <div class="filter-card card-glow">
        <div class="filter-grid">
          <div class="filter-group">
            <label class="filter-label">Search</label>
            <input type="text" [(ngModel)]="searchQuery" (input)="applyFilters()" class="filter-input" placeholder="Search by transaction ID..." />
          </div>

          <div class="filter-group">
            <label class="filter-label">Status</label>
            <select [(ngModel)]="statusFilter" (change)="applyFilters()" class="filter-input">
              <option value="">All Statuses</option>
              <option value="Pending">Pending</option>
              <option value="Processing">Processing</option>
              <option value="Completed">Completed</option>
              <option value="Failed">Failed</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Table Card -->
      <div class="table-card card-glow">
        <div class="table-wrapper">
          <table class="custom-table">
            <thead>
              <tr>
                <th>Refund ID</th>
                <th>Transaction ID</th>
                <th>Amount</th>
                <th>Status</th>
                <th>Reason</th>
                <th>Requested At</th>
              </tr>
            </thead>
            <tbody>
              @if (paginatedRefunds().length === 0) {
                <tr>
                  <td colspan="6" class="no-records">No refunds match your filters.</td>
                </tr>
              } @else {
                @for (rf of paginatedRefunds(); track rf.id) {
                  <tr class="animate-row">
                    <td><span class="text-semibold">{{ rf.id }}</span></td>
                    <td><a [routerLink]="['/transactions', rf.transactionId]" class="tx-link">{{ rf.transactionId }}</a></td>
                    <td>R {{ rf.amount | number:'1.2-2' }}</td>
                    <td>
                      <span class="badge" [class]="'badge-' + rf.status.toLowerCase()">
                        {{ rf.status }}
                      </span>
                    </td>
                    <td>{{ rf.reason }}</td>
                    <td>{{ rf.createdAt | date:'MMM d, yyyy h:mm a' }}</td>
                  </tr>
                }
              }
            </tbody>
          </table>
        </div>

        <!-- Pagination -->
        @if (totalPages() > 1) {
          <div class="pagination-container">
            <div class="pagination-info">
              Showing {{ (currentPage() - 1) * pageSize + 1 }} to {{ min(currentPage() * pageSize, filteredRefunds().length) }} of {{ filteredRefunds().length }} records
            </div>
            <div class="pagination-controls">
              <button class="btn btn-secondary btn-sm" [disabled]="currentPage() === 1" (click)="setPage(currentPage() - 1)">
                Previous
              </button>
              <span class="page-indicator">Page {{ currentPage() }} of {{ totalPages() }}</span>
              <button class="btn btn-secondary btn-sm" [disabled]="currentPage() === totalPages()" (click)="setPage(currentPage() + 1)">
                Next
              </button>
            </div>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .refunds-page {
      display: flex;
      flex-direction: column;
      gap: 24px;
    }
    .filter-card {
      padding: 20px;
    }
    .filter-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 16px;
    }
    .filter-group {
      display: flex;
      flex-direction: column;
      gap: 6px;
    }
    .filter-label {
      font-size: 12px;
      font-weight: 500;
      color: var(--text-secondary);
    }
    .filter-input {
      background: rgba(15, 23, 42, 0.4);
      border: 1px solid var(--border-subtle);
      border-radius: 6px;
      color: var(--text-primary);
      padding: 10px 12px;
      font-size: 13px;
      outline: none;
      transition: border-color var(--transition-base);
    }
    .filter-input:focus {
      border-color: var(--accent-indigo);
    }
    .no-records {
      text-align: center;
      padding: 48px !important;
      color: var(--text-secondary);
    }
    .tx-link {
      color: var(--accent-indigo-light);
      text-decoration: none;
      font-weight: 500;
      transition: color var(--transition-base);
    }
    .tx-link:hover {
      color: var(--accent-indigo);
      text-decoration: underline;
    }
    .pagination-container {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding-top: 20px;
      border-top: 1px solid var(--border-subtle);
      margin-top: 16px;
    }
    .pagination-info {
      font-size: 12px;
      color: var(--text-secondary);
    }
    .pagination-controls {
      display: flex;
      align-items: center;
      gap: 12px;
    }
    .page-indicator {
      font-size: 12px;
      color: var(--text-primary);
    }
  `]
})
export class RefundsComponent implements OnInit {
  private apiService = inject(ApiService);

  refunds = signal<Refund[]>([]);
  filteredRefunds = signal<Refund[]>([]);
  paginatedRefunds = signal<Refund[]>([]);

  searchQuery = '';
  statusFilter = '';

  currentPage = signal(1);
  pageSize = 10;
  totalPages = signal(1);

  ngOnInit(): void {
    // Generate mock refunds from mock transactions that are Refunded
    const refundedTxs = MOCK_TRANSACTIONS.filter(t => t.status === 'Refunded');
    const list: Refund[] = refundedTxs.map((t, idx) => ({
      id: `ref_${String(idx + 1).padStart(6, '0')}`,
      transactionId: t.id,
      merchantId: t.merchantId,
      amount: t.amount,
      currency: t.currency,
      status: 'Completed',
      reason: 'Customer requested refund.',
      createdAt: t.updatedAt,
      updatedAt: t.updatedAt
    }));

    this.refunds.set(list);
    this.applyFilters();
  }

  applyFilters(): void {
    let list = this.refunds();

    if (this.searchQuery.trim()) {
      const q = this.searchQuery.toLowerCase();
      list = list.filter(r => 
        r.transactionId.toLowerCase().includes(q) ||
        r.id.toLowerCase().includes(q)
      );
    }

    if (this.statusFilter) {
      list = list.filter(r => r.status === this.statusFilter);
    }

    this.filteredRefunds.set(list);
    this.currentPage.set(1);
    this.updatePagination();
  }

  updatePagination(): void {
    const list = this.filteredRefunds();
    const total = list.length;
    this.totalPages.set(Math.max(1, Math.ceil(total / this.pageSize)));

    const start = (this.currentPage() - 1) * this.pageSize;
    const end = start + this.pageSize;
    this.paginatedRefunds.set(list.slice(start, end));
  }

  setPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.updatePagination();
  }

  min(a: number, b: number): number {
    return Math.min(a, b);
  }
}
