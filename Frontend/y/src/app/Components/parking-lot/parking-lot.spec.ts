import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ParkingLot } from './parking-lot';

describe('ParkingLot', () => {
  let component: ParkingLot;
  let fixture: ComponentFixture<ParkingLot>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ParkingLot],
    }).compileComponents();

    fixture = TestBed.createComponent(ParkingLot);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
