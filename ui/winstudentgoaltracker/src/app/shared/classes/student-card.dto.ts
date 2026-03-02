export interface StudentCardDto {
    studentId: string;
    identifier: string;
    expectedGradDate: Date;
    lastEntryDate: Date | null;
    goalCount: number;
    progressEventCount: number;
}
