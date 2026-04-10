import { Component, inject, signal, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { StudentService } from '../../../shared/services/student.service';
import { ReportPromptService } from '../../../shared/services/report-prompt.service';
import { StudentCardDto } from '../../../shared/classes/student-card.dto';
import { toIsoDateString } from '../../../shared/utils/format-date';

interface GoalCheckItem {
  goalId: string;
  category: string;
  checked: boolean;
}

@Component({
  selector: 'app-student-progress-report',
  imports: [FormsModule],
  templateUrl: './student-progress-report.html',
  styleUrl: './student-progress-report.scss',
})
export class StudentProgressReport implements OnDestroy {

  // ************************** Constructor **************************

  constructor() {
    this.loadStudents();
    this.loadPrompt();
  }

  // ************************** Declarations *************************

  private readonly router = inject(Router);
  private readonly studentService = inject(StudentService);
  private readonly reportPromptService = inject(ReportPromptService);
  protected readonly students = signal<StudentCardDto[]>([]);
  protected readonly goalItems = signal<GoalCheckItem[]>([]);
  protected readonly running = signal(false);
  protected readonly promptSaved = signal(false);
  protected selectedStudentId = '';
  protected fromDate = '';
  protected toDate = '';
  protected promptText = '';
  private promptId = '';
  private debounceTimer: ReturnType<typeof setTimeout> | null = null;
  private savedTimer: ReturnType<typeof setTimeout> | null = null;

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  ngOnDestroy() {
    if (this.debounceTimer) clearTimeout(this.debounceTimer);
    if (this.savedTimer) clearTimeout(this.savedTimer);
  }

  // ************************ Event Handlers *************************

  onBack() {
    this.router.navigate(['/reports']);
  }

  // *****************************************************************
  // Handles student dropdown changes. Reads firstEntryDate and
  // lastEntryDate from student data, and loads goals for the
  // checklist with all items checked by default.
  // *****************************************************************
  async onStudentChange() {
    this.fromDate = '';
    this.toDate = '';
    this.goalItems.set([]);

    if (!this.selectedStudentId) return;

    const student = this.students().find(s => s.studentId === this.selectedStudentId);
    if (!student) return;

    if (student.firstEntryDate) {
      this.fromDate = toIsoDateString(new Date(student.firstEntryDate));
    }
    if (student.lastEntryDate) {
      this.toDate = toIsoDateString(new Date(student.lastEntryDate));
    }

    const goalsResult = await this.studentService.getGoalsForStudent(this.selectedStudentId);
    if (goalsResult.success && goalsResult.payload) {
      this.goalItems.set(goalsResult.payload.goals.map(g => ({
        goalId: g.goalId,
        category: g.category ?? '',
        checked: true,
      })));
    }
  }

  // *****************************************************************
  // Toggles a goal checkbox on or off.
  // *****************************************************************
  onToggleGoal(goalId: string) {
    this.goalItems.update(items =>
      items.map(g => g.goalId === goalId ? { ...g, checked: !g.checked } : g)
    );
  }

  // *****************************************************************
  // Debounces prompt changes and auto-saves after 1 second of
  // inactivity. Shows a brief "Saved" indicator on success.
  // *****************************************************************
  onPromptChange() {
    this.promptSaved.set(false);
    if (this.debounceTimer) clearTimeout(this.debounceTimer);

    if (!this.promptId) return;

    this.debounceTimer = setTimeout(async () => {
      const result = await this.reportPromptService.updatePrompt(this.promptId, this.promptText);
      if (result.success) {
        this.promptSaved.set(true);
        if (this.savedTimer) clearTimeout(this.savedTimer);
        this.savedTimer = setTimeout(() => this.promptSaved.set(false), 3000);
      }
    }, 1000);
  }

  // *****************************************************************
  // Calls the API to generate the markdown report, passing only
  // the checked goal IDs, and triggers a browser download.
  // *****************************************************************
  async onRun() {
    this.running.set(true);
    try {
      const checkedGoalIds = this.goalItems()
        .filter(g => g.checked)
        .map(g => g.goalId)
        .join(',');

      const result = await this.studentService.getStudentProgressReport(
        this.selectedStudentId, this.fromDate, this.toDate, checkedGoalIds || undefined
      );

      if (result.success && result.payload) {
        this.downloadMarkdown(result.payload);
      }
    } finally {
      this.running.set(false);
    }
  }

  // ********************** Support Procedures ***********************

  // *****************************************************************
  // Loads the list of students for the dropdown selector.
  // *****************************************************************
  private loadStudents() {
    this.studentService.getMyStudents().then(data => {
      if (data.success) {
        const sorted = (data.payload || []).sort((a, b) =>
          a.identifier.localeCompare(b.identifier, undefined, { sensitivity: 'base' })
        );
        this.students.set(sorted);
      }
    });
  }

  // *****************************************************************
  // Loads the prompt for 'progressreport' from the API.
  // *****************************************************************
  private async loadPrompt() {
    const result = await this.reportPromptService.getByReportname('progressreport');
    if (result.success && result.payload) {
      this.promptId = result.payload.reportPromptId;
      this.promptText = result.payload.prompt;
    } else {
      console.error('[loadPrompt] Failed to load prompt:', result.message);
    }
  }

  // *****************************************************************
  // Triggers a browser download of the given markdown content,
  // prepending the prompt text at the top of the file.
  // *****************************************************************
  private downloadMarkdown(content: string) {
    const student = this.students().find(s => s.studentId === this.selectedStudentId);
    const name = student ? student.identifier.replace(/\s+/g, '_') : 'report';
    const filename = `${name}_progress_report_${this.fromDate}_to_${this.toDate}.md`;

    // Prepend the prompt if one exists.
    let output = content;
    if (this.promptText.trim()) {
      output = this.promptText.trim() + '\n\n---\n\n' + content;
    }

    const blob = new Blob([output], { type: 'text/markdown;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  }
}
