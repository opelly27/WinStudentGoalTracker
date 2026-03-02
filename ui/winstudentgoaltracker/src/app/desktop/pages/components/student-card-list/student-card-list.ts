import { Component, inject, signal } from '@angular/core';
import { StudentCard } from '../student-card/student-card';
import { StudentService } from '../../../../shared/services/dummy-student.service';
import { StudentCardDto } from '../../../../shared/classes/student-card.dto';

export type DisplayMode = 'card' | 'list';

@Component({
  selector: 'app-student-card-list',
  imports: [StudentCard],
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

  public errorMessage = signal<String | null>(null);

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  onAddStudent() {
    // TODO: navigate to add-student form
  }

  // ********************** Support Procedures ***********************

  // *****************************************************************
  // Loads students from the service and populates the students signal.
  // *****************************************************************
  private loadStudents() {
    this.studentService.getStudentCards().then(data => {
      
      if(!data.success)
      {
        this.errorMessage.set(data.message);
      }
      else
      {
        this.students.set(data.payload || [])
      }

    });
  }
}
