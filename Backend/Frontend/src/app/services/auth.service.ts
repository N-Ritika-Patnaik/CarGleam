import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

private baseUrl = "https://localhost:7171/api/Auth";
 
  constructor(private http: HttpClient, private router: Router) {}
 
  login(login: { email: string, password: string }):Observable<any> {
return this.http.post<any>(`${this.baseUrl}/Login`, login, {
  headers: new HttpHeaders({
    "Content-Type": "application/json"
  })
}).pipe(
  tap(response => {
    if (response.token) {
      // localStorage.setItem('token', response.token);
      const decodedToken: any = jwtDecode(response.token);
      const userId = decodedToken.UserId;

      const user = {
        userId: userId,
        name: decodedToken.fullName,
      }

      localStorage.setItem('user', JSON.stringify(user));
      // Store the extracted information in local storage
      localStorage.setItem('userId', userId);
      // Call the storeToken method to store the token
      this.storeToken(response.token);
    }
  })
);
  }
 
  storeToken(token: string): void {
    localStorage.setItem('jwtToken', token);
  }
 
  logout(): void {
    localStorage.removeItem('jwtToken');
    this.router.navigate(['/signin']);
  }
 
  signup(signup: {fullname: string, email: string, password: string}): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Signup`, signup);
  }
}

