import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_service/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  constructor(private authService: AuthService) { }
    ngOnInit() {}
    login() {
      this.authService.login(this.model).subscribe(
        next => {
          console.log('Logged in successfully!');
        },
        err => {
          console.log('Failed to login');
        }
      );
    }
    loggedIn() {
      const token = localStorage.getItem('token');
      // shorthand for : if empty ->false else true
      return !!token;
    }
    logout() {
      localStorage.removeItem('token');
      console.log('logged out');
    }
  }
