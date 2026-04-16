import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DeviceService } from '../../services/device';
import { Device } from '../../models/device.model';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-device-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './device-list.html',
  styleUrl: './device-list.scss'
})
export class DeviceList implements OnInit {
  devices: Device[] = [];
  loading = true;
  error = '';
  toastMessage = '';
  toastType: 'success' | 'error' = 'success';
  searchQuery = '';
  searchTimeout: any = null;

  constructor(
    private deviceService: DeviceService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadDevices();
  }

  loadDevices(): void {
    this.loading = true;
    this.deviceService.getAll().subscribe({
      next: (data) => {
        this.devices = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.error = 'Failed to load devices. Make sure the API is running.';
        this.loading = false;
        this.cdr.detectChanges();
        console.error(err);
      }
    });
  }

  deleteDevice(id: number, name: string): void {
    if (!confirm(`Are you sure you want to delete "${name}"?`)) return;

    this.deviceService.delete(id).subscribe({
      next: () => {
        this.devices = this.devices.filter(d => d.id !== id);
        this.showToast(`"${name}" deleted successfully.`, 'success');
      },
      error: (err) => {
        this.error = 'Failed to delete device.';
        this.cdr.detectChanges();
        console.error(err);
      }
    });
  }

  private showToast(message: string, type: 'success' | 'error'): void {
    this.toastMessage = message;
    this.toastType = type;
    this.cdr.detectChanges();
    setTimeout(() => {
      this.toastMessage = '';
      this.cdr.detectChanges();
    }, 3000);
  }

  onSearch(): void {
    // Debounce: wait 300ms after user stops typing
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => {
      if (this.searchQuery.trim()) {
        this.loading = true;
        this.deviceService.search(this.searchQuery).subscribe({
          next: (data) => {
            this.devices = data;
            this.loading = false;
            this.cdr.detectChanges();
          },
          error: () => {
            this.error = 'Search failed.';
            this.loading = false;
            this.cdr.detectChanges();
          }
        });
      } else {
        this.loadDevices();
      }
    }, 300);
  }
}