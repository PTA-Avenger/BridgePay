import { Component, input, output, inject, signal, OnInit, DestroyRef } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs';
import { SupabaseService } from '../core/services/supabase.service';

@Component({
  selector: 'app-header',
  standalone: true,
  template: `
    <header class="header">
      <div class="header-left">
        <button class="hamburger" (click)="toggleSidebar.emit()">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <line x1="3" y1="6" x2="21" y2="6"></line>
            <line x1="3" y1="12" x2="21" y2="12"></line>
            <line x1="3" y1="18" x2="21" y2="18"></line>
          </svg>
        </button>
        <h1 class="page-title">{{ pageTitle() }}</h1>
      </div>

      <div class="header-right">
        <div class="search-wrapper">
          <svg class="search-icon" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="11" cy="11" r="8"></circle>
            <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
          </svg>
          <input type="text" class="search-input" placeholder="Search..." />
        </div>

        <button class="notification-btn">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"></path>
            <path d="M13.73 21a2 2 0 0 1-3.46 0"></path>
          </svg>
          <span class="notification-badge">3</span>
        </button>

        <div class="header-avatar">
          <span>{{ userInitials() }}</span>
        </div>
      </div>
    </header>
  `,
  styles: [`
    .header {
      position: sticky; top: 0; z-index: 50;
      display: flex; align-items: center; justify-content: space-between;
      padding: 0 32px; height: var(--header-height);
      background: rgba(15, 23, 42, 0.8);
      backdrop-filter: blur(12px); -webkit-backdrop-filter: blur(12px);
      border-bottom: 1px solid var(--border-subtle);
    }
    .header-left { display: flex; align-items: center; gap: 16px; }
    .hamburger {
      background: transparent; color: var(--text-secondary); padding: 8px;
      border-radius: var(--radius-sm); display: none;
    }
    .page-title { font-size: 1.25rem; font-weight: 700; color: var(--text-primary); letter-spacing: -0.02em; }
    .header-right { display: flex; align-items: center; gap: 16px; }
    .search-wrapper { position: relative; display: flex; align-items: center; }
    .search-icon { position: absolute; left: 12px; color: var(--text-muted); pointer-events: none; }
    .search-input {
      padding: 8px 16px 8px 40px;
      background: rgba(30, 41, 59, 0.6); backdrop-filter: blur(8px);
      border: 1px solid var(--border-subtle); border-radius: var(--radius-full);
      color: var(--text-primary); font-size: 0.875rem; width: 240px;
      transition: all var(--transition-base);
    }
    .search-input::placeholder { color: var(--text-muted); }
    .search-input:focus {
      width: 320px; border-color: var(--accent-indigo);
      box-shadow: 0 0 0 3px rgba(99, 102, 241, 0.1);
    }
    .notification-btn {
      position: relative; background: transparent; color: var(--text-secondary);
      padding: 8px; border-radius: var(--radius-sm); transition: all var(--transition-fast);
    }
    .notification-btn:hover { background: rgba(99, 102, 241, 0.1); color: var(--text-primary); }
    .notification-badge {
      position: absolute; top: 2px; right: 2px;
      width: 18px; height: 18px; background: var(--accent-rose); color: white;
      font-size: 0.625rem; font-weight: 700; border-radius: 50%;
      display: flex; align-items: center; justify-content: center;
      box-shadow: 0 0 8px rgba(244, 63, 94, 0.5);
    }
    .header-avatar {
      width: 36px; height: 36px; border-radius: 50%; background: var(--gradient-indigo);
      display: flex; align-items: center; justify-content: center;
      font-size: 0.8rem; font-weight: 700; color: white; cursor: pointer;
      transition: all var(--transition-fast);
    }
    .header-avatar:hover { transform: scale(1.05); box-shadow: 0 0 15px rgba(99, 102, 241, 0.4); }
    @media (max-width: 768px) {
      .header { padding: 0 16px; }
      .hamburger { display: flex; }
      .search-wrapper { display: none; }
    }
  `]
})
export class HeaderComponent implements OnInit {
  private router = inject(Router);
  private supabase = inject(SupabaseService);
  private destroyRef = inject(DestroyRef);

  sidebarCollapsed = input<boolean>(false);
  toggleSidebar = output<void>();

  pageTitle = signal('Dashboard');
  userInitials = signal('MU');

  private pageTitles: Record<string, string> = {
    '/dashboard': 'Dashboard',
    '/transactions': 'Transactions',
    '/refunds': 'Refunds',
    '/analytics': 'Analytics',
    '/settings': 'Settings'
  };

  ngOnInit() {
    const sub = this.router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd)
    ).subscribe(event => {
      const path = '/' + event.urlAfterRedirects.split('/').filter(Boolean)[0];
      this.pageTitle.set(this.pageTitles[path] || 'Dashboard');
    });
    this.destroyRef.onDestroy(() => sub.unsubscribe());

    const currentPath = '/' + this.router.url.split('/').filter(Boolean)[0];
    this.pageTitle.set(this.pageTitles[currentPath] || 'Dashboard');

    this.supabase.getUser().then(user => {
      if (user) {
        const name = user.user_metadata?.['business_name'] || user.email || 'MU';
        this.userInitials.set(name.split(' ').map((n: string) => n[0]).join('').substring(0, 2).toUpperCase());
      }
    });
  }
}
