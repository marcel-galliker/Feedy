import { TestBed } from '@angular/core/testing';

import { GESocketService } from './ge-socket.service';

describe('GESocketService', () => {
  let service: GESocketService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GESocketService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
