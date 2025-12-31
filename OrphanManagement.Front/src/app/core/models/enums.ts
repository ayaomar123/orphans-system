// Enum numeric values must match the backend C# enums.

export enum Gender {
  Male = 1,
  Female = 2
}

export enum SponsorshipStatus {
  NotSponsored = 1,
  PartiallySponsored = 2,
  FullySponsored = 3
}

export enum EducationStatus {
  NotEnrolled = 1,
  Preschool = 2,
  PrimarySchool = 3,
  MiddleSchool = 4,
  HighSchool = 5,
  University = 6,
  VocationalTraining = 7,
  Graduated = 8
}

export enum HealthStatus {
  Healthy = 1,
  MinorIssues = 2,
  ChronicCondition = 3,
  SpecialNeeds = 4
}

export enum EventType {
  Educational = 1,
  Recreational = 2,
  Medical = 3,
  Cultural = 4,
  Sports = 5,
  Arts = 6,
  CommunityService = 7,
  Other = 8
}

export enum UserRole {
  Admin = 1,
  SocialWorker = 2,
  Volunteer = 3,
  Viewer = 4
}
