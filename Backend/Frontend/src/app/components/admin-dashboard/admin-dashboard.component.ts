import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder } from '@angular/forms';

export interface ServiceLocation {
  serviceLocationId: number;
  serviceName: string;
  price: number;
  locationName: string;
}

export interface Machine {
  machineId: number;
  machineName: string;
  status: string;
  // machineType: string;
  duration: string; // Using string to represent TimeSpan
}

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
  selector: 'app-admin-dashboard',
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
  machines: Machine[] = [];
  serviceLocations: ServiceLocation[] = [];
  bookings: Booking[] = [];
 
  private baseUrl = "https://localhost:7171/api";

  constructor(private http: HttpClient,private fb: FormBuilder, private router: Router) { }
  
  logout(): void {
    localStorage.removeItem('jwtToken');
    this.router.navigate(['/home']);
  }

  ngOnInit(): void {
    this.fetchServiceLocations();
    this.fetchMachines();
    this.fetchBookings();
  }

  fetchServiceLocations(): void {
    this.http.get<ServiceLocation[]>(`${this.baseUrl}/ServiceLocation`).subscribe(data => {this.serviceLocations = data;});
  }

  fetchMachines(): void {
    this.http.get<Machine[]>(`${this.baseUrl}/Machine`).subscribe(data => {this.machines = data;});
  }

  fetchBookings(): void {
    this.http.get<Booking[]>(`${this.baseUrl}/Booking/GetAllBookings`).subscribe(data => {this.bookings = data;});
  }

  deleteServiceLocation(serviceLocationId: number): void {
    this.http.delete(`${this.baseUrl}/ServiceLocation/DeleteServiceLocation/${serviceLocationId}`).subscribe(() => {
      this.fetchServiceLocations();
    });
  }

  deleteMachine(machineId: number): void {
    this.http.delete(`${this.baseUrl}/Machine/DeleteMachine/${machineId}`).subscribe(() => {
      this.fetchMachines();
    });
  }

  showBookingDetails(booking: Booking): void {
      this.http.get(`${this.baseUrl}/Booking/GetAllBookings`).subscribe(() => {
      this.fetchBookings();
    });
  }
}






  // addBooking(booking: Booking): void {
  //   this.http.post(`${this.baseUrl}/Booking`, booking).subscribe(() => {
  //     this.fetchBookings();
  //   });
  // }
  // deleteBooking(bookingId: number): void {
  //   this.http.delete(`${this.baseUrl}/Booking/DeleteBooking/${bookingId}`).subscribe(() => {
  //     this.fetchBookings();
  //   });
  // }
  





  