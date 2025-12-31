import { EducationStatus, Gender, HealthStatus, SponsorshipStatus } from './enums';

export interface OrphanDto {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  gender: string;
  dateOfBirth: string;
  age: number;
  nationalId?: string | null;
  city: string;
  address?: string | null;
  healthStatus: string;
  healthNotes?: string | null;
  educationStatus: string;
  schoolName?: string | null;
  classLevel?: string | null;
  guardianName?: string | null;
  guardianPhone?: string | null;
  guardianRelationship?: string | null;
  sponsorshipStatus: string;
  photoUrl?: string | null;
  notes?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateOrphanRequest {
  firstName: string;
  lastName: string;
  gender: Gender;
  dateOfBirth: string | Date;
  nationalId?: string | null;
  city: string;
  address?: string | null;
  healthStatus: HealthStatus;
  healthNotes?: string | null;
  educationStatus: EducationStatus;
  schoolName?: string | null;
  classLevel?: string | null;
  guardianName?: string | null;
  guardianPhone?: string | null;
  guardianRelationship?: string | null;
  sponsorshipStatus: SponsorshipStatus;
  notes?: string | null;
}

export type UpdateOrphanRequest = CreateOrphanRequest;
