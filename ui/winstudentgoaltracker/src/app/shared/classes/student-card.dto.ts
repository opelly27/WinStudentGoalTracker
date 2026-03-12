export interface StudentCardDto {
    studentId: string;
    identifier: string;
    nextIepDate: Date;
    lastEntryDate: Date | null;
    goalCount: number;
    progressEventCount: number;
}
