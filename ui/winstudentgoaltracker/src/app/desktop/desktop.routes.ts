import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Workspace } from './components/workspace/workspace';
import { Reports } from './components/reports/reports';
import { StudentProgressReport } from './components/student-progress-report/student-progress-report';
export default [
    {
        path: '',
        component: Home,
        children: [
            { path: '', redirectTo: 'students', pathMatch: 'full' },
            { path: 'students', component: Workspace },
            { path: 'students/:studentId', component: Workspace },
            { path: 'students/:studentId/goals/:goalId', component: Workspace },
            { path: 'reports', component: Reports },
            { path: 'reports/student-progress', component: StudentProgressReport },
        ],
    },
] satisfies Routes;
