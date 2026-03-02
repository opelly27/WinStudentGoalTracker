import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

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
    getMeta(): Observable<MobileHomeMeta> {
        return of({
            programName: 'WIN Program',
            userName: 'Polly Balsillie',
        });
    }

    // ************************ Event Handlers *************************

    // ********************** Support Procedures ***********************
}
