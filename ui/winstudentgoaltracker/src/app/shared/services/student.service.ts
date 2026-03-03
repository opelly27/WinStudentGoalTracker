import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResult } from '../classes/api-result';
import { ResponseResult } from '../classes/auth.models';
import { describeHttpError } from '../classes/http-errors';
import { CreateStudentDto } from '../classes/create-student.dto';
import { StudentCardDto } from '../classes/student-card.dto';
import { StudentGoalSummary } from '../classes/student-goal';

@Injectable({
    providedIn: 'root',
})
export class StudentService {

    // ************************** Constructor **************************

    // ************************** Declarations *************************

    private readonly http = inject(HttpClient);
    private readonly base = environment.apiBaseUrl;

    // ************************** Properties ***************************

    // ************************ Public Methods *************************

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

    async addProgressEvent(studentId: string, goalId: string, content: string): Promise<ApiResult> {
        try {
            const result = await firstValueFrom(
                this.http.post<ResponseResult<void>>(`${this.base}/api/Student/${studentId}/progress-event`, { goalId, content })
            );
            return result.success
                ? ApiResult.empty()
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }

    // ************************ Event Handlers *************************

    // ********************** Support Procedures ***********************
}
