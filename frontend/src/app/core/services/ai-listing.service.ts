import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { AiGenerateListingRequest, AiImproveListingRequest, AiGeneratedListing } from '../models';

@Injectable({ providedIn: 'root' })
export class AiListingService {
  constructor(private api: ApiService) {}

  generate(request: AiGenerateListingRequest): Observable<AiGeneratedListing> {
    return this.api.post<AiGeneratedListing>('ailisting/generate', request);
  }

  improve(request: AiImproveListingRequest): Observable<AiGeneratedListing> {
    return this.api.post<AiGeneratedListing>('ailisting/improve', request);
  }
}
