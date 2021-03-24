import { Component, OnInit, Input } from '@angular/core';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { Validators } from '@angular/forms';
import { FormArray } from '@angular/forms';
import { validateConfig } from '@angular/router/src/config';
import { User } from '../Model';
import { NgbDatepickerDayView } from '@ng-bootstrap/ng-bootstrap/datepicker/datepicker.module';
import { UserService } from '../user.service';
import { DatepickerOptions } from 'ng2-datepicker';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';


@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.scss']
})
export class UserInfoComponent implements OnInit {
  @Input() user: User = {
      Username: '',
      FirstName: '',
      LastName: '',
      Password: '',
      ConfirmPassword: '',
      Address: '',
      AgeGroup: '',
      Document: null,
      Birthday: '',
      Email: '',
  };

  profileForm = this.fb.group({
    username: [this.user.Username, [Validators.required, Validators.minLength(5), Validators.maxLength(20)]],
    password: [this.user.Password, [Validators.required, Validators.minLength(6), Validators.maxLength(30)]],
    confirmpassword: [this.user.ConfirmPassword],
    firstname: [this.user.FirstName, Validators.required],
    lastname: [this.user.LastName, Validators.required],
    address: [this.user.Address],
    birthday: [this.user.Birthday],
    agegroup: [this.user.AgeGroup, Validators.required],
    email: [this.user.Email, Validators.email],
    document: [this.user.Document]
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

  constructor(private router: Router, private fb: FormBuilder, private userService: UserService) {
    this.date = new Date();
   }

  ngOnInit() {
    this.userService.getUser(localStorage.getItem('username')).subscribe(user => {this.user = user,

      this.profileForm.patchValue({
        username: user.Username,
        email: user.Email,
        password: user.Password,
        confirmpassword: user.ConfirmPassword,
        firstname: user.FirstName,
        address: user.Address,
        agegroup: user.AgeGroup,
        document: 'data:image/png;base64,' + user.Document,
        birthday: user.Birthday,
        lastname: user.LastName});


    });
  }

  onSubmit() {
    this.user.Username = this.profileForm.controls.username.value;
    this.user.FirstName = this.profileForm.controls.firstname.value;
    this.user.LastName = this.profileForm.controls.lastname.value;
    this.user.Email = this.profileForm.controls.email.value;
    this.user.Address = this.profileForm.controls.address.value;
    this.user.AgeGroup = this.profileForm.controls.agegroup.value;
    // this.user.Document = this.profileForm.controls.document.value.split(',')[1];
    this.user.Password = this.profileForm.controls.password.value;
    this.user.ConfirmPassword = this.profileForm.controls.confirmpassword.value;
    this.user.Birthday = this.profileForm.controls.birthday.value;

    this.userService.changeUser(this.user as User).subscribe(user => {this.user = user,

      this.profileForm.patchValue({
        username: user.Username,
        email: user.Email,
        password: user.Password,
        confirmpassword: user.ConfirmPassword,
        firstname: user.FirstName,
        address: user.Address,
        agegroup: user.AgeGroup,
        document: 'data:image/png;base64,' + user.Document ,
        birthday: user.Birthday,
        lastname: user.LastName}),
        this.router.navigate(['home']);

    },
    error => {
      alert('NE VALJA');
    });
  }

  onFileChanged(event) {
    if (event.target.files && event.target.files[0]) {

      const file = event.target.files[0];

      const reader = new FileReader();
      reader.onload = e => {this.user.Document = reader.result.toString().split(',')[1]; console.log(this.user.Document); };

      reader.readAsDataURL(file);
    }
  }

  onchange() {
    this.user.AgeGroup = this.profileForm.controls.agegroup.value;
  }
}
