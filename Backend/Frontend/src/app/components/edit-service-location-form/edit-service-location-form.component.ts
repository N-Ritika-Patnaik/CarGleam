import { Component,OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

export interface ServiceLocation {
  serviceLocationId: number;
  serviceName: string;
  price: number;
  locationName: string;
  serviceType: string; 
}

@Component({
  selector: 'app-edit-service-location-form',
  imports: [ReactiveFormsModule],
  templateUrl: './edit-service-location-form.component.html',
  styleUrl: './edit-service-location-form.component.css'
})
export class EditServiceLocationFormComponent {
  private baseUrl = "https://localhost:7171/api";
  serviceForm: FormGroup;

  constructor(private fb: FormBuilder, private http: HttpClient, private route: ActivatedRoute) {
    this.serviceForm = this.fb.group({
      servicelocationid: ['', Validators.required],
      servicename: ['', Validators.required],
      locationname: ['', Validators.required],
      servicetype: ['', Validators.required],
      price: ['', Validators.required],
    }); 
}

ngOnInit(): void {
  this.route.paramMap.subscribe(params => {
    const serviceLocationId = params.get('id');
    console.log('Fetched service ID:', serviceLocationId); // Add this line to debug
    if (serviceLocationId) {
      this.http.get<ServiceLocation>(`${this.baseUrl}/ServiceLocation/GetServiceLocationBy/${serviceLocationId}`).subscribe(data => {
        this.serviceForm.patchValue({
          servicelocationid: data.serviceLocationId,
          servicename: data.serviceName,
          locationname: data.locationName,
          servicetype: data.serviceType,
          price: data.price,
        });
      });
    } else {
      console.error('Service Location ID is null');
    }
  });
}

onSubmit(): void {
    console.log("strat 2",this.serviceForm.valid);
    console.log("strat 4",this.serviceForm.value);
  if (true) {
    console.log("start");
    console.log(this.serviceForm.value);
    const serviceLocationId = this.serviceForm.get('servicelocationid')?.value;
    this.http.put(`${this.baseUrl}/ServiceLocation/UpdateServiceLocation/${serviceLocationId}`, this.serviceForm.value).subscribe(response => {
      alert('Service successfully updated');
      console.log('Update Successful', response);
    });
    console.log("Form submitted!");
  }
}

}
