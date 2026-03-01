import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { DynamicFilterComponent } from './dynamic-filter.component';
import { AttributeDefinitionService } from '../../../core/services/attribute-definition.service';
import { AttributeDefinition } from '../../../core/models';
import { SimpleChange } from '@angular/core';

describe('DynamicFilterComponent', () => {
  let component: DynamicFilterComponent;
  let fixture: ComponentFixture<DynamicFilterComponent>;
  let attrServiceSpy: jest.Mocked<Partial<AttributeDefinitionService>>;

  const mockAttributes: AttributeDefinition[] = [
    {
      id: 'a1', name: 'Brand', slug: 'brand', type: 'Select', categoryId: 'c1',
      isFilterable: true, isRequired: false, displayOrder: 1, options: ['BMW', 'Audi']
    },
    {
      id: 'a2', name: 'Mileage', slug: 'mileage', type: 'Number', categoryId: 'c1',
      isFilterable: true, isRequired: false, displayOrder: 2, unit: 'km', minValue: 0, maxValue: 500000
    },
    {
      id: 'a3', name: 'Automatic', slug: 'automatic', type: 'Boolean', categoryId: 'c1',
      isFilterable: true, isRequired: false, displayOrder: 3
    }
  ];

  beforeEach(async () => {
    attrServiceSpy = {
      getByCategory: jest.fn().mockReturnValue(of(mockAttributes))
    };

    await TestBed.configureTestingModule({
      imports: [DynamicFilterComponent],
      providers: [
        { provide: AttributeDefinitionService, useValue: attrServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(DynamicFilterComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load attributes when categoryId changes', () => {
    component.categoryId = 'c1';
    component.ngOnChanges({
      categoryId: new SimpleChange(undefined, 'c1', true)
    });
    expect(attrServiceSpy.getByCategory).toHaveBeenCalledWith('c1', true);
    expect(component.attributes).toEqual(mockAttributes);
  });

  it('should clear filterValues when categoryId changes', () => {
    component.filterValues = { brand: 'BMW' };
    component.categoryId = 'c2';
    component.ngOnChanges({
      categoryId: new SimpleChange('c1', 'c2', false)
    });
    expect(component.filterValues).toEqual({});
  });

  it('should not load attributes when categoryId is empty', () => {
    component.categoryId = '';
    component.ngOnChanges({
      categoryId: new SimpleChange('c1', '', false)
    });
    expect(attrServiceSpy.getByCategory).not.toHaveBeenCalled();
  });

  describe('onFilterChange', () => {
    it('should set filter value and emit', () => {
      jest.spyOn(component.filterChanged, 'emit');
      component.onFilterChange('brand', 'BMW');
      expect(component.filterValues['brand']).toBe('BMW');
      expect(component.filterChanged.emit).toHaveBeenCalledWith({ brand: 'BMW' });
    });

    it('should remove filter value when empty and emit', () => {
      component.filterValues = { brand: 'BMW' };
      jest.spyOn(component.filterChanged, 'emit');
      component.onFilterChange('brand', '');
      expect(component.filterValues['brand']).toBeUndefined();
      expect(component.filterChanged.emit).toHaveBeenCalledWith({});
    });
  });

  describe('onBooleanChange', () => {
    it('should set value to "true" when checked', () => {
      jest.spyOn(component.filterChanged, 'emit');
      component.onBooleanChange('automatic', true);
      expect(component.filterValues['automatic']).toBe('true');
    });

    it('should clear value when unchecked', () => {
      component.filterValues = { automatic: 'true' };
      jest.spyOn(component.filterChanged, 'emit');
      component.onBooleanChange('automatic', false);
      expect(component.filterValues['automatic']).toBeUndefined();
    });
  });

  describe('onRangeChange', () => {
    it('should combine min and max with dash', () => {
      jest.spyOn(component.filterChanged, 'emit');
      component.onRangeChange('mileage', '1000', '50000');
      expect(component.filterValues['mileage']).toBe('1000-50000');
    });

    it('should use min: prefix for min-only', () => {
      jest.spyOn(component.filterChanged, 'emit');
      component.onRangeChange('mileage', '1000', '');
      expect(component.filterValues['mileage']).toBe('min:1000');
    });

    it('should use max: prefix for max-only', () => {
      jest.spyOn(component.filterChanged, 'emit');
      component.onRangeChange('mileage', '', '50000');
      expect(component.filterValues['mileage']).toBe('max:50000');
    });

    it('should clear value when both empty', () => {
      component.filterValues = { mileage: '1000-50000' };
      jest.spyOn(component.filterChanged, 'emit');
      component.onRangeChange('mileage', '', '');
      expect(component.filterValues['mileage']).toBeUndefined();
    });
  });

  describe('getMin', () => {
    it('should extract min from range value', () => {
      component.filterValues = { mileage: '1000-50000' };
      expect(component.getMin('mileage')).toBe('1000');
    });

    it('should extract min from min: prefix', () => {
      component.filterValues = { mileage: 'min:1000' };
      expect(component.getMin('mileage')).toBe('1000');
    });

    it('should return empty for max-only value', () => {
      component.filterValues = { mileage: 'max:50000' };
      expect(component.getMin('mileage')).toBe('');
    });

    it('should return empty for missing slug', () => {
      expect(component.getMin('nonexistent')).toBe('');
    });
  });

  describe('getMax', () => {
    it('should extract max from range value', () => {
      component.filterValues = { mileage: '1000-50000' };
      expect(component.getMax('mileage')).toBe('50000');
    });

    it('should extract max from max: prefix', () => {
      component.filterValues = { mileage: 'max:50000' };
      expect(component.getMax('mileage')).toBe('50000');
    });

    it('should return empty for min-only value', () => {
      component.filterValues = { mileage: 'min:1000' };
      expect(component.getMax('mileage')).toBe('');
    });

    it('should return empty for missing slug', () => {
      expect(component.getMax('nonexistent')).toBe('');
    });
  });
});
