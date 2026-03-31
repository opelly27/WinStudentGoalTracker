import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { firstValueFrom, Subject } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResult } from '../classes/api-result';
import { ResponseResult } from '../classes/auth.models';
import { describeHttpError } from '../classes/http-errors';
import { CreateStudentDto } from '../classes/create-student.dto';
import { CreateGoalDto } from '../classes/create-goal.dto';
import { StudentCardDto } from '../classes/student-card.dto';
import { StudentGoalSummary, StudentGoalItem } from '../classes/student-goal';
import { ProgressEventDto } from '../classes/progress-event.dto';
import { StudentBenchmarkSummary } from '../classes/benchmark.dto';
import { StudentProgressReportDto } from '../classes/student-progress-report.dto';

@Injectable({
    providedIn: 'root',
})
export class StudentService {

    // ************************** Constructor **************************

    // ************************** Declarations *************************

    private readonly http = inject(HttpClient);
    private readonly base = environment.apiBaseUrl;

    // Incremented after any data mutation so subscribers can refresh.
    readonly dataVersion = signal(0);

    // Emits targeted label updates for sidebar nodes without a full rebuild.
    private readonly _sidebarLabelUpdate = new Subject<{ routerLink: string[]; label: string }>();
    readonly sidebarLabelUpdate$ = this._sidebarLabelUpdate.asObservable();

    // ************************** Properties ***************************

    // ************************ Public Methods *************************

    // *****************************************************************
    // Increments the data version signal so subscribers can refresh.
    // *****************************************************************
    notifyDataChanged() {
        this.dataVersion.update(v => v + 1);
    }

    // *****************************************************************
    // Emits a targeted sidebar label update for a specific node,
    // avoiding the full tree rebuild that notifyDataChanged triggers.
    // *****************************************************************
    updateSidebarLabel(routerLink: string[], label: string) {
        this._sidebarLabelUpdate.next({ routerLink, label });
    }

    // *****************************************************************
    // Returns student card summaries for the authenticated user.
    // *****************************************************************
    async getMyStudents(): Promise<ApiResult<StudentCardDto[]>> {
        try {
            const result = await firstValueFrom(
                this.http.get<ResponseResult<StudentCardDto[]>>(`${this.base}/api/Student/my`)
            );
            return result.success && result.data
                ? ApiResult.ok(result.data)
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }

    // *****************************************************************
    // Returns goal summary for a given student.
    // *****************************************************************
    async getGoalsForStudent(studentId: string): Promise<ApiResult<StudentGoalSummary | null>> {
        try {
            const result = await firstValueFrom(
                this.http.get<ResponseResult<StudentGoalSummary>>(`${this.base}/api/Student/${studentId}/goals`)
            );
            return result.success && result.data
                ? ApiResult.ok(result.data)
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }

    // *****************************************************************
    // Creates a new student and returns the created student card.
    // *****************************************************************
    async createStudent(data: CreateStudentDto): Promise<ApiResult<StudentCardDto>> {
        try {
            const result = await firstValueFrom(
                this.http.post<ResponseResult<StudentCardDto>>(`${this.base}/api/Student`, data)
            );
            return result.success && result.data
                ? ApiResult.ok(result.data)
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }

    // *****************************************************************
    // Creates a new goal for a student and returns the created goal.
    // *****************************************************************
    async createGoal(studentId: string, data: CreateGoalDto): Promise<ApiResult<StudentGoalItem>> {
        try {
            const result = await firstValueFrom(
                this.http.post<ResponseResult<StudentGoalItem>>(`${this.base}/api/Student/${studentId}/goals`, data)
            );
            return result.success && result.data
                ? ApiResult.ok(result.data)
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }

    // *****************************************************************
    // Creates a new progress event, optionally with benchmark
    // associations. Returns the new progress event ID on success.
    // *****************************************************************
    async addProgressEvent(studentId: string, goalId: string, content: string, benchmarkIds?: string[]): Promise<ApiResult<any>> {
        try {
            const result = await firstValueFrom(
                this.http.post<ResponseResult<any>>(`${this.base}/api/Student/${studentId}/progress-event`, { goalId, content, benchmarkIds })
            );
            return result.success
                ? ApiResult.ok(result.data)
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }

    // *****************************************************************
    // Updates a progress event's content and benchmark associations.
    // *****************************************************************
    async updateProgressEvent(studentId: string, progressEventId: string, content: string, benchmarkIds?: string[]): Promise<ApiResult> {
        try {
            const result = await firstValueFrom(
                this.http.put<ResponseResult<void>>(`${this.base}/api/Student/${studentId}/progress-events/${progressEventId}`, { content, benchmarkIds })
            );
            return result.success
                ? ApiResult.empty()
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }

    // *****************************************************************
    // Returns benchmark IDs associated with a progress event.
    // *****************************************************************
    async getProgressEventBenchmarks(progressEventId: string): Promise<ApiResult<string[]>> {
        try {
            const result = await firstValueFrom(
                this.http.get<ResponseResult<string[]>>(`${this.base}/api/Student/progress-events/${progressEventId}/benchmarks`)
            );
            return result.success
                ? ApiResult.ok(result.data ?? [])
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }

    // *****************************************************************
    // Returns progress events for a given student goal.
    // *****************************************************************
    async getProgressEventsForGoal(goalId: string): Promise<ApiResult<ProgressEventDto[]>> {
        try {
            const result = await firstValueFrom(
                this.http.get<ResponseResult<ProgressEventDto[]>>(`${this.base}/api/Student/goals/${goalId}/progress-events`)
            );
            return result.success
                ? ApiResult.ok(result.data ?? [])
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }

    // *****************************************************************
    // Returns a full progress report for a student within a date
    // range, including goals, events, and benchmark associations.
    // *****************************************************************
    async getStudentProgressReport(studentId: string, fromDate: string, toDate: string, goalIds?: string): Promise<ApiResult<string>> {
        try {
            const params: any = { fromDate, toDate };
            if (goalIds) params.goalIds = goalIds;

            const result = await firstValueFrom(
                this.http.get<ResponseResult<string>>(
                    `${this.base}/api/Student/${studentId}/progress-report`,
                    { params }
                )
            );
            return result.success && result.data
                ? ApiResult.ok(result.data)
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }

    // ************************ Event Handlers *************************

    // ********************** Support Procedures ***********************

    // *****************************************************************
    // Returns a single student by ID.
    // *****************************************************************
    async getStudentById(studentId: string): Promise<ApiResult<StudentCardDto>> {
        try {
            const result = await firstValueFrom(
                this.http.get<ResponseResult<StudentCardDto>>(`${this.base}/api/Student/${studentId}`)
            );
            return result.success && result.data
                ? ApiResult.ok(result.data)
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }

    // *****************************************************************
    // Updates a student and returns the refreshed student data.
    // *****************************************************************
    async updateStudent(studentId: string, data: { identifier?: string; programYear?: number | null; enrollmentDate?: string | null; nextIepDate?: string | null }): Promise<ApiResult<StudentCardDto>> {
        try {
            const result = await firstValueFrom(
                this.http.put<ResponseResult<StudentCardDto>>(`${this.base}/api/Student/${studentId}`, data)
            );
            return result.success && result.data
                ? ApiResult.ok(result.data)
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }

    // *****************************************************************
    // Returns benchmarks for a given student.
    // *****************************************************************
    async getBenchmarksForStudent(studentId: string): Promise<ApiResult<StudentBenchmarkSummary | null>> {
        try {
            const result = await firstValueFrom(
                this.http.get<ResponseResult<StudentBenchmarkSummary>>(`${this.base}/api/Student/${studentId}/benchmarks`)
            );
            return result.success && result.data
                ? ApiResult.ok(result.data)
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }

    // *****************************************************************
    // Creates a new benchmark for a student.
    // *****************************************************************
    async createBenchmark(studentId: string, data: { goalId: string; benchmark: string; shortName?: string }): Promise<ApiResult<any>> {
        try {
            const result = await firstValueFrom(
                this.http.post<ResponseResult<any>>(`${this.base}/api/Student/${studentId}/benchmarks`, data)
            );
            return result.success
                ? ApiResult.ok(result.data)
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }

    // *****************************************************************
    // Updates a benchmark's text.
    // *****************************************************************
    async updateBenchmark(studentId: string, benchmarkId: string, benchmarkText: string, shortName?: string): Promise<ApiResult<any>> {
        try {
            const result = await firstValueFrom(
                this.http.put<ResponseResult<any>>(`${this.base}/api/Student/${studentId}/benchmarks/${benchmarkId}`, { benchmark: benchmarkText, shortName })
            );
            return result.success
                ? ApiResult.ok(result.data)
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }

    // *****************************************************************
    // Updates a goal's description, category, and baseline.
    // *****************************************************************
    async updateGoal(studentId: string, goalId: string, data: {
        description?: string;
        category?: string;
        baseline?: string;
        targetCompletionDate?: string | null;
        closeDate?: string | null;
        achieved?: boolean | null;
        closeNotes?: string | null;
    }): Promise<ApiResult<any>> {
        try {
            const result = await firstValueFrom(
                this.http.put<ResponseResult<any>>(`${this.base}/api/Student/${studentId}/goals/${goalId}`, data)
            );
            return result.success
                ? ApiResult.ok(result.data)
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }
}
