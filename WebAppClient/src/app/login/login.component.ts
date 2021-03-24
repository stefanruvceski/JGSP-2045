import { Component, OnInit } from '@angular/core';
import { LoginUser } from '../Model';
import { FormBuilder, Validators } from '@angular/forms';
import { UserService } from '../user.service';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})


export class LoginComponent implements OnInit {
  loginUser: LoginUser = {
    Username: 'AlanFord',
    Password: 'AlanFord_123',
  };
  errorMessage = 'Username or password is incorrect.';
  profileForm = this.fb.group({
    username: ['', Validators.required],
    password: ['', Validators.required] ,
    });
  constructor(private fb: FormBuilder, private userService: UserService, private router: Router, private modalService: NgbModal) { }

  ngOnInit() {
  }
  onSubmit(content) {
    this.loginUser.Username = this.profileForm.controls.username.value;
    this.loginUser.Password = this.profileForm.controls.password.value;
    this.userService.logIn(this.loginUser, () => this.returnFromService(content));
  }
  openWindowCustomClass(content) {
    this.modalService.open(content, { windowClass: 'dark-modal' });
  }
  returnFromService(content) {
    if (!localStorage.getItem('username')) {
      this.openWindowCustomClass(content);
    } else {
      this.router.navigate(['/home']);
    }
  }

}
