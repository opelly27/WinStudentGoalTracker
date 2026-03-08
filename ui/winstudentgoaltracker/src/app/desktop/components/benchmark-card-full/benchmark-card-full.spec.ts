import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BenchmarkCardFull } from './benchmark-card-full';

describe('BenchmarkCardFull', () => {
  let component: BenchmarkCardFull;
  let fixture: ComponentFixture<BenchmarkCardFull>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BenchmarkCardFull]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BenchmarkCardFull);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
