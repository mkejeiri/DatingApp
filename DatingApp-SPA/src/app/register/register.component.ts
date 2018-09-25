import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_service/auth.service';
import { AlertifyService } from '../_service/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
  model: any = {};

  constructor(private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }
  register() {
    this.authService.register(this.model).subscribe(
      (next) => {
        // console.log('Registration successful!');
        this.alertify.success('Registration successful!');
      },
      (err) => {
        // console.log(err);
        this.alertify.error(err);
      });
  }
  cancel() {
    this.cancelRegister.emit(false);
    // console.log('cancelled');
    this.alertify.warning('cancelled');
  }
}
