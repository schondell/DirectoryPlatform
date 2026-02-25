import { Component } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
@Component({ selector: 'app-settings', standalone: true, imports: [TranslateModule], template: '<h1>{{ "nav.settings" | translate }}</h1><p>Settings page coming soon.</p>' })
export class SettingsComponent {}
