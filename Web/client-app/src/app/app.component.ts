import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  auth = inject(AuthService);
  tokenInput = signal(this.auth.token() ?? '');
  showTokenBox = signal(false);

  saveToken(): void {
    const value = this.tokenInput().trim();
    if (value) {
      this.auth.setToken(value);
    } else {
      this.auth.clearToken();
    }
    this.showTokenBox.set(false);
  }
}
