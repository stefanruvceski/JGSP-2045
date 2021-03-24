import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LineNetworkComponent } from './line-network.component';

describe('LineNetworkComponent', () => {
  let component: LineNetworkComponent;
  let fixture: ComponentFixture<LineNetworkComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LineNetworkComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LineNetworkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
