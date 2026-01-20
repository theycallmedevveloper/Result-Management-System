import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from './auth/login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { StudentsComponent } from './students/students.component';
import { ResultComponent } from './result/result.component';

import { RoleGuard } from './auth/role-guard';

const routes: Routes = [
  { path: 'login', component: LoginComponent },

  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [RoleGuard],
    data: { roles: ['Admin', 'Student'] }
  },

  {
    path: 'students',
    component: StudentsComponent,
    canActivate: [RoleGuard],
    data: { roles: ['Admin'] }

    
  },

  {
    path: 'my-result',
    component: ResultComponent,
    canActivate: [RoleGuard],
    data: { roles: ['Student'] }
  },

  { path: '', redirectTo: 'login', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
