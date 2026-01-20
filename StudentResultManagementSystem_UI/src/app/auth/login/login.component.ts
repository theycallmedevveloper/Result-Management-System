import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [FormsModule, CommonModule],
    templateUrl: './login.component.html',
    styleUrl: './login.component.css'
})
export class LoginComponent {
    username = '';
    password = '';
    error = '';

    constructor(private authService: AuthService) { }

    login() {
        this.authService.login(this.username, this.password).subscribe({
            next: () => {
                alert('Login successful');
            },
            error: (err: any) => {
                this.error = err.error;
            }
        });
    }
}