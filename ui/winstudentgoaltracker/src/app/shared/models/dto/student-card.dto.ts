export interface StudentCardDto {
    studentId: string;
    identifier: string;
    age: number;
    lastEntryDate: string | null;
    goalCount: number;
    progressEventCount: number;
}
