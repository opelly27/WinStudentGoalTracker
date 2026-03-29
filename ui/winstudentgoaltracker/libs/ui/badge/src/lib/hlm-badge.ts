import { Directive, input } from '@angular/core';
import { classes } from '@spartan-ng/helm/utils';
import { type VariantProps, cva } from 'class-variance-authority';

const badgeVariants = cva(
	'focus-visible:border-ring focus-visible:ring-ring/50 group/badge inline-flex h-5 w-fit shrink-0 items-center justify-center gap-1 overflow-hidden rounded-full border border-transparent px-2 py-0.5 text-xs font-medium whitespace-nowrap transition-all focus-visible:ring-[3px]',
	{
		variants: {
			variant: {
				default: 'bg-primary text-primary-foreground',
				secondary: 'bg-secondary text-secondary-foreground',
				destructive: 'bg-destructive/10 text-destructive',
				outline: 'border-border text-foreground',
				ghost: 'hover:bg-muted hover:text-muted-foreground',
			},
		},
		defaultVariants: {
			variant: 'default',
		},
	},
);

export type BadgeVariants = VariantProps<typeof badgeVariants>;

@Directive({
	selector: '[hlmBadge],hlm-badge',
	host: {
		'data-slot': 'badge',
		'[attr.data-variant]': 'variant()',
	},
})
export class HlmBadge {
	public readonly variant = input<BadgeVariants['variant']>('default');

	constructor() {
		classes(() => badgeVariants({ variant: this.variant() }));
	}
}
