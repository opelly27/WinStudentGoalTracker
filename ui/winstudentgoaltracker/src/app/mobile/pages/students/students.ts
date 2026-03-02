import { Component, inject, signal } from '@angular/core';
import { StudentCard } from '../../components/student-card/student-card';
import { StudentService } from '../../../shared/services/student.service';
import { StudentCardDto } from '../../../shared/models/dto/student-card.dto';

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

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  // ********************** Support Procedures ***********************

  // *****************************************************************
  // Loads the list of students assigned to the current user.
  // *****************************************************************
  private loadStudents() {
    this.studentService.getDummyStudentsForUser().subscribe(data => {
      this.students.set(data);
    });
  }
}
