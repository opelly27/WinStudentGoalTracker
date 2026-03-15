import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProgressEdit } from './progress-edit';

describe('ProgressEdit', () => {
  let component: ProgressEdit;
  let fixture: ComponentFixture<ProgressEdit>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProgressEdit]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProgressEdit);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
