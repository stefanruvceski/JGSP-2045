import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../user.service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss']
})
export class NavBarComponent implements OnInit {
  username: string;
  role: string;

  constructor(private router: Router, private userService: UserService) {
    userService.username.subscribe ((newUsername) => {this.username = newUsername; });
    userService.role.subscribe(newRole => { this.role = newRole; });
  }

  ngOnInit() {
    this.username = localStorage.getItem('username');
    this.role = localStorage.getItem('role');
  }

  onClickLogout(event: Event): void {
    event.preventDefault(); // Prevents browser following the link
    // Here you can call your service method to logout the user
    // and then redirect with Router object, for example
    this.username = '';
    this.role = '';
    this.userService.logout();
    this.router.navigate(['login']);
  }
}
