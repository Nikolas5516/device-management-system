import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DeviceService } from '../../services/device';
import { Device } from '../../models/device.model';

@Component({
  selector: 'app-device-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './device-list.html',
  styleUrl: './device-list.scss'
})
export class DeviceList implements OnInit {
  devices: Device[] = [];
  loading = true;
  error = '';

  constructor(private deviceService: DeviceService) {}

  ngOnInit(): void {
    this.loadDevices();
  }

  loadDevices(): void {
    this.loading = true;
    this.deviceService.getAll().subscribe({
      next: (data) => {
        this.devices = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load devices. Make sure the API is running.';
        this.loading = false;
        console.error(err);
      }
    });
  }

  deleteDevice(id: number, name: string): void {
    if (!confirm(`Are you sure you want to delete "${name}"?`)) return;

    this.deviceService.delete(id).subscribe({
      next: () => {
        this.devices = this.devices.filter(d => d.id !== id);
      },
      error: (err) => {
        this.error = 'Failed to delete device.';
        console.error(err);
      }
    });
  }
}