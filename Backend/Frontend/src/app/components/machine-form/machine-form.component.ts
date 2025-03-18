import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule,FormBuilder,FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-machine-form',
  imports: [CommonModule,FormsModule,ReactiveFormsModule],
  templateUrl: './machine-form.component.html',
  styleUrl: './machine-form.component.css'
})

export class MachineFormComponent {

  private baseUrl = 'https://localhost:7171/api';
  machineForm: FormGroup;
  
  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.machineForm = this.fb.group({
      machineName: ['', Validators.required],
      machineType: ['', Validators.required],
      duration: ['', Validators.required],
      status: ['', Validators.required],    
    });
  }
  onSubmit(): void {
    if (true) {
      console.log("start");
      console.log(this.machineForm.value);
      this.http.post(`${this.baseUrl}/Machine`, this.machineForm.value).subscribe(response => {
        alert('Machine successfully added');
        console.log('Booking successful', response);
      });
    console.log("Form not submitted!");
  }
}
}




