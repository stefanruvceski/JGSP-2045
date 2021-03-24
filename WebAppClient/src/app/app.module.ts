import { BrowserModule } from '@angular/platform-browser';
import { Router, RouterModule } from '@angular/router';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { FooterComponent } from './footer/footer.component';
import { NavBarComponent } from './nav-bar/nav-bar.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { RegistrationComponent } from './registration/registration.component';
import { UserInfoComponent } from './user-info/user-info.component';
import { NgModule } from '@angular/core';
import { TimetableComponent } from './timetable/timetable.component';
import { PricelistComponent } from './pricelist/pricelist.component';
import { CurrentLocationComponent } from './current-location/current-location.component';
import { SlideshowModule } from 'ng-simple-slideshow';
import { NgDatepickerModule } from 'ng2-datepicker';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgBootstrapFormValidationModule } from 'ng-bootstrap-form-validation';
import { TokenInterceptor } from './interceptors/token.interceptor';
import { UserService } from './user.service';
import { TicketService } from './ticket.service';
import { ControllerVerificationUserComponent } from './controller-verification-user/controller-verification-user.component';
import { UserTicketsComponent } from './user-tickets/user-tickets.component';
import { UserGuard } from './guards/user.guard';
import { ControllerGuard } from './guards/controller.guard';
import { AdminGuard } from './guards/admin.guard';
import { AgmCoreModule } from '@agm/core';
import { AddNewLineComponent } from './add-new-line/add-new-line.component';
import { LineService } from './line.service';
import { LineNetworkComponent } from './line-network/line-network.component';
import { AdminTimetableComponent } from './admin-timetable/admin-timetable.component';
import { AdminPricelistComponent } from './admin-pricelist/admin-pricelist.component';
import { PricelistService } from './pricelist.service';
import { CurrentLocationService } from './current-location.service';

@NgModule({
  declarations: [
    AppComponent,
    FooterComponent,
    NavBarComponent,
    HomeComponent,
    LoginComponent,
    RegistrationComponent,
    UserInfoComponent,
    TimetableComponent,
    PricelistComponent,
    CurrentLocationComponent,
    ControllerVerificationUserComponent,
    UserTicketsComponent,
    AddNewLineComponent,
    LineNetworkComponent,
    AdminTimetableComponent,
    AdminPricelistComponent,

  ],
  imports: [
    NgbModule,
    BrowserModule,
    HttpClientModule,
    ReactiveFormsModule,
    NgBootstrapFormValidationModule.forRoot(),
    NgBootstrapFormValidationModule,
    AgmCoreModule.forRoot({apiKey: 'AIzaSyDnihJyw_34z5S1KZXp90pfTGAqhFszNJk'}),

    RouterModule.forRoot([{
      path: 'login', component: LoginComponent
    }, {
      path: 'registration', component: RegistrationComponent
    }, {
      path: 'home', component: HomeComponent
    }, {
      path: 'userinfo', component: UserInfoComponent, canActivate: [UserGuard]
    },  {
      path: 'currentlocation', component: CurrentLocationComponent
    },  {
      path: 'linenetwork', component: LineNetworkComponent
    }, {
      path: 'usertickets', component: UserTicketsComponent, canActivate: [UserGuard]
    }, {
      path: 'adminpricelist', component: AdminPricelistComponent, canActivate: [AdminGuard]
    },  {
      path: 'addnewline/:flag', component: AddNewLineComponent, canActivate: [AdminGuard]
    },  {
      path: 'timetable', component: TimetableComponent
    },  {
      path: 'admintimetable', component: AdminTimetableComponent, canActivate: [AdminGuard]
    }, {
      path: 'pricelist', component: PricelistComponent
    }, {
      path: 'controlerverificationusers', component: ControllerVerificationUserComponent, canActivate: [ControllerGuard]
    },  {
      path: '', redirectTo: 'home', pathMatch: 'full'
    }, {
      path: '**', redirectTo: 'home', pathMatch: 'full'
    },

  ]),
  SlideshowModule,

  NgDatepickerModule,
  ],
  providers: [UserGuard, ControllerGuard, AdminGuard, {provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true},
   UserService, TicketService, LineService, PricelistService, CurrentLocationService],
  bootstrap: [AppComponent]
})
export class AppModule { }
