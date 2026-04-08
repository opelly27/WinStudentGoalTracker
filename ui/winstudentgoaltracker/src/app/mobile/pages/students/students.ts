import { Component, inject, signal } from '@angular/core';
import { StudentCard } from '../../components/student-card/student-card';
import { DummyStudentService } from '../../../shared/services/dummy-student.service';
import { StudentCardDto } from '../../../shared/classes/student-card.dto';
import { StudentService } from '../../../shared/services/student.service';

@Component({
  selector: 'app-students',
  imports: [StudentCard],
  templateUrl: './students.html',
  styleUrl: './students.scss',
})
export class Students {

  // ************************** Constructor **************************

  constructor() {
    this.loadStudents();
  }

  // ************************** Declarations *************************

  private readonly studentService = inject(StudentService);
  protected readonly students = signal<StudentCardDto[]>([]);
  protected readonly loaded = signal(false);
  protected readonly showAll = signal(false);

  public errorMessage = signal<String | null>(null);

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  // *****************************************************************
  // Toggles between "My Students" and "All Students" scope, then
  // reloads the student list from the API.
  // *****************************************************************
  onToggleScope() {
    this.showAll.update(v => !v);
    this.loaded.set(false);
    this.loadStudents();
  }

  // ********************** Support Procedures ***********************

  // *****************************************************************
  // Loads the list of students assigned to the current user.
  // *****************************************************************
  private loadStudents() {
    const scope = this.showAll() ? 'all' : undefined;
    this.studentService.getMyStudents(scope).then(data => {

      if (!data.success)
      {
        this.errorMessage.set(data.message);
      }
      else
      {
        const sorted = (data.payload || []).sort((a, b) =>
          a.identifier.localeCompare(b.identifier, undefined, { sensitivity: 'base' })
        );
        this.students.set(sorted);
        this.loaded.set(true);
      }
    });
  }
}
