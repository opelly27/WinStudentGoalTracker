import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { StudentCardList } from './pages/components/student-card-list/student-card-list';

export default [
    {
        path: '',
        component: Home,
        children: [
            { path: '', redirectTo: 'students', pathMatch: 'full' },
            { path: 'students', component: StudentCardList },
        ],
    },
] satisfies Routes;
