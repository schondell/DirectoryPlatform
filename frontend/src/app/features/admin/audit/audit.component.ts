import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-audit',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  template: `
    <h1>{{ 'admin.audit' | translate }}</h1>
    @for (log of logs; track log.id) {
      <div class="audit-row">
        <span>{{ log.action }}</span>
        <span>{{ log.entityType }}</span>
        <span>{{ log.userName }}</span>
        <span>{{ log.createdAt | date:'short' }}</span>
      </div>
    }
  `
})
export class AuditComponent implements OnInit {
  logs: any[] = [];
  constructor(private api: ApiService) {}
  ngOnInit(): void { this.api.get<any>('admin/audit').subscribe(r => this.logs = r.items || []); }
}
