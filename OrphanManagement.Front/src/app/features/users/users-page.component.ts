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

import { UserService } from '../../core/services/user.service';
import { UserDto } from '../../core/models/user.models';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog.component';
import { UserFormDialogComponent } from './users-form-dialog.component';

@Component({
  selector: 'app-users-page',
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
  templateUrl: './users-page.component.html'
})
export class UsersPageComponent {
  private readonly userService = inject(UserService);
  private readonly dialog = inject(MatDialog);
  private readonly snack = inject(MatSnackBar);
  private readonly destroyRef = inject(DestroyRef);

  readonly search = new FormControl<string>('', { nonNullable: true });

  readonly displayedColumns = ['fullName', 'email', 'role', 'isActive', 'lastLoginAt', 'actions'];
  readonly dataSource = new MatTableDataSource<UserDto>([]);

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

  load() {
    this.loading = true;

    this.userService
      .getUsers({ pageNumber: this.pageIndex + 1, pageSize: this.pageSize, searchTerm: this.search.value })
      .subscribe({
        next: (result) => {
          this.loading = false;
          this.totalCount = result.totalCount;
          this.dataSource.data = result.items;
        },
        error: (err: unknown) => {
          this.loading = false;
          const message = err instanceof Error ? err.message : 'Failed to load users';
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
    const ref = this.dialog.open(UserFormDialogComponent, { width: '720px', data: {} });
    ref.afterClosed().subscribe((changed) => {
      if (changed) this.load();
    });
  }

  openEdit(user: UserDto) {
    const ref = this.dialog.open(UserFormDialogComponent, { width: '720px', data: { user } });
    ref.afterClosed().subscribe((changed) => {
      if (changed) this.load();
    });
  }

  delete(user: UserDto) {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '420px',
      data: {
        title: 'Delete user',
        message: `Are you sure you want to delete ${user.fullName}?`,
        confirmText: 'Delete'
      }
    });

    ref.afterClosed().subscribe((confirmed) => {
      if (!confirmed) return;

      this.userService.deleteUser(user.id).subscribe({
        next: () => {
          this.snack.open('User deleted', 'Dismiss', { duration: 3000 });
          this.load();
        },
        error: (err: unknown) => {
          const message = err instanceof Error ? err.message : 'Failed to delete user';
          this.snack.open(message, 'Dismiss', { duration: 5000 });
        }
      });
    });
  }
}
