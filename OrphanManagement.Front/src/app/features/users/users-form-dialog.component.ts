import { NgFor, NgIf } from '@angular/common';
import { Component, Inject, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { UserRole } from '../../core/models/enums';
import { UserDto } from '../../core/models/user.models';
import { UserService } from '../../core/services/user.service';

export interface UserFormDialogData {
  user?: UserDto;
}

function parseEnumValue<T extends Record<string, number | string>>(enumObj: T, key: string): number | null {
  const normalized = key.replace(/\s+/g, '').toLowerCase();

  for (const k of Object.keys(enumObj)) {
    if (!Number.isNaN(Number(k))) continue;
    if (k.toLowerCase() === normalized) return enumObj[k] as number;
  }

  return null;
}

@Component({
  selector: 'app-user-form-dialog',
  standalone: true,
  imports: [
    NgIf,
    NgFor,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './users-form-dialog.component.html'
})
export class UserFormDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly userService = inject(UserService);
  private readonly snack = inject(MatSnackBar);
  private readonly dialogRef = inject(MatDialogRef<UserFormDialogComponent>);

  readonly isEdit = !!this.data.user;
  saving = false;

  readonly roleOptions = [
    { value: UserRole.Admin, label: 'Admin' },
    { value: UserRole.SocialWorker, label: 'Social Worker' },
    { value: UserRole.Volunteer, label: 'Volunteer' },
    { value: UserRole.Viewer, label: 'Viewer' }
  ];

  readonly form = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email]],
    password: [''],
    role: [UserRole.Viewer, [Validators.required]],
    phone: [''],
    isActive: [true]
  });

  constructor(@Inject(MAT_DIALOG_DATA) public readonly data: UserFormDialogData) {
    if (data.user) {
      const u = data.user;
      this.form.patchValue({
        fullName: u.fullName,
        email: u.email,
        role: (parseEnumValue(UserRole, u.role) ?? UserRole.Viewer) as UserRole,
        phone: u.phone ?? '',
        isActive: u.isActive
      });
    } else {
      this.form.controls.password.setValidators([Validators.required, Validators.minLength(6)]);
      this.form.controls.password.updateValueAndValidity();
    }
  }

  save() {
    if (this.saving) return;

    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    this.saving = true;
    const value = this.form.getRawValue();

    const common = {
      fullName: value.fullName,
      email: value.email,
      role: value.role,
      phone: value.phone || null
    };

    const obs = this.data.user
      ? this.userService.updateUser(this.data.user.id, {
          ...common,
          isActive: value.isActive
        })
      : this.userService.createUser({
          ...common,
          password: value.password
        });

    obs.subscribe({
      next: () => {
        this.saving = false;
        this.snack.open(this.data.user ? 'User updated' : 'User created', 'Dismiss', { duration: 3000 });
        this.dialogRef.close(true);
      },
      error: (err: unknown) => {
        this.saving = false;
        const message = err instanceof Error ? err.message : 'Failed to save user';
        this.snack.open(message, 'Dismiss', { duration: 5000 });
      }
    });
  }
}
