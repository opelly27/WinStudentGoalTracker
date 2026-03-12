export interface CreateStudentDto {
    identifier: string;
    programYear: number | null;
    enrollmentDate: Date | null;
    nextIepDate: Date | null;
}
