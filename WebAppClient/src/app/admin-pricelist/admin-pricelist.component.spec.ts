import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminPricelistComponent } from './admin-pricelist.component';

describe('AdminPricelistComponent', () => {
  let component: AdminPricelistComponent;
  let fixture: ComponentFixture<AdminPricelistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdminPricelistComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminPricelistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
