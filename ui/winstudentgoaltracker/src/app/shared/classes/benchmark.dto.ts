export interface StudentBenchmarkSummary {
    studentIdentifier: string;
    benchmarks: BenchmarkDto[];
}

export interface BenchmarkDto {
    benchmarkId: string;
    goalId: string;
    goalCategory: string;
    benchmark: string;
    shortName?: string;
    createdByName: string;
    createdAt: Date;
    updatedAt: Date | null;
}
