import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router, RouterLink } from '@angular/router';
import { BookingsComponent } from '../bookings/bookings.component';
 
export interface Booking {
  bookingId: number;
  userId: number;
  fullName: string;
  serviceLocationId: number;
  serviceName: string;
  locationName: string;
  machineId: number;
  machineName: string;
  serviceDate: string; // Using string to represent DateTime
}
 
@Component({
  selector: 'app-user-dashboard',
  imports: [CommonModule,ReactiveFormsModule,RouterLink,BookingsComponent],
  standalone: true,
  templateUrl: './user-dashboard.component.html',
  styleUrls: ['./user-dashboard.component.css']
})
export class UserDashboardComponent implements OnInit {
  bookings: Booking[] = [];
  
  private baseUrl = "https://localhost:7171/api";

  constructor(private http: HttpClient,private fb: FormBuilder, private router: Router) { }
 
  ngOnInit(): void {
    this.fetchBookings();
    console.log('bookings:', this.bookings);
  }

  fetchBookings(): void {
    const userId = localStorage.getItem('userId');
    this.http.get<Booking[]>(`${this.baseUrl}/Booking/GetBookingsByUserId/${userId}`
      // ,{headers: {Authorization: `Bearer ${localStorage.getItem('jwtToken')}`}}
  ).subscribe(
      (data) => {
        this.bookings = data;
        console.log('Bookings:', this.bookings);
      },
      (error) => {
        console.error('Error fetching bookings', error);
      }
    );
  }

}
 
