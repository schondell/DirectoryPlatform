import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { MessageService } from '../../../core/services/message.service';
import { Message } from '../../../core/models';

@Component({
  selector: 'app-messages',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  template: `
    <h1>{{ 'messages.inbox' | translate }}</h1>
    @for (msg of messages; track msg.id) {
      <div class="message-row" [class.unread]="!msg.isRead">
        <span class="sender">{{ msg.senderName }}</span>
        <span class="subject">{{ msg.subject }}</span>
        <span class="date">{{ msg.createdAt | date:'short' }}</span>
      </div>
    }
    @if (messages.length === 0) { <p>{{ 'messages.noMessages' | translate }}</p> }
  `
})
export class MessagesComponent implements OnInit {
  messages: Message[] = [];
  constructor(private messageService: MessageService) {}
  ngOnInit(): void { this.messageService.getInbox().subscribe(m => this.messages = m); }
}
