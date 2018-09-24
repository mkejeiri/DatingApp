import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
private baseUrl = 'http://localhost:5000/api/auth/';
constructor(private http: HttpClient) { }

login(model: any) {
      // in angular 6 to use rxjs we need to pass through a pipe!
  return this.http.post(this.baseUrl + 'login', model)
  .pipe(
    map(
    (response: any) => {
      const user = response;
      if (user) {
        console.log(response);
        localStorage.setItem('token', user.token);
      }
    }));
  }
  register(model: any) {
      return this.http.post(this.baseUrl + 'register', model).subscribe(
        (next) => {
          console.log('Registration successful!');
        },
        (err) => {
          console.log(err);
        });
  }
}
