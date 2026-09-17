import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../../Services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './register-component.html',
  styleUrls: ['./register-component.css']
})
export class RegisterComponent {
  registerForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  showPassword = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      name: [
        '',
        [
          Validators.required,
          Validators.minLength(2)
        ]
      ],
      email: [
        '',
        [
          Validators.required,
          Validators.email
        ]
      ],
      phoneNumber: [
        '',
        [
          Validators.required,
          Validators.pattern(/^[0-9]{10,15}$/)
        ]
      ],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(6)
        ]
      ],
      confirmPassword: [
        '',
        Validators.required
      ]
    });
  }

  onRegister(): void {
    this.errorMessage = '';

    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const password = this.registerForm.value.password;
    const confirmPassword = this.registerForm.value.confirmPassword;

    if (password !== confirmPassword) {
      this.errorMessage = 'Passwords do not match';
      return;
    }

    this.isLoading = true;

    const registerRequest = {
      name: this.registerForm.value.name,
      email: this.registerForm.value.email,
      phoneNumber: this.registerForm.value.phoneNumber,
      password: this.registerForm.value.password
    };

    this.authService.register(registerRequest).subscribe({
      next: (response) => {
        this.isLoading = false;
        alert('Registration successful! Please login.');
        this.router.navigate(['/']);
      },
      error: (error) => {
        this.isLoading = false;
        const errorText = error?.error?.message || error?.error || 'Registration failed';

        if (error.status === 400 || error.status === 409) {
          this.errorMessage = errorText;
        } else {
          this.errorMessage = 'Registration failed. Please try again.';
        }
      }
    });
  }

  navigateToLogin(): void {
    this.router.navigate(['/']);
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  get name() {
    return this.registerForm.get('name');
  }

  get email() {
    return this.registerForm.get('email');
  }

  get phoneNumber() {
    return this.registerForm.get('phoneNumber');
  }

  get password() {
    return this.registerForm.get('password');
  }

  get confirmPassword() {
    return this.registerForm.get('confirmPassword');
  }
}