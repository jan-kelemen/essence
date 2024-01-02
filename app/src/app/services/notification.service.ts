import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { NotificationInternalService } from "./notification-internal.service";

@Injectable({
  providedIn: "root"
})
export class NotificationService {
  constructor(private readonly internalService: NotificationInternalService){}

  public showError(message: string): void {
    this.internalService.errorSubject$.next(message);
  }
}
