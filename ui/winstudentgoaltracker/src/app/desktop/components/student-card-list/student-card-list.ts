import { Component, inject, signal } from '@angular/core';
import { StudentCard } from '../student-card/student-card';
import { AddStudentModal } from '../add-student-modal/add-student-modal';
import { DummyStudentService } from '../../../shared/services/dummy-student.service';
import { StudentCardDto } from '../../../shared/classes/student-card.dto';
import { StudentService } from '../../../shared/services/student.service';

export type DisplayMode = 'card' | 'list';

@Component({
  selector: 'app-student-card-list',
  imports: [StudentCard, AddStudentModal],
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
  protected readonly students = signal<StudentCardDto[]>([]);
  protected readonly displayMode = signal<DisplayMode>('card');
  protected readonly showAddModal = signal(false);
  protected readonly loaded = signal(false);

  public errorMessage = signal<String | null>(null);

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  onAddStudent() {
    this.showAddModal.set(true);
  }

  onStudentCreated(student: StudentCardDto) {
    this.students.update(list => this.sortByIdentifier([...list, student]));
    this.showAddModal.set(false);
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
  // *****************************************************************
  private loadStudents() {
    this.studentService.getMyStudents().then(data => {

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
