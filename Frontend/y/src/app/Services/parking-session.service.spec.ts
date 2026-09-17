import { TestBed } from '@angular/core/testing';

import { ParkingSessionService } from './parking-session.service';

describe('ParkingSessionService', () => {
  let service: ParkingSessionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ParkingSessionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
