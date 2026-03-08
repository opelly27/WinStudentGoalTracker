import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BenchmarkCard } from './benchmark-card';

describe('BenchmarkCard', () => {
  let component: BenchmarkCard;
  let fixture: ComponentFixture<BenchmarkCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BenchmarkCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BenchmarkCard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
