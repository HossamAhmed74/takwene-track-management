import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TrackService } from '../../../core/services/track.service';
import { DspService } from '../../../core/services/dsp.service';
import { Dsp, TrackDetail } from '../../../core/models/track.models';

@Component({
  selector: 'app-track-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './track-detail.component.html',
  styleUrl: './track-detail.component.css'
})
export class TrackDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private trackService = inject(TrackService);
  private dspService = inject(DspService);

  track = signal<TrackDetail | null>(null);
  dsps = signal<Dsp[]>([]);
  selectedDspIds = signal<number[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  actionError = signal<string | null>(null);
  submitting = signal(false);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadTrack(id);
    this.dspService.getDsps().subscribe({
      next: (dsps) => this.dsps.set(dsps),
      error: () => {
        /* DSP list is a nice-to-have for the distribute form; ignore failures silently. */
      }
    });
  }

  loadTrack(id: number): void {
    this.loading.set(true);
    this.error.set(null);
    this.trackService.getTrackById(id).subscribe({
      next: (track) => {
        this.track.set(track);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(this.describeError(err));
        this.loading.set(false);
      }
    });
  }

  toggleDsp(dspId: number, checked: boolean): void {
    const current = this.selectedDspIds();
    this.selectedDspIds.set(
      checked ? [...current, dspId] : current.filter((id) => id !== dspId)
    );
  }

  submitDistribution(): void {
    const track = this.track();
    if (!track || this.selectedDspIds().length === 0) return;

    this.submitting.set(true);
    this.actionError.set(null);
    this.trackService.distributeTrack(track.id, { dspIds: this.selectedDspIds() }).subscribe({
      next: (updated) => {
        this.track.set(updated);
        this.selectedDspIds.set([]);
        this.submitting.set(false);
      },
      error: (err) => {
        this.actionError.set(this.describeError(err));
        this.submitting.set(false);
      }
    });
  }

  private describeError(err: any): string {
    if (err?.status === 0) {
      return 'Could not reach the API. Check that the backend is running and the API URL in environment.ts is correct.';
    }
    if (err?.status === 401) {
      return 'Unauthorized. Provide a valid JWT token above.';
    }
    if (err?.status === 404) {
      return 'Track not found.';
    }
    return err?.error?.message ?? 'Something went wrong.';
  }
}
