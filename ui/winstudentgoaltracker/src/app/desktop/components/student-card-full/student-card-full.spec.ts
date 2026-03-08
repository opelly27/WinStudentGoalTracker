import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentCardFull } from './student-card-full';

describe('StudentCardFull', () => {
  let component: StudentCardFull;
  let fixture: ComponentFixture<StudentCardFull>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StudentCardFull]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentCardFull);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
