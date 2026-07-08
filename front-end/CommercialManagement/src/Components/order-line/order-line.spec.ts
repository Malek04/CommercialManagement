import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderLine } from './order-line';

describe('OrderLine', () => {
  let component: OrderLine;
  let fixture: ComponentFixture<OrderLine>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderLine],
    }).compileComponents();

    fixture = TestBed.createComponent(OrderLine);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
