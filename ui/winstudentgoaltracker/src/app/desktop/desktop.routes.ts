import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { StudentCardList } from './components/student-card-list/student-card-list';
import { StudentCardFull } from './components/student-card-full/student-card-full';
import { GoalList } from './components/goal-list/goal-list';
import { GoalCardFull } from './components/goal-card-full/goal-card-full';
import { ProgressList } from './components/progress-list/progress-list';
import { BenchmarkList } from './components/benchmark-list/benchmark-list';
import { BenchmarkCardFull } from './components/benchmark-card-full/benchmark-card-full';

export default [
    {
        path: '',
        component: Home,
        children: [
            { path: '', redirectTo: 'students', pathMatch: 'full' },
            { path: 'students', component: StudentCardList },
            { path: 'students/:studentId', component: StudentCardFull },
            { path: 'students/:studentId/goals', component: GoalList },
            { path: 'students/:studentId/goals/:goalId', component: GoalCardFull },
            { path: 'students/:studentId/goals/:goalId/progress', component: ProgressList },
            { path: 'students/:studentId/goals/:goalId/benchmarks', component: BenchmarkList },
            { path: 'students/:studentId/goals/:goalId/benchmarks/new', component: BenchmarkCardFull },
            { path: 'students/:studentId/goals/:goalId/benchmarks/:benchmarkId', component: BenchmarkCardFull },
            { path: 'students/:studentId/benchmarks', component: BenchmarkList },
        ],
    },
] satisfies Routes;
