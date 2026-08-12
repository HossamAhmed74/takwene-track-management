import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink],
  template: `
    <header class="app-header">
      <a routerLink="/" class="brand">🎵 Track Distribution Manager</a>
      <span class="muted">Artists · Tracks · DSP Distribution</span>
    </header>

    <main class="app-main">
      <router-outlet />
    </main>
  `
})
export class AppComponent {}