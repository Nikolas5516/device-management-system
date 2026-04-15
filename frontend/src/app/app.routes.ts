import { Routes } from '@angular/router';
import { DeviceList } from './components/device-list/device-list';
import { DeviceDetail } from './components/device-detail/device-detail';
import { DeviceForm } from './components/device-form/device-form';
import { Login } from './components/login/login';
import { Register } from './components/register/register';
import { authGuard } from './services/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'devices', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'devices', component: DeviceList, canActivate: [authGuard] },
  { path: 'devices/new', component: DeviceForm, canActivate: [authGuard] },
  { path: 'devices/:id', component: DeviceDetail, canActivate: [authGuard] },
  { path: 'devices/:id/edit', component: DeviceForm, canActivate: [authGuard] },
];