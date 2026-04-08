import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { StudentCard } from '../student-card/student-card';
import { AddStudentModal } from '../add-student-modal/add-student-modal';
import { DummyStudentService } from '../../../shared/services/dummy-student.service';
import { StudentCardDto } from '../../../shared/classes/student-card.dto';
import { StudentService } from '../../../shared/services/student.service';

export type DisplayMode = 'card' | 'list';

@Component({
  selector: 'app-student-card-list',
  imports: [StudentCard, AddStudentModal, RouterLink, DatePipe],
  templateUrl: './student-card-list.html',
  styleUrl: './student-card-list.scss',
})
export class StudentCardList {

  // ************************** Constructor **************************

  constructor() {
    this.loadStudents();
  }

  // ************************** Declarations *************************

  private readonly studentService = inject(StudentService);
  private readonly router = inject(Router);
  protected readonly students = signal<StudentCardDto[]>([]);
  protected readonly displayMode = signal<DisplayMode>('card');
  protected readonly showAddModal = signal(false);
  protected readonly loaded = signal(false);
  protected readonly showAll = signal(false);

  public errorMessage = signal<String | null>(null);

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  setDisplayMode(mode: DisplayMode) {
    this.displayMode.set(mode);
  }

  onAddStudent() {
    this.showAddModal.set(true);
  }

  // *****************************************************************
  // Toggles between "My Students" and "All Students" scope, then
  // reloads the student list from the API with the new scope.
  // *****************************************************************
  onToggleScope() {
    this.showAll.update(v => !v);
    this.loaded.set(false);
    this.loadStudents();
    this.studentService.notifyDataChanged();
  }

  onStudentCreated(student: StudentCardDto) {
    this.students.update(list => this.sortByIdentifier([...list, student]));
    this.showAddModal.set(false);
    this.studentService.notifyDataChanged();
    this.router.navigate(['/students', student.studentId]);
  }

  onModalCancelled() {
    this.showAddModal.set(false);
  }

  // ********************** Support Procedures ***********************

  // *****************************************************************
  // Sorts an array of students alphabetically by identifier.
  // *****************************************************************
  private sortByIdentifier(students: StudentCardDto[]): StudentCardDto[] {
    return students.sort((a, b) =>
      a.identifier.localeCompare(b.identifier, undefined, { sensitivity: 'base' })
    );
  }

  // *****************************************************************
  // Loads students from the service and populates the students signal.
  // Uses scope 'all' when the toggle is active.
  // *****************************************************************
  private loadStudents() {
    const scope = this.showAll() ? 'all' : undefined;
    this.studentService.getMyStudents(scope).then(data => {

      if (!data.success) {
        this.errorMessage.set(data.message);
      }
      else {
        this.students.set(this.sortByIdentifier(data.payload || []))
      }

      this.loaded.set(true);
    });
  }
}
