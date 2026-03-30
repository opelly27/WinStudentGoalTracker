export interface StudentCardDto {
    studentId: string;
    identifier: string;
    nextIepDate: Date;
    firstEntryDate: Date | null;
    lastEntryDate: Date | null;
    goalCount: number;
    progressEventCount: number;
    benchmarkCount: number;
}
