import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { SupabaseService } from '../../core/services/supabase.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="settings-page page-enter">
      <div class="settings-grid">
        <!-- Left Column: Profile & API keys -->
        <div class="settings-column">
          <!-- Profile Card -->
          <div class="settings-card card-glow">
            <h3 class="card-title">Merchant Profile</h3>
            <p class="card-subtitle">Manage your registered business details.</p>

            <form (ngSubmit)="updateProfile()" class="settings-form">
              <div class="floating-input-group">
                <input type="text" class="floating-input" id="businessName" [(ngModel)]="businessName" name="businessName" placeholder=" " required />
                <label for="businessName" class="floating-label">Business Name</label>
              </div>

              <div class="floating-input-group">
                <input type="email" class="floating-input" id="email" [(ngModel)]="email" name="email" placeholder=" " readonly />
                <label for="email" class="floating-label">Account Email (Read-only)</label>
              </div>

              <div class="form-actions">
                <button type="submit" class="btn btn-primary" [disabled]="updatingProfile()">
                  {{ updatingProfile() ? 'Saving...' : 'Save Changes' }}
                </button>
              </div>
            </form>
          </div>

          <!-- API Credentials Card -->
          <div class="settings-card card-glow">
            <h3 class="card-title">API Integration Credentials</h3>
            <p class="card-subtitle">Use these credentials to authenticate payments from your e-commerce system.</p>

            <div class="credentials-group">
              <div class="credential-item">
                <span class="cred-label">API Key</span>
                <div class="cred-value-row">
                  <input type="text" [value]="apiKey()" class="cred-input" readonly />
                  <button class="btn btn-secondary btn-icon-only" (click)="copyToClipboard(apiKey())" title="Copy Key">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                      <rect x="9" y="9" width="13" height="13" rx="2" ry="2"></rect>
                      <path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1"></path>
                    </svg>
                  </button>
                </div>
              </div>

              <div class="credential-item">
                <span class="cred-label">API Secret</span>
                <div class="cred-value-row">
                  <input [type]="showSecret() ? 'text' : 'password'" [value]="apiSecret()" class="cred-input" readonly />
                  <button class="btn btn-secondary btn-icon-only" (click)="showSecret.set(!showSecret())" title="Toggle Secret visibility">
                    @if (showSecret()) {
                      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"></path>
                        <line x1="1" y1="1" x2="23" y2="23"></line>
                      </svg>
                    } @else {
                      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path>
                        <circle cx="12" cy="12" r="3"></circle>
                      </svg>
                    }
                  </button>
                  <button class="btn btn-secondary btn-icon-only" (click)="copyToClipboard(apiSecret())" title="Copy Secret">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                      <rect x="9" y="9" width="13" height="13" rx="2" ry="2"></rect>
                      <path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1"></path>
                    </svg>
                  </button>
                </div>
              </div>
            </div>

            <div class="credentials-actions">
              <button class="btn btn-secondary" (click)="regenerateKeys()" [disabled]="regeneratingKeys()">
                {{ regeneratingKeys() ? 'Regenerating...' : 'Regenerate API Keys' }}
              </button>
            </div>
          </div>
        </div>

        <!-- Right Column: Danger Zone -->
        <div class="settings-column">
          <div class="settings-card card-glow border-rose">
            <h3 class="card-title text-rose">Danger Zone</h3>
            <p class="card-subtitle">Irreversible actions associated with your merchant account.</p>

            <div class="danger-box">
              <div class="danger-info">
                <span class="danger-title">Deactivate Account</span>
                <p class="danger-desc">Temporarily freeze all incoming API transactions and refund queries. Can be reactivated later.</p>
              </div>
              <button class="btn btn-danger btn-sm" (click)="deactivateAccount()" [disabled]="isDeactivated()">
                {{ isDeactivated() ? 'Deactivated' : 'Deactivate Account' }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .settings-page {
      display: flex;
      flex-direction: column;
      gap: 24px;
    }
    .settings-grid {
      display: grid;
      grid-template-columns: 3fr 2fr;
      gap: 24px;
    }
    .settings-column {
      display: flex;
      flex-direction: column;
      gap: 24px;
    }
    .settings-card {
      padding: 28px;
    }
    .border-rose {
      border-color: rgba(244, 63, 94, 0.3) !important;
    }
    .settings-form {
      display: flex;
      flex-direction: column;
      gap: 20px;
      margin-top: 16px;
    }
    .form-actions {
      display: flex;
      justify-content: flex-end;
      margin-top: 8px;
    }
    .credentials-group {
      display: flex;
      flex-direction: column;
      gap: 16px;
      margin-top: 20px;
    }
    .credential-item {
      display: flex;
      flex-direction: column;
      gap: 6px;
    }
    .cred-label {
      font-size: 12px;
      color: var(--text-secondary);
      font-weight: 500;
    }
    .cred-value-row {
      display: flex;
      gap: 8px;
    }
    .cred-input {
      flex: 1;
      background: rgba(15, 23, 42, 0.4);
      border: 1px solid var(--border-subtle);
      border-radius: 6px;
      color: var(--text-primary);
      padding: 10px 12px;
      font-size: 13px;
      font-family: monospace;
      outline: none;
    }
    .credentials-actions {
      margin-top: 24px;
      border-top: 1px solid var(--border-subtle);
      padding-top: 20px;
    }
    .danger-box {
      display: flex;
      justify-content: space-between;
      align-items: center;
      background: rgba(244, 63, 94, 0.02);
      border: 1px solid rgba(244, 63, 94, 0.15);
      border-radius: 8px;
      padding: 16px;
      margin-top: 20px;
      gap: 12px;
    }
    .danger-info {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }
    .danger-title {
      font-size: 14px;
      font-weight: 600;
      color: var(--text-primary);
    }
    .danger-desc {
      font-size: 12px;
      color: var(--text-secondary);
      margin: 0;
    }
    .text-rose {
      color: var(--accent-rose) !important;
    }
    @media (max-width: 992px) {
      .settings-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class SettingsComponent implements OnInit {
  private apiService = inject(ApiService);
  private supabaseService = inject(SupabaseService);

  businessName = 'BridgePay Merchant';
  email = 'merchant@bridgepay.co.za';
  apiKey = signal('bp_live_8F3kKzLa9Q1mD2v7eN5w0xJ9pG4s5t8y');
  apiSecret = signal('bp_sec_9H2vKzLa8Q1mD3v7eN5w0xJ8pG4s5t9y4q1r2s3t4u5v6w7x8y9z0');

  updatingProfile = signal(false);
  regeneratingKeys = signal(false);
  showSecret = signal(false);
  isDeactivated = signal(false);

  ngOnInit(): void {
    // Load current merchant profile details
    this.supabaseService.getUser().then(user => {
      if (user) {
        this.email = user.email || this.email;
        this.businessName = user.user_metadata?.['business_name'] || this.businessName;
      }
    });
  }

  updateProfile(): void {
    this.updatingProfile.set(true);
    setTimeout(() => {
      this.updatingProfile.set(false);
      alert('Merchant profile updated successfully.');
    }, 1000);
  }

  regenerateKeys(): void {
    if (confirm('Are you sure you want to regenerate your API credentials? Your existing API integrations will stop working.')) {
      this.regeneratingKeys.set(true);
      setTimeout(() => {
        // Generate new random mock keys
        const randKey = Array.from({ length: 32 }, () => Math.random().toString(36)[2]).join('');
        const randSec = Array.from({ length: 48 }, () => Math.random().toString(36)[2]).join('');
        this.apiKey.set(`bp_live_${randKey}`);
        this.apiSecret.set(`bp_sec_${randSec}`);
        this.regeneratingKeys.set(false);
        alert('API keys regenerated successfully.');
      }, 1500);
    }
  }

  copyToClipboard(text: string): void {
    navigator.clipboard.writeText(text).then(() => {
      alert('Copied to clipboard!');
    });
  }

  deactivateAccount(): void {
    if (confirm('Are you sure you want to deactivate your merchant account? This will halt all operations.')) {
      this.isDeactivated.set(true);
      alert('Merchant account has been deactivated.');
    }
  }
}
