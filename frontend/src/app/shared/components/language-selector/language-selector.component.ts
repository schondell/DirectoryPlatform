import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-language-selector',
  standalone: true,
  imports: [CommonModule],
  template: `
    <select (change)="onLanguageChange($event)" [value]="currentLang">
      <option value="en">English</option>
      <option value="fr">Français</option>
      <option value="de">Deutsch</option>
      <option value="it">Italiano</option>
      <option value="es">Español</option>
    </select>
  `
})
export class LanguageSelectorComponent {
  currentLang: string;
  constructor(private translate: TranslateService) {
    this.currentLang = this.translate.currentLang || 'en';
  }
  onLanguageChange(event: Event): void {
    const lang = (event.target as HTMLSelectElement).value;
    this.translate.use(lang);
    this.currentLang = lang;
    if (typeof localStorage !== 'undefined') localStorage.setItem('lang', lang);
  }
}
