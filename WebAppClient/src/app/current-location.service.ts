import { Injectable, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs';

declare var $;

@Injectable()
export class CurrentLocationService {


  private proxy: any;
  private proxyName = 'notifications';
  private connection: any;
  public connectionExists: boolean;

  public notificationReceived: EventEmitter < string >;

  constructor() {
      this.notificationReceived = new EventEmitter<string>();
      this.connectionExists = false;
      // create a hub connection
      this.connection = $.hubConnection('http://localhost:52295/');

      // create new proxy with the given name
      this.proxy = this.connection.createHubProxy(this.proxyName);

  }

  // browser console will display whether the connection was successful
  public startConnection(): Observable<boolean> {

    return Observable.create((observer) => {

        this.connection.start()
        .done((data: any) => {
            console.log('Now connected ' + data.transport.name + ', connection ID= ' + data.id);
            this.connectionExists = true;

            observer.next(true);
            observer.complete();
        })
        .fail((error: any) => {
            console.log('Could not connect ' + error);
            this.connectionExists = false;

            observer.next(false);
            observer.complete();
        });
      });
  }

  public registerForClickEvents(): void {

      this.proxy.on('userClicked', (data: string) => {
          console.log('received notification: ' + data);
          this.notificationReceived.emit(data);
      });
  }

  public registerForTimerEvents(): Observable<string> {

    return Observable.create((observer) => {

        this.proxy.on('SendCoordinates', (data: string) => {
           // console.log('coordinates: ' + data);
            observer.next(data);
        });
    });
  }

  public StopTimer(line: string) {
      this.proxy.invoke('StopTimeServerUpdates', line);
  }

  public StartTimer(line: string) {
      this.proxy.invoke('TimeServerUpdates', line);
  }
}
