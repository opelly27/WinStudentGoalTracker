import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProgressItem } from './progress-item';

describe('ProgressItem', () => {
  let component: ProgressItem;
  let fixture: ComponentFixture<ProgressItem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProgressItem]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProgressItem);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
