import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { ResponseResult } from '../classes/auth.models';
import { firstValueFrom } from 'rxjs';

export interface ProgramDto {
    programId: string;
    name: string;
    description: string;
    createdAt: string;
}

export interface DistrictUserDto {
    userId: string;
    email: string;
    name: string;
    programId: string;
    programName: string;
    roleId: string;
    roleName: string;
    createdAt: string;
}

export interface RoleDto {
    roleId: string;
    name: string;
    internalName: string;
    description: string;
}

@Injectable({
    providedIn: 'root',
})
export class AdminService {
    private readonly http = inject(HttpClient);
    private readonly base = environment.apiBaseUrl;

    // ************************ Programs *************************

    async getPrograms(): Promise<ProgramDto[]> {
        const result = await firstValueFrom(
            this.http.get<ResponseResult<ProgramDto[]>>(`${this.base}/api/Admin/programs`)
        );
        return result.success && result.data ? result.data : [];
    }

    async createProgram(name: string, description?: string): Promise<ResponseResult<object>> {
        return await firstValueFrom(
            this.http.post<ResponseResult<object>>(`${this.base}/api/Admin/programs`, { name, description })
        );
    }

    async updateProgram(programId: string, name: string, description?: string): Promise<ResponseResult<object>> {
        return await firstValueFrom(
            this.http.put<ResponseResult<object>>(`${this.base}/api/Admin/programs/${programId}`, { name, description })
        );
    }

    // ************************ Users *************************

    async getUsers(): Promise<DistrictUserDto[]> {
        const result = await firstValueFrom(
            this.http.get<ResponseResult<DistrictUserDto[]>>(`${this.base}/api/Admin/users`)
        );
        return result.success && result.data ? result.data : [];
    }

    async createUser(email: string, name: string, password: string, programId: string, roleId: string): Promise<ResponseResult<object>> {
        return await firstValueFrom(
            this.http.post<ResponseResult<object>>(`${this.base}/api/Admin/users`, { email, name, password, programId, roleId })
        );
    }

    // ************************ Roles *************************

    async getRoles(): Promise<RoleDto[]> {
        const result = await firstValueFrom(
            this.http.get<ResponseResult<RoleDto[]>>(`${this.base}/api/Admin/roles`)
        );
        return result.success && result.data ? result.data : [];
    }

    // ************************ Backup *************************

    // *****************************************************************
    // Downloads a full database backup as a .sql file. The API streams
    // the dump as a blob; this method triggers a browser file-save
    // dialog using a temporary anchor element.
    // *****************************************************************
    async backupDatabase(): Promise<void> {
        const blob = await firstValueFrom(
            this.http.get(`${this.base}/api/Admin/backup`, { responseType: 'blob' })
        );
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        const timestamp = new Date().toISOString().replace(/[:.]/g, '-').slice(0, 19);
        a.download = `winstudentgoaltracker_backup_${timestamp}.sql`;
        a.click();
        window.URL.revokeObjectURL(url);
    }
}
