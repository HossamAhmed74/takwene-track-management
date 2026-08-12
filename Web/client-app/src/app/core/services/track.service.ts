import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateTrackRequest,
  DistributeTrackRequest,
  Track,
  TrackDetail,
  TrackFilter,
  UpdateTrackStatusRequest
} from '../models/track.models';

@Injectable({ providedIn: 'root' })
export class TrackService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiBaseUrl}/tracks`;

  getTracks(filter?: TrackFilter): Observable<Track[]> {
    let params = new HttpParams();
    if (filter?.artistId) params = params.set('artistId', filter.artistId);
    if (filter?.genre) params = params.set('genre', filter.genre);
    if (filter?.status) params = params.set('status', filter.status);
    return this.http.get<Track[]>(`${this.baseUrl}/GetAllTracks`, { params });
  }

  getTrackById(id: number): Observable<TrackDetail> {
    return this.http.get<TrackDetail>(`${this.baseUrl}/GetTrackById/${id}`);
  }

  createTrack(payload: CreateTrackRequest): Observable<Track> {
    return this.http.post<Track>(`${this.baseUrl}/CreateTrack`, payload);
  }

  distributeTrack(id: number, payload: DistributeTrackRequest): Observable<TrackDetail> {
    return this.http.post<TrackDetail>(`${this.baseUrl}/${id}/distribute`, payload);
  }

  updateTrackStatus(id: number, payload: UpdateTrackStatusRequest): Observable<Track> {
    return this.http.patch<Track>(`${this.baseUrl}/${id}/status`, payload);
  }
}
