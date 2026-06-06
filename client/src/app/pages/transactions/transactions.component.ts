import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { MOCK_TRANSACTIONS } from '../../core/data/mock-data';
import { Transaction } from '../../core/models';

@Component({
  selector: 'app-transactions',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="transactions-page page-enter">
      <!-- Filter Card -->
      <div class="filter-card card-glow">
        <div class="filter-grid">
          <div class="filter-group">
            <label class="filter-label">Search</label>
            <input type="text" [(ngModel)]="searchQuery" (input)="applyFilters()" class="filter-input" placeholder="Search by customer, reference..." />
          </div>

          <div class="filter-group">
            <label class="filter-label">Status</label>
            <select [(ngModel)]="statusFilter" (change)="applyFilters()" class="filter-input">
              <option value="">All Statuses</option>
              <option value="Pending">Pending</option>
              <option value="Processing">Processing</option>
              <option value="Completed">Completed</option>
              <option value="Failed">Failed</option>
              <option value="Refunded">Refunded</option>
            </select>
          </div>

          <div class="filter-group">
            <label class="filter-label">Bank</label>
            <select [(ngModel)]="bankFilter" (change)="applyFilters()" class="filter-input">
              <option value="">All Banks</option>
              <option value="StandardBank">Standard Bank</option>
              <option value="FNB">FNB</option>
              <option value="Absa">Absa</option>
              <option value="Nedbank">Nedbank</option>
              <option value="Capitec">Capitec</option>
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
                <th>Reference</th>
                <th>Customer</th>
                <th>Bank</th>
                <th>Amount</th>
                <th>Status</th>
                <th>Created At</th>
              </tr>
            </thead>
            <tbody>
              @if (paginatedTransactions().length === 0) {
                <tr>
                  <td colspan="6" class="no-records">No transactions match your filters.</td>
                </tr>
              } @else {
                @for (tx of paginatedTransactions(); track tx.id) {
                  <tr [routerLink]="['/transactions', tx.id]" class="clickable-row animate-row">
                    <td><span class="text-semibold">{{ tx.reference }}</span></td>
                    <td>
                      <div class="customer-info">
                        <span class="customer-name">{{ tx.customerName }}</span>
                        <span class="customer-email">{{ tx.customerEmail }}</span>
                      </div>
                    </td>
                    <td>{{ tx.bank }}</td>
                    <td>R {{ tx.amount | number:'1.2-2' }}</td>
                    <td>
                      <span class="badge" [class]="'badge-' + tx.status.toLowerCase()">
                        {{ tx.status }}
                      </span>
                    </td>
                    <td>{{ tx.createdAt | date:'MMM d, yyyy h:mm a' }}</td>
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
              Showing {{ (currentPage() - 1) * pageSize + 1 }} to {{ min(currentPage() * pageSize, filteredTransactions().length) }} of {{ filteredTransactions().length }} records
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
    .transactions-page {
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
    .customer-info {
      display: flex;
      flex-direction: column;
    }
    .customer-name {
      color: var(--text-primary);
      font-weight: 500;
    }
    .customer-email {
      color: var(--text-tertiary);
      font-size: 11px;
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
export class TransactionsComponent implements OnInit {
  private apiService = inject(ApiService);

  transactions = signal<Transaction[]>([]);
  filteredTransactions = signal<Transaction[]>([]);
  paginatedTransactions = signal<Transaction[]>([]);

  searchQuery = '';
  statusFilter = '';
  bankFilter = '';

  currentPage = signal(1);
  pageSize = 10;
  totalPages = signal(1);

  ngOnInit(): void {
    // Attempt api call, fallback to mock data
    this.transactions.set(MOCK_TRANSACTIONS);
    this.applyFilters();
  }

  applyFilters(): void {
    let list = this.transactions();

    if (this.searchQuery.trim()) {
      const q = this.searchQuery.toLowerCase();
      list = list.filter(t => 
        t.reference.toLowerCase().includes(q) ||
        t.customerName.toLowerCase().includes(q) ||
        t.customerEmail.toLowerCase().includes(q)
      );
    }

    if (this.statusFilter) {
      list = list.filter(t => t.status === this.statusFilter);
    }

    if (this.bankFilter) {
      list = list.filter(t => t.bank === this.bankFilter);
    }

    this.filteredTransactions.set(list);
    this.currentPage.set(1);
    this.updatePagination();
  }

  updatePagination(): void {
    const list = this.filteredTransactions();
    const total = list.length;
    this.totalPages.set(Math.max(1, Math.ceil(total / this.pageSize)));

    const start = (this.currentPage() - 1) * this.pageSize;
    const end = start + this.pageSize;
    this.paginatedTransactions.set(list.slice(start, end));
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
