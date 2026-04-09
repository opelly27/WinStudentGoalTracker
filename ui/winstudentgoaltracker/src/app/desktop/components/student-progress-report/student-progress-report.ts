import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { StudentService } from '../../../shared/services/student.service';
import { StudentCardDto } from '../../../shared/classes/student-card.dto';
import { StudentGoalItem } from '../../../shared/classes/student-goal';
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
export class StudentProgressReport {

  // ************************** Constructor **************************

  constructor() {
    this.loadStudents();
  }

  // ************************** Declarations *************************

  private readonly router = inject(Router);
  private readonly studentService = inject(StudentService);
  protected readonly students = signal<StudentCardDto[]>([]);
  protected readonly goalItems = signal<GoalCheckItem[]>([]);
  protected readonly running = signal(false);
  protected selectedStudentId = '';
  protected fromDate = '';
  protected toDate = '';

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

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
  // Triggers a browser download of the given markdown content.
  // *****************************************************************
  private downloadMarkdown(content: string) {
    const student = this.students().find(s => s.studentId === this.selectedStudentId);
    const name = student ? student.identifier.replace(/\s+/g, '_') : 'report';
    const filename = `${name}_progress_report_${this.fromDate}_to_${this.toDate}.md`;

    const blob = new Blob([content], { type: 'text/markdown;charset=utf-8' });
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
