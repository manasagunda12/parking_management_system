import { Component, OnInit, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
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
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './login-component.html',
  styleUrls: ['./login-component.css']
})
export class LoginComponent implements OnInit {

  loginForm!: FormGroup;
  showPassword = false;
  isLoading = false;
  errorMessage = '';

  private platformId = inject(PLATFORM_ID);

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: [
        '',
        [
          Validators.required,
          Validators.email
        ]
      ],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(6)
        ]
      ],
      rememberMe: [false]
    });
  }

  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  onLogin(): void {

    this.errorMessage = '';

    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;

    const loginRequest = {
      email: this.loginForm.value.email,
      password: this.loginForm.value.password
    };

    this.authService.login(loginRequest).subscribe({

      next: (response: any) => {

        this.isLoading = false;

        const token = response.token ?? response.Token;

        if (!token) {
          this.errorMessage = 'Token not received from server';
          return;
        }

        if (isPlatformBrowser(this.platformId)) {
          localStorage.setItem('token', token);
        }

        const payload = this.authService.decodeToken(token);
        const role =
          payload?.role ??
          payload?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

        switch (role) {

          case 'Customer':
            alert('Successfully Logged In. Welcome Customer');
            this.router.navigate(['/dashboard']);
            break;

          case 'Admin':
            alert('Successfully Logged In. Welcome Admin');
            this.router.navigate(['/admin']);
            break;

          case 'Operator':
            alert('Successfully Logged In. Welcome Operator');
            this.router.navigate(['/operator']);
            break;

          default:
            this.errorMessage = 'Unauthorized Role';
        }
      },

      error: (error) => {

        this.isLoading = false;

        this.errorMessage =
          error?.error?.message ||
          'Invalid Email or Password';
      }
    });
  }

  navigateToRegister(): void {
    this.router.navigate(['/register']);
  }

}