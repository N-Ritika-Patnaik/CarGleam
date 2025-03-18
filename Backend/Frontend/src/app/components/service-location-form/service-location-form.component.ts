import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { ReactiveFormsModule,FormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-service-location-form',
  imports: [CommonModule,ReactiveFormsModule,FormsModule],
  templateUrl: './service-location-form.component.html',
  styleUrl: './service-location-form.component.css'
})
export class ServiceLocationFormComponent {
  private baseUrl = "https://localhost:7171/api";
  serviceForm: FormGroup;

  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.serviceForm = this.fb.group({
      serviceName: ['', Validators.required],
      locationName: ['', Validators.required],
      serviceType: ['', Validators.required],
      price: ['', Validators.required],
    }); 
}
onSubmit() {
  if(true)
  {
    console.log("start");
    console.log(this.serviceForm.value);
    this.http.post(`${this.baseUrl}/ServiceLocation`,this.serviceForm.value).subscribe(response => {
      alert('Service and Location successfully added');
      console.log('Booking successful', response);
    });
    console.log('Form not submitted!');
  }
}

}
