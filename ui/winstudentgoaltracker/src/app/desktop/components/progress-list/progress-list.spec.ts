import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProgressList } from './progress-list';

describe('ProgressList', () => {
  let component: ProgressList;
  let fixture: ComponentFixture<ProgressList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProgressList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProgressList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
