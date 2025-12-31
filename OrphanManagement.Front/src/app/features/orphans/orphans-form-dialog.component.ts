import { Component, Inject, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { EducationStatus, Gender, HealthStatus, SponsorshipStatus } from '../../core/models/enums';
import { OrphanDto } from '../../core/models/orphan.models';
import { OrphanService } from '../../core/services/orphan.service';

export interface OrphanFormDialogData {
  orphan?: OrphanDto;
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
  selector: 'app-orphan-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './orphans-form-dialog.component.html'
})
export class OrphanFormDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly orphanService = inject(OrphanService);
  private readonly snack = inject(MatSnackBar);
  private readonly dialogRef = inject(MatDialogRef<OrphanFormDialogComponent>);

  get isEdit(): boolean {
    return !!this.data.orphan;
  }
  saving = false;

  readonly genderOptions = [
    { value: Gender.Male, label: 'Male' },
    { value: Gender.Female, label: 'Female' }
  ];

  readonly sponsorshipOptions = [
    { value: SponsorshipStatus.NotSponsored, label: 'Not Sponsored' },
    { value: SponsorshipStatus.PartiallySponsored, label: 'Partially Sponsored' },
    { value: SponsorshipStatus.FullySponsored, label: 'Fully Sponsored' }
  ];

  readonly educationOptions = [
    { value: EducationStatus.NotEnrolled, label: 'Not Enrolled' },
    { value: EducationStatus.Preschool, label: 'Preschool' },
    { value: EducationStatus.PrimarySchool, label: 'Primary School' },
    { value: EducationStatus.MiddleSchool, label: 'Middle School' },
    { value: EducationStatus.HighSchool, label: 'High School' },
    { value: EducationStatus.University, label: 'University' },
    { value: EducationStatus.VocationalTraining, label: 'Vocational Training' },
    { value: EducationStatus.Graduated, label: 'Graduated' }
  ];

  readonly healthOptions = [
    { value: HealthStatus.Healthy, label: 'Healthy' },
    { value: HealthStatus.MinorIssues, label: 'Minor Issues' },
    { value: HealthStatus.ChronicCondition, label: 'Chronic Condition' },
    { value: HealthStatus.SpecialNeeds, label: 'Special Needs' }
  ];

  readonly form = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(50)]],
    lastName: ['', [Validators.required, Validators.maxLength(50)]],
    gender: [Gender.Male, [Validators.required]],
    dateOfBirth: [new Date(), [Validators.required]],
    nationalId: [''],
    city: ['', [Validators.required, Validators.maxLength(100)]],
    address: [''],
    healthStatus: [HealthStatus.Healthy, [Validators.required]],
    healthNotes: [''],
    educationStatus: [EducationStatus.NotEnrolled, [Validators.required]],
    schoolName: [''],
    classLevel: [''],
    guardianName: [''],
    guardianPhone: [''],
    guardianRelationship: [''],
    sponsorshipStatus: [SponsorshipStatus.NotSponsored, [Validators.required]],
    notes: ['']
  });

  constructor(@Inject(MAT_DIALOG_DATA) public readonly data: OrphanFormDialogData) {
    if (data.orphan) {
      const o = data.orphan;

      this.form.patchValue({
        firstName: o.firstName,
        lastName: o.lastName,
        gender: (parseEnumValue(Gender, o.gender) ?? Gender.Male) as Gender,
        dateOfBirth: new Date(o.dateOfBirth),
        nationalId: o.nationalId ?? '',
        city: o.city,
        address: o.address ?? '',
        healthStatus: (parseEnumValue(HealthStatus, o.healthStatus) ?? HealthStatus.Healthy) as HealthStatus,
        healthNotes: o.healthNotes ?? '',
        educationStatus: (parseEnumValue(EducationStatus, o.educationStatus) ?? EducationStatus.NotEnrolled) as EducationStatus,
        schoolName: o.schoolName ?? '',
        classLevel: o.classLevel ?? '',
        guardianName: o.guardianName ?? '',
        guardianPhone: o.guardianPhone ?? '',
        guardianRelationship: o.guardianRelationship ?? '',
        sponsorshipStatus: (parseEnumValue(SponsorshipStatus, o.sponsorshipStatus) ?? SponsorshipStatus.NotSponsored) as SponsorshipStatus,
        notes: o.notes ?? ''
      });
    }
  }

  save() {
    if (this.saving) return;

    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    this.saving = true;
    const value = this.form.getRawValue();

    const request = {
      ...value,
      nationalId: value.nationalId || null,
      address: value.address || null,
      healthNotes: value.healthNotes || null,
      schoolName: value.schoolName || null,
      classLevel: value.classLevel || null,
      guardianName: value.guardianName || null,
      guardianPhone: value.guardianPhone || null,
      guardianRelationship: value.guardianRelationship || null,
      notes: value.notes || null
    };

    const obs = this.data.orphan
      ? this.orphanService.updateOrphan(this.data.orphan.id, request)
      : this.orphanService.createOrphan(request);

    obs.subscribe({
      next: () => {
        this.saving = false;
        this.snack.open(this.data.orphan ? 'Orphan updated' : 'Orphan created', 'Dismiss', { duration: 3000 });
        this.dialogRef.close(true);
      },
      error: (err: unknown) => {
        this.saving = false;
        const message = err instanceof Error ? err.message : 'Failed to save orphan';
        this.snack.open(message, 'Dismiss', { duration: 5000 });
      }
    });
  }
}
