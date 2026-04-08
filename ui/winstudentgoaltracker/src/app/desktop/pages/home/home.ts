import { Component, effect, inject, signal } from '@angular/core';
import { RouterLink, RouterOutlet, Router } from '@angular/router';
import { Auth } from '../../../shared/services/auth';
import { StudentService } from '../../../shared/services/student.service';
import { StudentCardDto } from '../../../shared/classes/student-card.dto';
import { AddStudentModal } from '../../components/add-student-modal/add-student-modal';
import { EditStudentModal } from '../../components/edit-student-modal/edit-student-modal';

@Component({
    selector: 'app-home',
    imports: [RouterOutlet, RouterLink, AddStudentModal, EditStudentModal],
    templateUrl: './home.html',
    styleUrl: './home.scss',
})
export class Home {

    // ************************** Constructor **************************

    constructor() {
        this.loadStudents();

        // Reload student list when data changes elsewhere.
        let initialized = false;
        effect(() => {
            this.studentService.dataVersion();
            if (initialized) {
                this.loadStudents();
            }
            initialized = true;
        });
    }

    // ************************** Declarations *************************

    protected readonly auth = inject(Auth);
    private readonly router = inject(Router);
    private readonly studentService = inject(StudentService);

    protected readonly students = signal<StudentCardDto[]>([]);
    protected readonly selectedStudentId = signal<string | null>(null);
    protected readonly showAll = signal(false);
    protected readonly showAddStudentModal = signal(false);
    protected readonly editingStudent = signal<StudentCardDto | null>(null);

    // ************************ Event Handlers *************************

    onSelectStudent(student: StudentCardDto) {
        this.selectedStudentId.set(student.studentId);
        this.router.navigate(['/students', student.studentId]);
    }

    onToggleScope() {
        this.showAll.update(v => !v);
        this.loadStudents();
    }

    onAddStudent() {
        this.showAddStudentModal.set(true);
    }

    onStudentCreated(student: StudentCardDto) {
        this.showAddStudentModal.set(false);
        this.studentService.notifyDataChanged();
        this.selectedStudentId.set(student.studentId);
        this.router.navigate(['/students', student.studentId]);
    }

    onEditStudent(student: StudentCardDto, event: Event) {
        event.stopPropagation();
        this.editingStudent.set(student);
    }

    onEditStudentSaved() {
        this.editingStudent.set(null);
        this.loadStudents();
    }

    onLogout() {
        this.auth.logout().subscribe();
        this.auth.forceLogout();
    }

    // ************************ Formatting Helpers **********************

    formatDate(d: Date | null): string {
        if (!d) return '';
        return new Date(d).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
    }

    // ********************** Support Procedures ***********************

    private loadStudents() {
        const scope = this.showAll() ? 'all' : undefined;
        this.studentService.getMyStudents(scope).then(data => {
            if (data.success) {
                const sorted = (data.payload || []).sort((a, b) =>
                    a.identifier.localeCompare(b.identifier, undefined, { sensitivity: 'base' })
                );
                this.students.set(sorted);
            }
        });
    }
}
