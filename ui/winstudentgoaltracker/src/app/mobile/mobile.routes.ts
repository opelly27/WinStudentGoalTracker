import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Students } from './pages/students/students';
import { StudentGoals } from './pages/student-goals/student-goals';
import { AddProgressEvent } from './pages/add-progress-event/add-progress-event';

export default [
    {
        path: '',
        component: Home,
        children: [
            { path: '', redirectTo: 'students', pathMatch: 'full' },
            { path: 'students', component: Students },
            { path: 'students/:studentId/goals', component: StudentGoals },
            { path: 'students/:studentId/goals/:goalId/add-event', component: AddProgressEvent },
        ],
    },
] satisfies Routes;
