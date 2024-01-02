import { Component, Input } from '@angular/core';
import { NotificationInternalService } from '../../services/notification-internal.service';

@Component({
  selector: 'notification',
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.css'],
})
export class NotificationComponent {
  error$ = this.service.error$;

  constructor(private readonly service: NotificationInternalService) {}

  clearError() {
    this.service.clearError();
  }
}
