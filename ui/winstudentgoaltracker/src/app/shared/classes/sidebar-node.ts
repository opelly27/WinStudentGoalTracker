export interface SidebarNode {
    label: string;
    routerLink?: string[];
    children?: SidebarNode[];
    expanded?: boolean;
    loadChildren?: () => Promise<SidebarNode[]>;
    childCount?: number;
}
