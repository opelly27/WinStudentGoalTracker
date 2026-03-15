export interface CreateGoalDto {
    description: string;
    category: string;
    baseline: string;
    goalParentId: string | null;
    targetCompletionDate: string | null;
}
