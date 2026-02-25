import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
@Component({ selector: 'app-member-dashboard', standalone: true, imports: [CommonModule, TranslateModule], template: '<h1>{{ "nav.dashboard" | translate }}</h1><p>Welcome to your dashboard.</p>' })
export class MemberDashboardComponent {}
