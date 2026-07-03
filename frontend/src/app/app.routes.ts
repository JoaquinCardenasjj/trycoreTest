import { Routes } from '@angular/router';


export const routes: Routes = [

    {
        path: '',
        loadComponent: () => import('../features/dashboard/dashboard.component').then(m => m.DashboardComponent)
    },
    {
        path: 'dashboard',
        loadComponent: () => import('../features/dashboard/dashboard.component').then(m => m.DashboardComponent)
    },
    // Comodín para manejar páginas no encontradas (404)
    {
        path: '**',
        redirectTo: 'dashboard'
    }
];
