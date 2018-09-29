import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { AlertifyService } from './alertify.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from '../../environments/environment';
import { User } from '../_models/User';
import { BehaviorSubject } from 'rxjs/';
// import { JwtModule } from '@auth0/angular-jwt';
// import { HttpClientModule } from '@angular/common/http';
// import { TypeaheadOptions } from 'ngx-bootstrap';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = environment.apiUrl + 'auth/';
  constructor(private http: HttpClient, private alertify: AlertifyService) { }
  jwtHelper = new JwtHelperService();
  currentUser: User;
  decodedToken: any;
  currentPhotoSubject = new BehaviorSubject<string>('../../assets/user.png');


  // changeMemberPhoto(photoUrl: string) {
  //   this.photoUrlSubject.next(photoUrl);
  // }

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
              localStorage.setItem('user', JSON.stringify(user.user));
              this.currentUser = user.user;
              this.currentPhotoSubject.next(this.currentUser.photoUrl);
            }
          }));
  }
  register(model: any) {
    return this.http.post(this.baseUrl + 'register', model);
  }


  loggedIn() {
    const token = localStorage.getItem('token');
    this.currentUser = JSON.parse(localStorage.getItem('user'));
    return !this.jwtHelper.isTokenExpired(token);
  }
  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.decodedToken = null;
    this.currentUser = null;
    // console.log('logged out');
    this.alertify.message('logged out');
  }
}
