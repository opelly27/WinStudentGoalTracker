import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BenchmarkList } from './benchmark-list';

describe('BenchmarkList', () => {
  let component: BenchmarkList;
  let fixture: ComponentFixture<BenchmarkList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BenchmarkList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BenchmarkList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
