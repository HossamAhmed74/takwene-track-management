import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Dsp } from '../models/track.models';

@Injectable({ providedIn: 'root' })
export class DspService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiBaseUrl}/dsps`;

  getDsps(): Observable<Dsp[]> {
    return this.http.get<Dsp[]>(this.baseUrl);
  }
}
