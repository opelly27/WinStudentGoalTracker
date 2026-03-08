import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GoalCardFull } from './goal-card-full';

describe('GoalCardFull', () => {
  let component: GoalCardFull;
  let fixture: ComponentFixture<GoalCardFull>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GoalCardFull]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GoalCardFull);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
