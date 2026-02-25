import { Component } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
@Component({ selector: 'app-approval', standalone: true, imports: [TranslateModule], template: '<h1>{{ "admin.approval" | translate }}</h1><p>Listing approval workflow.</p>' })
export class ApprovalComponent {}
