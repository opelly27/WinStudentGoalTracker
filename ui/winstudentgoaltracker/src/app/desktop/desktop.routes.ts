import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { StudentCardList } from './components/student-card-list/student-card-list';
import { GoalList } from './components/goal-list/goal-list';

export default [
    {
        path: '',
        component: Home,
        children: [
            { path: '', redirectTo: 'students', pathMatch: 'full' },
            { path: 'students', component: StudentCardList },
            { path: 'students/:studentId/goals', component: GoalList },
        ],
    },
] satisfies Routes;
