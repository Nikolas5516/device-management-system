import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DeviceService } from '../../services/device';
import { Device } from '../../models/device.model';
import { AuthService } from '../../services/auth';

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
  currentUserId: number | null = null;
  assigning = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private deviceService: DeviceService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.currentUserId = this.authService.getCurrentUserId();
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
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'Device not found.';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  deleteDevice(): void {
    if (!this.device) return;
    if (!confirm(`Delete "${this.device.name}"?`)) return;

    this.deviceService.delete(this.device.id).subscribe({
      next: () => this.router.navigate(['/devices']),
      error: () => {
        this.error = 'Failed to delete device.';
        this.cdr.detectChanges();
      }
    });
  }

  canAssign(): boolean {
    return this.authService.isLoggedIn()
      && this.device !== null
      && this.device.assignedUserId === null;
  }

  canUnassign(): boolean {
    return this.authService.isLoggedIn()
      && this.device !== null
      && this.device.assignedUserId === this.currentUserId;
  }

  assignDevice(): void {
    if (!this.device) return;
    this.assigning = true;

    this.deviceService.assign(this.device.id).subscribe({
      next: (updated) => {
        this.device = updated;
        this.assigning = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to assign device.';
        this.assigning = false;
        this.cdr.detectChanges();
      }
    });
  }

  unassignDevice(): void {
    if (!this.device) return;
    this.assigning = true;

    this.deviceService.unassign(this.device.id).subscribe({
      next: (updated) => {
        this.device = updated;
        this.assigning = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to unassign device.';
        this.assigning = false;
        this.cdr.detectChanges();
      }
    });
  }
}