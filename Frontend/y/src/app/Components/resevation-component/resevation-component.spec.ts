import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResevationComponent } from './resevation-component';

describe('ResevationComponent', () => {
  let component: ResevationComponent;
  let fixture: ComponentFixture<ResevationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ResevationComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ResevationComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
