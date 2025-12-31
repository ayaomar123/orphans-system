import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/role.guard';
import { LoginComponent } from './features/login/login.component';
import { ShellComponent } from './features/orphan-management/shell.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { OrphansPageComponent } from './features/orphans/orphans-page.component';
import { EventsPageComponent } from './features/events/events-page.component';
import { UsersPageComponent } from './features/users/users-page.component';

export const appRoutes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: ShellComponent,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'orphans', component: OrphansPageComponent },
      { path: 'events', component: EventsPageComponent },
      { path: 'users', component: UsersPageComponent, canActivate: [adminGuard] }
    ]
  },
  { path: '**', redirectTo: '' }
];
