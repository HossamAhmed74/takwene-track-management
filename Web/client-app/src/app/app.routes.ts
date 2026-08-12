import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'tracks' },
  {
    path: 'tracks',
    loadComponent: () =>
      import('./features/tracks/track-list/track-list.component').then((m) => m.TrackListComponent)
  },
  {
    path: 'tracks/:id',
    loadComponent: () =>
      import('./features/tracks/track-detail/track-detail.component').then(
        (m) => m.TrackDetailComponent
      )
  },
  { path: '**', redirectTo: 'tracks' }
];
