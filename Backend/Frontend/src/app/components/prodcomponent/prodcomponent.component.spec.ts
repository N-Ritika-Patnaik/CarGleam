import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProdcomponentComponent } from './prodcomponent.component';

describe('ProdcomponentComponent', () => {
  let component: ProdcomponentComponent;
  let fixture: ComponentFixture<ProdcomponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProdcomponentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProdcomponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
