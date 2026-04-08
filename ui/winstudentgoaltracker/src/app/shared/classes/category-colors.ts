export interface CategoryColor {
    bg: string;
    border: string;
    text: string;
    accent: string;
}

/** Single color used for all goals, regardless of category. */
export const GOAL_COLOR: CategoryColor = {
    bg: '#EEF2FF',
    border: '#818CF8',
    text: '#4338CA',
    accent: '#6366F1',
};
