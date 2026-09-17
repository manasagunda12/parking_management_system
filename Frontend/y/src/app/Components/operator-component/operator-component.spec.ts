import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperatorComponent } from './operator-component';

describe('OperatorComponent', () => {
  let component: OperatorComponent;
  let fixture: ComponentFixture<OperatorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OperatorComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(OperatorComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
