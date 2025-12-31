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
import { OrphanService } from '../../core/services/orphan.service';
import { OrphanDto } from '../../core/models/orphan.models';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog.component';
import { OrphanFormDialogComponent } from './orphans-form-dialog.component';

@Component({
  selector: 'app-orphans-page',
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
  templateUrl: './orphans-page.component.html'
})
export class OrphansPageComponent {
  private readonly orphanService = inject(OrphanService);
  private readonly dialog = inject(MatDialog);
  private readonly snack = inject(MatSnackBar);
  private readonly destroyRef = inject(DestroyRef);

  readonly auth = inject(AuthService);

  readonly search = new FormControl<string>('', { nonNullable: true });

  readonly displayedColumns = ['fullName', 'gender', 'age', 'city', 'sponsorshipStatus', 'updatedAt', 'actions'];
  readonly dataSource = new MatTableDataSource<OrphanDto>([]);

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

    this.orphanService
      .getOrphans({ pageNumber: this.pageIndex + 1, pageSize: this.pageSize, searchTerm: this.search.value })
      .subscribe({
        next: (result) => {
          this.loading = false;
          this.totalCount = result.totalCount;
          this.dataSource.data = result.items;
        },
        error: (err: unknown) => {
          this.loading = false;
          const message = err instanceof Error ? err.message : 'Failed to load orphans';
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
    const ref = this.dialog.open(OrphanFormDialogComponent, {
      width: '720px',
      data: {}
    });

    ref.afterClosed().subscribe((changed) => {
      if (changed) this.load();
    });
  }

  openEdit(orphan: OrphanDto) {
    const ref = this.dialog.open(OrphanFormDialogComponent, {
      width: '720px',
      data: { orphan }
    });

    ref.afterClosed().subscribe((changed) => {
      if (changed) this.load();
    });
  }

  delete(orphan: OrphanDto) {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '420px',
      data: {
        title: 'Delete orphan',
        message: `Are you sure you want to delete ${orphan.fullName}? This action cannot be undone.`,
        confirmText: 'Delete'
      }
    });

    ref.afterClosed().subscribe((confirmed) => {
      if (!confirmed) return;

      this.orphanService.deleteOrphan(orphan.id).subscribe({
        next: () => {
          this.snack.open('Orphan deleted', 'Dismiss', { duration: 3000 });
          this.load();
        },
        error: (err: unknown) => {
          const message = err instanceof Error ? err.message : 'Failed to delete orphan';
          this.snack.open(message, 'Dismiss', { duration: 5000 });
        }
      });
    });
  }
}
