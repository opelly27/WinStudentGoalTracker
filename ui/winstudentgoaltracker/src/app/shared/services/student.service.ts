import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { StudentCardDto } from '../models/dto/student-card.dto';

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
    getStudentCards(): Observable<StudentCardDto[]> {
        return of([
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
        ]);
    }

    // ************************ Event Handlers *************************

    // ********************** Support Procedures ***********************
}
