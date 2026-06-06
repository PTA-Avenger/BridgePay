import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { MOCK_TRANSACTIONS } from '../../core/data/mock-data';
import { Transaction } from '../../core/models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="dashboard-page page-enter">
      <!-- KPI Grid -->
      <div class="kpi-grid">
        <div class="kpi-card bg-glow-indigo">
          <div class="kpi-icon">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <rect x="2" y="5" width="20" height="14" rx="2" ry="2"></rect>
              <line x1="2" y1="10" x2="22" y2="10"></line>
            </svg>
          </div>
          <div class="kpi-content">
            <span class="kpi-label">Total Transactions</span>
            <h2 class="kpi-value">{{ totalTransactions() }}</h2>
            <span class="kpi-change positive">
              <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
                <polyline points="18 15 12 9 6 15"></polyline>
              </svg>
              12% last 30d
            </span>
          </div>
        </div>

        <div class="kpi-card bg-glow-violet">
          <div class="kpi-icon">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <line x1="12" y1="1" x2="12" y2="23"></line>
              <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"></path>
            </svg>
          </div>
          <div class="kpi-content">
            <span class="kpi-label">Total Volume (ZAR)</span>
            <h2 class="kpi-value">R {{ totalVolume() | number:'1.2-2' }}</h2>
            <span class="kpi-change positive">
              <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
                <polyline points="18 15 12 9 6 15"></polyline>
              </svg>
              8.5% last 30d
            </span>
          </div>
        </div>

        <div class="kpi-card bg-glow-emerald">
          <div class="kpi-icon">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path>
              <polyline points="22 4 12 14.01 9 11.01"></polyline>
            </svg>
          </div>
          <div class="kpi-content">
            <span class="kpi-label">Success Rate</span>
            <h2 class="kpi-value">{{ successRate() }}%</h2>
            <span class="kpi-change positive">
              <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
                <polyline points="18 15 12 9 6 15"></polyline>
              </svg>
              1.2% last 30d
            </span>
          </div>
        </div>

        <div class="kpi-card bg-glow-amber">
          <div class="kpi-icon">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M21.5 2v6h-6M21.34 15.57a10 10 0 1 1-.57-8.38l5.67-5.67"></path>
            </svg>
          </div>
          <div class="kpi-content">
            <span class="kpi-label">Active Refunds</span>
            <h2 class="kpi-value">{{ activeRefunds() }}</h2>
            <span class="kpi-change neutral">Stable</span>
          </div>
        </div>
      </div>

      <!-- Charts Section -->
      <div class="dashboard-charts">
        <div class="chart-card card-glow">
          <h3 class="chart-title">Transaction Volume (Last 7 Days)</h3>
          <!-- Responsive custom SVG Area Chart -->
          <div class="svg-chart-container">
            <svg class="area-chart" viewBox="0 0 500 150" width="100%" height="150" preserveAspectRatio="none">
              <defs>
                <linearGradient id="areaGrad" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="0%" stop-color="var(--accent-indigo)" stop-opacity="0.4"/>
                  <stop offset="100%" stop-color="var(--accent-indigo)" stop-opacity="0"/>
                </linearGradient>
              </defs>
              <!-- Chart Grid Lines -->
              <line x1="0" y1="37.5" x2="500" y2="37.5" stroke="rgba(255,255,255,0.05)" stroke-width="1"/>
              <line x1="0" y1="75" x2="500" y2="75" stroke="rgba(255,255,255,0.05)" stroke-width="1"/>
              <line x1="0" y1="112.5" x2="500" y2="112.5" stroke="rgba(255,255,255,0.05)" stroke-width="1"/>

              <!-- Filled Area -->
              <path d="M 0 150 L 0 80 L 83 95 L 166 60 L 250 110 L 333 45 L 416 30 L 500 50 L 500 150 Z" fill="url(#areaGrad)"></path>
              <!-- Line Path -->
              <path d="M 0 80 L 83 95 L 166 60 L 250 110 L 333 45 L 416 30 L 500 50" fill="none" stroke="var(--accent-indigo)" stroke-width="3"></path>

              <!-- Data Dots -->
              <circle cx="0" cy="80" r="4" fill="var(--accent-indigo)"></circle>
              <circle cx="83" cy="95" r="4" fill="var(--accent-indigo)"></circle>
              <circle cx="166" cy="60" r="4" fill="var(--accent-indigo)"></circle>
              <circle cx="250" cy="110" r="4" fill="var(--accent-indigo)"></circle>
              <circle cx="333" cy="45" r="4" fill="var(--accent-indigo)"></circle>
              <circle cx="416" cy="30" r="4" fill="var(--accent-indigo)"></circle>
              <circle cx="500" cy="50" r="4" fill="var(--accent-indigo)"></circle>
            </svg>
          </div>
          <div class="chart-labels">
            <span>May 30</span>
            <span>May 31</span>
            <span>Jun 1</span>
            <span>Jun 2</span>
            <span>Jun 3</span>
            <span>Jun 4</span>
            <span>Jun 5</span>
          </div>
        </div>

        <div class="chart-card card-glow">
          <h3 class="chart-title">Volume by Bank Provider</h3>
          <div class="bank-bars">
            @for (bank of bankVolumes(); track bank.name) {
              <div class="bank-bar-row">
                <div class="bank-info">
                  <span class="bank-name">{{ bank.name }}</span>
                  <span class="bank-amount">R {{ bank.volume | number:'1.0-0' }}</span>
                </div>
                <div class="bar-outer">
                  <div class="bar-inner" [style.width.%]="bank.percentage" [style.background]="bank.color"></div>
                </div>
              </div>
            }
          </div>
        </div>
      </div>

      <!-- Recent Transactions Section -->
      <div class="dashboard-table card-glow">
        <div class="table-header">
          <h3>Recent Transactions</h3>
          <a routerLink="/transactions" class="btn btn-secondary btn-sm">View All</a>
        </div>
        <div class="table-wrapper">
          <table class="custom-table">
            <thead>
              <tr>
                <th>Reference</th>
                <th>Customer</th>
                <th>Bank</th>
                <th>Amount</th>
                <th>Status</th>
                <th>Date</th>
              </tr>
            </thead>
            <tbody>
              @for (tx of recentTransactions(); track tx.id) {
                <tr [routerLink]="['/transactions', tx.id]" class="clickable-row">
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
                  <td>{{ tx.createdAt | date:'MMM d, h:mm a' }}</td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-page {
      display: flex;
      flex-direction: column;
      gap: 32px;
    }
    .kpi-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
      gap: 24px;
    }
    .dashboard-charts {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(340px, 1fr));
      gap: 24px;
    }
    .svg-chart-container {
      height: 150px;
      margin-top: 16px;
      position: relative;
    }
    .chart-labels {
      display: flex;
      justify-content: space-between;
      margin-top: 8px;
      color: var(--text-secondary);
      font-size: 11px;
    }
    .bank-bars {
      display: flex;
      flex-direction: column;
      gap: 16px;
      margin-top: 16px;
    }
    .bank-bar-row {
      display: flex;
      flex-direction: column;
      gap: 6px;
    }
    .bank-info {
      display: flex;
      justify-content: space-between;
      font-size: 13px;
    }
    .bank-name {
      color: var(--text-primary);
      font-weight: 500;
    }
    .bank-amount {
      color: var(--text-secondary);
    }
    .bar-outer {
      background: rgba(255, 255, 255, 0.05);
      border-radius: 4px;
      height: 8px;
      overflow: hidden;
      width: 100%;
    }
    .bar-inner {
      height: 100%;
      border-radius: 4px;
      transition: width var(--transition-base);
    }
    .table-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
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
    @media (max-width: 768px) {
      .dashboard-charts {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class DashboardComponent implements OnInit {
  private apiService = inject(ApiService);

  totalTransactions = signal(0);
  totalVolume = signal(0);
  successRate = signal(0);
  activeRefunds = signal(0);
  recentTransactions = signal<Transaction[]>([]);

  bankVolumes = signal<any[]>([]);

  ngOnInit(): void {
    // Attempt to call API, fallback to mock data
    this.recentTransactions.set(MOCK_TRANSACTIONS.slice(0, 5));

    // Calculate aggregated KPI values from mock data
    const txs = MOCK_TRANSACTIONS;
    this.totalTransactions.set(txs.length);

    const completed = txs.filter(t => t.status === 'Completed');
    const volume = completed.reduce((sum, t) => sum + t.amount, 0);
    this.totalVolume.set(volume);

    const rate = txs.length > 0 ? (completed.length / txs.length) * 100 : 0;
    this.successRate.set(Math.round(rate * 10) / 10);

    const refunds = txs.filter(t => t.status === 'Refunded').length;
    this.activeRefunds.set(refunds);

    // Calculate bank distribution
    const bankMap = new Map<string, number>();
    const bankColors: Record<string, string> = {
      'StandardBank': 'var(--accent-blue)',
      'FNB': 'var(--accent-emerald)',
      'Absa': 'var(--accent-rose)',
      'Nedbank': 'var(--accent-violet)',
      'Capitec': 'var(--accent-amber)'
    };

    completed.forEach(t => {
      bankMap.set(t.bank, (bankMap.get(t.bank) ?? 0) + t.amount);
    });

    const maxVol = Math.max(...Array.from(bankMap.values()));

    const list = Array.from(bankMap.entries()).map(([name, vol]) => ({
      name,
      volume: vol,
      percentage: maxVol > 0 ? (vol / maxVol) * 100 : 0,
      color: bankColors[name] ?? 'var(--accent-indigo)'
    })).sort((a, b) => b.volume - a.volume);

    this.bankVolumes.set(list);
  }
}
