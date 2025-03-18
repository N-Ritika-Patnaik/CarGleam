import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
// import { ProdcomponentComponent } from './components/prodcomponent/prodcomponent.component';
import { SignupComponent } from './components/signup/signup.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { CommonModule } from '@angular/common';
import { BookingsComponent } from './components/bookings/bookings.component';
import { LoginComponent } from './components/login/login.component';
import { UserDashboardComponent } from './components/user-dashboard/user-dashboard.component';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { PaymentComponent } from './components/payment/payment.component';
import { ServiceLocationFormComponent } from './components/service-location-form/service-location-form.component';
import { MachineFormComponent } from './components/machine-form/machine-form.component';
import { EditMachineFormComponent } from './components/edit-machine-form/edit-machine-form.component';
import { EditServiceLocationFormComponent } from './components/edit-service-location-form/edit-service-location-form.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,SignupComponent,LoginComponent,
    NavbarComponent,BookingsComponent,AdminDashboardComponent,
    UserDashboardComponent,CommonModule,PaymentComponent,
    ServiceLocationFormComponent,MachineFormComponent,EditMachineFormComponent,
  EditServiceLocationFormComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'CarGleamAngular';
  showLayout = true;
  constructor(private router: Router){
    this.router.events.subscribe(()=>{this.showLayout = !['/signup', '/login','/home', '/adminDashboard'].includes(this.router.url);})
  }
}

