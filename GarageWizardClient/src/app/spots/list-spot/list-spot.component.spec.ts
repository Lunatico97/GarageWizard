import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListSpotComponent } from './list-spot.component';

describe('ListSpotComponent', () => {
  let component: ListSpotComponent;
  let fixture: ComponentFixture<ListSpotComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ListSpotComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ListSpotComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
