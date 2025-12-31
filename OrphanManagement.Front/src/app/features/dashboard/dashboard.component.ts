import { AsyncPipe, DatePipe, KeyValuePipe, NgFor, NgIf } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { catchError, of } from 'rxjs';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';

import { EventService } from '../../core/services/event.service';
import { OrphanService } from '../../core/services/orphan.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    AsyncPipe,
    DatePipe,
    KeyValuePipe,
    NgFor,
    NgIf,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatListModule,
    MatChipsModule
  ],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent {
  private readonly orphanService = inject(OrphanService);
  private readonly eventService = inject(EventService);

  readonly cityStats$ = this.orphanService.getStatsByCity().pipe(catchError(() => of({} as Record<string, number>)));
  readonly ageStats$ = this.orphanService
    .getStatsByAgeGroup()
    .pipe(catchError(() => of({} as Record<string, number>)));
  readonly upcomingEvents$ = this.eventService.getUpcomingEvents(6).pipe(catchError(() => of([])));
}
