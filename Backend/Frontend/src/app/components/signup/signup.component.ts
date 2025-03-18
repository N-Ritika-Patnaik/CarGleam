import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-signup',
  imports: [FormsModule, ReactiveFormsModule, CommonModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css'
})
export class SignupComponent {

  email: string = '';
  fullname: string = '';
  password: string = '';

  constructor(private authService: AuthService, private router: Router) { }

  onSignUp(): void {

    const signup = { email: this.email, fullname: this.fullname, password: this.password }

    this.authService.signup(signup).subscribe( {
      next: (response) => {
        const token = response.token;
        this.authService.storeToken(token);
        console.log("User signed up successfully", response);
        alert("Redirecting to Home Page ! Login to book a service!!")
        this.router.navigate(['/home']);
      },
      error: (error) => {
        console.error("Error during signup ", error);
      }
    });
  }
}

