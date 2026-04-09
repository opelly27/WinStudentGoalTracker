import { Component, computed, effect, inject, signal } from '@angular/core';
import { RouterLink, RouterOutlet, Router } from '@angular/router';
import { Auth } from '../../../shared/services/auth';
import { StudentService } from '../../../shared/services/student.service';
import { StudentCardDto } from '../../../shared/classes/student-card.dto';
import { StudentModal } from '../../components/student-modal/student-modal';
import { EditIcon } from '../../components/edit-icon/edit-icon';
import { formatDate } from '../../../shared/utils/format-date';

@Component({
    selector: 'app-home',
    imports: [RouterOutlet, RouterLink, StudentModal, EditIcon],
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
    protected readonly showStudentModal = signal<StudentCardDto | 'add' | null>(null);

    // Groups students by owner when "All" is active.
    protected readonly groupedStudents = computed(() => {
        const all = this.students();
        if (!this.showAll()) {
            return [{ label: null, students: all }];
        }

        const mine = all.filter(s => s.isMine);
        const others = all.filter(s => !s.isMine);

        // Group others by ownerName.
        const byOwner = new Map<string, StudentCardDto[]>();
        for (const s of others) {
            const key = s.ownerName || 'Unknown';
            const list = byOwner.get(key) || [];
            list.push(s);
            byOwner.set(key, list);
        }

        const groups: { label: string | null; students: StudentCardDto[] }[] = [];
        if (mine.length) {
            groups.push({ label: 'My Students', students: mine });
        }
        // Sort other teachers alphabetically.
        const sortedKeys = [...byOwner.keys()].sort((a, b) =>
            a.localeCompare(b, undefined, { sensitivity: 'base' })
        );
        for (const key of sortedKeys) {
            groups.push({ label: key, students: byOwner.get(key)! });
        }
        return groups;
    });

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
        this.showStudentModal.set('add');
    }

    onStudentCreated(student: StudentCardDto) {
        this.showStudentModal.set(null);
        this.studentService.notifyDataChanged();
        this.selectedStudentId.set(student.studentId);
        this.router.navigate(['/students', student.studentId]);
    }

    onEditStudent(student: StudentCardDto, event: Event) {
        event.stopPropagation();
        this.showStudentModal.set(student);
    }

    onStudentSaved() {
        this.showStudentModal.set(null);
        this.loadStudents();
    }

    onLogout() {
        this.auth.logout().subscribe();
        this.auth.forceLogout();
    }

    formatDate = formatDate;

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
