import { DatePipe, NgIf } from '@angular/common';
import { Component, DestroyRef, inject } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';

import { AuthService } from '../../core/services/auth.service';
import { EventService } from '../../core/services/event.service';
import { EventDto } from '../../core/models/event.models';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog.component';
import { EventFormDialogComponent } from './events-form-dialog.component';

@Component({
  selector: 'app-events-page',
  standalone: true,
  imports: [
    NgIf,
    DatePipe,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule,
    MatPaginatorModule,
    MatTooltipModule,
    MatDialogModule,
    MatSnackBarModule
  ],
  templateUrl: './events-page.component.html'
})
export class EventsPageComponent {
  private readonly eventService = inject(EventService);
  private readonly dialog = inject(MatDialog);
  private readonly snack = inject(MatSnackBar);
  private readonly destroyRef = inject(DestroyRef);

  readonly auth = inject(AuthService);

  readonly search = new FormControl<string>('', { nonNullable: true });

  readonly displayedColumns = [
    'title',
    'eventType',
    'startDate',
    'endDate',
    'location',
    'participantCount',
    'actions'
  ];
  readonly dataSource = new MatTableDataSource<EventDto>([]);

  loading = false;
  totalCount = 0;
  pageIndex = 0;
  pageSize = 10;

  constructor() {
    this.load();

    this.search.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.pageIndex = 0;
        this.load();
      });
  }

  canManage() {
    const role = this.auth.role();
    return role === 'Admin' || role === 'SocialWorker';
  }

  canDelete() {
    return this.auth.role() === 'Admin';
  }

  load() {
    this.loading = true;

    this.eventService
      .getEvents({ pageNumber: this.pageIndex + 1, pageSize: this.pageSize, searchTerm: this.search.value })
      .subscribe({
        next: (result) => {
          this.loading = false;
          this.totalCount = result.totalCount;
          this.dataSource.data = result.items;
        },
        error: (err: unknown) => {
          this.loading = false;
          const message = err instanceof Error ? err.message : 'Failed to load events';
          this.snack.open(message, 'Dismiss', { duration: 5000 });
        }
      });
  }

  pageChanged(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.load();
  }

  openCreate() {
    const ref = this.dialog.open(EventFormDialogComponent, { width: '720px', data: {} });
    ref.afterClosed().subscribe((changed) => {
      if (changed) this.load();
    });
  }

  openEdit(event: EventDto) {
    const ref = this.dialog.open(EventFormDialogComponent, { width: '720px', data: { event } });
    ref.afterClosed().subscribe((changed) => {
      if (changed) this.load();
    });
  }

  delete(event: EventDto) {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '420px',
      data: {
        title: 'Delete event',
        message: `Are you sure you want to delete ${event.title}?`,
        confirmText: 'Delete'
      }
    });

    ref.afterClosed().subscribe((confirmed) => {
      if (!confirmed) return;

      this.eventService.deleteEvent(event.id).subscribe({
        next: () => {
          this.snack.open('Event deleted', 'Dismiss', { duration: 3000 });
          this.load();
        },
        error: (err: unknown) => {
          const message = err instanceof Error ? err.message : 'Failed to delete event';
          this.snack.open(message, 'Dismiss', { duration: 5000 });
        }
      });
    });
  }
}
