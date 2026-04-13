import { Routes } from '@angular/router';
import { DeviceList } from './components/device-list/device-list';
import { DeviceDetail } from './components/device-detail/device-detail';
import { DeviceForm } from './components/device-form/device-form';

export const routes: Routes = [
  { path: '', redirectTo: 'devices', pathMatch: 'full' },
  { path: 'devices', component: DeviceList },
  { path: 'devices/new', component: DeviceForm },
  { path: 'devices/:id', component: DeviceDetail },
  { path: 'devices/:id/edit', component: DeviceForm },
];