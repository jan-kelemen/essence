import { Injectable } from "@angular/core";
import { Subject } from "rxjs";

@Injectable({
  providedIn: "root"
})
export class NotificationInternalService {
  public errorSubject$ = new Subject<string>();

  public get error$() {
    return this.errorSubject$.asObservable();
  }

  public clearError() {
    this.errorSubject$.next('');
  }
}
