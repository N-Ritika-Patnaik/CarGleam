import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [FormsModule, ReactiveFormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  email: string = '';
  password: string = '';

  // Hardcoded admin credentials
  private adminEmail: string = 'admin1@email.com';
  private adminPassword: string = 'admin!1';
 
  constructor(private authService: AuthService, private router: Router) { }
 
  onSignIn(): void {
    const login = {email: this.email, password: this.password}
 
    this.authService.login(login).subscribe( {
      next: (response) => {
        const token = response.token;
        this.authService.storeToken(token);

        // Check if the credentials match the hardcoded admin credentials
        if (this.email === this.adminEmail && this.password === this.adminPassword) {
          this.router.navigate(['/adminDashboard']);
          alert("Admin Login Successful");
        } else {
          this.router.navigate(['/userDashboard']);
          alert("Welcome User !!");
        }
        // this.router.navigate(['/adminDashboard']);
        console.log("User Authenticated and token is stored", token)
      },

      error: (error) => {
        console.error("Error during authentication ", error);
      }
    })
  }
}
