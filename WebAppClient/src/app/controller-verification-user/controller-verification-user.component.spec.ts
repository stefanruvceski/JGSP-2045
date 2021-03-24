import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ControllerVerificationUserComponent } from './controller-verification-user.component';

describe('ControllerVerificationUserComponent', () => {
  let component: ControllerVerificationUserComponent;
  let fixture: ComponentFixture<ControllerVerificationUserComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ControllerVerificationUserComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ControllerVerificationUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
