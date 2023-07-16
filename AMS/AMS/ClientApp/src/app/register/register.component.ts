import { Component } from '@angular/core';
import { UserService } from '../../services/user.service';
import { User } from '../models/user';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  user: User = {
    userName: '',
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    age: 0
  };

  isError: boolean = false;
  errorMessage: string = '';

  constructor(private userService: UserService) {}

  register(): void {
    this.userService.register(this.user).subscribe(
      () => {
        console.log('Registration successful');
        this.user = {
          userName: '',
          firstName: '',
          lastName: '',
          email: '',
          password: '',
          age: 0
        };
        this.isError = false;
        this.errorMessage = '';
      },
      (error) => {
        console.error('Registration failed', error);
        this.isError = true;
        this.errorMessage = 'Registration failed. Please try again.';
      }
    );
  }
}
