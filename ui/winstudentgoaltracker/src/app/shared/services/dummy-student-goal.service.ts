import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

// *****************************************************************
// TODO: This dummy service should be replaced by StudentGoalService,
// which will fetch real data from the API.
// *****************************************************************

export interface StudentGoalSummary {
    studentIdentifier: string;   // student.identifier — varchar(50)
    goals: StudentGoalItem[];
}

export interface StudentGoalItem {
    goalId: string;              // goal.id_goal — char(36)
    title: string;               // goal.title — varchar(255)
    description: string;         // goal.description — text
    category: string;            // goal.category — varchar(100)
    progressEventCount: number;  // count of progress_event rows for this goal
}

@Injectable({
    providedIn: 'root',
})
export class DummyStudentGoalService {

    // ************************** Constructor **************************

    // ************************** Declarations *************************

    // *****************************************************************
    // TODO: DUMMY DATA — Maps studentId to identifier and goals.
    // Replace with StudentGoalService calling
    // GET /api/students/:id/goals
    // *****************************************************************
    private readonly data: Record<string, StudentGoalSummary> = {
        '1': {
            studentIdentifier: 'J.B',
            goals: [
                { goalId: 'g1', title: 'Improve reading comprehension', description: 'Work on main-idea identification and inference skills across fiction and nonfiction texts.', category: 'Academics', progressEventCount: 5 },
                { goalId: 'g2', title: 'Complete algebra module', description: 'Finish all units in the algebra course including linear equations and graphing.', category: 'Academics', progressEventCount: 2 },
                { goalId: 'g3', title: 'Weekly journal entries', description: 'Write a reflective journal entry each week to build writing fluency.', category: 'Communication', progressEventCount: 8 },
            ],
        },
        '2': {
            studentIdentifier: 'M.K',
            goals: [
                { goalId: 'g4', title: 'Pass certification exam', description: 'Prepare for and pass the industry certification exam by end of quarter.', category: 'Career Readiness', progressEventCount: 3 },
                { goalId: 'g5', title: 'Attendance above 90%', description: 'Maintain consistent attendance throughout the term.', category: 'Behavior', progressEventCount: 0 },
                { goalId: 'g6', title: 'Complete internship hours', description: 'Log the required 40 hours at the assigned internship site.', category: 'Career Readiness', progressEventCount: 12 },
                { goalId: 'g7', title: 'Portfolio project', description: 'Build a personal portfolio showcasing completed coursework and projects.', category: 'Career Readiness', progressEventCount: 1 },
            ],
        },
        '3': {
            studentIdentifier: 'A.R',
            goals: [
                { goalId: 'g8', title: 'GED preparation', description: 'Complete practice tests and study modules for GED math and reading sections.', category: 'Academics', progressEventCount: 6 },
                { goalId: 'g9', title: 'Resume workshop', description: 'Attend the resume writing workshop and produce a final draft.', category: 'Career Readiness', progressEventCount: 0 },
            ],
        },
        '4': {
            studentIdentifier: 'T.W',
            goals: [
                { goalId: 'g10', title: 'Public speaking practice', description: 'Present in front of the class at least once per month.', category: 'Communication', progressEventCount: 4 },
                { goalId: 'g11', title: 'Math placement improvement', description: 'Move up one placement level in math by the end of the semester.', category: 'Academics', progressEventCount: 7 },
                { goalId: 'g12', title: 'Conflict resolution strategies', description: 'Learn and apply at least three de-escalation techniques.', category: 'Behavior', progressEventCount: 2 },
                { goalId: 'g13', title: 'Daily attendance streak', description: 'Achieve a 30-day unbroken attendance streak.', category: 'Behavior', progressEventCount: 0 },
                { goalId: 'g14', title: 'Job shadow experience', description: 'Complete a job shadow day in a field of interest.', category: 'Career Readiness', progressEventCount: 1 },
            ],
        },
        '5': {
            studentIdentifier: 'L.C',
            goals: [
                { goalId: 'g15', title: 'Improve typing speed', description: 'Reach 40 WPM with 95% accuracy on typing assessments.', category: 'Career Readiness', progressEventCount: 3 },
            ],
        },
    };

    // ************************** Properties ***************************

    // ************************ Public Methods *************************

    // *****************************************************************
    // Returns the student's identifier and their list of goals,
    // given a student ID.
    // *****************************************************************
    getGoalsForStudent(studentId: string): Observable<StudentGoalSummary | null> {
        return of(this.data[studentId] ?? null);
    }

    // ************************ Event Handlers *************************

    // ********************** Support Procedures ***********************
}
