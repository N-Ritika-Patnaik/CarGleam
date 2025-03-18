import { Routes } from '@angular/router';
import { SignupComponent } from './components/signup/signup.component';
import { BookingsComponent } from './components/bookings/bookings.component';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { UserDashboardComponent } from './components/user-dashboard/user-dashboard.component';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { PaymentComponent } from './components/payment/payment.component';
import { ServiceLocationFormComponent } from './components/service-location-form/service-location-form.component';
import { MachineFormComponent } from './components/machine-form/machine-form.component';
import { EditMachineFormComponent } from './components/edit-machine-form/edit-machine-form.component';
import { EditServiceLocationFormComponent } from './components/edit-service-location-form/edit-service-location-form.component';

export const routes: Routes = [
    { path: "signup", component: SignupComponent},
    { path: "login", component: LoginComponent},
    { path: "", redirectTo: "/home", pathMatch: "full"},
    // { path: "", redirectTo: "/login", pathMatch: "full"},
    // { path: "home", component: NavbarComponent},
    // { path: "bookings", component: BookingsComponent},
    {path:"userDashboard", component:UserDashboardComponent},
    {path:"adminDashboard", component:AdminDashboardComponent},
    {path:"payment", component:PaymentComponent},
    {path:"servicelocationform", component:ServiceLocationFormComponent}, //add new
    {path: "editserviceform/:id", component: EditServiceLocationFormComponent}, //edit existing
    {path:"machineform", component:MachineFormComponent},
    {path:"editmachineform/:id", component:EditMachineFormComponent},
    {path:"home", component:HomeComponent},

];
