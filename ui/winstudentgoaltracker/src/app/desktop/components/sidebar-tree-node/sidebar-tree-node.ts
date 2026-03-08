import { Component, input } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { SidebarNode } from '../../../shared/classes/sidebar-node';

@Component({
    selector: 'app-sidebar-tree-node',
    imports: [RouterLink, RouterLinkActive, SidebarTreeNode],
    templateUrl: './sidebar-tree-node.html',
    styleUrl: './sidebar-tree-node.scss',
})
export class SidebarTreeNode {

    // ************************** Constructor **************************

    // ************************** Declarations *************************

    readonly nodes = input.required<SidebarNode[]>();
    readonly depth = input<number>(0);

    // ************************** Properties ***************************

    // *****************************************************************
    // Computed indentation in rem based on depth.
    // *****************************************************************
    indent(): string {
        return (1 + this.depth() * 1.5) + 'rem';
    }

    // ************************ Public Methods *************************

    // *****************************************************************
    // Returns true if a node should show the +/- toggle.
    // A node is expandable if it has static children, or has a
    // loadChildren function with a non-zero childCount.
    // *****************************************************************
    hasToggle(node: SidebarNode): boolean {
        if (node.children && node.children.length > 0) return true;
        if (node.loadChildren && node.childCount !== 0) return true;
        return false;
    }

    // ************************ Event Handlers *************************

    // *****************************************************************
    // Toggles a node's expanded state. On first expand of a lazy node,
    // calls loadChildren and caches the result in node.children.
    // *****************************************************************
    async onToggle(node: SidebarNode, event: Event) {
        if (!this.hasToggle(node)) return;

        event.preventDefault();
        event.stopPropagation();

        if (node.expanded) {
            node.expanded = false;
            return;
        }

        if (node.loadChildren && !node.children) {
            node.children = await node.loadChildren();
        }

        node.expanded = true;
    }

    // ********************** Support Procedures ***********************
}
