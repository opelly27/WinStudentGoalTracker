import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentGoals } from './student-goals';

describe('StudentGoals', () => {
  let component: StudentGoals;
  let fixture: ComponentFixture<StudentGoals>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StudentGoals]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentGoals);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
