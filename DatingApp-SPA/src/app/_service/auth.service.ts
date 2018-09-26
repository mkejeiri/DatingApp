import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { AlertifyService } from './alertify.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { JwtModule } from '@auth0/angular-jwt';
import { HttpClientModule } from '@angular/common/http';
import { environment } from '../../environments/environment';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = environment.apiUrl + 'auth/';
  constructor(private http: HttpClient, private alertify: AlertifyService) { }
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  // const expirationDate = jwtHelper.getTokenExpirationDate(myRawToken);
  // const isExpired = jwtHelper.isTokenExpired(myRawToken);

  login(model: any) {
    // in angular 6 to use rxjs we need to go through a pipe!
    return this.http.post(this.baseUrl + 'login', model)
      .pipe(
        map(
          (response: any) => {
            const user = response;
            if (user) {
              // console.log(response);
              localStorage.setItem('token', user.token);
              this.decodedToken = this.jwtHelper.decodeToken(user.token);
            }
          }));
  }
  register(model: any) {
    return this.http.post(this.baseUrl + 'register', model);
  }


  loggedIn() {
    const token = localStorage.getItem('token');
    // shorthand for : if empty ->false else true
    // return !!token;
    return !this.jwtHelper.isTokenExpired(token);
  }
  logout() {
    localStorage.removeItem('token');
    // console.log('logged out');
    this.alertify.message('logged out');
  }
}
