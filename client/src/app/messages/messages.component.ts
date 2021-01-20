import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/pagainataion';
import { ConfirmService } from '../_services/confirm.service';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css'],
})
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;
  loading = false;

  constructor(
    private messageService: MessageService,
    private confirmService: ConfirmService
  ) {}

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    // this.loading=true;
    this.messageService
      .getMessages(this.pageNumber, this.pageSize, this.container)
      .subscribe((response) => {
        this.messages = response.result;

        this.pagination = response.pagination;
        // this.loading = false;
      });
  }

  deleteMessage(messageId: number) {
    this.confirmService
      .confirm('are you sure you want to delete?', 'really???')
      .subscribe((result) => {
        if (result) {
          this.messageService.deleteMessage(messageId).subscribe(() => {
            this.messages.splice(
              this.messages.findIndex((m) => m.id === messageId),
              1
            );
          });
        }
      });
  }

  pageChanged(event: any): void {
    this.pageNumber = event.page;

    this.loadMessages();
  }
}
