import { Component, input, output, inject, signal, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { SupabaseService } from '../core/services/supabase.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  template: `
    <aside class="sidebar" [class.collapsed]="collapsed()">
      <div class="sidebar-header">
        <div class="logo">
          <div class="logo-icon">
            <svg width="32" height="32" viewBox="0 0 32 32" fill="none">
              <rect width="32" height="32" rx="8" fill="url(#sidebarLogoGrad)"/>
              <path d="M8 16L14 10L20 16L14 22Z" fill="white" opacity="0.9"/>
              <path d="M14 16L20 10L26 16L20 22Z" fill="white" opacity="0.6"/>
              <defs>
                <linearGradient id="sidebarLogoGrad" x1="0" y1="0" x2="32" y2="32">
                  <stop stop-color="#6366f1"/>
                  <stop offset="1" stop-color="#8b5cf6"/>
                </linearGradient>
              </defs>
            </svg>
          </div>
          @if (!collapsed()) {
            <span class="logo-text gradient-text">BridgePay</span>
          }
        </div>
      </div>

      <nav class="nav-menu">
        @for (item of navItems; track item.route) {
          <a
            class="nav-item"
            [routerLink]="item.route"
            routerLinkActive="active"
            [routerLinkActiveOptions]="{ exact: item.exact }"
          >
            <span class="nav-icon" [innerHTML]="item.icon"></span>
            @if (!collapsed()) {
              <span class="nav-label">{{ item.label }}</span>
            }
          </a>
        }
      </nav>

      <div class="sidebar-footer">
        <button class="collapse-btn" (click)="toggleCollapse.emit()">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            @if (collapsed()) {
              <polyline points="9 18 15 12 9 6"></polyline>
            } @else {
              <polyline points="15 18 9 12 15 6"></polyline>
            }
          </svg>
        </button>

        @if (!collapsed()) {
          <div class="user-info">
            <div class="user-avatar">
              <span>{{ userInitials() }}</span>
            </div>
            <div class="user-details">
              <span class="user-name">{{ userName() }}</span>
              <span class="user-role">Merchant</span>
            </div>
            <button class="logout-btn" (click)="logout()" title="Sign out">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"></path>
                <polyline points="16 17 21 12 16 7"></polyline>
                <line x1="21" y1="12" x2="9" y2="12"></line>
              </svg>
            </button>
          </div>
        }
      </div>
    </aside>
  `,
  styles: [`
    .sidebar {
      position: fixed;
      top: 0; left: 0;
      width: var(--sidebar-width);
      height: 100vh;
      background: rgba(15, 23, 42, 0.95);
      backdrop-filter: blur(20px);
      -webkit-backdrop-filter: blur(20px);
      border-right: 1px solid var(--border-subtle);
      display: flex;
      flex-direction: column;
      z-index: 100;
      transition: width var(--transition-base);
      overflow: hidden;
    }
    .sidebar.collapsed { width: var(--sidebar-collapsed); }
    .sidebar-header { padding: 20px 16px; border-bottom: 1px solid var(--border-subtle); }
    .logo { display: flex; align-items: center; gap: 12px; }
    .logo-icon { flex-shrink: 0; display: flex; align-items: center; justify-content: center; width: 40px; height: 40px; }
    .logo-text { font-size: 1.25rem; font-weight: 800; letter-spacing: -0.02em; white-space: nowrap; }
    .nav-menu { flex: 1; padding: 16px 12px; display: flex; flex-direction: column; gap: 4px; overflow-y: auto; }
    .nav-item {
      display: flex; align-items: center; gap: 12px;
      padding: 12px 14px; border-radius: var(--radius-sm);
      color: var(--text-secondary); text-decoration: none;
      transition: all var(--transition-base);
      position: relative; overflow: hidden; white-space: nowrap;
    }
    .nav-item:hover { color: var(--text-primary); background: rgba(99, 102, 241, 0.08); }
    .nav-item.active {
      color: white;
      background: linear-gradient(135deg, rgba(99, 102, 241, 0.2), rgba(139, 92, 246, 0.15));
      box-shadow: 0 0 20px rgba(99, 102, 241, 0.15), inset 0 0 20px rgba(99, 102, 241, 0.05);
    }
    .nav-item.active::before {
      content: '';
      position: absolute; left: 0; top: 50%; transform: translateY(-50%);
      width: 3px; height: 60%;
      background: var(--gradient-indigo);
      border-radius: 0 var(--radius-full) var(--radius-full) 0;
    }
    .nav-icon { flex-shrink: 0; width: 20px; height: 20px; display: flex; align-items: center; justify-content: center; }
    .nav-label { font-size: 0.9rem; font-weight: 500; }
    .sidebar-footer { padding: 16px; border-top: 1px solid var(--border-subtle); }
    .collapse-btn {
      width: 100%; display: flex; align-items: center; justify-content: center;
      padding: 8px; background: transparent; color: var(--text-secondary);
      border-radius: var(--radius-sm); transition: all var(--transition-fast); margin-bottom: 12px;
    }
    .collapse-btn:hover { background: rgba(99, 102, 241, 0.1); color: var(--text-primary); }
    .user-info {
      display: flex; align-items: center; gap: 10px; padding: 10px;
      border-radius: var(--radius-sm); background: rgba(99, 102, 241, 0.05);
    }
    .user-avatar {
      width: 36px; height: 36px; border-radius: 50%; background: var(--gradient-indigo);
      display: flex; align-items: center; justify-content: center;
      font-size: 0.8rem; font-weight: 700; color: white; flex-shrink: 0;
    }
    .user-details { flex: 1; min-width: 0; }
    .user-name {
      display: block; font-size: 0.8125rem; font-weight: 600; color: var(--text-primary);
      white-space: nowrap; overflow: hidden; text-overflow: ellipsis;
    }
    .user-role { display: block; font-size: 0.7rem; color: var(--text-tertiary); }
    .logout-btn {
      background: transparent; color: var(--text-tertiary); padding: 6px;
      border-radius: var(--radius-sm); transition: all var(--transition-fast); display: flex; align-items: center;
    }
    .logout-btn:hover { color: var(--accent-rose); background: rgba(244, 63, 94, 0.1); }
    @media (max-width: 768px) {
      .sidebar { transform: translateX(-100%); }
      .sidebar.mobile-open { transform: translateX(0); }
    }
  `]
})
export class SidebarComponent implements OnInit {
  private router = inject(Router);
  private supabase = inject(SupabaseService);

  collapsed = input<boolean>(false);
  toggleCollapse = output<void>();

  userName = signal('Merchant User');
  userInitials = signal('MU');

  navItems = [
    { label: 'Dashboard', route: '/dashboard', exact: true, icon: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="3" width="7" height="7"></rect><rect x="14" y="3" width="7" height="7"></rect><rect x="14" y="14" width="7" height="7"></rect><rect x="3" y="14" width="7" height="7"></rect></svg>' },
    { label: 'Transactions', route: '/transactions', exact: false, icon: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="16" y1="13" x2="8" y2="13"></line><line x1="16" y1="17" x2="8" y2="17"></line></svg>' },
    { label: 'Refunds', route: '/refunds', exact: false, icon: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="23 4 23 10 17 10"></polyline><path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"></path></svg>' },
    { label: 'Analytics', route: '/analytics', exact: false, icon: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="20" x2="18" y2="10"></line><line x1="12" y1="20" x2="12" y2="4"></line><line x1="6" y1="20" x2="6" y2="14"></line></svg>' },
    { label: 'Settings', route: '/settings', exact: false, icon: '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="3"></circle><path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1 0 2.83 2 2 0 0 1-2.83 0l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-2 2 2 2 0 0 1-2-2v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83 0 2 2 0 0 1 0-2.83l.06-.06A1.65 1.65 0 0 0 4.68 15a1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1-2-2 2 2 0 0 1 2-2h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 0-2.83 2 2 0 0 1 2.83 0l.06.06A1.65 1.65 0 0 0 9 4.68a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 2-2 2 2 0 0 1 2 2v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 0 2 2 0 0 1 0 2.83l-.06.06A1.65 1.65 0 0 0 19.4 9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 2 2 2 2 0 0 1-2 2h-.09a1.65 1.65 0 0 0-1.51 1z"></path></svg>' }
  ];

  async ngOnInit() {
    const user = await this.supabase.getUser();
    if (user) {
      const name = user.user_metadata?.['business_name'] || user.email || 'Merchant';
      this.userName.set(name);
      this.userInitials.set(name.split(' ').map((n: string) => n[0]).join('').substring(0, 2).toUpperCase());
    }
  }

  async logout() {
    await this.supabase.signOut();
    this.router.navigate(['/login']);
  }
}
