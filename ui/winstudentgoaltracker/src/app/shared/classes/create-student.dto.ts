export interface CreateStudentDto {
    identifier: string;
    programYear: number | null;
    enrollmentDate: Date | null;
    expectedGrad: Date | null;
}
