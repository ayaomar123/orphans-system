import { EventType } from './enums';

export interface EventDto {
  id: string;
  title: string;
  description?: string | null;
  startDate: string;
  endDate: string;
  location: string;
  eventType: string;
  maxParticipants?: number | null;
  budget?: number | null;
  notes?: string | null;
  participantCount: number;
  isUpcoming: boolean;
  isOngoing: boolean;
  isCompleted: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateEventRequest {
  title: string;
  description?: string | null;
  startDate: string | Date;
  endDate: string | Date;
  location: string;
  eventType: EventType;
  maxParticipants?: number | null;
  budget?: number | null;
  notes?: string | null;
}

export type UpdateEventRequest = CreateEventRequest;
