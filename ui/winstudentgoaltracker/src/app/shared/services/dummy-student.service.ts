import { Injectable } from '@angular/core';
import { StudentCardDto } from '../classes/student-card.dto';
import { ApiResult } from '../classes/api-result';
import { StudentGoalSummary } from '../classes/student-goal';

@Injectable({
    providedIn: 'root',
})
export class DummyStudentService {

    // ************************** Constructor **************************

    // ************************** Declarations *************************

    private readonly data: Record<string, StudentGoalSummary> = {
        '1': {
            studentIdentifier: 'J.B',
            goals: [
                { goalId: 'g1', goalParentId: null, title: 'Improve reading comprehension', description: 'Work on main-idea identification and inference skills across fiction and nonfiction texts.', category: 'Academics', progressEventCount: 5 },
                { goalId: 'g2', goalParentId: null, title: 'Complete algebra module', description: 'Finish all units in the algebra course including linear equations and graphing.', category: 'Academics', progressEventCount: 2 },
                { goalId: 'g3', goalParentId: null,  title: 'Weekly journal entries', description: 'Write a reflective journal entry each week to build writing fluency.', category: 'Communication', progressEventCount: 8 },
            ],
        },
        '2': {
            studentIdentifier: 'M.K',
            goals: [
                { goalId: 'g4', goalParentId: null,  title: 'Pass certification exam', description: 'Prepare for and pass the industry certification exam by end of quarter.', category: 'Career Readiness', progressEventCount: 3 },
                { goalId: 'g5', goalParentId: null,  title: 'Attendance above 90%', description: 'Maintain consistent attendance throughout the term.', category: 'Behavior', progressEventCount: 0 },
                { goalId: 'g6', goalParentId: null,  title: 'Complete internship hours', description: 'Log the required 40 hours at the assigned internship site.', category: 'Career Readiness', progressEventCount: 12 },
                { goalId: 'g7', goalParentId: null,  title: 'Portfolio project', description: 'Build a personal portfolio showcasing completed coursework and projects.', category: 'Career Readiness', progressEventCount: 1 },
            ],
        },
        '3': {
            studentIdentifier: 'A.R',
            goals: [
                { goalId: 'g8', goalParentId: null,  title: 'GED preparation', description: 'Complete practice tests and study modules for GED math and reading sections.', category: 'Academics', progressEventCount: 6 },
                { goalId: 'g9', goalParentId: null,  title: 'Resume workshop', description: 'Attend the resume writing workshop and produce a final draft.', category: 'Career Readiness', progressEventCount: 0 },
            ],
        },
        '4': {
            studentIdentifier: 'T.W',
            goals: [
                { goalId: 'g10', goalParentId: null,  title: 'Public speaking practice', description: 'Present in front of the class at least once per month.', category: 'Communication', progressEventCount: 4 },
                { goalId: 'g11', goalParentId: null,  title: 'Math placement improvement', description: 'Move up one placement level in math by the end of the semester.', category: 'Academics', progressEventCount: 7 },
                { goalId: 'g12', goalParentId: null,  title: 'Conflict resolution strategies', description: 'Learn and apply at least three de-escalation techniques.', category: 'Behavior', progressEventCount: 2 },
                { goalId: 'g13', goalParentId: null,  title: 'Daily attendance streak', description: 'Achieve a 30-day unbroken attendance streak.', category: 'Behavior', progressEventCount: 0 },
                { goalId: 'g14', goalParentId: null,  title: 'Job shadow experience', description: 'Complete a job shadow day in a field of interest.', category: 'Career Readiness', progressEventCount: 1 },
            ],
        },
        '5': {
            studentIdentifier: 'L.C',
            goals: [
                { goalId: 'g15', goalParentId: null,  title: 'Improve typing speed', description: 'Reach 40 WPM with 95% accuracy on typing assessments.', category: 'Career Readiness', progressEventCount: 3 },
            ],
        },
    };

    // ************************** Properties ***************************

    // ************************ Public Methods *************************

    // *****************************************************************
    // Returns student card summaries. Currently returns dummy data
    // until the API endpoint is available.
    // *****************************************************************
    async getMyStudents(): Promise<ApiResult<StudentCardDto[]>> {
        var payload =  [
            {
                studentId: '1',
                identifier: 'J.B',
                expectedGradDate: new Date('2027-02-27'),
                lastEntryDate: new Date('2026-02-21'),
                goalCount: 3,
                progressEventCount: 5,
            },
            {
                studentId: '2',
                identifier: 'M.K',
                expectedGradDate: new Date('2027-02-27'),
                lastEntryDate: new Date('2026-02-25'),
                goalCount: 4,
                progressEventCount: 8,
            },
            {
                studentId: '3',
                identifier: 'A.R',
                expectedGradDate: new Date('2027-02-27'),
                lastEntryDate: null,
                goalCount: 2,
                progressEventCount: 0,
            },
        ];

        return ApiResult.ok(payload);
    }

    async getGoalsForStudent(studentId: string): Promise<ApiResult<StudentGoalSummary | null>> {
        var goals = this.data[studentId] ?? null;
        if (goals === null)
        {
            return ApiResult.fail('Student not found');
        }

        return ApiResult.ok(goals);

    }

    async addProgressEvent(studentId: string, goalId: string, content: string): Promise<ApiResult> {
        return ApiResult.empty();
    }


    // ************************ Event Handlers *************************

    // ********************** Support Procedures ***********************
}
