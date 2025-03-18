// import { Component } from '@angular/core';

// @Component({
//   selector: 'app-payment',
//   imports: [],
//   templateUrl: './payment.component.html',
//   styleUrl: './payment.component.css'
// })
// export class PaymentComponent {

// }
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
 
@Component({
  selector: 'app-payment',
  imports: [CommonModule,ReactiveFormsModule,FormsModule],
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.css']
})
export class PaymentComponent implements OnInit {
  paymentForm!: FormGroup;
  paymentMethods = ['Cash', 'Card', 'Upi'];
  isCardSelected = false;
  isUpiSelected = false;
  totalAmount: number = 500; // Example static amount
 
  constructor(private fb: FormBuilder) {}
 
  ngOnInit(): void {
    this.initializeForm();
  }
 
  initializeForm(): void {
    this.paymentForm = this.fb.group({
      paymentMethod: ['', Validators.required],
      paymentAmount: [{ value: this.totalAmount, disabled: true }, [Validators.required, Validators.min(1)]],
      cardNumber: [''],
      cardExpiry: [''],
      upiId: [''],
    });
 
    this.paymentForm.get('paymentMethod')?.valueChanges.subscribe((method) => {
      this.updatePaymentFields(method);
    });
  }
 
  updatePaymentFields(method: string): void {
    this.isCardSelected = method === 'Card';
    this.isUpiSelected = method === 'Upi';
 
    if (this.isCardSelected) {
      this.paymentForm.get('cardNumber')?.setValidators([Validators.required, Validators.pattern('^[0-9]{16}$')]);
      this.paymentForm.get('cardExpiry')?.setValidators([Validators.required, Validators.pattern('^(0[1-9]|1[0-2])/(\\d{2})$')]);
      this.paymentForm.get('upiId')?.clearValidators();
    } else if (this.isUpiSelected) {
      this.paymentForm.get('upiId')?.setValidators([Validators.required, Validators.email]);
      this.paymentForm.get('cardNumber')?.clearValidators();
      this.paymentForm.get('cardExpiry')?.clearValidators();
    } else {
      this.paymentForm.get('cardNumber')?.clearValidators();
      this.paymentForm.get('cardExpiry')?.clearValidators();
      this.paymentForm.get('upiId')?.clearValidators();
    }
 
    this.paymentForm.get('cardNumber')?.updateValueAndValidity();
    this.paymentForm.get('cardExpiry')?.updateValueAndValidity();
    this.paymentForm.get('upiId')?.updateValueAndValidity();
  }
 
  onSubmit(): void {
    if (this.paymentForm.invalid) {
      alert("Please fill in the required fields.");
      return;
    }
    console.log('Payment Details:', this.paymentForm.value);
    alert('Payment submitted successfully!');
  }
}
 
