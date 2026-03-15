export interface StudentGoalSummary {
    studentIdentifier: string;   // student.identifier — varchar(50)
    goals: StudentGoalItem[];
}

export interface StudentGoalItem {
    goalId: string;              // goal.id_goal — char(36)
    goalParentId: string | null;
    description: string;         // goal.description — text
    category: string;            // goal.category — varchar(100)
    baseline: string;            // goal.baseline — text
    targetCompletionDate: string | null;  // goal.target_completion_date — date
    closeDate: string | null;    // goal.close_date — date
    achieved: boolean | null;    // goal.achieved — tinyint(1), null until closed
    closeNotes: string | null;   // goal.close_notes — text
    progressEventCount: number;  // count of progress_event rows for this goal
    benchmarkCount: number;      // count of benchmark rows for this goal
}
