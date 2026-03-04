import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { StudentCardList } from './components/student-card-list/student-card-list';
import { GoalList } from './components/goal-list/goal-list';
import { ProgressList } from './components/progress-list/progress-list';

export default [
    {
        path: '',
        component: Home,
        children: [
            { path: '', redirectTo: 'students', pathMatch: 'full' },
            { path: 'students', component: StudentCardList },
            { path: 'students/:studentId/goals', component: GoalList },
            { path: 'students/:studentId/goals/:goalId/progress', component: ProgressList },
        ],
    },
] satisfies Routes;
