import { Injectable } from '@angular/core';
import { StudentCardDto } from '../classes/student-card.dto';
import { ApiResult } from '../classes/api-result';
import { StudentGoalSummary, StudentGoalItem } from '../classes/student-goal';
import { CreateGoalDto } from '../classes/create-goal.dto';
import { ProgressEventDto } from '../classes/progress-event.dto';
import { StudentBenchmarkSummary, BenchmarkDto } from '../classes/benchmark.dto';

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
                { goalId: 'g1', goalParentId: null, title: 'Improve reading comprehension', description: 'Work on main-idea identification and inference skills across fiction and nonfiction texts.', category: 'Academics', progressEventCount: 5, benchmarkCount: 2 },
                { goalId: 'g2', goalParentId: null, title: 'Complete algebra module', description: 'Finish all units in the algebra course including linear equations and graphing.', category: 'Academics', progressEventCount: 2, benchmarkCount: 0 },
                { goalId: 'g3', goalParentId: null, title: 'Weekly journal entries', description: 'Write a reflective journal entry each week to build writing fluency.', category: 'Communication', progressEventCount: 8, benchmarkCount: 1 },
            ],
        },
        '2': {
            studentIdentifier: 'M.K',
            goals: [
                { goalId: 'g4', goalParentId: null, title: 'Pass certification exam', description: 'Prepare for and pass the industry certification exam by end of quarter.', category: 'Career Readiness', progressEventCount: 3, benchmarkCount: 0 },
                { goalId: 'g5', goalParentId: null, title: 'Attendance above 90%', description: 'Maintain consistent attendance throughout the term.', category: 'Behavior', progressEventCount: 0, benchmarkCount: 0 },
                { goalId: 'g6', goalParentId: null, title: 'Complete internship hours', description: 'Log the required 40 hours at the assigned internship site.', category: 'Career Readiness', progressEventCount: 12, benchmarkCount: 0 },
                { goalId: 'g7', goalParentId: null, title: 'Portfolio project', description: 'Build a personal portfolio showcasing completed coursework and projects.', category: 'Career Readiness', progressEventCount: 1, benchmarkCount: 0 },
            ],
        },
        '3': {
            studentIdentifier: 'A.R',
            goals: [
                { goalId: 'g8', goalParentId: null, title: 'GED preparation', description: 'Complete practice tests and study modules for GED math and reading sections.', category: 'Academics', progressEventCount: 6, benchmarkCount: 0 },
                { goalId: 'g9', goalParentId: null, title: 'Resume workshop', description: 'Attend the resume writing workshop and produce a final draft.', category: 'Career Readiness', progressEventCount: 0, benchmarkCount: 0 },
            ],
        },
        '4': {
            studentIdentifier: 'T.W',
            goals: [
                { goalId: 'g10', goalParentId: null, title: 'Public speaking practice', description: 'Present in front of the class at least once per month.', category: 'Communication', progressEventCount: 4, benchmarkCount: 0 },
                { goalId: 'g11', goalParentId: null, title: 'Math placement improvement', description: 'Move up one placement level in math by the end of the semester.', category: 'Academics', progressEventCount: 7, benchmarkCount: 0 },
                { goalId: 'g12', goalParentId: null, title: 'Conflict resolution strategies', description: 'Learn and apply at least three de-escalation techniques.', category: 'Behavior', progressEventCount: 2, benchmarkCount: 0 },
                { goalId: 'g13', goalParentId: null, title: 'Daily attendance streak', description: 'Achieve a 30-day unbroken attendance streak.', category: 'Behavior', progressEventCount: 0, benchmarkCount: 0 },
                { goalId: 'g14', goalParentId: null, title: 'Job shadow experience', description: 'Complete a job shadow day in a field of interest.', category: 'Career Readiness', progressEventCount: 1, benchmarkCount: 0 },
            ],
        },
        '5': {
            studentIdentifier: 'L.C',
            goals: [
                { goalId: 'g15', goalParentId: null, title: 'Improve typing speed', description: 'Reach 40 WPM with 95% accuracy on typing assessments.', category: 'Career Readiness', progressEventCount: 3, benchmarkCount: 0 },
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
        var payload = [
            {
                studentId: '1',
                identifier: 'J.B',
                nextIepDate: new Date('2027-02-27'),
                lastEntryDate: new Date('2026-02-21'),
                goalCount: 3,
                progressEventCount: 5,
            },
            {
                studentId: '2',
                identifier: 'M.K',
                nextIepDate: new Date('2027-02-27'),
                lastEntryDate: new Date('2026-02-25'),
                goalCount: 4,
                progressEventCount: 8,
            },
            {
                studentId: '3',
                identifier: 'A.R',
                nextIepDate: new Date('2027-02-27'),
                lastEntryDate: null,
                goalCount: 2,
                progressEventCount: 0,
            },
        ];

        return ApiResult.ok(payload);
    }

    async getGoalsForStudent(studentId: string): Promise<ApiResult<StudentGoalSummary | null>> {
        var goals = this.data[studentId] ?? null;
        if (goals === null) {
            return ApiResult.fail('Student not found');
        }

        return ApiResult.ok(goals);

    }

    async createGoal(studentId: string, data: CreateGoalDto): Promise<ApiResult<StudentGoalItem>> {
        const student = this.data[studentId];
        if (!student) {
            return ApiResult.fail('Student not found');
        }

        const newGoal: StudentGoalItem = {
            goalId: `g${Date.now()}`,
            goalParentId: null,
            title: data.title,
            description: data.description,
            category: data.category,
            progressEventCount: 0,
            benchmarkCount: 0,
        };

        student.goals.push(newGoal);
        return ApiResult.ok(newGoal);
    }

    async addProgressEvent(studentId: string, goalId: string, content: string): Promise<ApiResult> {
        return ApiResult.empty();
    }

    // *****************************************************************
    // Returns hardcoded progress events for a given goal. The real
    // service will call the API with the goal ID.
    // TODO: Replace with actual API call
    // *****************************************************************
    async getProgressEventsForGoal(goalId: string): Promise<ApiResult<ProgressEventDto[]>> {
        const events: ProgressEventDto[] = [
            { progressEventId: 'pe1', content: 'Student demonstrated strong understanding of the topic during today\'s session. Completed all assigned exercises independently.', createdAt: new Date('2026-02-28T10:30:00'), createdByName: 'Sarah Johnson' },
            { progressEventId: 'pe2', content: 'Reviewed previous week\'s material. Student needed some additional guidance but showed improvement by end of session.', createdAt: new Date('2026-02-27T14:15:00'), createdByName: 'Mike Thompson' },
            { progressEventId: 'pe3', content: 'Initial assessment completed. Identified key areas for focused practice going forward.', createdAt: new Date('2026-02-26T09:00:00'), createdByName: 'Sarah Johnson' },
            { progressEventId: 'pe4', content: 'Practiced problem-solving strategies with real-world scenarios. Student engaged well and asked thoughtful questions.', createdAt: new Date('2026-02-25T11:00:00'), createdByName: 'Lisa Martinez' },
            { progressEventId: 'pe5', content: 'Worked on time management skills. Created a weekly planner and discussed prioritization techniques.', createdAt: new Date('2026-02-24T13:45:00'), createdByName: 'Mike Thompson' },
            { progressEventId: 'pe6', content: 'Student completed a timed practice exercise. Performance improved compared to last week.', createdAt: new Date('2026-02-21T10:00:00'), createdByName: 'Sarah Johnson' },
            { progressEventId: 'pe7', content: 'Discussed long-term objectives and broke them into smaller milestones. Student is motivated and on track.', createdAt: new Date('2026-02-20T15:30:00'), createdByName: 'Lisa Martinez' },
            { progressEventId: 'pe8', content: 'Reviewed feedback from previous assignment. Student made corrections independently with minimal prompting.', createdAt: new Date('2026-02-19T09:30:00'), createdByName: 'Mike Thompson' },
            { progressEventId: 'pe9', content: 'Collaborative session with peer group. Student contributed actively and helped explain concepts to others.', createdAt: new Date('2026-02-18T14:00:00'), createdByName: 'Sarah Johnson' },
            { progressEventId: 'pe10', content: 'Introduced new topic area. Student showed curiosity and took detailed notes for independent review.', createdAt: new Date('2026-02-14T11:15:00'), createdByName: 'Lisa Martinez' },
            { progressEventId: 'pe11', content: 'Student struggled with today\'s material but remained focused. Will revisit key concepts next session.', createdAt: new Date('2026-02-13T10:45:00'), createdByName: 'Mike Thompson' },
            { progressEventId: 'pe12', content: 'Follow-up on yesterday\'s challenging session. Student showed marked improvement after overnight reflection.', createdAt: new Date('2026-02-12T09:00:00'), createdByName: 'Mike Thompson' },
            { progressEventId: 'pe13', content: 'Mid-term progress check. Student is meeting expectations in most areas with room for growth in written expression.', createdAt: new Date('2026-02-11T13:00:00'), createdByName: 'Sarah Johnson' },
            { progressEventId: 'pe14', content: 'Worked through a series of practice problems. Accuracy rate was 85%, up from 70% two weeks ago.', createdAt: new Date('2026-02-10T10:30:00'), createdByName: 'Lisa Martinez' },
            { progressEventId: 'pe15', content: 'Student presented a short summary of recent learning to the group. Showed confidence and clarity.', createdAt: new Date('2026-02-07T14:30:00'), createdByName: 'Sarah Johnson' },
            { progressEventId: 'pe16', content: 'Explored supplementary resources together. Student identified two additional practice tools to use independently.', createdAt: new Date('2026-02-06T11:00:00'), createdByName: 'Mike Thompson' },
            { progressEventId: 'pe17', content: 'Reviewed study habits and discussed strategies for staying consistent. Student committed to a daily review routine.', createdAt: new Date('2026-02-05T09:15:00'), createdByName: 'Lisa Martinez' },
            { progressEventId: 'pe18', content: 'Hands-on activity session. Student completed the project ahead of schedule with strong attention to detail.', createdAt: new Date('2026-02-04T13:30:00'), createdByName: 'Sarah Johnson' },
            { progressEventId: 'pe19', content: 'Addressed gaps identified in the initial assessment. Student showed solid understanding of foundational concepts.', createdAt: new Date('2026-02-03T10:00:00'), createdByName: 'Mike Thompson' },
            { progressEventId: 'pe20', content: 'First session of the term. Established rapport and set expectations for the upcoming weeks.', createdAt: new Date('2026-01-31T09:00:00'), createdByName: 'Sarah Johnson' },
        ];

        return ApiResult.ok(events);
    }

    // *****************************************************************
    // Returns hardcoded benchmarks for a given student.
    // TODO: Replace with actual API call
    // *****************************************************************
    async getBenchmarksForStudent(studentId: string): Promise<ApiResult<StudentBenchmarkSummary | null>> {
        const studentGoals = this.data[studentId];
        if (!studentGoals) {
            return ApiResult.fail('Student not found');
        }

        const benchmarks: BenchmarkDto[] = [
            { benchmarkId: 'bm1', goalId: 'g1', goalTitle: 'Improve reading comprehension', benchmark: 'Student will identify the main idea of a grade-level nonfiction passage with 80% accuracy.', createdByName: 'Jane Smith', createdAt: new Date('2026-02-15'), updatedAt: null },
            { benchmarkId: 'bm2', goalId: 'g1', goalTitle: 'Improve reading comprehension', benchmark: 'Student will make at least two supported inferences per reading session.', createdByName: 'Jane Smith', createdAt: new Date('2026-02-16'), updatedAt: new Date('2026-02-20') },
            { benchmarkId: 'bm3', goalId: 'g3', goalTitle: 'Weekly journal entries', benchmark: 'Student will complete a minimum of one paragraph (5 sentences) per journal entry.', createdByName: 'John Doe', createdAt: new Date('2026-02-18'), updatedAt: null },
        ];

        return ApiResult.ok({
            studentIdentifier: studentGoals.studentIdentifier,
            benchmarks: benchmarks
        });
    }


    // ************************ Event Handlers *************************

    // ********************** Support Procedures ***********************
}
