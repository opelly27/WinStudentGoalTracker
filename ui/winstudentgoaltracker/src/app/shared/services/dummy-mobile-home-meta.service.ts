import { Injectable } from '@angular/core';
import { ApiResult } from '../classes/api-result';
import { MobileHomeMeta } from '../classes/mobile-home-meta';

// *****************************************************************
// TODO: This dummy service should be replaced by MobileHomeMeta,
// which will fetch real data from the API.
// *****************************************************************

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
