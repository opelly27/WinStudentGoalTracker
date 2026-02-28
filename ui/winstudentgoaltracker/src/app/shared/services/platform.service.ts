import { computed, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';

export type FormFactor = 'mobile' | 'desktop';

@Injectable({
    providedIn: 'root',
})
export class PlatformService {
    private readonly router = inject(Router);

    // ──── Raw Hardware Signals ────

    private readonly isCoarsePointer =
        typeof window !== 'undefined' && window.matchMedia('(pointer: coarse)').matches;

    private readonly screenWidth =
        typeof window !== 'undefined' ? window.innerWidth : 1920;

    private readonly screenHeight =
        typeof window !== 'undefined' ? window.innerHeight : 1080;

    private readonly minDimension = Math.min(this.screenWidth, this.screenHeight);
    private readonly maxDimension = Math.max(this.screenWidth, this.screenHeight);

    // ──── Override Layer ────

    /** When non-null, overrides auto-detection. */
    private readonly formFactorOverride = signal<FormFactor | null>(null);

    // ──── Public API ────

    /** The resolved form factor — auto-detected or overridden. */
    readonly formFactor = computed<FormFactor>(() =>
        this.formFactorOverride() ?? this.resolveFormFactor(),
    );

    /** True when the user has manually toggled away from auto-detection. */
    readonly isOverridden = computed(() => this.formFactorOverride() !== null);

    /**
     * Switch to a specific form factor at runtime.
     * Forces a route re-evaluation so the user immediately sees the other experience.
     */
    switchTo(target: FormFactor): void {
        this.formFactorOverride.set(target);
        this.router.navigateByUrl(this.router.url);
    }

    /** Clear the override and return to auto-detected form factor. */
    resetToAuto(): void {
        this.formFactorOverride.set(null);
        this.router.navigateByUrl(this.router.url);
    }

    // ──── Device Classification Policy ────

    private resolveFormFactor(): FormFactor {
        // Non-touch devices are always desktop
        if (!this.isCoarsePointer) return 'desktop';

        // Large tablets (iPad Pro 12.9" portrait = 1024px) → desktop
        if (this.minDimension >= 1024) return 'desktop';

        // Medium tablets in landscape with sufficient width → desktop
        if (this.maxDimension >= 1180 && this.screenWidth > this.screenHeight) {
            return 'desktop';
        }

        // Phones and small/portrait tablets → mobile
        return 'mobile';
    }
}
