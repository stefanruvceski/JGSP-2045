import { Component, OnInit } from '@angular/core';
// import {DatepickerOptions} from '../ng2-datepicker/component/ng2-datepicker.component';
import * as enLocale from 'date-fns/locale/en';
import * as frLocale from 'date-fns/locale/fr';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Validators } from '@angular/forms';
import { FormArray } from '@angular/forms';
import { validateConfig } from '@angular/router/src/config';
import { User } from '../Model';
import { NgbDatepickerDayView } from '@ng-bootstrap/ng-bootstrap/datepicker/datepicker.module';
import { UserService } from '../user.service';
import { DatepickerOptions } from 'ng2-datepicker';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';
// import { read } from 'fs';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})

export class RegistrationComponent implements OnInit {
  passValue = '';

  user: User = {
      Username: '',
      FirstName: '',
      LastName: '',
      Password: '',
      ConfirmPassword: '',
      Address: '',
      AgeGroup: '',
      Document: '',
      Birthday: '',
      Email: '',
  };
  profileForm = this.fb.group({
    username: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(20)]],
    password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(30)]],
    confirmpassword: [''],
    firstname: ['', Validators.required],
    lastname: ['', Validators.required],
    address: [''],
    birthday: [''],
    agegroup: ['Regular', Validators.required],
    email: ['', Validators.email],
    document: ['']
    });

  Type: any = ['Regular', 'Student', 'Pensioner'];
  typeName: string;
  options: DatepickerOptions = {
    minYear: 1970,
    maxYear: 2030,
    // position: 'top-right',
    displayFormat: 'MMM D[,] YYYY',
    barTitleFormat: 'MMMM YYYY',
    dayNamesFormat: 'dd',
    firstCalendarDay: 1, // 0 - Sunday, 1 - Monday
    // locale: frLocale,
    // minDate: new Date(Date.now()), // Minimal selectable date
    // maxDate: new Date(Date.now()),  // Maximal selectable date
    barTitleIfEmpty: 'Click to select a date',
    placeholder: 'Date of birth', // HTML input placeholder attribute (default: '')
    addClass: 'form-control', // Optional, value to pass on to [ngClass] on the input field
    addStyle: {}, // Optional, value to pass to [ngStyle] on the input field
    fieldId: 'my-date-picker', // ID to assign to the input field. Defaults to datepicker-<counter>
    useEmptyBarTitle: false, // Defaults to true. If set to false then barTitleIfEmpty will be disregarded and
  };
  date: Date;
  errorMessage: string;
  imageUrl: string;
  
  constructor(private fb: FormBuilder, private router: Router, private userService: UserService, private modalService: NgbModal) {
    this.date = new Date();
   }

  ngOnInit() {
    this.user.AgeGroup = 'Regular';
  }


    onSubmit(content) {
      this.user.Username = this.profileForm.controls.username.value;
      this.user.FirstName = this.profileForm.controls.firstname.value;
      this.user.LastName = this.profileForm.controls.lastname.value;
      this.user.Email = this.profileForm.controls.email.value;
      this.user.Address = this.profileForm.controls.address.value;
      this.user.AgeGroup = this.profileForm.controls.agegroup.value;
      // this.user.Document = this.profileForm.controls.document.value;
      this.user.Password = this.profileForm.controls.password.value;
      this.user.ConfirmPassword = this.profileForm.controls.confirmpassword.value;
      this.user.Birthday = this.profileForm.controls.birthday.value;

      this.userService.addUser(this.user as User).subscribe(data => {},
        error => {
          this.errorMessage = 'Username or email is already in use.';
          this.openWindowCustomClass(content);
         // alert('Username or email is already in use.');
        });

      this.router.navigate(['login']);
     }
     openWindowCustomClass(content) {
      this.modalService.open(content, { windowClass: 'dark-modal' });
    }
    checkPasswords(group: FormGroup) { // here we have the 'passwords' group
    let pass = group.controls.password.value;
    let confirmPass = group.controls.confirmPass.value;

    return pass === confirmPass ? null : { notSame: true }
  }

      onFileChanged(event) {
        if (event.target.files && event.target.files[0]) {

          const file = event.target.files[0];

          const reader = new FileReader();
          reader.onload = e => {this.user.Document = reader.result.toString().split(',')[1]; console.log(this.user.Document); };

          reader.readAsDataURL(file);
          // this.user.Document = file;
        }
      }

      onchange() {
        this.user.AgeGroup = this.profileForm.controls.agegroup.value;
      }

      passwordFunc(event) {
        this.passValue = event.target.value; 
        //alert(this.passValue);
        this.profileForm.controls.confirmpassword.setValidators([Validators.required, Validators.pattern(this.passValue)])
        this.profileForm.controls.confirmpassword.updateValueAndValidity();
      }
}

