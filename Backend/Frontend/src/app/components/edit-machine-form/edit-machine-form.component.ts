import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

export interface Machine {
  machineId: number;
  machineName: string;
  status: string;
  machineType: string;
  duration: string; // Using string to represent TimeSpan
}

@Component({
  selector: 'app-edit-machine-form',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './edit-machine-form.component.html',
  styleUrls: ['./edit-machine-form.component.css']
})
export class EditMachineFormComponent implements OnInit {
  private baseUrl = "https://localhost:7171/api";
  machineForm: FormGroup;

  constructor(private fb: FormBuilder, private http: HttpClient, private route: ActivatedRoute) {
    this.machineForm = this.fb.group({
      machineid: ['', Validators.required],
      machinename: ['', Validators.required],
      machinetype: ['', Validators.required],
      duration: ['', Validators.required],
      status: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const machineId = params.get('id');
      console.log('Fetched Machine ID:', machineId); // Add this line to debug
      if (machineId) {
        this.http.get<Machine>(`${this.baseUrl}/Machine/GetMachineBy/${machineId}`).subscribe(data => {
          this.machineForm.patchValue({
            machineid: data.machineId,
            machinename: data.machineName,
            machinetype: data.machineType,
            duration: data.duration,
            status: data.status,
          });
        });
      } else {
        console.error('Machine ID is null');
      }
    });
  }

  onSubmit(): void {
    console.log("strat 2",this.machineForm.valid);
    console.log("strat 4",this.machineForm.value);
    if (true) {
      console.log("start");
      console.log(this.machineForm.value);
      const machineId = this.machineForm.get('machineid')?.value;
      this.http.put(`${this.baseUrl}/Machine/UpdateMachine/${machineId}`, this.machineForm.value).subscribe(response => {
        alert('Machine successfully updated');
        console.log('Update Successful', response);
      });
      console.log("Form submitted!");
    }
    else{
      alert("Fill all fields!")
    }
  }
}