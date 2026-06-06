import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from './sidebar.component';
import { HeaderComponent } from './header.component';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, SidebarComponent, HeaderComponent],
  template: `
    <div class="layout" [class.sidebar-collapsed]="isCollapsed()">
      <app-sidebar
        [collapsed]="isCollapsed()"
        (toggleCollapse)="isCollapsed.set(!isCollapsed())"
      />
      <div class="main-area">
        <app-header [sidebarCollapsed]="isCollapsed()" (toggleSidebar)="isCollapsed.set(!isCollapsed())" />
        <main class="content page-enter">
          <router-outlet />
        </main>
      </div>
    </div>
  `,
  styles: [`
    .layout {
      display: flex;
      min-height: 100vh;
      background: var(--bg-primary);
    }
    .main-area {
      flex: 1;
      margin-left: var(--sidebar-width);
      transition: margin-left var(--transition-base);
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }
    .sidebar-collapsed .main-area {
      margin-left: var(--sidebar-collapsed);
    }
    .content {
      flex: 1;
      padding: 24px 32px;
      overflow-y: auto;
    }
    @media (max-width: 768px) {
      .main-area { margin-left: 0 !important; }
    }
  `]
})
export class LayoutComponent {
  isCollapsed = signal(false);
}
