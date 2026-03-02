import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ApiResult } from '../classes/api-result';

// *****************************************************************
// TODO: This dummy service should be replaced by SaveProgressEvent,
// which will POST real data to the API.
// *****************************************************************

@Injectable({
    providedIn: 'root',
})
export class DummySaveProgressEvent {

    // ************************** Constructor **************************

    // ************************** Declarations *************************

    // ************************** Properties ***************************

    // ************************ Public Methods *************************

    // *****************************************************************
    // TODO: DUMMY — Always returns success. Replace with
    // SaveProgressEvent calling POST /api/progress-events
    // *****************************************************************
    async save(studentId: string, goalId: string, content: string): Promise<ApiResult> {
        return ApiResult.empty();
    }

    // ************************ Event Handlers *************************

    // ********************** Support Procedures ***********************
}
