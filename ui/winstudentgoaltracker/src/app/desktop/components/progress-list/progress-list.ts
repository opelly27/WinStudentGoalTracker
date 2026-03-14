import { Component, computed, inject, signal, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { ProgressItem } from '../progress-item/progress-item';
import { ProgressEventDto } from '../../../shared/classes/progress-event.dto';
import { StudentService } from '../../../shared/services/student.service';

@Component({
  selector: 'app-progress-list',
  imports: [ProgressItem],
  templateUrl: './progress-list.html',
  styleUrl: './progress-list.scss',
})
export class ProgressList implements OnDestroy {

  // ************************** Constructor **************************

  constructor() {
    this.studentId = this.route.snapshot.paramMap.get('studentId')!;
    this.goalId = this.route.snapshot.paramMap.get('goalId')!;
    this.loadEvents();
    this.loadGoalCategory();

    this.searchInput$.pipe(debounceTime(300)).subscribe(term => {
      this.searchTerm.set(term);
    });
  }

  // ************************** Declarations *************************

  private readonly studentService = inject(StudentService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  private readonly studentId: string;
  private readonly goalId: string;
  private readonly searchInput$ = new Subject<string>();

  protected readonly studentIdentifier = signal<string | null>(null);
  protected readonly goalCategory = signal<string | null>(null);
  protected readonly events = signal<ProgressEventDto[]>([]);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly rawSearchText = signal('');
  protected readonly searchTerm = signal('');
  protected readonly showAddModal = signal(false);

  // ************************** Properties ***************************

  // *****************************************************************
  // Returns events filtered by the debounced search term. Matches
  // against the event content (case-insensitive). Only filters when
  // the term is at least 2 characters.
  // *****************************************************************
  protected readonly filteredEvents = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    if (term.length < 2) return this.events();
    return this.events().filter(e => e.content.toLowerCase().includes(term));
  });

  protected readonly isFiltered = computed(() => {
    return this.searchTerm().trim().length >= 2 && this.filteredEvents().length !== this.events().length;
  });

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  onAddProgressEvent() {
    this.showAddModal.set(true);
    // TODO: Wire up add-progress-event modal component
  }

  // *****************************************************************
  // Navigates back to the parent goal detail.
  // *****************************************************************
  onBack() {
    this.router.navigate(['/students', this.studentId, 'goals', this.goalId]);
  }

  // *****************************************************************
  // Pushes the raw input value into the debounce stream.
  // *****************************************************************
  onSearchInput(value: string) {
    this.rawSearchText.set(value);
    this.searchInput$.next(value);
  }

  // *****************************************************************
  // Clears the search box and resets the filter.
  // *****************************************************************
  onClearSearch() {
    this.rawSearchText.set('');
    this.searchTerm.set('');
  }

  ngOnDestroy() {
    this.searchInput$.complete();
  }

  // ********************** Support Procedures ***********************

  // *****************************************************************
  // Loads progress events for the given goal from the API, sorted
  // newest-first by createdAt.
  // *****************************************************************
  private loadEvents() {
    this.studentService.getProgressEventsForGoal(this.goalId).then(result => {
      if (!result.success) {
        this.errorMessage.set(result.message);
      } else {
        const sorted = (result.payload ?? [])
          .slice()
          .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        this.events.set(sorted);
      }
    });
  }

  // *****************************************************************
  // Loads the goal category from the student's goal list so the heading
  // can display "Progress for <goal category>".
  // *****************************************************************
  private loadGoalCategory() {
    this.studentService.getGoalsForStudent(this.studentId).then(result => {
      if (!result.success || !result.payload) return;
      this.studentIdentifier.set(result.payload.studentIdentifier);
      const goal = result.payload.goals.find(g => g.goalId === this.goalId);
      this.goalCategory.set(goal?.category ?? null);
    });
  }
}
