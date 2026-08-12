import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Artist, CreateArtistRequest } from '../models/track.models';

@Injectable({ providedIn: 'root' })
export class ArtistService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiBaseUrl}/artists`;

  getArtists(): Observable<Artist[]> {
    return this.http.get<Artist[]>(`${this.baseUrl}/GetAllArtists`);
  }

  createArtist(payload: CreateArtistRequest): Observable<Artist> {
    return this.http.post<Artist>(`${this.baseUrl}/CreateArtist`, payload);
  }
}
