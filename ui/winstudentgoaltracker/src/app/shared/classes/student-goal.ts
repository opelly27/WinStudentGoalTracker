export interface StudentGoalSummary {
    studentIdentifier: string;   // student.identifier — varchar(50)
    goals: StudentGoalItem[];
}

export interface StudentGoalItem {
    goalId: string;              // goal.id_goal — char(36)
    goalParentId: string | null;
    title: string;               // goal.title — varchar(255)
    description: string;         // goal.description — text
    category: string;            // goal.category — varchar(100)
    progressEventCount: number;  // count of progress_event rows for this goal
    benchmarkCount: number;      // count of benchmark rows for this goal
}
