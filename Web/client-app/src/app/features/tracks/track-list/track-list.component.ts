import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TrackService } from '../../../core/services/track.service';
import { Track, TrackStatus } from '../../../core/models/track.models';

@Component({
  selector: 'app-track-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './track-list.component.html',
  styleUrl: './track-list.component.css'
})
export class TrackListComponent implements OnInit {
  private trackService = inject(TrackService);

  tracks = signal<Track[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  statusFilter: '' | TrackStatus = '';
  readonly statuses: TrackStatus[] = ['drafted', 'submitted', 'distributed'];

  ngOnInit(): void {
    this.loadTracks();
  }

  loadTracks(): void {
    this.loading.set(true);
    this.error.set(null);
    this.trackService
      .getTracks(this.statusFilter ? { status: this.statusFilter } : undefined)
      .subscribe({
        next: (tracks) => {
          this.tracks.set(tracks);
          this.loading.set(false);
        },
        error: (err) => {
          this.error.set(this.describeError(err));
          this.loading.set(false);
        }
      });
  }

  onFilterChange(): void {
    this.loadTracks();
  }

  private describeError(err: any): string {
    if (err?.status === 0) {
      return 'Could not reach the API. Check that the backend is running and the API URL in environment.ts is correct.';
    }
    if (err?.status === 401) {
      return 'Unauthorized. Provide a valid JWT token above.';
    }
    return err?.error?.message ?? 'Failed to load tracks.';
  }
}
