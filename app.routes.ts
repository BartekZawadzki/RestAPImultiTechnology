import { Routes } from '@angular/router';
import { DataComponent } from './data/data.component';

export const routes: Routes = [
  { path: 'data', component: DataComponent },
  { path: '', redirectTo: '/data', pathMatch: 'full' }, 
];
