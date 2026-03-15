import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ToggleBenchmark } from './toggle-benchmark';

describe('ToggleBenchmark', () => {
  let component: ToggleBenchmark;
  let fixture: ComponentFixture<ToggleBenchmark>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ToggleBenchmark]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ToggleBenchmark);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
