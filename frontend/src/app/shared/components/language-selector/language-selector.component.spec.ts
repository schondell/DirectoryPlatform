import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LanguageSelectorComponent } from './language-selector.component';
import { TranslateService } from '@ngx-translate/core';

describe('LanguageSelectorComponent', () => {
  let component: LanguageSelectorComponent;
  let fixture: ComponentFixture<LanguageSelectorComponent>;
  let translateSpy: jest.Mocked<Partial<TranslateService>>;

  beforeEach(async () => {
    translateSpy = {
      currentLang: 'en',
      use: jest.fn()
    };

    await TestBed.configureTestingModule({
      imports: [LanguageSelectorComponent],
      providers: [
        { provide: TranslateService, useValue: translateSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LanguageSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with current language', () => {
    expect(component.currentLang).toBe('en');
  });

  it('should render select with 5 language options', () => {
    const el = fixture.nativeElement as HTMLElement;
    const options = el.querySelectorAll('option');
    expect(options.length).toBe(5);
  });

  it('should change language on select change', () => {
    const event = {
      target: { value: 'fr' }
    } as unknown as Event;

    component.onLanguageChange(event);

    expect(translateSpy.use).toHaveBeenCalledWith('fr');
    expect(component.currentLang).toBe('fr');
    expect(localStorage.getItem('lang')).toBe('fr');
  });

  it('should store language in localStorage', () => {
    localStorage.removeItem('lang');
    const event = {
      target: { value: 'de' }
    } as unknown as Event;

    component.onLanguageChange(event);

    expect(localStorage.getItem('lang')).toBe('de');
  });

  it('should default to en when currentLang is undefined', async () => {
    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [LanguageSelectorComponent],
      providers: [
        { provide: TranslateService, useValue: { currentLang: undefined, use: jest.fn() } }
      ]
    }).compileComponents();

    const fix = TestBed.createComponent(LanguageSelectorComponent);
    const comp = fix.componentInstance;
    expect(comp.currentLang).toBe('en');
  });
});
