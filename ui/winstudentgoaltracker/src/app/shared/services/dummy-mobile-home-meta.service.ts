import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ApiResult } from '../classes/api-result';

// *****************************************************************
// TODO: This dummy service should be replaced by MobileHomeMeta,
// which will fetch real data from the API.
// *****************************************************************

export interface MobileHomeMeta {
    programName: string;   // program.name — varchar(255)
    userName: string;      // user.name — varchar(255)
}

@Injectable({
    providedIn: 'root',
})
export class DummyMobileHomeMeta {

    // ************************** Constructor **************************

    // ************************** Declarations *************************

    // ************************** Properties ***************************

    // ************************ Public Methods *************************

    // *****************************************************************
    // TODO: DUMMY DATA — Returns hardcoded program and user info.
    // Replace with MobileHomeMeta service that calls
    // GET /api/mobile/home-meta (or similar).
    // *****************************************************************
    async getMeta(): Promise<ApiResult<MobileHomeMeta | null>> {
        var payload = {
            programName: 'WIN Program',
            userName: 'Polly Balsillie',
        }

        return ApiResult.ok(payload);
    }

    // ************************ Event Handlers *************************

    // ********************** Support Procedures ***********************
}
