import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminService, ProgramDto, DistrictUserDto, RoleDto } from '../../../shared/services/admin.service';
import { ModalShell } from '../../components/modal-shell/modal-shell';

@Component({
    selector: 'app-admin',
    imports: [FormsModule, ModalShell],
    templateUrl: './admin.html',
    styleUrl: './admin.scss',
})
export class Admin {

    // ************************** Constructor **************************

    constructor() {
        this.loadAll();
    }

    // ************************** Declarations *************************

    private readonly adminService = inject(AdminService);

    protected readonly activeTab = signal<'programs' | 'users'>('programs');
    protected readonly programs = signal<ProgramDto[]>([]);
    protected readonly users = signal<DistrictUserDto[]>([]);
    protected readonly roles = signal<RoleDto[]>([]);
    protected readonly error = signal<string | null>(null);

    // Program modal
    protected readonly showProgramModal = signal(false);
    protected readonly editingProgram = signal<ProgramDto | null>(null);
    protected programName = '';
    protected programDescription = '';

    // User modal
    protected readonly showUserModal = signal(false);
    protected userName = '';
    protected userEmail = '';
    protected userPassword = '';
    protected userProgramId = '';
    protected userRoleId = '';

    // Backup
    protected readonly backingUp = signal(false);
    protected readonly backupSuccess = signal<string | null>(null);

    // ************************ Event Handlers *************************

    onSwitchTab(tab: 'programs' | 'users') {
        this.activeTab.set(tab);
    }

    // --- Programs ---

    onAddProgram() {
        this.editingProgram.set(null);
        this.programName = '';
        this.programDescription = '';
        this.showProgramModal.set(true);
    }

    onEditProgram(program: ProgramDto) {
        this.editingProgram.set(program);
        this.programName = program.name;
        this.programDescription = program.description || '';
        this.showProgramModal.set(true);
    }

    async onSaveProgram() {
        this.error.set(null);
        const editing = this.editingProgram();

        if (editing) {
            const result = await this.adminService.updateProgram(editing.programId, this.programName, this.programDescription);
            if (!result.success) {
                this.error.set(result.message);
                return;
            }
        } else {
            const result = await this.adminService.createProgram(this.programName, this.programDescription);
            if (!result.success) {
                this.error.set(result.message);
                return;
            }
        }

        this.showProgramModal.set(false);
        this.programs.set(await this.adminService.getPrograms());
    }

    // --- Users ---

    onAddUser() {
        this.userName = '';
        this.userEmail = '';
        this.userPassword = '';
        this.userProgramId = '';
        this.userRoleId = '';
        this.showUserModal.set(true);
    }

    async onSaveUser() {
        this.error.set(null);
        const result = await this.adminService.createUser(
            this.userEmail, this.userName, this.userPassword,
            this.userProgramId, this.userRoleId
        );
        if (!result.success) {
            this.error.set(result.message);
            return;
        }
        this.showUserModal.set(false);
        this.users.set(await this.adminService.getUsers());
    }

    // --- Backup ---

    async onBackupDatabase() {
        this.backingUp.set(true);
        this.backupSuccess.set(null);
        this.error.set(null);
        try {
            await this.adminService.backupDatabase();
            this.backupSuccess.set('Backup downloaded successfully.');
        } catch {
            this.error.set('Database backup failed. Please try again.');
        } finally {
            this.backingUp.set(false);
        }
    }

    // ********************** Support Procedures ***********************

    private async loadAll() {
        this.programs.set(await this.adminService.getPrograms());
        this.users.set(await this.adminService.getUsers());
        this.roles.set(await this.adminService.getRoles());
    }
}

