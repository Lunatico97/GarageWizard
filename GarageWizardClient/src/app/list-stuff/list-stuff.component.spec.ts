import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListStuffComponent } from './list-stuff.component';

describe('ListStuffComponent', () => {
  let component: ListStuffComponent;
  let fixture: ComponentFixture<ListStuffComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ListStuffComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ListStuffComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
