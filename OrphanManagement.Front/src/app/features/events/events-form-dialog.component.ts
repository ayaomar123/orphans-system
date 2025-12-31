import { Component, Inject, inject } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { EventType } from '../../core/models/enums';
import { EventDto } from '../../core/models/event.models';
import { EventService } from '../../core/services/event.service';

export interface EventFormDialogData {
  event?: EventDto;
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
  selector: 'app-event-form-dialog',
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
  templateUrl: './events-form-dialog.component.html'
})
export class EventFormDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly eventService = inject(EventService);
  private readonly snack = inject(MatSnackBar);
  private readonly dialogRef = inject(MatDialogRef<EventFormDialogComponent>);

  get isEdit(): boolean {
    return !!this.data.event;
  }
  saving = false;

  readonly eventTypeOptions = [
    { value: EventType.Educational, label: 'Educational' },
    { value: EventType.Recreational, label: 'Recreational' },
    { value: EventType.Medical, label: 'Medical' },
    { value: EventType.Cultural, label: 'Cultural' },
    { value: EventType.Sports, label: 'Sports' },
    { value: EventType.Arts, label: 'Arts' },
    { value: EventType.CommunityService, label: 'Community Service' },
    { value: EventType.Other, label: 'Other' }
  ];

  readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    startDate: [new Date(), [Validators.required]],
    endDate: [new Date(), [Validators.required]],
    location: ['', [Validators.required, Validators.maxLength(200)]],
    eventType: [EventType.Other, [Validators.required]],
    maxParticipants: new FormControl<number | null>(null),
    budget: new FormControl<number | null>(null),
    notes: ['']
  });

  constructor(@Inject(MAT_DIALOG_DATA) public readonly data: EventFormDialogData) {
    if (data.event) {
      const e = data.event;

      this.form.patchValue({
        title: e.title,
        description: e.description ?? '',
        startDate: new Date(e.startDate),
        endDate: new Date(e.endDate),
        location: e.location,
        eventType: (parseEnumValue(EventType, e.eventType) ?? EventType.Other) as EventType,
        maxParticipants: e.maxParticipants ?? null,
        budget: e.budget ?? null,
        notes: e.notes ?? ''
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
      description: value.description || null,
      notes: value.notes || null,
      maxParticipants: value.maxParticipants ?? null,
      budget: value.budget ?? null
    };

    const obs = this.data.event
      ? this.eventService.updateEvent(this.data.event.id, request)
      : this.eventService.createEvent(request);

    obs.subscribe({
      next: () => {
        this.saving = false;
        this.snack.open(this.data.event ? 'Event updated' : 'Event created', 'Dismiss', { duration: 3000 });
        this.dialogRef.close(true);
      },
      error: (err: unknown) => {
        this.saving = false;
        const message = err instanceof Error ? err.message : 'Failed to save event';
        this.snack.open(message, 'Dismiss', { duration: 5000 });
      }
    });
  }
}
