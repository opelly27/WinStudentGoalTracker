import { Component, input } from '@angular/core';

@Component({
    selector: 'app-edit-icon',
    imports: [],
    template: `
        <button class="edit-icon" [attr.aria-label]="ariaLabel()">
            <svg [attr.width]="size()" [attr.height]="size()" viewBox="0 0 16 16" fill="none" [attr.stroke]="color()" stroke-width="1.5"
                stroke-linecap="round" stroke-linejoin="round">
                <path d="M11.5 1.5l3 3L5 14H2v-3L11.5 1.5z" />
            </svg>
        </button>
    `,
    styles: [`
        .edit-icon {
            background: none;
            border: none;
            cursor: pointer;
            padding: 2px;
            flex-shrink: 0;
            display: flex;
            align-items: center;
        }
        .edit-icon:hover svg {
            stroke: #555 !important; /* Force hover color since original styles did the same */
        }
        :host-context(.student-item) .edit-icon {
            opacity: 0;
            transition: opacity 0.15s ease;
        }
        :host-context(.student-item:hover) .edit-icon {
            opacity: 1;
        }
    `]
})
export class EditIcon {
    readonly size = input<number | string>(14);
    readonly color = input<string>('#999');
    readonly ariaLabel = input<string>('Edit');
}
