import { Component, OnInit, Input } from '@angular/core';
import { User, UserStatus } from '../Model';
import { UserService } from '../user.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';


@Component({
  selector: 'app-controller-verification-user',
  templateUrl: './controller-verification-user.component.html',
  styleUrls: ['./controller-verification-user.component.scss']
})
export class ControllerVerificationUserComponent implements OnInit {
  @Input() usersVerificated: UserStatus[];
  @Input() usersPending: UserStatus[];
  @Input() usersNonVerificated: UserStatus[];
  selectedUser: UserStatus;

  constructor(private router: Router, private modalService: NgbModal, private userService: UserService) { }

  ngOnInit() {
    this.getUsers();
  }

  getUsers() {
    this.userService.getusers(1).subscribe(users => this.usersVerificated = users);
    this.userService.getusers(0).subscribe(users => this.usersPending = users);
    this.userService.getusers(2).subscribe(users => this.usersNonVerificated = users);
  }

  selectUser(username: string) {

  }

  showUserDetails(user: UserStatus, content) {
    this.selectedUser = user;
    this.modalService.open(content, { windowClass: 'dark-modal' });
  }

  ok() {

    this.selectedUser.Status = 'Yes';       // indikator serveru da korisnik postaje verifikovan
    this.userService.changeUserStatus(this.selectedUser).subscribe(user => {
      this.getUsers();

    });
    this.modalService.dismissAll();

  }

  cancel() {
    this.getUsers();
    this.selectedUser.Status = 'No';       // indikator serveru da je korisnikov zahtev odbijen (nije verifikovan)
    this.userService.changeUserStatus(this.selectedUser).subscribe(user => {
      this.getUsers();

    });
    this.modalService.dismissAll();
  }


}
