import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ParkingSpace } from './parking-space';

describe('ParkingSpace', () => {
  let component: ParkingSpace;
  let fixture: ComponentFixture<ParkingSpace>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ParkingSpace],
    }).compileComponents();

    fixture = TestBed.createComponent(ParkingSpace);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
