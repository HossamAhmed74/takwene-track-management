export interface Artist {
  id: number;
  name: string;
  email: string;
  country: string;
}

export type TrackStatus = 'drafted' | 'submitted' | 'distributed';
export type DistributionStatus = 'pending' | 'live' | 'rejected';

export interface Dsp {
  id: number;
  name: string;
}

export interface TrackDistribution {
  id: number;
  trackId: number;
  dspId: number;
  dspName?: string;
  submittedAt: string;
  status: DistributionStatus;
}

export interface Track {
  id: number;
  title: string;
  artistId: number;
  artistName?: string;
  isrc: string;
  releaseDate: string;
  genre: string;
  status: TrackStatus;
}

export interface TrackDetail extends Track {
  distributions: TrackDistribution[];
}

export interface TrackFilter {
  artistId?: number | null;
  genre?: string | null;
  status?: TrackStatus | null;
}

export interface CreateArtistRequest {
  name: string;
  email: string;
  country: string;
}

export interface CreateTrackRequest {
  title: string;
  artistId: number;
  isrc: string;
  releaseDate: string;
  genre: string;
}

export interface DistributeTrackRequest {
  dspIds: number[];
}

export interface UpdateTrackStatusRequest {
  status: TrackStatus;
}
