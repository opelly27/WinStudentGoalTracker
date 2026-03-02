import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { StudentCardDto } from '../classes/student-card.dto';
import { ApiResult } from '../classes/api-result';

@Injectable({
    providedIn: 'root',
})
export class StudentService {

    // ************************** Constructor **************************

    // ************************** Declarations *************************

    // ************************** Properties ***************************

    // ************************ Public Methods *************************

    // *****************************************************************
    // Returns student card summaries. Currently returns dummy data
    // until the API endpoint is available.
    // *****************************************************************
    async getStudentCards(): Promise<ApiResult<StudentCardDto[]>> {
        var payload =  [
            {
                studentId: '1',
                identifier: 'J.B',
                age: 21,
                lastEntryDate: '2026-02-21',
                goalCount: 3,
                progressEventCount: 5,
            },
            {
                studentId: '2',
                identifier: 'M.K',
                age: 19,
                lastEntryDate: '2026-02-25',
                goalCount: 4,
                progressEventCount: 8,
            },
            {
                studentId: '3',
                identifier: 'A.R',
                age: 22,
                lastEntryDate: null,
                goalCount: 2,
                progressEventCount: 0,
            },
        ];

        return ApiResult.ok(payload);
    }

    // *****************************************************************
    // TODO: DUMMY DATA — Replace with getStudentsPerUser, which will
    // call GET /api/users/:id/students to return real data.
    // Returns students assigned to the current user with their
    // identifier, age, goal count, and progress event count.
    // *****************************************************************
    async getDummyStudentsForUser(): Promise<ApiResult<StudentCardDto[]>> {
        var payload = [
            { studentId: '1', identifier: 'J.B', age: 21, lastEntryDate: '2026-02-21', goalCount: 3, progressEventCount: 5 },
            { studentId: '2', identifier: 'M.K', age: 19, lastEntryDate: '2026-02-25', goalCount: 4, progressEventCount: 8 },
            { studentId: '3', identifier: 'A.R', age: 22, lastEntryDate: null, goalCount: 2, progressEventCount: 0 },
            { studentId: '4', identifier: 'T.W', age: 20, lastEntryDate: '2026-02-18', goalCount: 5, progressEventCount: 12 },
            { studentId: '5', identifier: 'L.C', age: 18, lastEntryDate: '2026-02-27', goalCount: 1, progressEventCount: 2 },
        ];

        return ApiResult.ok(payload);
    }

    // ************************ Event Handlers *************************

    // ********************** Support Procedures ***********************
}
