import { computed, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';

export type FormFactor = 'mobile' | 'desktop';

@Injectable({
    providedIn: 'root',
})
export class PlatformService {

    // ************************** Constructor **************************

    private readonly router = inject(Router);

    // ************************** Declarations *************************

    // *****************************************************************
    // Checks if the device uses a touch screen (like a phone or tablet)
    // rather than a mouse or trackpad.
    // *****************************************************************
    private readonly isCoarsePointer =
        typeof window !== 'undefined' && window.matchMedia('(pointer: coarse)').matches;

    // *****************************************************************
    // Captures the screen width and height so we can figure out what
    // kind of device the user is on. Defaults to a large desktop size
    // if running on the server.
    // *****************************************************************
    private readonly screenWidth =
        typeof window !== 'undefined' ? window.innerWidth : 1920;

    private readonly screenHeight =
        typeof window !== 'undefined' ? window.innerHeight : 1080;

    // *****************************************************************
    // The shortest and longest edges of the screen, regardless of
    // whether the device is held in portrait or landscape.
    // *****************************************************************
    private readonly minDimension = Math.min(this.screenWidth, this.screenHeight);
    private readonly maxDimension = Math.max(this.screenWidth, this.screenHeight);

    // *****************************************************************
    // Lets the user (or the app) manually force mobile or desktop mode
    // instead of relying on automatic detection. When set to null, the
    // app just figures it out on its own.
    // *****************************************************************
    private readonly formFactorOverride = signal<FormFactor | null>(null);

    // ************************** Properties ***************************

    // *****************************************************************
    // The final answer: are we showing the "mobile" or "desktop"
    // experience? Uses the manual override if one was set, otherwise
    // auto-detects based on the device hardware.
    // *****************************************************************
    readonly formFactor = computed<FormFactor>(() =>
        this.formFactorOverride() ?? this.resolveFormFactor(),
    );

    // *****************************************************************
    // True when the current mode was manually chosen by the user,
    // false when it was detected automatically.
    // *****************************************************************
    readonly isOverridden = computed(() => this.formFactorOverride() !== null);

    // ************************ Public Methods *************************

    // *****************************************************************
    // Switches between mobile and desktop mode on the fly. After
    // switching, the page reloads so the correct layout appears
    // immediately.
    // *****************************************************************
    switchTo(target: FormFactor): void {
        this.formFactorOverride.set(target);
        this.router.navigateByUrl(this.router.url);
    }

    // *****************************************************************
    // Clears any manual override and goes back to letting the device
    // decide which mode to show.
    // *****************************************************************
    resetToAuto(): void {
        this.formFactorOverride.set(null);
        this.router.navigateByUrl(this.router.url);
    }

    // ************************ Event Handlers *************************

    // ********************** Support Procedures ***********************

    // *****************************************************************
    // The rules for deciding whether a device gets the mobile or
    // desktop experience:
    //   - Mouse/trackpad users always get desktop
    //   - Large tablets (like iPad Pro) get desktop
    //   - Medium tablets held sideways get desktop
    //   - Everything else (phones, small tablets) gets mobile
    // *****************************************************************
    private resolveFormFactor(): FormFactor {
        if (!this.isCoarsePointer) return 'desktop';

        if (this.minDimension >= 1024) return 'desktop';

        if (this.maxDimension >= 1180 && this.screenWidth > this.screenHeight) {
            return 'desktop';
        }

        return 'mobile';
    }
}
