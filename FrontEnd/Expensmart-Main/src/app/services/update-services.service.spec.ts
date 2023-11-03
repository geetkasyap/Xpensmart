import { TestBed } from '@angular/core/testing';

import { UpdateServicesService } from './update-services.service';

describe('UpdateServicesService', () => {
  let service: UpdateServicesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UpdateServicesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
