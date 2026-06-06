import { Component, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SupabaseService } from '../../core/services/supabase.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  template: `
    <div class="auth-page mesh-bg">
      <div class="auth-container" [class.shake]="hasError()">
        <div class="auth-brand">
          <svg width="48" height="48" viewBox="0 0 32 32" fill="none">
            <rect width="32" height="32" rx="8" fill="url(#loginLogoGrad)"/>
            <path d="M8 16L14 10L20 16L14 22Z" fill="white" opacity="0.9"/>
            <path d="M14 16L20 10L26 16L20 22Z" fill="white" opacity="0.6"/>
            <defs>
              <linearGradient id="loginLogoGrad" x1="0" y1="0" x2="32" y2="32">
                <stop stop-color="#6366f1"/>
                <stop offset="1" stop-color="#8b5cf6"/>
              </linearGradient>
            </defs>
          </svg>
          <h1 class="gradient-text">BridgePay</h1>
          <p class="auth-subtitle">Welcome back. Sign in to your merchant dashboard.</p>
        </div>

        @if (errorMessage()) {
          <div class="error-alert">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <circle cx="12" cy="12" r="10"></circle>
              <line x1="15" y1="9" x2="9" y2="15"></line>
              <line x1="9" y1="9" x2="15" y2="15"></line>
            </svg>
            {{ errorMessage() }}
          </div>
        }

        <form (ngSubmit)="onLogin()" class="auth-form">
          <div class="floating-input-group">
            <input type="email" class="floating-input" id="email" [(ngModel)]="email" name="email" placeholder=" " required autocomplete="email" />
            <label class="floating-label" for="email">Email address</label>
          </div>
          <div class="floating-input-group">
            <input type="password" class="floating-input" id="password" [(ngModel)]="password" name="password" placeholder=" " required autocomplete="current-password" />
            <label class="floating-label" for="password">Password</label>
          </div>
          <button type="submit" class="btn btn-primary btn-lg auth-submit" [disabled]="isLoading()">
            @if (isLoading()) {
              <span class="spinner"></span>
              Signing in...
            } @else {
              Sign In
            }
          </button>
        </form>

        <p class="auth-footer">
          Don't have an account? <a routerLink="/register">Create one</a>
        </p>
      </div>

      <div class="auth-orbs">
        <div class="orb orb-1"></div>
        <div class="orb orb-2"></div>
        <div class="orb orb-3"></div>
      </div>
    </div>
  `,
  styles: [`
    .auth-page {
      min-height: 100vh; display: flex; align-items: center; justify-content: center;
      position: relative; overflow: hidden; padding: 20px;
    }
    .auth-container {
      width: 100%; max-width: 420px;
      backdrop-filter: blur(20px); -webkit-backdrop-filter: blur(20px);
      background: rgba(15, 23, 42, 0.85); border: 1px solid var(--border-default);
      border-radius: var(--radius-xl); padding: 40px;
      position: relative; z-index: 10; animation: fadeInUp 0.6s ease-out;
    }
    .auth-brand { text-align: center; margin-bottom: 32px; }
    .auth-brand svg { width: 48px; height: 48px; margin-bottom: 16px; }
    .auth-brand h1 { font-size: 2rem; font-weight: 800; margin-bottom: 8px; }
    .auth-subtitle { font-size: 0.9rem; color: var(--text-secondary); }
    .error-alert {
      display: flex; align-items: center; gap: 8px; padding: 12px 16px;
      background: rgba(244, 63, 94, 0.1); border: 1px solid rgba(244, 63, 94, 0.3);
      border-radius: var(--radius-sm); color: var(--accent-rose-light); font-size: 0.875rem;
      margin-bottom: 20px;
    }
    .auth-form { margin-bottom: 24px; }
    .auth-submit { width: 100%; padding: 14px; font-size: 1rem; margin-top: 8px; }
    .auth-footer { text-align: center; font-size: 0.875rem; color: var(--text-secondary); }
    .auth-footer a { color: var(--accent-indigo-light); font-weight: 600; }
    .auth-footer a:hover { color: var(--accent-violet-light); }
    .auth-orbs { position: absolute; inset: 0; pointer-events: none; overflow: hidden; }
    .orb { position: absolute; border-radius: 50%; filter: blur(80px); opacity: 0.3; }
    .orb-1 { width: 400px; height: 400px; background: var(--accent-indigo); top: -100px; right: -100px; animation: float 8s ease-in-out infinite; }
    .orb-2 { width: 300px; height: 300px; background: var(--accent-violet); bottom: -50px; left: -100px; animation: float 10s ease-in-out infinite reverse; }
    .orb-3 { width: 250px; height: 250px; background: var(--accent-emerald); top: 50%; left: 50%; animation: float 12s ease-in-out infinite; }
    @keyframes float {
      0%, 100% { transform: translate(0, 0) scale(1); }
      25% { transform: translate(30px, -30px) scale(1.05); }
      50% { transform: translate(-20px, 20px) scale(0.95); }
      75% { transform: translate(20px, 10px) scale(1.02); }
    }
  `]
})
export class LoginComponent {
  private router = inject(Router);
  private supabase = inject(SupabaseService);

  email = '';
  password = '';
  isLoading = signal(false);
  hasError = signal(false);
  errorMessage = signal('');

  async onLogin() {
    if (!this.email || !this.password) {
      this.showError('Please fill in all fields.');
      return;
    }
    this.isLoading.set(true);
    this.errorMessage.set('');
    try {
      const { error } = await this.supabase.signIn(this.email, this.password);
      if (error) {
        this.showError(error.message);
      } else {
        this.router.navigate(['/dashboard']);
      }
    } catch (err: any) {
      this.showError(err.message || 'An unexpected error occurred.');
    } finally {
      this.isLoading.set(false);
    }
  }

  private showError(message: string) {
    this.errorMessage.set(message);
    this.hasError.set(true);
    setTimeout(() => this.hasError.set(false), 500);
  }
}
