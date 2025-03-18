import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-bookings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './bookings.component.html',
  styleUrls: ['./bookings.component.css']
})
export class BookingsComponent implements OnInit {
  bookingForm: FormGroup;
  serviceLocations: any[] = [];
  selectedServiceType: string = '';
  private baseUrl = "https://localhost:7171/api";
  machineData: any[] = [];

  name: string = '';
  userId: number = 0;
  
  getUser(): void {
    const user = localStorage.getItem('user');
    if (user) {
      const userObj = JSON.parse(user);
      console.log('userObj:', userObj);
      this.name = userObj.name;
      this.userId = userObj.userId;

      // Set the values of the form controls
      this.bookingForm.patchValue({
        userid: this.userId,
        fullname: this.name
      });
    }
  }

  constructor(private fb: FormBuilder, private http: HttpClient, private cdr: ChangeDetectorRef) {
    this.bookingForm = this.fb.group({
      userid: [''],
      fullname: [''],
      servicelocationid: [''],
      machineid: [''],
      servicelocationdate: [''],
    });
  }

  ngOnInit(): void {
    this.loadServiceLocations();
    this.getUser();
  }

  loadServiceLocations(): void {
    this.http.get(`${this.baseUrl}/ServiceLocation`, {headers:{Authorization: `Bearer ${localStorage.getItem('jwtToken')}`}}).subscribe((data: any) => {
      this.serviceLocations = data;
    });
  }

  onServiceTypeChange(event: Event): void {
    const selectedServiceLocationId = +(event.target as HTMLSelectElement).value;
    const selectedService = this.serviceLocations.find(service => service.serviceLocationId === selectedServiceLocationId);
    if (selectedService) {
      console.log("Selected Service Type:", selectedService.serviceType);
      this.selectedServiceType = selectedService.serviceType;
      // Load machines based on selected service type and location
      this.loadMachinesByServiceType(this.selectedServiceType);
    } else {
      console.log("Service not found!");
    }
  }

  loadMachinesByServiceType(serviceType: string): void {
    if (!serviceType) {
      console.warn("Service Type is empty. Cannot load machines.");
      return;
    }
    const apiUrl = `${this.baseUrl}/Booking/GetBookingsByServiceType/${encodeURIComponent(serviceType)}`;
    this.http.get<any[]>(apiUrl).subscribe({
      next: (data) => {
        console.log("API response data:", data);
        if (data && Array.isArray(data)) {
          this.machineData = data.flatMap(location => location.machines)
          .filter((value, index, self) =>
            index === self.findIndex((t) => (
                t.machineId === value.machineId )) );
          console.log("Machines loaded for service type:", serviceType, this.machineData);
        } else {
          console.warn("Unexpected API response format:", data);
        }
        // Trigger change detection
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error("Error fetching machines:", error);
      }
    });
  }

  onSubmit(): void {
    if (true) {
        const formValue = this.bookingForm.value;
        formValue.serviceDate = new Date(formValue.servicelocationdate).toISOString();
        this.http.post(`${this.baseUrl}/Booking/AddBooking`, formValue).subscribe(response => {
        console.log('booking form ', this.bookingForm.value);
        alert('Your Booking is successful!');
        console.log('working ', response);
        console.log('Booking successful', response);
      });
    }
  }
}