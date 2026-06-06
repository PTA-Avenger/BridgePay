import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { MOCK_TRANSACTIONS } from '../../core/data/mock-data';

@Component({
  selector: 'app-analytics',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="analytics-page page-enter">
      <!-- Date Filter -->
      <div class="filter-card card-glow">
        <div class="filter-row">
          <h3>Analytics Overview</h3>
          <div class="date-selectors">
            <input type="date" [(ngModel)]="startDate" (change)="loadData()" class="date-input" />
            <span class="date-sep">to</span>
            <input type="date" [(ngModel)]="endDate" (change)="loadData()" class="date-input" />
          </div>
        </div>
      </div>

      <!-- Aggregated Stats Row -->
      <div class="stats-grid">
        <div class="stat-card card-glow">
          <span class="stat-label">Average Processing Time</span>
          <h2 class="stat-value">345 ms</h2>
          <p class="stat-sub">Across all Standard Bank, FNB, and Absa calls</p>
        </div>
        <div class="stat-card card-glow">
          <span class="stat-label">Total Fee Contribution</span>
          <h2 class="stat-value">R {{ totalFees() | number:'1.2-2' }}</h2>
          <p class="stat-sub">Gateway fees retained at processing stage</p>
        </div>
        <div class="stat-card card-glow">
          <span class="stat-label">Net Settlement Amount</span>
          <h2 class="stat-value">R {{ netSettlement() | number:'1.2-2' }}</h2>
          <p class="stat-sub">Transferred to merchant bank account</p>
        </div>
      </div>

      <!-- Charts Section -->
      <div class="analytics-charts">
        <!-- Area Chart -->
        <div class="chart-large card-glow">
          <h3 class="chart-title">Daily Transaction Value (Last 7 Days)</h3>
          <div class="svg-chart-container">
            <svg viewBox="0 0 600 200" width="100%" height="200" preserveAspectRatio="none">
              <defs>
                <linearGradient id="valueGrad" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="0%" stop-color="var(--accent-violet)" stop-opacity="0.4"/>
                  <stop offset="100%" stop-color="var(--accent-violet)" stop-opacity="0"/>
                </linearGradient>
              </defs>
              <line x1="0" y1="50" x2="600" y2="50" stroke="rgba(255,255,255,0.05)" stroke-width="1"/>
              <line x1="0" y1="100" x2="600" y2="100" stroke="rgba(255,255,255,0.05)" stroke-width="1"/>
              <line x1="0" y1="150" x2="600" y2="150" stroke="rgba(255,255,255,0.05)" stroke-width="1"/>

              <path d="M 0 200 L 0 120 L 100 135 L 200 90 L 300 160 L 400 80 L 500 50 L 600 70 L 600 200 Z" fill="url(#valueGrad)"></path>
              <path d="M 0 120 L 100 135 L 200 90 L 300 160 L 400 80 L 500 50 L 600 70" fill="none" stroke="var(--accent-violet)" stroke-width="3"></path>

              <circle cx="0" cy="120" r="4" fill="var(--accent-violet)"></circle>
              <circle cx="100" cy="135" r="4" fill="var(--accent-violet)"></circle>
              <circle cx="200" cy="90" r="4" fill="var(--accent-violet)"></circle>
              <circle cx="300" cy="160" r="4" fill="var(--accent-violet)"></circle>
              <circle cx="400" cy="80" r="4" fill="var(--accent-violet)"></circle>
              <circle cx="500" cy="50" r="4" fill="var(--accent-violet)"></circle>
              <circle cx="600" cy="70" r="4" fill="var(--accent-violet)"></circle>
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

        <!-- Doughnut / Pie Chart -->
        <div class="chart-donut card-glow">
          <h3 class="chart-title">Transaction Success vs Fails</h3>
          <div class="donut-wrapper">
            <svg viewBox="0 0 100 100" class="donut-svg">
              <!-- Success segment (85%) -->
              <circle cx="50" cy="50" r="40" fill="transparent" stroke="var(--accent-emerald)" stroke-width="12" stroke-dasharray="213 251" stroke-dashoffset="0"></circle>
              <!-- Fail segment (15%) -->
              <circle cx="50" cy="50" r="40" fill="transparent" stroke="var(--accent-rose)" stroke-width="12" stroke-dasharray="38 251" stroke-dashoffset="-213"></circle>
            </svg>
            <div class="donut-center">
              <span class="donut-percentage">85%</span>
              <span class="donut-label">Success Rate</span>
            </div>
          </div>
          <div class="donut-legend">
            <div class="legend-item">
              <span class="legend-color bg-emerald"></span>
              <span class="legend-text">Completed (85%)</span>
            </div>
            <div class="legend-item">
              <span class="legend-color bg-rose"></span>
              <span class="legend-text">Failed (15%)</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .analytics-page {
      display: flex;
      flex-direction: column;
      gap: 24px;
    }
    .filter-card {
      padding: 20px;
    }
    .filter-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    .filter-row h3 { margin: 0; }
    .date-selectors {
      display: flex;
      align-items: center;
      gap: 12px;
    }
    .date-input {
      background: rgba(15, 23, 42, 0.4);
      border: 1px solid var(--border-subtle);
      border-radius: 6px;
      color: var(--text-primary);
      padding: 8px 12px;
      font-size: 13px;
      outline: none;
    }
    .date-sep {
      color: var(--text-secondary);
      font-size: 13px;
    }
    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 24px;
    }
    .stat-card {
      padding: 24px;
      display: flex;
      flex-direction: column;
      gap: 8px;
    }
    .stat-label {
      font-size: 12px;
      color: var(--text-secondary);
    }
    .stat-value {
      font-size: 28px;
      font-weight: 700;
      color: var(--text-primary);
      margin: 0;
    }
    .stat-sub {
      font-size: 11px;
      color: var(--text-tertiary);
      margin: 0;
    }
    .analytics-charts {
      display: grid;
      grid-template-columns: 3fr 2fr;
      gap: 24px;
    }
    .chart-large, .chart-donut {
      padding: 28px;
    }
    .svg-chart-container {
      height: 200px;
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
    .donut-wrapper {
      position: relative;
      width: 160px;
      height: 160px;
      margin: 24px auto;
    }
    .donut-svg {
      transform: rotate(-90deg);
      width: 100%;
      height: 100%;
    }
    .donut-center {
      position: absolute;
      top: 50%; left: 50%;
      transform: translate(-50%, -50%);
      display: flex;
      flex-direction: column;
      align-items: center;
    }
    .donut-percentage {
      font-size: 24px;
      font-weight: 700;
      color: var(--text-primary);
    }
    .donut-label {
      font-size: 10px;
      color: var(--text-secondary);
      text-transform: uppercase;
    }
    .donut-legend {
      display: flex;
      justify-content: center;
      gap: 20px;
      margin-top: 16px;
    }
    .legend-item {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 12px;
    }
    .legend-color {
      width: 10px; height: 10px;
      border-radius: 50%;
    }
    .bg-emerald { background: var(--accent-emerald); }
    .bg-rose { background: var(--accent-rose); }
    .legend-text {
      color: var(--text-secondary);
    }
    @media (max-width: 992px) {
      .analytics-charts {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class AnalyticsComponent implements OnInit {
  private apiService = inject(ApiService);

  startDate = '';
  endDate = '';

  totalFees = signal(0);
  netSettlement = signal(0);

  ngOnInit(): void {
    const end = new Date();
    const start = new Date();
    start.setDate(end.getDate() - 30);

    this.startDate = start.toISOString().substring(0, 10);
    this.endDate = end.toISOString().substring(0, 10);

    this.loadData();
  }

  loadData(): void {
    const completed = MOCK_TRANSACTIONS.filter(t => t.status === 'Completed');
    
    // Simulate simple fee calculation aggregation: 1.8% + R2.00 flat fee default
    const fees = completed.reduce((sum, t) => sum + (t.amount * 0.018 + 2.00), 0);
    this.totalFees.set(fees);

    const volume = completed.reduce((sum, t) => sum + t.amount, 0);
    this.netSettlement.set(volume - fees);
  }
}
