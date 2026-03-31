export interface StudentProgressReportDto {
    studentIdentifier: string;
    goals: ProgressReportGoalDto[];
}

export interface ProgressReportGoalDto {
    goalId: string;
    category: string;
    description: string;
    progressEvents: ProgressReportEventDto[];
}

export interface ProgressReportEventDto {
    progressEventId: string;
    content: string;
    createdAt: Date;
    benchmarkNames: string | null;
}
