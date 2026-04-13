import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DeviceService } from '../../services/device';
import { Device } from '../../models/device.model';

@Component({
  selector: 'app-device-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './device-detail.html',
  styleUrl: './device-detail.scss'
})
export class DeviceDetail implements OnInit {
  device: Device | null = null;
  loading = true;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private deviceService: DeviceService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (isNaN(id)) {
      this.error = 'Invalid device ID.';
      this.loading = false;
      return;
    }

    this.deviceService.getById(id).subscribe({
      next: (data) => {
        this.device = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Device not found.';
        this.loading = false;
      }
    });
  }

  deleteDevice(): void {
    if (!this.device) return;
    if (!confirm(`Delete "${this.device.name}"?`)) return;

    this.deviceService.delete(this.device.id).subscribe({
      next: () => this.router.navigate(['/devices']),
      error: () => this.error = 'Failed to delete device.'
    });
  }
}