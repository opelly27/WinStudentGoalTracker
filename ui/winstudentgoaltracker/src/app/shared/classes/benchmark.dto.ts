export interface StudentBenchmarkSummary {
    studentIdentifier: string;
    benchmarks: BenchmarkDto[];
}

export interface BenchmarkDto {
    benchmarkId: string;
    goalId: string;
    goalTitle: string;
    benchmark: string;
    createdByName: string;
    createdAt: Date;
    updatedAt: Date | null;
}
