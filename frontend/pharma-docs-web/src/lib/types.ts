// TypeScript types mirroring PharmaDocs backend DTOs and enums.
// Keep these in sync with PharmaDocs.Application/DTOs and PharmaDocs.Domain/Enums.

export type UserRole = "RegAffairsOfficer" | "Viewer";

export type SubmissionStatus =
  | "Draft"
  | "InProgress"
  | "Submitted"
  | "UnderReview"
  | "Approved"
  | "Rejected"
  | "Withdrawn";

export type DocumentStatus = "Draft" | "Final" | "Superseded";

export type DocumentType =
  | "CertificateOfAnalysis"
  | "ProductMonograph"
  | "BatchRecord"
  | "TemperatureStorageLog"
  | "ProductSpecificationSheet"
  | "ImportClearance"
  | "SubmissionCertificate"
  | "ClinicalStudyReport"
  | "NonClinicalStudyReport"
  | "QualitySummary";

export type SubmissionType = "NDS" | "ANDS" | "SNDS" | "DMF";

export type AuditAction =
  | "Created"
  | "Updated"
  | "Archived"
  | "Unarchived"
  | "StatusChanged";

// Mirrors PharmaDocs.Application.DTOs.LoginDto
export interface LoginDto {
  email: string;
  password: string;
}

// Mirrors PharmaDocs.Application.DTOs.AuthResponseDto
export interface AuthResponseDto {
  token: string;
  fullName: string;
  email: string;
  role: UserRole;
  expiresAt: string;
}

// The user data exposed to the client after login — never includes the raw JWT.
export interface SessionUser {
  fullName: string;
  email: string;
  role: UserRole;
}
