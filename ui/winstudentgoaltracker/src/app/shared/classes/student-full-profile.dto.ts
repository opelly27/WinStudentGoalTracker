import { StudentCardDto } from './student-card.dto';
import { StudentGoalItem } from './student-goal';
import { BenchmarkDto } from './benchmark.dto';

export interface StudentFullProfileDto {
    student: StudentCardDto;
    goals: StudentGoalItem[];
    benchmarks: BenchmarkDto[];
    progressEvents: ProgressEventWithGoalDto[];
    progressEventBenchmarks: ProgressEventBenchmarkLink[];
}

export interface ProgressEventWithGoalDto {
    progressEventId: string;
    goalId: string;
    content: string;
    createdAt: Date;
    createdByName: string;
}

export interface ProgressEventBenchmarkLink {
    progressEventId: string;
    benchmarkId: string;
}
