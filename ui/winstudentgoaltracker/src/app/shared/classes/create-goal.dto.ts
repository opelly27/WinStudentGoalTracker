export interface CreateGoalDto {
    title: string;
    description: string;
    category: string;
    goalParentId: string | null;
}
