import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentCard } from './student-card';

describe('StudentCard', () => {
  let component: StudentCard;
  let fixture: ComponentFixture<StudentCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StudentCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentCard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
