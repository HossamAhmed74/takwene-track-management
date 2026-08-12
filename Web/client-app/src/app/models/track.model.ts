export type TrackStatus = 'draft' | 'submitted' | 'distributed';
export type DspStatus = 'pending' | 'live' | 'rejected';

export interface Artist {
  id: number;
  name: string;
  email?: string;
  country?: string;
}

export interface Dsp {
  id: number;
  name: string;
}

export interface TrackDistribution {
  id?: number;
  dspId?: number;
  dsp?: Dsp;
  dspName?: string;
  submittedAt?: string;
  status: DspStatus;
}

export interface Track {
  id: number;
  title: string;
  artistId?: number;
  artist?: Artist;
  artistName?: string;
  isrc?: string;
  releaseDate?: string;
  genre?: string;
  status: TrackStatus;
  distributions?: TrackDistribution[];
  trackDistributions?: TrackDistribution[]; // tolerate alternate naming
}