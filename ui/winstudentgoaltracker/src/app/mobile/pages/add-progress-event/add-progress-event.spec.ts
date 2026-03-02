import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddProgressEvent } from './add-progress-event';

describe('AddProgressEvent', () => {
  let component: AddProgressEvent;
  let fixture: ComponentFixture<AddProgressEvent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddProgressEvent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddProgressEvent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
