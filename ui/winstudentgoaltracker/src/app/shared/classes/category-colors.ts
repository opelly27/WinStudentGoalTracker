export interface CategoryColor {
    bg: string;
    border: string;
    text: string;
    accent: string;
}

const CATEGORY_COLORS: Record<string, CategoryColor> = {
    Reading:  { bg: '#EEF2FF', border: '#818CF8', text: '#4338CA', accent: '#6366F1' },
    Math:     { bg: '#FFF7ED', border: '#FB923C', text: '#C2410C', accent: '#F97316' },
    Writing:  { bg: '#F0FDF4', border: '#4ADE80', text: '#15803D', accent: '#22C55E' },
    Behavior: { bg: '#FDF4FF', border: '#C084FC', text: '#7E22CE', accent: '#A855F7' },
    Speech:   { bg: '#FFF1F2', border: '#FB7185', text: '#BE123C', accent: '#F43F5E' },
};

const DEFAULT_COLOR: CategoryColor = {
    bg: '#F5F5F0', border: '#A0A090', text: '#444', accent: '#666',
};

export function getCategoryColor(category: string): CategoryColor {
    return CATEGORY_COLORS[category] ?? DEFAULT_COLOR;
}
