import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DeviceService } from '../../services/device';

@Component({
  selector: 'app-device-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './device-form.html',
  styleUrl: './device-form.scss'
})
export class DeviceForm implements OnInit {
  form!: FormGroup;
  isEditMode = false;
  deviceId: number | null = null;
  loading = false;
  submitting = false;
  error = '';
  successMessage = '';

  // Options for the Type dropdown
  deviceTypes = ['phone', 'tablet'];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private deviceService: DeviceService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.initForm();

    // Check if we're in edit mode
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEditMode = true;
      this.deviceId = Number(idParam);
      this.loadDevice();
    }
  }

  private initForm(): void {
    this.form = this.fb.group({
      name:            ['', [Validators.required, Validators.maxLength(100)]],
      manufacturer:    ['', [Validators.required, Validators.maxLength(100)]],
      type:            ['', [Validators.required]],
      operatingSystem: ['', [Validators.required, Validators.maxLength(50)]],
      osVersion:       ['', [Validators.required, Validators.maxLength(50)]],
      processor:       ['', [Validators.required, Validators.maxLength(100)]],
      ramAmount:       ['', [Validators.required, Validators.maxLength(50)]],
      description:     ['']
    });
  }

  private loadDevice(): void {
    if (!this.deviceId) return;
    this.loading = true;

    this.deviceService.getById(this.deviceId).subscribe({
      next: (device) => {
        this.form.patchValue({
          name: device.name,
          manufacturer: device.manufacturer,
          type: device.type,
          operatingSystem: device.operatingSystem,
          osVersion: device.osVersion,
          processor: device.processor,
          ramAmount: device.ramAmount,
          description: device.description || ''
        });
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'Failed to load device.';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onSubmit(): void {
    // Mark all fields as touched so validation errors show
    this.form.markAllAsTouched();

    if (this.form.invalid) return;

    this.submitting = true;
    this.error = '';
    this.successMessage = '';

    const formData = this.form.value;

    if (this.isEditMode && this.deviceId) {
      // UPDATE
      this.deviceService.update(this.deviceId, formData).subscribe({
        next: () => {
          this.router.navigate(['/devices', this.deviceId]);
        },
        error: (err) => {
          this.submitting = false;
          this.error = err.error?.message || 'Failed to update device.';
          this.cdr.detectChanges();
        }
      });
    } else {
      // CREATE
      this.deviceService.create(formData).subscribe({
        next: (created) => {
          this.router.navigate(['/devices', created.id]);
        },
        error: (err) => {
          this.submitting = false;
          if (err.status === 409) {
            this.error = err.error?.message || 'A device with this name already exists.';
          } else {
            this.error = err.error?.message || 'Failed to create device.';
          }
          this.cdr.detectChanges();
        }
      });
    }
  }

  // Helper to check if a field has an error
  isFieldInvalid(fieldName: string): boolean {
    const field = this.form.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  getFieldError(fieldName: string): string {
    const field = this.form.get(fieldName);
    if (!field || !field.errors || !field.touched) return '';

    if (field.errors['required']) return `${this.formatFieldName(fieldName)} is required.`;
    if (field.errors['maxlength']) {
      const max = field.errors['maxlength'].requiredLength;
      return `${this.formatFieldName(fieldName)} must be ${max} characters or less.`;
    }
    return '';
  }

  private formatFieldName(fieldName: string): string {
    const names: Record<string, string> = {
      name: 'Name',
      manufacturer: 'Manufacturer',
      type: 'Type',
      operatingSystem: 'Operating System',
      osVersion: 'OS Version',
      processor: 'Processor',
      ramAmount: 'RAM Amount',
      description: 'Description'
    };
    return names[fieldName] || fieldName;
  }
}